using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyNotSpawn : MonoBehaviour
{
    public bool notSpawn = false; //プレイヤースポーンのフラグ

    public void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("APlayer") || other.CompareTag("TPlayer"))
        {
            notSpawn = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("APlayer") || other.CompareTag("TPlayer"))
        {
            notSpawn = false;
        }
    }
}
