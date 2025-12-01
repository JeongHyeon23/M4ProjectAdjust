using Data;
using Google.Protobuf.Protocol;
using Spine;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using static Define;

public class BaseObject : InitBase
{
    public int Id { get; set; }
    public EObjectType ObjectType { get; protected set; } = EObjectType.None;
    public SkeletonAnimation SkeletonAnim { get; private set; }
    public Vector3 UpPosition { get { return transform.position + Vector3.up * 2f; } }
    public Vector3 CenterPosition { get { return transform.position + Vector3.up * 1f; } }

    StatInfo _stat = new StatInfo();
    public virtual StatInfo Stat
    {
        get { return _stat; }
        set
        {
            if (_stat.Equals(value))
                return;

            _stat.MergeFrom(value);

        }
    }

    public float MoveSpeed
    {
        get { return Stat.MoveSpeed; }
        set { Stat.MoveSpeed = value; }
    }
    public virtual float Hp
    {
        get { return Stat.Hp; }
        set
        {
            Stat.Hp = value;
        }
    }

    bool _lookLeft = true;
    public bool LookLeft
    {
        get { return _lookLeft; }
        set
        {
            _lookLeft = value;
            Flip(!value);
        }
    }
    protected PositionInfo _positionInfo = new PositionInfo();

    public virtual PositionInfo PosInfo
    {
        get { return _positionInfo; }
        set
        {
            if (_positionInfo.Equals(value))
                return;

            EObjectType type = Managers.Object.GetObjectTypeById(Id);
            // 충돌구현하는 몬스터 히어로만 map이동구현
            if (type == EObjectType.Monster || type == EObjectType.Hero)
            {
                Vector2Int cellPos = new Vector2Int(value.Posx, value.Posy);
                MoveDir = value.Dir;
                Managers.Map.MoveTo(this, cellPos);
            }
            if (type != EObjectType.Monster && type != EObjectType.Hero)
            {
                CellPos = new Vector2Int(value.Posx, value.Posy);
                MoveDir = value.Dir;
            }

            // 내 플레이어는 상태 덮어쓰지 않고, 알아서 관리한다.
            bool isMyHero = this is MyHero;
            if (isMyHero == false)
            {
                CreatureState = value.State;
            }

        }
    }
    [SerializeField]
    protected ECreatureState _creatureState = ECreatureState.StateNone;

    public virtual ECreatureState CreatureState
    {
        get { return PosInfo.State; }
        set
        {
            if (_creatureState == value)
                return;

            _creatureState = value;
            PosInfo.State = value;
            UpdateAnimation();
        }
    }

    [SerializeField]
    protected EMoveDir _moveDir = EMoveDir.MoveNone;

    public EMoveDir MoveDir
    {
        get { return PosInfo.Dir; }
        set
        {
            if (_moveDir == value)
                return;

            _moveDir = value;
            PosInfo.Dir = value;
            UpdateAnimation();
        }
    }

    protected virtual void Start()
    {
        StartCoroutine(CoUpdateAI());
    }

    protected virtual void Update()
    {
       
    }
    

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        SkeletonAnim = GetComponent<SkeletonAnimation>();

