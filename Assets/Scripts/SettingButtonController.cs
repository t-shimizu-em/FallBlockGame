using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingButtonController : MonoBehaviour
{
    public void OnClickSettingButton()
    {
        SoundManager.Instance.playButtonSe();
        SceneManager.LoadScene("SettingScene");
    }
}
