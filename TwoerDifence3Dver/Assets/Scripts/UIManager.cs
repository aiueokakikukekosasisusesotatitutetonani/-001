using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Probability probability;

    [SerializeField] private float fadeDuration = 1.0f; //フェードイン時間
    public Image image0; // フェードインさせたいimage
    public Image image1; // フェードインさせたいimage

    private void Start()
    {
        StartCoroutine(FadeIn());
    }

    public IEnumerator FadeIn()
    {
        // 初期状態を透明に設定
        Color color = image0.color;
        color.a = 0;
        image0.color = color;

        float elapsedTime = 0;

        while (elapsedTime < fadeDuration)
        {
            // 経過時間に応じて透明度を増加
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Clamp01(elapsedTime / fadeDuration);
            image0.color = color;

            yield return null; //次のフレームまで待つ
        }

        //完全に不透明
        color.a = 1;
        image0.color = color;

        yield return new WaitForSeconds(1f);

        while (elapsedTime < fadeDuration)
        {
            // 経過時間に応じて不透明度を増加
            elapsedTime -= Time.deltaTime;
            color.a = Mathf.Clamp01(elapsedTime / fadeDuration);
            image0.color = color;

            yield return null; //次のフレームまで待つ
        }

        //完全に透明
        color.a = 0;
        image0.color = color;
    }

    public IEnumerator CimageFadeIn()
    {
        int radValue = probability.rad;

        if (radValue >= 1 && radValue <= 100)
        {
            // 初期状態を透明に設定
            Color color = image1.color;
            color.a = 0;
            image1.color = color;

            float elapsedTime = 0;

            while (elapsedTime < fadeDuration)
            {
                // 経過時間に応じて透明度を増加
                elapsedTime += Time.deltaTime;
                color.a = Mathf.Clamp01(elapsedTime / fadeDuration);
                image1.color = color;

                yield return null; //次のフレームまで待つ
            }

            //完全に不透明
            color.a = 1;
            image1.color = color;

            while (elapsedTime < fadeDuration)
            {
                // 経過時間に応じて透明度を増加
                elapsedTime += Time.deltaTime;
                color.a = Mathf.Clamp01(elapsedTime / fadeDuration);
                image1.color = color;

                yield return null; //次のフレームまで待つ
            }
        }
        else
        {
            Debug.Log(radValue);
        }

    }
}
