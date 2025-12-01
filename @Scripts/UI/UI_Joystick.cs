using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static Define;

public class UI_Joystick : UI_Base
{
    enum GameObjects
    {
        JoystickBG,
        JoystickCursor,
        AttackButton,
        SkillAButton,
        SkillBButton
    }

    private GameObject _background;
    private GameObject _cursor;
    private float _radius;
    private Vector2 _cursorpos;
    //private RectTransform _backgroundpos;
    private GameObject _attackbutton;
    private Vector2 _touchpos;

    public override bool Init()
    {
        if( base.Init() == false)
            return false;

        BindObjects(typeof(GameObjects));

        _background = GetObject((int)GameObjects.JoystickBG).gameObject;
        _cursor = GetObject((int)GameObjects.JoystickCursor).gameObject;
        _cursorpos = _cursor.GetComponent<RectTransform>().anchoredPosition;
        _attackbutton = GetObject((int)GameObjects.AttackButton).gameObject;
        _radius = _background.GetComponent<RectTransform>().sizeDelta.y / 6;

        gameObject.BindEvent(OnPointerUp, type: EUIEvent.PointerUp);
        gameObject.BindEvent(OnDrag, type: EUIEvent.Drag);

        GetObject((int)GameObjects.AttackButton).BindEvent(OnClickAttack);
        GetObject((int)GameObjects.SkillAButton).BindEvent(OnClickSkillA);
        GetObject((int)GameObjects.SkillBButton).BindEvent(OnClickSkillB);

        GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
        GetComponent<Canvas>().worldCamera = Camera.main;

        return true;
    }

    #region Event
    public void OnPointerDown(PointerEventData eventData)
    {
        _touchpos = Input.mousePosition;
        #region 초기버전
        //Vector2 touchDir = (eventData.position - _touchpos);

        //float moveDist = Mathf.Min(touchDir.magnitude, _radius);
        //Vector2 moveDir = touchDir.normalized;
        //Vector2 newPosition = _cursorpos + moveDir * moveDist;

        //Vector2 worldPos = Camera.main.ScreenToWorldPoint(newPosition);
        //_cursor.transform.position = worldPos;

        //Vector2 touchDir = eventData.position - _cursorpos.anchoredPosition;

        //float moveDist = Mathf.Min(touchDir.magnitude, _radius);
        //Vector2 moveDir = touchDir.normalized;
        //Vector2 newposition = _cursorpos.anchoredPosition + moveDir * moveDist;

        //Vector2 worldPos = Camera.main.ScreenToWorldPoint(newposition);
        //_cursor.transform.position = newposition; 

        //float moveDist = Mathf.Min(touchDir.magnitude, _radius);
        //Vector2 moveDir = touchDir.normalized;
        //Vector2 newPosition = cursorpos + moveDir * moveDist;
        //_cursor.transform.position = newPosition;
        //Managers.Game.MoveDir = moveDir;
        #endregion
        Managers.Game.JoystickState = EJoystickState.PointerDown;
       
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _cursor.transform.localPosition = Vector3.zero;
        Managers.Game.JoystickState = EJoystickState.PointerUp;
    }
    public void OnDrag(PointerEventData eventData)
    {
        Vector2 touchDir = (eventData.position - _cursorpos);

        float moveDist = Mathf.Min(touchDir.magnitude, _radius);
        Vector2 moveDir = touchDir.normalized;
        Vector2 newPosition = _cursorpos + moveDir * moveDist;

        Vector2 worldPos = Camera.main.ScreenToWorldPoint(newPosition);
        _cursor.transform.position = worldPos;

        Managers.Game.MoveDir = GetMoveDir(moveDir);
        Managers.Game.JoystickState = EJoystickState.Drag;
        
    }

    public void OnClickAttack(PointerEventData eventData)
    {
        Debug.Log("OnClickAttack");
       Managers.Game.JoystickState = EJoystickState.OnClickAttack;

    }

    public void OnClickSkillA(PointerEventData eventData)
    {
        Debug.Log("OnClickSkillA");
        Managers.Game.JoystickState = EJoystickState.OnClickSkillA;
    }

    public void OnClickSkillB(PointerEventData eventData)
    {
        Debug.Log("OnClickSkillB");
        Managers.Game.JoystickState = EJoystickState.OnClickSkillB;
    }


    #endregion

    public EMoveDir GetMoveDir(Vector2 dir)
    {
        if (dir == Vector2.zero)
            return EMoveDir.MoveNone;

        float angle = (Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);

        if (107.5 < angle && angle <= 152.5)
        {
            return EMoveDir.MoveUpleft;
        }
        else if (62.5 < angle && angle <= 107.5)
        {
            return EMoveDir.MoveUp;
        }
        else if (17.5 < angle && angle <= 62.5)
        {
            return EMoveDir.MoveUpright;
        }
        else if (-27.5 < angle && angle <= 17.5)
        {
            return EMoveDir.MoveRight;
        }
        else if (-72.5 < angle && angle <= -27.5)
        {
            return EMoveDir.MoveDownright;
        }
        else if (-117.5 < angle && angle <= -72.5)
        {
            return EMoveDir.MoveDown;
        }
        else if (-162.5 < angle && angle <= -117.5)
        {
            return EMoveDir.MoveDownleft;
        }
        else
        {
            return EMoveDir.MoveLeft;
        }
    }


}
