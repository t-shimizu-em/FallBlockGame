using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RetryButtonController : MonoBehaviour
{
    public void OnClickRetryButton()
    {
        SceneManager.LoadScene("GameScene");
    }
}