        return true;
    }

    protected virtual void OnDisable()
    {
        if (SkeletonAnim == null)
            return;
        if (SkeletonAnim.AnimationState == null)
            return;
    }

    #region Spine
    protected virtual void SetSpineAnimation(string dataLabel, int sortingOrder)
    {
        if (SkeletonAnim == null)
            return;

        SkeletonAnim.skeletonDataAsset = Managers.Resource.Load<SkeletonDataAsset>(dataLabel);
        SkeletonAnim.Initialize(true);
        // 얼리는 스킬애니메이션 너무 빨라 따로 timescale 조절
        if (dataLabel == "debuff_freezing")
            SkeletonAnim.timeScale = 0.25f;
        // 독장판 스킬이내메이션 너무 빨라 따로 timesclae 조절
        if (dataLabel == "loren_skill_1_SkeletonData")
            SkeletonAnim.timeScale = 0.4f;

        else
            SkeletonAnim.timeScale = 1.0f;

        SortingGroup sg = Util.GetOrAddComponent<SortingGroup>(gameObject);
        sg.sortingOrder = sortingOrder;
    }

    protected virtual void UpdateAnimation()
    {

    }
    public TrackEntry PlayAnimation(int trackIndex, string animName, bool loop)
    {
        if (SkeletonAnim == null)
            return null;
        if (SkeletonAnim.AnimationState == null)
            return null;

        TrackEntry entry = SkeletonAnim.AnimationState.SetAnimation(trackIndex, animName, loop);
        

        if (animName == AnimName.DEAD)
            entry.MixDuration = 0;
        else
            entry.MixDuration = 0.2f;

        return entry;
    }
    public void Flip(bool flag)
    {
        if (SkeletonAnim == null)
            return;

        SkeletonAnim.Skeleton.ScaleX = flag ? -1 : 1;
    }
    #endregion

    #region Map
   
    public bool LerpCellPosCompleted { get; protected set; }

    [SerializeField] Vector2Int _cellPos;
    public Vector2Int CellPos
    {
        get { return _cellPos; }
        protected set
        {
            _cellPos = value;
            LerpCellPosCompleted = false;
        }
    }

    public void SetCellPos(Vector2Int cellPos, bool forceMove = false)
    {
        CellPos = cellPos;
        LerpCellPosCompleted = false;

        // 처음 등장시 생성할때씀
        if (forceMove)
        {
            // temp
            transform.position = Managers.Map.Cell2World(cellPos);
            LerpCellPosCompleted = true;
        }
    }

    public void LerpToCellPos(float moveSpeed)
    {
        if (LerpCellPosCompleted)
            return;

        Vector3 destPos = Managers.Map.Cell2World(CellPos);
        Vector3 dir = destPos - transform.position;

        LookAtTarget(dir);

        float moveDist = moveSpeed * Time.deltaTime;
        if (dir.magnitude < moveDist)
        {
            // 맵매니저 갱신
            Managers.Map.MoveTo(this, CellPos, true);
            transform.position = destPos;
            LerpCellPosCompleted = true;
            return;
        }
        transform.position += dir.normalized * moveDist;
    }

    public void LookAtTarget(BaseObject target)
    {
        if (target == null)
            return;

        Vector2 dir = target.transform.position - transform.position;
        LookAtTarget(dir);
    }

    public void LookAtTarget(Vector3 dir)
    {
        if (dir.x < 0)
            LookLeft = true;
        else if (dir.x > 0)
            LookLeft = false;
    }
   
    #endregion

    #region AI
    public float UpdateAITick { get; protected set; } = 0.0f;

    protected virtual void UpdateAI()
    {
        while (true)
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
                case ECreatureState.StateSkillelse:
                    UpdateSkillelse();
                    break;
                case ECreatureState.StateOndamaged:
                    UpdateOnDamaged();
                    break;
                case ECreatureState.StateDie:
                    UpdateDead();
                    break;
            }
        }
    }

    protected virtual IEnumerator CoUpdateAI()
    {
        while (true)
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
                case ECreatureState.StateSkillelse:
                    UpdateSkillelse();
                    break;
                case ECreatureState.StateOndamaged:
                    UpdateOnDamaged();
                    break;
                case ECreatureState.StateDie:
                    UpdateDead();
                    break;
            }
            yield return null;
        }
    }

    protected virtual void UpdateIdle() { }
    protected virtual void UpdateMove() { }
    protected virtual void UpdateSkill() { }
    protected virtual void UpdateSkillelse() { }
    protected virtual void UpdateDead() { }

    protected virtual void UpdateOnDamaged()
    { }

    #endregion

    #region Battle

    protected Creature FindTargetInRange(int id, float range)
    {
        GameObject target = null;
        float bestDistanceSqr = int.MaxValue;
        float searchDistanceSqr = range * range;

        foreach (GameObject obj in Managers.Object._objects.Values)
        {
            Vector3 dir = obj.transform.position - transform.position;
            float distToTargetSqr = dir.sqrMagnitude;

            // AoE는 대상에서 제외
            if (obj.GetComponent<BaseObject>().ObjectType == EObjectType.Aoe)
                continue;

            // 나 자신은 대상에서 제외
            if (obj.GetComponent<BaseObject>().Id == id)
                continue;

            // 이펙트는 대상에서 제외
            if (obj.GetComponent<BaseObject>().ObjectType == EObjectType.Effect)
                continue;

            // 서치 범위보다 멀리있으면 스킵
            if (distToTargetSqr > searchDistanceSqr)
                continue;

            // 더 좋은 후보를 찾았을 경우 스킵
            if (distToTargetSqr > bestDistanceSqr)
                continue;

            target = obj;
            bestDistanceSqr = distToTargetSqr;
        }
        if (target == null)
            return null;
        Creature co = target.GetComponent<Creature>();
        return co;
    }

    //protected BaseObject FindClosestInRange(float range, IEnumerable<BaseObject> objs, Func<BaseObject, bool> func = null)
    //{
    //    BaseObject target = null;
    //    float bestDistanceSqr = int.MaxValue;
    //    float searchDistanceSqr = range * range;

    //    foreach (BaseObject obj in objs)
    //    {
    //        Vector3 dir = obj.transform.position - transform.position;
    //        float distToTargetSqr = dir.sqrMagnitude;

    //        // 서치 범위보다 멀리있으면 스킵
    //        if (distToTargetSqr > searchDistanceSqr)
    //            continue;

    //        // 더 좋은 후보를 찾았을 경우 스킵
    //        if (distToTargetSqr > bestDistanceSqr)
    //            continue;

    //        // 추가 조건
    //        if (func != null && func.Invoke(obj) == false)
    //            continue;

    //        target = obj;
    //        bestDistanceSqr = distToTargetSqr;
    //    }

    //    return target;
    //}

    //protected void ChaseOrAttackTarget(float chaseRange, float attackRange)
    //{

    //    //float distToTargetSqr = DistToTargetSqr;
    //    float attackDistanceSqr = attackRange * attackRange;

    //    if (distToTargetSqr <= attackDistanceSqr)
    //    {
    //        CreatureState = ECreatureState.StateSkill;
    //        //skill.DoSkill();
    //        return;
    //    }
    //    else
    //    {
    //        //공격범위 밖이라면 추적
    //        //FindPathAndMoveToCellPos(Target.CellPos, HERO_DEFAULT_MOVE_DEPTH);

    //        // 너무멀어지면 포기
    //        float searchDistanceSqr = chaseRange * chaseRange;
    //        if (distToTargetSqr > searchDistanceSqr)
    //        {
    //            Target = null;
    //            CreatureState = ECreatureState.StateMove;
    //        }
    //        return;
    //    }
    //}


    #endregion
}
