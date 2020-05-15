using Cinemachine;
using Firebase.Database;
using GodSpeedGames.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public enum Difficulty
{
    None,
    Easy,
    Medium,
    Hard
}

public class LevelManager : Singleton<LevelManager>
{
    static bool level_fresh_start = true;

    static WaitForSeconds ONE_SEC = new WaitForSeconds(1);

    public static System.Action<GSGGameEvent> OnGameEvent;

    public float RespawnDelay = .5f;

    public GameDataTracker tracker;
    public BlockData blockData;
    public LevelData levelData;

    public GameObject PlayerPrefab;

    [Header("Testing")]
    public bool testBlockFuntions;
    public GameObject[] testBlocks;

    public bool testLevelDifficulty;
    [Condition("testLevelDifficulty", true)]
    public LevelDifficulty testLevelDifficultyInfo;

    public bool testSpecificLevel;
    [Condition("testSpecificLevel", true)]
    public int specificLevelIndex;

    public CheckPoint DebugSpawn;

    [Header("Camera")]
    public CinemachineVirtualCamera[] vCams;

    [Header("Cutscene")]
    public CutScene cutscene;

    [ReadOnly]
    public CheckPoint CurrentCheckPoint;

    public List<CheckPoint> Checkpoints { get; protected set; }
    public Character Player { get; protected set; }

    private List<GameObject> selectedBlocks;
    [ReadOnly] public LevelDifficulty _currentLevelDifficulty;

    private float _deltaTime;
    private bool _startTimer = false;
    private bool _gameOver = false;

    public PlayerGameData playerGameData;

    protected override void Awake()
    {
        base.Awake();

        StopCoroutine("RunGameTimer");

        InstantiatePlayableCharacters();
    }

    private void OnDestroy()
    {
        Debug.Log("OnDestroy");
    }

    protected virtual void InstantiatePlayableCharacters()
    {
        if (PlayerPrefab == null) { return; }

        GameObject newPlayer = Instantiate<GameObject>(PlayerPrefab, Vector3.zero, Quaternion.identity);
        Player = newPlayer.GetComponent<Character>();
    }

 
    
    public virtual IEnumerator Start()
    {
        yield return new WaitForSeconds(0.1f);

        if (PlayerSession.Instance != null)
            PlayerSession.Instance.StartSPSession("joyfuljumps");

        if (Player != null)
        {
            Initialization();

            // we handle the spawn of the character(s)
            SpawnCharacter();

            Player.ConditionState.ChangeState(CharacterStates.CharacterConditions.Normal);
            //InputController.Instance.EnableInput();

            if (!testBlockFuntions && level_fresh_start)
            {
                StartTimer();
            }

            OnGameEvent?.Invoke(GSGGameEvent.LevelStart);

            if (UnityFitmatBridge.Instance != null)
                UnityFitmatBridge.Instance.EnableGameInput();
        }
    }

    bool Tutorial
    {
        get
        {
            return (playerGameData.GetCurrentLevel() < blockData.tutorialBlocks.Length );
        }
    }

    public void ExcerciseCutScene()
    {
        //  InputController.Instance.DisableInput();
        Player.PlayStartDemoCutsceneAnimation();
        cutscene.PlayTimelineAnimation(Player.gameObject.transform.position);
    }

    public void StartTimer()
    {
        StopCoroutine("RunGameTimer");
        if (!testBlockFuntions)
            StartCoroutine("RunGameTimer");
    }

    IEnumerator RunGameTimer()
    {
        yield return ONE_SEC;

        if (level_fresh_start)
            yield return ONE_SEC;

        OnGameEvent?.Invoke(GSGGameEvent.StartTimer);

        InputController.Instance.EnableInput();

        level_fresh_start = true;

        while (!_gameOver)
        {
            if (tracker.timeElapsed >= tracker.totalTime && !_gameOver)
            {
                tracker.timeElapsed = tracker.totalTime;
                UiManager.Instance.LoadPopup(UiManager.Popup.LevelFailedScreen);
                _startTimer = false;
                _gameOver = true;
                yield break;
            }

            if (!_gameOver)
            {
                tracker.timeElapsed = tracker.timeElapsed + 1;
                yield return ONE_SEC;
            }
        }
    }

    protected virtual void Initialization()
    {

        EnableVirtualCameras();
        SetupLevelBlocks();

        // we store all the checkpoints present in the level, ordered by their x value
        Checkpoints = FindObjectsOfType<CheckPoint>().OrderBy(o => o.transform.position.z).ToList();

        // we assign the first checkpoint
        CurrentCheckPoint = Checkpoints.Count > 0 ? Checkpoints[0] : null;

        _startTimer = false;
    }

