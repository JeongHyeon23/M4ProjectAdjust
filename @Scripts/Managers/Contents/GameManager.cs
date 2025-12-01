using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class GameManager
{
    #region MyHero
    private EMoveDir _moveDir;
    private EJoystickState _joystickState;
    public EMoveDir MoveDir
    {
        get { return _moveDir; }
        set
        {
            _moveDir = value;
            OnJoystickStateChanged?.Invoke(_joystickState, _moveDir);
        }
    }
    public EJoystickState JoystickState
    {
        get { return _joystickState; }
        set
        {
            _joystickState = value;
            OnJoystickStateChanged?.Invoke(_joystickState, _moveDir);
        }
    }
    #endregion

    #region Action
    public event Action<EJoystickState, EMoveDir> OnJoystickStateChanged;
    #endregion
}
