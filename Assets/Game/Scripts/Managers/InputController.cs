using GodSpeedGames.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YipliFMDriverCommunication;


public enum Move
{
    None,
    Right,
    Left,
    Jump,
    Bend,
    Running,
    StopRunning,
    KeyPause,
}

public enum MatKey
{
    None,
    KeyLeft,
    KeyRight,
    KeyEnter
}

public class InputController : PersistentSingleton<InputController>
{
    public bool buildForMobile = false;

    public static System.Action<Move> OnSetMove;
    public static System.Action<MatKey> OnGotKey;

    private Move _currentMove = Move.None;
    private bool isRunning = false;

    [SerializeField] protected bool isInputActive = false;

    private string _lastMatData = "";

    protected virtual void OnEnable()
    {
        UnityFitmatBridge.OnGotActionFromBridge += OnGotActionFromBridge;
        //SwipeControl.OnSwipe += OnFingerSwipe;
        GSGButtons.OnGotActionFromButton += GotActionFromButton;
    }

    protected virtual void OnDisable()
    {
        UnityFitmatBridge.OnGotActionFromBridge -= OnGotActionFromBridge;
        //SwipeControl.OnSwipe -= OnFingerSwipe;
        GSGButtons.OnGotActionFromButton -= GotActionFromButton;
    }

    public virtual void EnableInput()
    {
        isInputActive = true;
    }

    public virtual void DisableInput()
    {
        isInputActive = false;
    }

    public virtual void PlayMove(Move move)
    {
        OnSetMove?.Invoke(move);
    }

#if UNITY_EDITOR
    protected virtual void Update()
    {
        if (buildForMobile)
            return;

        if (Input.GetKeyDown(KeyCode.RightArrow))
            SetKeyMove(MatKey.KeyRight);

        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            SetKeyMove(MatKey.KeyLeft);

        // no need for Keyboard return mapping. It is mapped by default
        //  else if (Input.GetKeyDown(KeyCode.Return))
        //SetKeyMove(MatKey.KeyEnter);

        if (Input.GetAxis("Vertical") > 0)
        {
            isRunning = true;
            SetMove(Move.Running);
            if (PlayerSession.Instance != null)
                PlayerSession.Instance.AddPlayerAction(YipliPlayerActivity.PlayerActions.RUNNING);
        }
        else if (isRunning)
        {
            isRunning = false;
            SetMove(Move.StopRunning);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            SetMove(Move.Jump);
            if (PlayerSession.Instance != null)
                PlayerSession.Instance.AddPlayerAction(YipliPlayerActivity.PlayerActions.JUMP);
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
            SetMove(Move.KeyPause);

    }
#endif

    protected virtual void GotActionFromButton(GSGButtons.ButtonType button)
    {
        if (button == GSGButtons.ButtonType.Run)
        {
            isRunning = true;
            SetMove(Move.Running);
            if (PlayerSession.Instance != null)
                PlayerSession.Instance.AddPlayerAction(YipliPlayerActivity.PlayerActions.RUNNING);
        }
        else if (isRunning)
        {
            isRunning = false;
            SetMove(Move.StopRunning);
        }

        if (button == GSGButtons.ButtonType.Jump)
        {
            SetMove(Move.Jump);
            if (PlayerSession.Instance != null)
                PlayerSession.Instance.AddPlayerAction(YipliPlayerActivity.PlayerActions.JUMP);
        }
    }

    protected virtual void OnFingerSwipe(SwipeControl.SwipeDirection direction)
    {
        switch (direction)
        {
            case SwipeControl.SwipeDirection.Null:
                SetMove(Move.None);
                break;

            case SwipeControl.SwipeDirection.Jump:
                SetMove(Move.Jump);
                break;

            case SwipeControl.SwipeDirection.Right:
                SetMove(Move.Right);
                break;

            case SwipeControl.SwipeDirection.Left:
                SetMove(Move.Left);
                break;

            default:
                break;
        }
    }

    protected virtual void OnGotActionFromBridge(string data)
    {
        try {
            //Debug.Log("Current Cluster Id is : " + YipliHelper.GetGameClusterId());
        }
        catch(Exception exp)
        {
            Debug.Log("Failed to get GameId : " + exp.Message);
        }
        if (data.Equals(ActionAndGameInfoManager.getActionIDFromActionName(YipliPlayerActivity.PlayerActions.JUMP), StringComparison.OrdinalIgnoreCase))
        {
            SetMove(Move.Jump);

            int jump = PlayerData.JumpStep;
            PlayerData.JumpStep = jump + 1;
            if (PlayerSession.Instance != null)
                PlayerSession.Instance.AddPlayerAction(YipliPlayerActivity.PlayerActions.JUMP);
        }

        else if (data.Equals(ActionAndGameInfoManager.getActionIDFromActionName(YipliPlayerActivity.PlayerActions.RUNNING), StringComparison.OrdinalIgnoreCase))
        {
            SetMove(Move.Running);
            if (PlayerSession.Instance != null)
                PlayerSession.Instance.AddPlayerAction(YipliPlayerActivity.PlayerActions.RUNNING);
        }
        else if (data.Equals(ActionAndGameInfoManager.getActionIDFromActionName(YipliPlayerActivity.PlayerActions.RUNNINGSTOPPED), StringComparison.OrdinalIgnoreCase))
        {
            SetMove(Move.StopRunning);
        }
        else if (data.Equals(ActionAndGameInfoManager.getActionIDFromActionName(YipliPlayerActivity.PlayerActions.PAUSE), StringComparison.OrdinalIgnoreCase))//&& _lastMatData != "Pause")
        {
            Debug.Log("Recieved Pause in InputController. Processing it.");
            SetMove(Move.KeyPause);
        }

        else if (data.Equals(ActionAndGameInfoManager.getActionIDFromActionName(YipliPlayerActivity.PlayerActions.LEFT), StringComparison.OrdinalIgnoreCase))
            SetKeyMove(MatKey.KeyLeft);

        else if (data.Equals(ActionAndGameInfoManager.getActionIDFromActionName(YipliPlayerActivity.PlayerActions.RIGHT), StringComparison.OrdinalIgnoreCase))
            SetKeyMove(MatKey.KeyRight);

        else if (data.Equals(ActionAndGameInfoManager.getActionIDFromActionName(YipliPlayerActivity.PlayerActions.ENTER), StringComparison.OrdinalIgnoreCase))
            SetKeyMove(MatKey.KeyEnter);

        _lastMatData = data;
    }

    protected virtual void SetMove(Move move)
    {
        if (isInputActive)
        {
            _currentMove = move;
            OnSetMove?.Invoke(_currentMove);
        }
    }

    protected virtual void SetKeyMove(MatKey move)
    {
        OnGotKey?.Invoke(move);
    }
}
