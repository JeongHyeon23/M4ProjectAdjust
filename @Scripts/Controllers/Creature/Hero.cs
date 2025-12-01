using Data;
using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using static Define;

public class Hero : Creature
{
    EHeroMoveState _heroMoveState = EHeroMoveState.None;
    public EHeroMoveState HeroMoveState
    {
        get { return _heroMoveState; }
        set
        {
            _heroMoveState = value;
            switch (value)
            {
                case EHeroMoveState.CollectEnv:
                    break;
                case EHeroMoveState.TargetMonster:
                    break;
                case EHeroMoveState.ForceMove:
                    break;
                case EHeroMoveState.None:
                    break;
            }
        }
    }

    public override bool Init()
    {
        if( base.Init() == false)
            return false;

        ObjectType = EObjectType.Hero;

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
    protected override void UpdateAnimation()
    {
        base.UpdateAnimation();
        switch (CreatureState)
        {
            case ECreatureState.StateIdle:
                PlayAnimation(0, AnimName.IDLE, true);
                break;
            case ECreatureState.StateSkill:
                PlayAnimation(0, AnimName.ATTACK_A, true);
                break;
            case ECreatureState.StateSkillelse:
                PlayAnimation(0, AnimName.IDLE, true);
                break;
            case ECreatureState.StateMove:
                PlayAnimation(0, AnimName.MOVE, true);
                break;
            case ECreatureState.StateOndamaged:
                PlayAnimation(0, AnimName.IDLE, true);
                break;
            case ECreatureState.StateDie:
                PlayAnimation(0, AnimName.DEAD, false);
                break;
        }
    }
    public override void SetInfo(int templateID)
    {
        base.SetInfo(templateID);
        MoveSpeed = Stat.MoveSpeed;
        Hp = Stat.MaxHp;
        AddHpBar();
        UpdateHpBar();
    }


    protected override void UpdateIdle()
    {
        base.UpdateIdle();
        //if (LerpCellPosCompleted)
        //    LerpCellPosCompleted = false;
        //    CreatureState = ECreatureState.StateMove;
    }

    protected override void UpdateMove()
    {
        base.UpdateMove();

        if (LerpCellPosCompleted)
        {
            CreatureState = ECreatureState.StateIdle;
            return;
        }
    }

    protected override void UpdateSkill()
    {
        base.UpdateSkill();
    }

    protected override void UpdateSkillelse()
    {
        base.UpdateSkillelse();
    }

    #region skill
    public override void UseSkill(int skillId, int targetId)
    {
        base.UseSkill(skillId, targetId);
        if (DataTemplateID == HERO_ALCHEMIST_ID || DataTemplateID == HERO_OWEN_ID)
        {
            GameObject obj = null;
            
            if (Managers.Object._objects.TryGetValue(targetId, out obj) == false)
                return;

            BaseObject target = obj.GetComponent<BaseObject>();
            // 스킬 매즈기 구현    
            if (skillId == CC_FREEZE || skillId == CC_STUN)
            {
                target.CreatureState = ECreatureState.StateOndamaged;
            }
            // 메즈기 해제
            if (skillId == CC_RELEASE)
            {
                target.CreatureState = ECreatureState.StateIdle;
            }
            LookAtTarget(target);
            // temp 수정 (나머지 스킬 패킷들도 타겟보게 수정
        }
    }



    #endregion






}
