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

    public Button okButton; // OK�{�^���̎Q��

    private List<GameObject> tutorialSteps;
    private int currentStep = 0;

    void Start()
    {
        okButton.gameObject.SetActive(true);
        backImage.SetActive(true);

        // �`���[�g���A��UI�����X�g�ɓo�^
        tutorialSteps =
            new List<GameObject> { startText, ui1, ui2, ui3, ui4, ui5, ui6, ui7, ui8, ui9, ui10, ui11, ui12, endText };

        // ���ׂĂ�UI���\���ɂ��āA�ŏ���UI�̂ݕ\��
        foreach (var step in tutorialSteps)
        {
            step.SetActive(false);
        }
        tutorialSteps[currentStep].SetActive(true);

        // wizardImage �̏�����Ԑݒ�
        wizardImage.SetActive(false);
        wizardImage1.SetActive(true);

        // OK�{�^���̃��X�i�[�ݒ�
        okButton.onClick.AddListener(NextStep);
    }

    private void NextStep()
    {
        // ���݂�UI���\��
        tutorialSteps[currentStep].SetActive(false);

        // ����UI��\��
        currentStep++;
        if (currentStep < tutorialSteps.Count)
        {
            backImage.SetActive(false);
            tutorialSteps[currentStep].SetActive(true);

            // currentStep �� 8 �̏ꍇ wizardImage ��\������ wizardImage1 ���\��
            if (currentStep == 8)
            {
                wizardImage.SetActive(true);
                wizardImage1.SetActive(false);
            }
            // currentStep �� 9 �̏ꍇ wizardImage1 ��\������ wizardImage ���\��
            else if (currentStep == 9)
            {
                wizardImage.SetActive(false);
                wizardImage1.SetActive(true);
            }else if(currentStep == 13)
            {
                backImage.SetActive(true); // �Ō��UI�\������ backImage ��\��
            }
        }
        else
        {
            // �Ō��UI���\�����ꂽ��{�^�����\���ɂ���
            okButton.gameObject.SetActive(false);
            title.SetActive(true);
            backImage.SetActive(false); // �Ō��UI�\������ backImage ��\��
        }
    }
}
