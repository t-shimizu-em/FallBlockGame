///
///  @brief コルーチンに対するDOTweenのSequence風クラス
///

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// コルーチンに対するDOTweenのSequence風クラス
/// </summary>
public class CoroutineSequence
{
    /// <summary>
    /// Insertで追加されたEnumeratorを管理するクラス
    /// </summary>
    private class InsertedEnumerator
    {
        /// <summary>
        /// 位置
        /// </summary>
        private float _atPosition;

        /// <summary>
        /// IEnumerator
        /// </summary>
        public IEnumerator Enumerator { get; private set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public InsertedEnumerator(float atPosition, IEnumerator enumerator)
        {
            _atPosition = atPosition;
            Enumerator = enumerator;
        }

        /// <summary>
        /// Enumeratorの取得
        /// </summary>
        public IEnumerator GetEnumerator(Action callback)
        {
            if (_atPosition > 0f)
            {
                yield return new WaitForSeconds(_atPosition);
            }
            yield return Enumerator;
            if (callback != null)
            {
                callback();
            }
        }
    }

    /// <summary>
    /// Insertされたenumerator
    /// </summary>
    private List<InsertedEnumerator> _insertedEnumerators;

    /// <summary>
    /// Appendされたenumerator
    /// </summary>
    private List<IEnumerator> _appendedEnumerators;

    /// <summary>
    /// 終了時に実行するAction
    /// </summary>
    private Action OnCompletedAction;

    /// <summary>
    /// コルーチンの実行者
    /// </summary>
    private MonoBehaviour _owner;

    /// <summary>
    /// 内部で実行されたコルーチンのリスト
    /// </summary>
    private List<Coroutine> _coroutines;

    /// <summary>
    /// 追加されたCoroutineSequenceのリスト
    /// </summary>
    private List<CoroutineSequence> _sequences;

    /// <summary>
    /// シーケンスの実行が終了しているかどうか
    /// </summary>
    public bool IsComplete { get; private set; }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public CoroutineSequence(MonoBehaviour owner)
    {
        _owner = owner;
        _insertedEnumerators = new List<InsertedEnumerator>();
        _appendedEnumerators = new List<IEnumerator>();
        _coroutines = new List<Coroutine>();
        _sequences = new List<CoroutineSequence>();
    }

    /// <summary>
    /// enumeratorをatPositionにInsertする
    /// atPosition秒後にenumeratorが実行される
    /// </summary>
    public CoroutineSequence Insert(float atPosition, IEnumerator enumerator)
    {
        _insertedEnumerators.Add(new InsertedEnumerator(atPosition, enumerator));
        return this;
    }

    /// <summary>
    /// callbackをatPositionにInsertする
    /// </summary>
    public CoroutineSequence InsertCallback(float atPosition, Action callback)
    {
        _insertedEnumerators.Add(new InsertedEnumerator(atPosition, GetCallbackEnumerator(callback)));
        return this;
    }

    /// <summary>
    /// CoroutineSequenceをatPositionにInsertする
    /// </summary>
    public CoroutineSequence InsertSequence(float atPosition, CoroutineSequence sequence)
    {
        _insertedEnumerators.Add(new InsertedEnumerator(atPosition, sequence.GetEnumerator()));
        _sequences.Add(sequence);
        return this;
    }

    /// <summary>
    /// enumeratorをAppendする
    /// Appendされたenumeratorは、Insertされたenumeratorが全て実行された後に順番に実行される
    /// </summary>
    public CoroutineSequence Append(IEnumerator enumerator)
    {
        _appendedEnumerators.Add(enumerator);
        return this;
    }

    /// <summary>
    /// callbackをAppendする
    /// </summary>
    public CoroutineSequence AppendCallback(Action callback)
    {
        _appendedEnumerators.Add(GetCallbackEnumerator(callback));
        return this;
    }

    /// <summary>
    /// CoroutineSequenceをAppendする
    /// </summary>
    public CoroutineSequence AppendSequence(CoroutineSequence sequence)
    {
        _appendedEnumerators.Add(sequence.GetEnumerator());
        _sequences.Add(sequence);
        return this;
    }

    /// <summary>
    /// 待機をAppendする
    /// </summary>
    public CoroutineSequence AppendInterval(float seconds)
    {
        _appendedEnumerators.Add(GetWaitForSecondsEnumerator(seconds));
        return this;
    }

    /// <summary>
    /// 終了時の処理を追加する
    /// </summary>
    public CoroutineSequence OnCompleted(Action action)
    {
        if (IsComplete)
        {
            // 既に終了しているなら即時実行する
            action?.Invoke();
        }
        else
        {
            // まだ終了していないならOnCompletedActionに追加
            OnCompletedAction += action;
        }
        return this;
    }

    /// <summary>
    /// シーケンスのIEnumeratorを取得する
    /// </summary>
    public IEnumerator GetEnumerator()
    {
        // InsertされたIEnumeratorの実行
        int counter = _insertedEnumerators.Count;
        foreach (InsertedEnumerator insertedEnumerator in _insertedEnumerators)
        {
            Coroutine coroutine = _owner.StartCoroutine(insertedEnumerator.GetEnumerator(() =>
            {
                counter--;
            }));
            _coroutines.Add(coroutine);
        }
        // InsertされたIEnumeratorが全て実行されるのを待つ
        while (counter > 0)
        {
            yield return null;
        }
        // AppendされたIEnumeratorの実行
        foreach (IEnumerator appendedEnumerator in _appendedEnumerators)
        {
            yield return appendedEnumerator;
        }
        // 終了時の処理
        if (OnCompletedAction != null)
        {
            OnCompletedAction();
        }
        IsComplete = true;
    }

    /// <summary>
    /// シーケンスを実行する
    /// </summary>
    public Coroutine Play()
    {
        IsComplete = false;
        Coroutine coroutine = _owner.StartCoroutine(GetEnumerator());
        _coroutines.Add(coroutine);
        return coroutine;
    }

    /// <summary>
    /// シーケンスを止める
    /// </summary>
    public void Stop()
    {
        foreach (Coroutine coroutine in _coroutines)
        {
            if (coroutine != null)
            {
                _owner.StopCoroutine(coroutine);
            }
        }
        foreach (CoroutineSequence sequence in _sequences)
        {
            if (sequence != null)
            {
                sequence.Stop();
            }
        }
        _coroutines.Clear();
        _sequences.Clear();
        IsComplete = true;
    }

    /// <summary>
    /// callbackを実行するIEnumeratorを返す
    /// </summary>
    private IEnumerator GetCallbackEnumerator(Action callback)
    {
        callback();
        yield break;
    }

    /// <summary>
    /// 待機Enumeratorを返す
    /// </summary>
    private IEnumerator GetWaitForSecondsEnumerator(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }

    /// <summary>
    /// シーケンスを止めるのと初期化をする
    /// </summary>
    public void StopAndInitialize()
    {
        foreach (Coroutine coroutine in _coroutines)
        {
            _owner.StopCoroutine(coroutine);
        }
        foreach (InsertedEnumerator insertedEnumerator in _insertedEnumerators)
        {
            _owner.StopCoroutine(insertedEnumerator.Enumerator);
        }
        foreach (IEnumerator enumerator in _appendedEnumerators)
        {
            _owner.StopCoroutine(enumerator);
        }
        foreach (CoroutineSequence sequence in _sequences)
        {
            sequence.Stop();
        }
        _coroutines.Clear();
        _insertedEnumerators.Clear();
        _appendedEnumerators.Clear();
        _sequences.Clear();
        IsComplete = true;
    }
}
