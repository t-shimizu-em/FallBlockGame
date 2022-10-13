using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackButtonController : MonoBehaviour
{
    public void OnClickBackButton()
    {
        SoundManager.Instance.playButtonSe();
        SceneManager.LoadScene("TitleScene");
    }
}