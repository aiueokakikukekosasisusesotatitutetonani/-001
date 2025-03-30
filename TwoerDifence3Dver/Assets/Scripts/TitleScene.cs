using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleScene : MonoBehaviour
{
    public bool isOn = false;

    public GameObject startText;
    public GameObject menu;

    public AudioSource gameStart;
    public AudioSource decision;
    public AudioSource cancel;


    void Start()
    {
        Time.timeScale = 1;  //タイムスケールを１にして時間を進める

        startText.SetActive(true);
        menu.SetActive(false);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!isOn)
            {
                isOn = true;
                gameStart.Play();
            }

            startText.SetActive(false);
            menu.SetActive(true);
            Debug.Log("MouseClick");
        }
    }
    
    public void OnClickEasy()
    {
        Debug.Log("Easyが押された");
        decision.Play();
        DifficultyManager.Instance.difficulty = 0;
        StartCoroutine(ChangeScene());
    }

    public void OnClickNormal()
    {
        Debug.Log("Normalが押された");
        decision.Play();
        DifficultyManager.Instance.difficulty = 1;
        StartCoroutine(ChangeScene());
    }

    public void OnClickHard()
    {
        Debug.Log("Hardが押された");
        decision.Play();
        DifficultyManager.Instance.difficulty = 2;
        StartCoroutine(ChangeScene());
    }

    public void OnClickReturn()
    {
        isOn = false;
        cancel.Play();
        startText.SetActive(true);
        menu.SetActive(false);
    }

    public void OnClickTutorial()
    {
        decision.Play();
        SceneManager.LoadScene("TutorialScene");
    }

    public IEnumerator ChangeScene()
    {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("MainScene");
    }
}
