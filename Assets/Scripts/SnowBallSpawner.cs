using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowBallSpawner : MonoBehaviour
{
    public Transform SpawnPoint;
    public GameObject SnowBallPrefab;

    private void Start()
    {
        InvokeRepeating("SpawnSnowBall", 2f, 5f);
    }

    void SpawnSnowBall()
    {
        GameObject aSnowBall = Instantiate(SnowBallPrefab, SpawnPoint.position, Quaternion.identity);
    }
}
