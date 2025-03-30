using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNotSpawn : MonoBehaviour
{
    public bool notSpawn = false; //プレイヤースポーンのフラグ

    public void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("AEnemy") || other.CompareTag("TEnemy"))
        {
            notSpawn = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("AEnemy") || other.CompareTag("TEnemy"))
        {
            notSpawn = false;
        }
    }
}
