using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Projectile : BaseObject
{
    private Creature _target;
    Vector3 StartPosition;
    Vector3 TargetPosition;
    float elapsedTime;
    float length;
    float time;

    public override bool Init()
    {
        if( base.Init() == false)
            return false;

        ObjectType = EObjectType.Projectile;
        CreatureState = ECreatureState.StateMove;

        return true;
    }

    public override PositionInfo PosInfo
    {
        get { return _positionInfo; }

        set
        {
            _positionInfo = value;

            Vector2Int cellPos = new Vector2Int(value.Posx, value.Posy);
            SetCellPos(cellPos, true);
        }

    }

    public void SetInfo(int targetId)
    {
        _target = Managers.Object.FindbyIdCreatrue(targetId);
        //StartPosition = transform.position + Vector3.up;
        //TargetPosition = _target.transform.position + Vector3.up;
    }

    protected override void UpdateAnimation()
    {

    }

    protected override void Update()
    {
        if (_target == null)
        {
            Managers.Object.Remove(Id, false);
            return;
        }

        if (StartPosition == Vector3.zero)
        {
            StartPosition = transform.position + Vector3.up;
            TargetPosition = _target.transform.position + Vector3.up;
            length = Vector3.Distance(TargetPosition, StartPosition);
            time = length / PROJECTILE_MOVE_SPEED;
        } 

        // 도착시간 전까지
        if (elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;

            float normalizedTime = elapsedTime / time;
            transform.position = Vector3.Lerp(StartPosition, TargetPosition, normalizedTime);
            return;
        }
        // 도착시간 지난경우
        else if (elapsedTime >= time)
        {
            elapsedTime = 0;
            StartPosition = Vector3.zero;
            TargetPosition = Vector3.zero;
            Managers.Object.Remove(Id, false);
            return;
        }

        //Vector3 destPos = _target.transform.position + Vector3.up;
        //Vector3 dir = destPos - (transform.position + Vector3.up);


        //float moveDist = PROJECTILE_MOVE_SPEED * Time.deltaTime;
        //if (dir.magnitude < moveDist)
        //{
        //    transform.position = destPos;
        //    Managers.Object.Remove(Id);
        //    return;
        //}

        //transform.position += dir.normalized * moveDist;
    }
}