    protected virtual void SetupLevelBlocks()
    {
        if (selectedBlocks == null)
            selectedBlocks = new List<GameObject>();
        selectedBlocks.Clear();

        if (testSpecificLevel)
            playerGameData.SetCurrentLevel(specificLevelIndex);

        // add starting block
        selectedBlocks.Add(blockData.startBlocks[UnityEngine.Random.Range(0, blockData.startBlocks.Length)]);

        if (testBlockFuntions)
        {
            for (int i = 0; i < testBlocks.Length; i++)
                selectedBlocks.Add(testBlocks[i]);
        }
        else if (playerGameData.GetCurrentLevel() < blockData.tutorialBlocks.Length)
        {
            selectedBlocks.Add(blockData.tutorialBlocks[playerGameData.GetCurrentLevel()]);
        }
        else
        {
            if (testLevelDifficulty)
                _currentLevelDifficulty = testLevelDifficultyInfo;
            else
            {
                int currentLevel = playerGameData.GetCurrentLevel();

                if (currentLevel >= ( levelData.levelInfo.Length - 1 ))
                    currentLevel = levelData.levelInfo.Length - 1;

                // getting actuall dynamic level index by subtracting tutorial levels
                currentLevel -= blockData.tutorialBlocks.Length;

                _currentLevelDifficulty = levelData.levelInfo[currentLevel];
            }

            GetBlockLength(_currentLevelDifficulty.lenght, out float easyLength, out float normalLength, out float hardLength);

            float _generatedLength = SelectBlocks(blockData.easyBlocks, easyLength);

            if (_generatedLength < _currentLevelDifficulty.lenght)
                _generatedLength += SelectBlocks(blockData.mediumBlocks, normalLength);

            if (_generatedLength < _currentLevelDifficulty.lenght)
                _generatedLength += SelectBlocks(blockData.hardBlocks, hardLength);
        }

        // add end block
        selectedBlocks.Add(blockData.endBlocks[UnityEngine.Random.Range(0, blockData.endBlocks.Length)]);

        GameObject parent = new GameObject();
        parent.name = "World";
        parent.transform.position = Vector3.zero;
        parent.transform.localRotation = Quaternion.identity;

        float nextZPosition = 0f;

        int totalTime = 0;

        for (int i = 0; i < selectedBlocks.Count; i++)
        {
            GameObject _temp = Instantiate<GameObject>(selectedBlocks[i]);
            Block _block = _temp.GetComponent<Block>();
            totalTime += (int) _block.targetTime;

            Vector3 position = Vector3.zero;
            if (nextZPosition > 0)
                position.z = nextZPosition + Mathf.Abs(_block.StartingZPposition);
            else
                position.z = nextZPosition;

            _temp.transform.position = position;

            nextZPosition = _block.EndingZPposition;

            _temp.transform.SetParent(parent.transform);
        }

        // adjusting time
        if (_currentLevelDifficulty.difficulty == Difficulty.Easy)
            totalTime = (int) ( totalTime * 2f );
        else if (_currentLevelDifficulty.difficulty == Difficulty.Medium)
            totalTime = (int) ( totalTime * 1.5f );

        tracker.totalTime = totalTime;

        // adding boundary
        nextZPosition += 10f;  // adding buffer zone
        GameObject boundary = new GameObject();
        boundary.name = "Boundary";
        boundary.transform.SetParent(parent.transform);
        boundary.transform.position = ( Vector3.forward * nextZPosition * .5f ) + Vector3.down * 15f;
        BoxCollider collider = boundary.AddComponent<BoxCollider>();
        collider.size = new Vector3(10, 2f, nextZPosition);
        collider.isTrigger = true;
        boundary.AddComponent<KillPlayerOnTouch>().isFloor = true; ;
        Boundary _boundaryScript = boundary.AddComponent<Boundary>();
        _boundaryScript.ignoredObjetLayer = ( 1 << LayerMask.NameToLayer("Player") );
    }

    protected virtual float SelectBlocks(BlockInfo[] info, float targetLength)
    {
        float _targetLenght = targetLength;
        float _generatedLength = 0f;
        while (_generatedLength <= ( targetLength - 30 ))
        {
            // if (_generatedLength <= (targetLength - 30))
            {
                float length = GetBlockPrebab(info, _targetLenght);
                _generatedLength += length;
                _targetLenght = targetLength - _generatedLength;
            }
        }

        return _generatedLength;
    }

    float GetBlockPrebab(BlockInfo[] info, float targetLength)
    {
        BlockInfo _blockInfo = info[UnityEngine.Random.Range(0, info.Length - 1)];

        selectedBlocks.Add(_blockInfo.blockPrefab);
        return _blockInfo.length;
    }

    void GetBlockLength(float totalLength, out float easy, out float normal, out float hard)
    {
        if (_currentLevelDifficulty.difficulty == Difficulty.Easy)
        {
            easy = totalLength * .6f;
            normal = totalLength * .3f;
            hard = totalLength * .1f;
            return;
        }
        else if (_currentLevelDifficulty.difficulty == Difficulty.Medium)
        {
            easy = totalLength * .3f;
            normal = totalLength * .5f;
            hard = totalLength * .2f;
            return;
        }

        easy = totalLength * .3f;
        normal = totalLength * .2f;
        hard = totalLength * .5f;
    }

