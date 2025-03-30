using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;  // UI要素のインポート

public class PlayerTower : MonoBehaviour
{
    public int towerHp = 500;
    public float damageInterval = 1f;
    public GameObject playerTower;
    public GameObject explosion;
    public GameObject missileExplosioin;

    public EAttackerScript eAttackerScript;
    public ETankScript eTankScript;
    private Coroutine damageCoroutine; // ダメージ処理のコルーチンを保持

    // AudioSource
    public AudioSource explosionSound;
    public AudioSource sword;
    public AudioSource punch;

    // UI関連
    public Slider hpSlider;  // HPスライダーへの参照

    void Start()
    {
        explosion.SetActive(false);
        missileExplosioin.SetActive(false);
        if (hpSlider != null)
        {
            hpSlider.maxValue = towerHp;  // スライダーの最大値をタワーの最大HPに設定
            hpSlider.value = towerHp;     // 初期値をタワーの現在のHPに設定
        }
    }

    // トリガー内に入ったときの処理
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("AEnemy"))
        {
            sword.Play();
            eAttackerScript = other.GetComponent<EAttackerScript>();

            if (damageCoroutine == null)
            {
                damageCoroutine = StartCoroutine(ADamageOverTime());
            }
        }
        else if (other.CompareTag("TEnemy"))
        {
            punch.Play();
            eTankScript = other.GetComponent<ETankScript>();

            if (damageCoroutine == null)
            {
                damageCoroutine = StartCoroutine(TDamageOverTime());
            }
        }
        else if (other.CompareTag("EnemyMissile"))
        {
            explosionSound.Play();
            StartCoroutine(EnemyMissileTakeDamage());
        }
        else if (other.CompareTag("EnemyRock"))
        {
            explosionSound.Play();
            StartCoroutine(RockTakeDamage());
        }
    }

    // トリガーから出たときの処理
    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("AEnemy"))
        {
            sword.Stop();

            if (damageCoroutine != null)
            {
                StopCoroutine(damageCoroutine);
                damageCoroutine = null;
            }
        }

        if (other.CompareTag("TEnemy"))
        {
            punch.Stop();

            if (damageCoroutine != null)
            {
                StopCoroutine(damageCoroutine);
                damageCoroutine = null;
            }
        }
    }

    private IEnumerator ADamageOverTime()
    {
        while (towerHp > 0)
        {
            TowerTakeDamage(eAttackerScript.enemyAttackerPower);
            yield return new WaitForSeconds(damageInterval);
        }
        damageCoroutine = null;
    }

    public IEnumerator RockTakeDamage()
    {
        missileExplosioin.SetActive(true);
        towerHp -= 100;
        UpdateHPUI();  // HPスライダーを更新
        if (towerHp <= 0)
        {
            TowerBroken();
        }
        yield return new WaitForSeconds(1f);
        missileExplosioin.SetActive(false);
    }

    private IEnumerator TDamageOverTime()
    {
        while (towerHp > 0)
        {
            TowerTakeDamage(eTankScript.enemyTackPower);
            yield return new WaitForSeconds(damageInterval);
        }
        damageCoroutine = null;
    }

    public IEnumerator EnemyMissileTakeDamage()
    {
        missileExplosioin.SetActive(true);
        towerHp -= 100;
        UpdateHPUI();  // HPスライダーを更新
        if (towerHp <= 0)
        {
            TowerBroken();
        }
        yield return new WaitForSeconds(1f);
        missileExplosioin.SetActive(false);
    }

    public void TowerTakeDamage(int damage)
    {
        towerHp -= damage;
        UpdateHPUI();  // HPスライダーを更新

        if (towerHp <= 0)
        {
            TowerBroken();
        }
    }

    public void TowerBroken()
    {
        explosion.SetActive(true);
        Destroy(playerTower, 1f);
        Debug.Log("YOU LOSE");
    }

    // HPバーUIを更新するメソッド
    private void UpdateHPUI()
    {
        if (hpSlider != null)
        {
            hpSlider.value = towerHp;  // スライダーの値を更新
        }
    }
}
