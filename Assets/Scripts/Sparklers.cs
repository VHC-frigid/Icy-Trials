using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sparklers : MonoBehaviour
{
    public ParticleSystem sparkles1;
    public ParticleSystem sparkles2;
    public ParticleSystem sparkles3;
    public ParticleSystem sparkles4;

    void Start()
    {
        sparkles1.Stop();
        sparkles2.Stop();
        sparkles3.Stop();
        sparkles4.Stop();
    }
    void OnTriggerEnter(Collider other)
    {
        sparkles1.Play();
        sparkles2.Play();
        sparkles3.Play();
        sparkles4.Play();
    }
}
