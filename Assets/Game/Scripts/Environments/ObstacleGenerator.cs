using GodSpeedGames.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleGenerator : MonoBehaviour
{
    protected static WaitForSeconds Wait;

    public float interval = 1f;

    public bool isRandom = false;

    [Condition("isRandom", true)]
    public float minInterval = 5f;

    [Condition("isRandom", true)]
    public float maxInterval = 6f;

    public GSGObjectPooler ObjectPooler { get; set; }

    Coroutine _routine;

    private bool _generatorStarted = false;
    private bool _produceNow = false;
    public float _deltaTime;

    protected virtual void OnEnable()
    {
        LevelManager.OnGameEvent += LevelEnd;
    }

    protected virtual void OnDisable()
    {
        LevelManager.OnGameEvent -= LevelEnd;
    }

    protected virtual void Start()
    {
        if (isRandom)
            interval = UnityEngine.Random.Range(minInterval, maxInterval);

        InitialisePooler();
    }

    protected virtual void InitialisePooler()
    {
        if (GetComponent<GSGMultipleObjectPooler>() != null)
        {
            ObjectPooler = GetComponent<GSGMultipleObjectPooler>();
        }
        if (GetComponent<GSGSimpleObjectPooler>() != null)
        {
            ObjectPooler = GetComponent<GSGSimpleObjectPooler>();
        }
        if (ObjectPooler == null)
        {
            Debug.LogWarning(this.name + " : no object pooler (simple or multiple) is attached to this object, it won't be able to shoot anything.");
            return;
        }

        _generatorStarted = true;
        _produceNow = true;
    }

    protected virtual void Update()
    {
        if (_generatorStarted && ObjectPooler)
        {

            if (_produceNow)
            {
                GameObject nextGameObject = ObjectPooler.GetPooledGameObject();

                if (nextGameObject != null)
                {
                    if (nextGameObject.GetComponent<GSGPoolableObject>() == null)
                    {
                        throw new Exception(gameObject.name + " is trying to spawn objects that don't have a PoolableObject component.");
                    }

                    nextGameObject.transform.position = transform.position;
                    nextGameObject.GetComponent<IObstacle>().ResetRolling();
                    nextGameObject.gameObject.SetActive(true);
                }

                _produceNow = false;
            }

            _deltaTime += Time.deltaTime;
            if (_deltaTime >= interval)
            {
                _deltaTime = 0f;
                _produceNow = true;
            }
        }
    }

    IEnumerator GenerateObstacle()
    {
        while (true)
        {
            if (ObjectPooler)
            {
                GameObject nextGameObject = ObjectPooler.GetPooledGameObject();

                if (nextGameObject != null)
                {
                    if (nextGameObject.GetComponent<GSGPoolableObject>() == null)
                    {
                        throw new Exception(gameObject.name + " is trying to spawn objects that don't have a PoolableObject component.");
                    }

                    nextGameObject.transform.position = transform.position;
                    nextGameObject.GetComponent<IObstacle>().ResetRolling();
                    nextGameObject.gameObject.SetActive(true);
                }

                yield return Wait;
            }
            else
                yield return null;
        }
    }

    protected virtual void LevelEnd(GSGGameEvent _event)
    {
        if (_event == GSGGameEvent.PlayerDead || _event == GSGGameEvent.LevelFailed || _event == GSGGameEvent.LevelOver)
        {
            _deltaTime = 0f;
            _generatorStarted = false;
        }
        else if (_event == GSGGameEvent.PlayerRespawn || _event == GSGGameEvent.LevelStart)
        {
            _deltaTime = 0f;
            _generatorStarted = true;
            _produceNow = true;
        }
    }
}
