using Data;
using Google.Protobuf.Protocol;
using Spine;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Monster : Creature
{
    public override ECreatureState CreatureState
    {
        get {  return base.CreatureState; }
        set
        {
            base.CreatureState = value;
            switch (value)
            {
                case ECreatureState.StateIdle:
                    UpdateAITick = 0.5f;
                    break;
                case ECreatureState.StateMove:
                    UpdateAITick = 0.0f;
                    break;
                case ECreatureState.StateSkill:
                    UpdateAITick = 0.1f;
                    break;
                case ECreatureState.StateDie:
                    UpdateAITick = 1.0f;
                    break;
            }
        }
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        ObjectType = EObjectType.Monster;

        return true;
    }
    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        LerpToCellPos(MoveSpeed);
    }
    public override void SetInfo(int templateID)
    {
        base.SetInfo(templateID);
        MoveSpeed = Stat.MoveSpeed;
        Hp = Stat.MaxHp;
        AddHpBar();
        UpdateHpBar();
    }

    protected override void UpdateAnimation()
    {
        base.UpdateAnimation();
    }

    Vector3 _initpos;
    Vector3 _destpos;

    // 탐색 타겟발견시 -> move
    // 순찰
    protected override void UpdateIdle()
    {
        base.UpdateIdle();
    }

    // 순찰이동
    protected override void UpdateMove()
    {
        base.UpdateMove();
    }

    // 스킬범위내에 들어오면 스킬사용
    protected override void UpdateSkill()
    {
        base.UpdateSkill();
    }

    public override void UseSkill(int skillId, int targetId)
    {
        base.UseSkill(skillId, targetId);
        GameObject go = Managers.Object.FindById(targetId);
        BaseObject bo = go.GetComponent<BaseObject>();
        LookAtTarget(bo);
    }


    protected override void UpdateDead()
    {
        base.UpdateDead();
    }





}
