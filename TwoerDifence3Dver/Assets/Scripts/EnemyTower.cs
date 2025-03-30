using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;  // UI要素のインポート

public class EnemyTower : MonoBehaviour
{
    public int towerHp = 500;
    public float damageInterval = 1f;
    public GameObject enemyTower;
    public GameObject explosion;
    public GameObject missileExplosion;

    public PAttackerScript pAttackerScript;
    public PTankScript pTankScript;
    private Coroutine damageCoroutine; // ダメージ処理のコルーチンを保持

    // AudioSource
    public AudioSource explosionSound;
    public AudioSource sword;
    public AudioSource punch;

    // UI関連
    public Slider hpSlider;  // HPスライダーへの参照

    // 初期化と爆発の非表示
    public void Start()
    {
        explosion.SetActive(false);
        missileExplosion.SetActive(false);
        if (hpSlider != null)
        {
            hpSlider.maxValue = towerHp;  // スライダーの最大値をタワーの最大HPに設定
            hpSlider.value = towerHp;     // 初期値をタワーの現在のHPに設定
        }
    }

    // トリガー内に入ったときの処理
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("APlayer"))
        {
            sword.Play();
            pAttackerScript = other.GetComponent<PAttackerScript>();

            if (damageCoroutine == null)
            {
                damageCoroutine = StartCoroutine(ADamageOverTime());
            }
        }
        else if (other.CompareTag("TPlayer"))
        {
            punch.Play();
            pTankScript = other.GetComponent<PTankScript>();

            if (damageCoroutine == null)
            {
                damageCoroutine = StartCoroutine(TDamageOverTime());
            }
        }
        else if (other.CompareTag("PlayerMissile"))
        {
            explosionSound.Play();
            StartCoroutine(MissileTakeDamage());
        }
        else if (other.CompareTag("PlayerRock"))
        {
            explosionSound.Play();
            StartCoroutine(RockTakeDamage());
        }
    }

    // トリガーから出たときの処理
    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("APlayer"))
        {
            sword.Stop();

            if (damageCoroutine != null)
            {
                StopCoroutine(damageCoroutine);
                damageCoroutine = null;
            }
        }

        if (other.CompareTag("TPlayer"))
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
            TowerTakeDamage(pAttackerScript.playerAttackerPower);
            yield return new WaitForSeconds(damageInterval);
        }
        damageCoroutine = null;
    }

    private IEnumerator TDamageOverTime()
    {
        while (towerHp > 0)
        {
            TowerTakeDamage(pTankScript.playerTackPower);
            yield return new WaitForSeconds(damageInterval);
        }
        damageCoroutine = null;
    }

    public IEnumerator RockTakeDamage()
    {
        missileExplosion.SetActive(true);
        towerHp -= 100;
        UpdateHPUI();  // HPスライダーを更新
        if (towerHp <= 0)
        {
            TowerBroken();
        }
        yield return new WaitForSeconds(1f);
        missileExplosion.SetActive(false);
    }

    public IEnumerator MissileTakeDamage()
    {
        missileExplosion.SetActive(true);
        towerHp -= 100;
        UpdateHPUI();  // HPスライダーを更新
        if (towerHp <= 0)
        {
            TowerBroken();
        }
        yield return new WaitForSeconds(1f);
        missileExplosion.SetActive(false);
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
        Destroy(enemyTower, 1f);
        Debug.Log("YOU WIN");
    }

    // HPバーUIを更新するメソッド
    private void UpdateHPUI()
    {
        if (hpSlider != null)
        {
            hpSlider.value = towerHp;  // スライダーの値を更新
        }
    }

    public void WaitTime()
    {
        TowerTakeDamage(pTankScript.playerTackPower);
    }
}
