using Data;
using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class AoE : BaseObject
{
    int DataTemplateId { get; set; }
    Data.AoEData AoEData { get; set; }
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        ObjectType = EObjectType.Aoe;
        return true;
    }

    public void SetInfo(int id)
    {
        DataTemplateId = Managers.Object.GetTemplateIdById(id);
        SkillData skilldata = Managers.Data.SkillDic[DataTemplateId];
        AoEData = Managers.Data.AoEDic[skilldata.AoEId];

        SetSpineAnimation(AoEData.SkeletonDataID, SortingLayers.SPELL_INDICATOR);
    }

    protected override void UpdateAnimation()
    {
        // 진짜 영웅 상태가 아닌 AOE별 애니메이션 틀어주기 위한 상태 분기
        // 1. idle 독 장판
        // 2, stateskill  화염 장판

        switch (CreatureState)
        {
            case ECreatureState.StateIdle:
                PlayAnimation(0, AnimName.IDLE, false);
                break;
            case ECreatureState.StateSkill:
                PlayAnimation(0, AnimName.FireAoeBig, false);
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
}
