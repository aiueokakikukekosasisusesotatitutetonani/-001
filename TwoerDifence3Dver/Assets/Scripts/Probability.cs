using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Probability : MonoBehaviour
{
    public int rad { get; private set; }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void StartSelection()
    {
        rad = Random.Range(1, 101);

        if (rad >= 1 && rad <= 48)
        {
            Debug.Log("A");
        }
        else if (rad >= 49 && rad <= 80)
        {
            Debug.Log("B");
        }
        else if (rad >= 81 && rad <= 100)
        {
            Debug.Log("C");
        }
    }
}