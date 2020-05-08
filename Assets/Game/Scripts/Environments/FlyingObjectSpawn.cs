using GodSpeedGames.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingObjectSpawn : ObstacleGenerator
{

    protected override void Start()
    {
        base.Start();
        Wait = new WaitForSeconds(interval);
        StartCoroutine("SpwanFlyingObjects");
    }

    IEnumerator SpwanFlyingObjects()
    {
        while (true)
        {
            yield return Wait;

            GameObject nextGameObject = ObjectPooler.GetPooledGameObject();

            if (nextGameObject != null)
            {
                if (nextGameObject.GetComponent<GSGPoolableObject>() == null)
                {
                    throw new Exception(gameObject.name + " is trying to spawn objects that don't have a PoolableObject component.");
                }

                nextGameObject.GetComponent<FlyingObject>().SetDirection(transform.position);
                nextGameObject.gameObject.SetActive(true);
            }
        }
    }

    protected override void LevelEnd(GSGGameEvent _event) { }
    protected override void Update() { }
}