    /// <summary>
    /// Kills the player.
    /// </summary>
    public virtual void KillPlayer(Character player, bool isFloor)
    {
        Health characterHealth = player.GetComponent<Health>();
        if (characterHealth == null)
        {
            return;
        }
        else
        {
            if (player.ConditionState.CurrentState == CharacterStates.CharacterConditions.Dead)
                return;

            if (isFloor)
            {
                int fall = PlayerData.FallCount;
                PlayerData.FallCount = fall + 1;
            }

            // we kill the character
            OnGameEvent?.Invoke(GSGGameEvent.PlayerDead);
            characterHealth.Kill(isFloor);

            StopCoroutine("RunGameTimer");
            StopCoroutine("SoloModeRestart");
            StartCoroutine("SoloModeRestart");
        }
    }

    public virtual void LevelSuccess(Character player)
    {
        Health characterHealth = player.GetComponent<Health>();
        if (characterHealth == null)
        {
            return;
        }
        else
        {
            // we won
            InputController.Instance.DisableInput();
            cutscene.PlayTimelineAnimation(player.gameObject.transform.position);
            characterHealth.WinLevel();

            OnGameEvent?.Invoke(GSGGameEvent.LevelSuccess);

            StopCoroutine("RunGameTimer");

            GameManager.Instance.CalculateRewardPoint(_currentLevelDifficulty, tracker);

            int totalRewardPoint = playerGameData.GetTotalScore() + tracker.totalPointsEarned;
            playerGameData.SetTotalScore(totalRewardPoint);
            playerGameData.SetCurrentLevel(playerGameData.GetCurrentLevel() + 1);
            if (PlayerSession.Instance != null)
            {
                if (YipliHelper.checkInternetConnection())
                {
                    Dictionary<string, string> gameData = new Dictionary<string, string>();
                    gameData.Add("reward-coins", playerGameData.GetTotalScore().ToString());
                    gameData.Add("current-level", playerGameData.GetCurrentLevel().ToString());
                    PlayerSession.Instance.UpdateGameData(gameData);
                }
                PlayerSession.Instance.StoreSPSession(tracker.totalPointsEarned);
            }

            StopCoroutine("PlayEndCutScene");
            StartCoroutine("PlayEndCutScene");
        }
    }

    IEnumerator EndLevel()
    {
        yield return new WaitForSeconds(4f);
        OnGameEvent?.Invoke(GSGGameEvent.LevelOver);
    }

    IEnumerator PlayEndCutScene()
    {
        yield return new WaitForSeconds(2f);

        UiManager.Instance.LoadUI(UiManager.SCREEN.GameOverScreen);

        yield return new WaitForSeconds(2f);

        Player.ConditionState.ChangeState(CharacterStates.CharacterConditions.Normal);
        cutscene.PlayEndCutSceneJump();

        yield return new WaitForSeconds(1f);

        level_fresh_start = false;

        UiManager.Instance.RemoveAllScreen();
        LoadingManager.Instance.LoadScreen(Constant.GAME_SCENE_NAME, true);
    }

    public virtual void SetCurrentCheckpoint(CheckPoint newCheckPoint)
    {
        CurrentCheckPoint = newCheckPoint;
    }

    protected virtual void EnableVirtualCameras()
    {
        for (int i = 0; i < vCams.Length; i++)
        {
            vCams[i].Follow = Player.transform;
            vCams[i].LookAt = Player.transform;
        }
    }

    protected virtual void DisableVirtualCameras()
    {
        for (int i = 0; i < vCams.Length; i++)
        {
            vCams[i].Follow = null;
            vCams[i].LookAt = null;
        }
    }

    /// <summary>
    /// Coroutine that kills the player, stops the camera, resets the points.
    /// </summary>
    /// <returns>The player co.</returns>
    protected virtual IEnumerator SoloModeRestart()
    {

        //TODO: check for maximum lives and game over

        InputController.Instance.DisableInput();

        yield return new WaitForSeconds(RespawnDelay);

        DisableVirtualCameras();
        LoadingManager.Instance.ShowFadeInFadeOut();

        // make sure that the object is really dead after a while
        Player.CollisionsOff(false);
        Player.ConditionState.ChangeState(CharacterStates.CharacterConditions.Dead);

        yield return new WaitForSeconds(RespawnDelay * .5f);

        if (CurrentCheckPoint != null)
        {
            CurrentCheckPoint.SpawnPlayer(Player);
        }

        EnableVirtualCameras();
        OnGameEvent?.Invoke(GSGGameEvent.PlayerRespawn);
        if (!testBlockFuntions)
            StartCoroutine("RunGameTimer");

        yield return new WaitForSeconds(1.5f);
        // InputController.Instance.EnableInput();
    }

    protected virtual void SpawnCharacter()
    {
#if UNITY_EDITOR
        if (DebugSpawn != null)
        {
            DebugSpawn.SpawnPlayer(Player);
            return;
        }
        else
        {
            if (CurrentCheckPoint != null)
            {
                CurrentCheckPoint.SpawnPlayer(Player);
                return;
            }
        }
#else
		if (CurrentCheckPoint != null)
        {
            CurrentCheckPoint.SpawnPlayer(Player);
            return;
        }
#endif
    }
}
