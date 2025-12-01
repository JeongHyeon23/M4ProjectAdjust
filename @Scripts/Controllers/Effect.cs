using Data;
using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Define;

public class Effect : BaseObject
{
    int EffectId { get; set; }
    int TargetId { get; set; }
    Data.EffectData EffectData { get; set; }
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        ObjectType = EObjectType.Effect;
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
            UpdateAnimation();
        }
    }

    public void SetInfo(int targetId)
    {
        EffectId = Managers.Object.GetTemplateIdById(Id);
        TargetId = targetId;
        EffectData = Managers.Data.EffectDic[EffectId];
        SetSpineAnimation(EffectData.SkeletonDataID, SortingLayers.SKILL_EFFECT);
    }

    protected override void UpdateAnimation()
    {
        // effect 별 애니메이션 상태 분기 CreatureState
        // 1. Idle 불타는 연출
        // 2. 화염장판 나오는 연출
        // 3. 얼리는 애니메이션 연출(속도조절된)

        switch (CreatureState)
        {
            case ECreatureState.StateIdle:
                PlayAnimation(0, AnimName.IDLE, true);
                break;
            case ECreatureState.StateSkill:
                PlayAnimation(0, AnimName.FireAoeBig, false);
                break;
            case ECreatureState.StateMove:
                PlayAnimation(0, AnimName.IDLE, false);
                break;
            case ECreatureState.StateOndamaged:
                PlayAnimation(0, AnimName.IDLE, true);
                break;
            case ECreatureState.StateDie:
                PlayAnimation(0, AnimName.DEAD, false);
                break;
        }
    }
    protected override void UpdateAI()
    {
        switch (CreatureState)
        {
            case ECreatureState.StateIdle:
                UpdateIdle();
                break;
            case ECreatureState.StateMove:
                UpdateMove();
                break;
            case ECreatureState.StateSkill:
                UpdateSkill();
                break;
            case ECreatureState.StateOndamaged:
                UpdateOnDamaged();
                break;
            case ECreatureState.StateDie:
                UpdateDead();
                break;
        }
    }

    protected override void UpdateIdle()
    {
        base.UpdateIdle();
        if (Managers.Object.FindById(TargetId) != null)
        {
            Creature target = Managers.Object.FindbyIdCreatrue(TargetId);
            transform.position = target.transform.position;
            return;
        }
        return;
    }

    protected override void Update()
    {
        
    }
    private void LateUpdate()
    {
        
    }
}
