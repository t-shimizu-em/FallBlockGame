using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RetryButtonController : MonoBehaviour
{
    public void OnClickRetryButton()
    {
        SoundManager.Instance.playButtonSe();
        SceneManager.LoadScene("GameScene");
    }
}
