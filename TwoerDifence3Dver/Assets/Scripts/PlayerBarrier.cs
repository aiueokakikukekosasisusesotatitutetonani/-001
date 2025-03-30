using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBarrier : MonoBehaviour
{
    public int barrierHp = 100;   //バリア耐久値
    public PlayerWizard playerWizard;


    private void Start()
    {
        playerWizard = FindAnyObjectByType<PlayerWizard>();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemyMissile") || other.CompareTag("EnemyRock"))
        {

            StartCoroutine(BarrierTakeDamage());
        }
    }

    public IEnumerator BarrierTakeDamage()
    {
        barrierHp -= 100;
        if (barrierHp <= 0)
        {
           playerWizard.isBarrier = false;
            Destroy(gameObject, 0.5f);
        }
        yield return new WaitForSeconds(1f);
    }
}
