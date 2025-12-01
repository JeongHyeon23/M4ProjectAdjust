using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;
using Data;
using Google.Protobuf.Protocol;
using System.IO;

public class Creature : BaseObject
{
    protected HpBar _hpBar;
    public int DataTemplateID { get; set; }
    public Data.CreatureData CreatureData { get; private set; }
   
    public override StatInfo Stat
    {
        get { return base.Stat; }
        set
        {
            base.Stat = value;
            UpdateHpBar();
        }
    }

    public override float Hp
    {
        get { return base.Stat.Hp; }
        set
        {
            base.Hp = value;
            UpdateHpBar();
        }
    }

    protected void AddHpBar()
    {
        GameObject go = Managers.Resource.Instantiate("HpBar", transform);
        go.transform.localPosition = new Vector3(0, -0.3f, 0);
        go.name = "HpBar";
        _hpBar = go.GetComponent<HpBar>();
    }
    protected void UpdateHpBar()
    {
        if (_hpBar == null)
            return;

        float ratio = 0.0f;
        if (Stat.MaxHp > 0)
        {
            ratio = ((float)Hp / Stat.MaxHp);
        }
        _hpBar.SetHpBar(ratio);
    }

    public override bool Init()
    {
        if( base.Init() == false)
            return false;

        return true;
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void Start()
    {
        base.Start();
    }

    public virtual void SetInfo(int templateID)
    {
        DataTemplateID = templateID;
        if (ObjectType == EObjectType.Hero)
            CreatureData = Managers.Data.HeroDic[DataTemplateID];
        else 
            CreatureData = Managers.Data.MonsterDic[DataTemplateID];
        // hpbar 정보 선입력

        // Spine
        SetSpineAnimation(CreatureData.SkeletonDataID, SortingLayers.CREATURE);

        CreatureState = ECreatureState.StateIdle;
    }

    protected override void UpdateAnimation()
    {
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

    #region AI
    protected override void UpdateMove()
    {
        base.UpdateMove();

        // 이동 끝 // temp 로직 확인할것
        //if (LerpCellPosCompleted)
        //{
        //    CreatureState = ECreatureState.StateIdle;
        //    return;
        //}

    }

    #endregion

    #region Wait
    protected Coroutine _coWait;

    protected void StartWait(float seconds)
    {
        CancelWait();
        _coWait = StartCoroutine(CoWait(seconds));
    }

    IEnumerator CoWait(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        _coWait = null;
    }

    protected void CancelWait()
    {
        if (_coWait != null)
            StopCoroutine(_coWait);
        _coWait = null;
    }
    #endregion

    #region Map
    public EFindPathResult FindPathAndCellPos(Vector3 destWorldPos, int maxDepth, out List<Vector2Int> path, bool forceMoveCloser = false)
    {
        Vector2Int destCellPos = Managers.Map.World2Cell(destWorldPos);
        return FindPathAndCellPos(destCellPos, maxDepth, out path, forceMoveCloser);
    }

    public EFindPathResult FindPathAndCellPos(Vector2Int destCellPos, int maxDepth, out List<Vector2Int> path, bool forceMoveCloser = false)
    {
        path = new List<Vector2Int>();

        //if (CellPos == destCellPos)
        //    return; 

        if (LerpCellPosCompleted == false)
            return EFindPathResult.Fail_LerpCell;

        // A*
        path = Managers.Map.FindPath(this, CellPos, destCellPos, maxDepth);
        if (path.Count < 2)
            return EFindPathResult.Fail_NoPath;

        if (forceMoveCloser)
        {
            Vector2Int diff1 = CellPos - destCellPos;
            Vector2Int diff2 = path[1] - destCellPos;
            if (diff1.sqrMagnitude <= diff2.sqrMagnitude)
                return EFindPathResult.Fail_NoPath;
        }

        Vector2Int dirCellPos = path[1] - CellPos;
        Vector2Int nextPos = CellPos + dirCellPos;

        //if (Managers.Map.MoveTo(this, nextPos) == false)
        //    return EFindPathResult.Fail_MoveTo;

        return EFindPathResult.Success;
    }

    //public bool MoveToCellPos(Vector2Int destCellPos, int maxDepth = 2, bool forceMoveCloser = false)
    //{
    //    if (LerpCellPosCompleted == false)
    //        return false;

    //    return Managers.Map.MoveTo(this, destCellPos);
    //}

    #endregion

    #region Skill

    public virtual void UseSkill(int skillId, int targetId)
    {

    }

    #endregion
}
