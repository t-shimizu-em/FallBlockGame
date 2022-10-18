using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleController : MonoBehaviour
{
    public GameObject startButton;

    void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            SoundManager.Instance.playButtonSe();
            SceneManager.LoadScene("GameScene");
        }
    }
}
