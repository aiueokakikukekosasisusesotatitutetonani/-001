using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public GameObject startText;
    public GameObject endText;
    public GameObject backImage;
    public GameObject wizardImage;
    public GameObject wizardImage1;
    public GameObject title;

    public GameObject ui1;
    public GameObject ui2;
    public GameObject ui3;
    public GameObject ui4;
    public GameObject ui5;
    public GameObject ui6;
    public GameObject ui7;
    public GameObject ui8;
    public GameObject ui9;
    public GameObject ui10;
    public GameObject ui11;
    public GameObject ui12;

    public Button okButton; // OKボタンの参照

    private List<GameObject> tutorialSteps;
    private int currentStep = 0;

    void Start()
    {
        okButton.gameObject.SetActive(true);
        backImage.SetActive(true);

        // チュートリアルUIをリストに登録
        tutorialSteps =
            new List<GameObject> { startText, ui1, ui2, ui3, ui4, ui5, ui6, ui7, ui8, ui9, ui10, ui11, ui12, endText };

        // すべてのUIを非表示にして、最初のUIのみ表示
        foreach (var step in tutorialSteps)
        {
            step.SetActive(false);
        }
        tutorialSteps[currentStep].SetActive(true);

        // wizardImage の初期状態設定
        wizardImage.SetActive(false);
        wizardImage1.SetActive(true);

        // OKボタンのリスナー設定
        okButton.onClick.AddListener(NextStep);
    }

    private void NextStep()
    {
        // 現在のUIを非表示
        tutorialSteps[currentStep].SetActive(false);

        // 次のUIを表示
        currentStep++;
        if (currentStep < tutorialSteps.Count)
        {
            backImage.SetActive(false);
            tutorialSteps[currentStep].SetActive(true);

            // currentStep が 8 の場合 wizardImage を表示して wizardImage1 を非表示
            if (currentStep == 8)
            {
                wizardImage.SetActive(true);
                wizardImage1.SetActive(false);
            }
            // currentStep が 9 の場合 wizardImage1 を表示して wizardImage を非表示
            else if (currentStep == 9)
            {
                wizardImage.SetActive(false);
                wizardImage1.SetActive(true);
            }else if(currentStep == 13)
            {
                backImage.SetActive(true); // 最後のUI表示時に backImage を表示
            }
        }
        else
        {
            // 最後のUIが表示されたらボタンを非表示にする
            okButton.gameObject.SetActive(false);
            title.SetActive(true);
            backImage.SetActive(false); // 最後のUI表示時に backImage を表示
        }
    }
}
