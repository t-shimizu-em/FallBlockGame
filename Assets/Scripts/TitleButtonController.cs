using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleButtonController : MonoBehaviour
{
    public void OnClickTitleButton()
    {
        SoundManager.Instance.playButtonSe();
        SceneManager.LoadScene("TitleScene");
    }
}
