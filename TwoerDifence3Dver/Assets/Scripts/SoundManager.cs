using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource sword;
    public AudioSource punch;

    void Start()
    {
    }

    public void SwordPlay()
    {
        sword.Play();
    }
    public void SwordStop()
    {
        sword.Stop();
    }
    public void PunchPlay()
    {
        punch.Play();
    }
    public void PunchStop()
    {
        punch.Stop();
    }
}
