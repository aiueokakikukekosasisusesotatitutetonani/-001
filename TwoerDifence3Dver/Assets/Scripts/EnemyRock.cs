using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRock : MonoBehaviour
{
    public float speed = 10f; // 移動速度

    void Update()
    {
        // 右下方向 (X軸正方向 & Y軸負方向) に移動
        transform.position += new Vector3(-1f, -0.13f, 0f) * speed * Time.deltaTime;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerTower"))
        {
            gameObject.transform.Translate(500f, 0f, 0f);
            Destroy(gameObject, 0.5f);
        }

        if (other.CompareTag("PlayerBarrier"))
        {
            gameObject.transform.Translate(500f, 0f, 0f);
            Destroy(gameObject);
        }
    }
}
