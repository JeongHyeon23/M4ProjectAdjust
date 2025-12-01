using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf.Protocol;
using static Define;
using Unity.VisualScripting;
using Data;
using System;

public class MyHero : Hero
{
    protected bool _sendMovePacket = false;
    Vector2Int _desirepos; 
    Vector2Int _destPos;
    EMoveDir _joystickDir;
    [SerializeField] 
    private EJoystickState _joystickState;

    [SerializeField]
    public Creature Target { get; set; }

    public float DistToTargetSqr
    {
        get
        {
            Vector3 dir = (Target.transform.position - transform.position);
            float distToTarget = dir.magnitude;
            return distToTarget * distToTarget;
        }
    }
    [SerializeField]
    public float SearchRange;
    [SerializeField]
    public float AttackRange;
    public Vector2Int DestPos
    {
        get { return _destPos; }
        set
        {
            if (_destPos == value)
                return;

            _destPos = value;
            _sendMovePacket = true;
        }
    }
    public SkillData DefaultSkill { get; set; }
    public SkillData SkillA { get; set; }
    public SkillData SkillB { get; set; }

    public long _nextDefaultskillTime { get; set; } = 0;
    public long _nextASkillTime { get; set; } = 0;
    public long _nextBSkillTime { get; set; } = 0;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        ObjectType = EObjectType.Hero;

        Managers.Game.OnJoystickStateChanged -= HandleOnJoystickStateChanged;
        Managers.Game.OnJoystickStateChanged += HandleOnJoystickStateChanged;

        return true;
    }

    protected override void Start()
    {
        base.Start();
        LerpCellPosCompleted = false;
    }

    protected override void Update()
    {
        LerpToCellPos(MoveSpeed);
        SendMovePacket();
    }



    public override void SetInfo(int templateID)
    {
        base.SetInfo(templateID);
        MoveSpeed = Stat.MoveSpeed;
        Hp = Stat.MaxHp;
        AddHpBar();
        UpdateHpBar();
        SearchRange = CreatureData.SearchRange;
        AttackRange = CreatureData.AtkRange;
        DefaultSkill = Managers.Data.SkillDic[CreatureData.DefaultSkillId];
        SkillA = Managers.Data.SkillDic[CreatureData.SkillAId];
        SkillB = Managers.Data.SkillDic[CreatureData.SkillBId];
        CameraController cc = Camera.main.GetOrAddComponent<CameraController>();
        if (cc != null)
            cc.Target = this;

    }

    #region 초기 만들었던거
    //private void HandleOnMoveDirChanged(Vector2 dir)
    //{
    //    EMoveDir eMoveDir = Managers.Game.GetMoveDir(dir);
    //    if (_movepressed == false)
    //        return;
    //    switch (eMoveDir)
    //    {
    //        case EMoveDir.MoveNone:
    //            CellPos = CellPos;
    //            _movepressed = false;
    //            CreatureState = ECreatureState.StateIdle;
    //            break;
    //        case EMoveDir.MoveUpleft:
    //            CellPos = CellPos + Managers.Map.GetFrontCellPos(eMoveDir);
    //            _movepressed = false;
    //            CreatureState = ECreatureState.StateMove;
    //            break;
    //        case EMoveDir.MoveUp:
    //            CellPos = CellPos + Managers.Map.GetFrontCellPos(eMoveDir);
    //            _movepressed = false;
    //            CreatureState = ECreatureState.StateMove;
    //            break;
    //        case EMoveDir.MoveUpright:
    //            CellPos = CellPos + Managers.Map.GetFrontCellPos(eMoveDir);
    //            _movepressed = false;
    //            CreatureState = ECreatureState.StateMove;
    //            break;
    //        case EMoveDir.MoveRight:
    //            CellPos = CellPos + Managers.Map.GetFrontCellPos(eMoveDir);
    //            _movepressed = false;
    //            CreatureState = ECreatureState.StateMove;
    //            break;
    //        case EMoveDir.MoveDownright:
    //            CellPos = CellPos + Managers.Map.GetFrontCellPos(eMoveDir);
    //            _movepressed = false;
    //            CreatureState = ECreatureState.StateMove;
    //            break;
    //        case EMoveDir.MoveDown:
    //            CellPos = CellPos + Managers.Map.GetFrontCellPos(eMoveDir);
    //            _movepressed = false;
    //            CreatureState = ECreatureState.StateMove;
    //            break;
    //        case EMoveDir.MoveDownleft:
    //            CellPos = CellPos + Managers.Map.GetFrontCellPos(eMoveDir);
    //            _movepressed = false;
    //            CreatureState = ECreatureState.StateMove;
    //            break;
    //        case EMoveDir.MoveLeft:
    //            CellPos = CellPos + Managers.Map.GetFrontCellPos(eMoveDir);
    //            _movepressed = false;
    //            CreatureState = ECreatureState.StateMove;
    //            break;
    //        default:
    //            break;

    //    }
    //}

    #endregion

    #region 조작 관련
    void HandleOnJoystickStateChanged(EJoystickState joystickState, EMoveDir dir)
    {
        // 메즈기 적용시 조작 불가상태로 만듬
        if (CreatureState == ECreatureState.StateOndamaged)
            return;

        _joystickState = joystickState;

        if (joystickState == EJoystickState.Drag)
        {
            HeroMoveState = EHeroMoveState.ForceMove;
            GetDesireCellpos(dir);
        }
        if (joystickState == EJoystickState.PointerUp)
        {
            HeroMoveState = EHeroMoveState.None;
        }
        if (joystickState == EJoystickState.OnClickAttack)
        {
            HeroMoveState = EHeroMoveState.TargetMonster;
        }
        if (joystickState == EJoystickState.OnClickSkillA)
        {
            HeroMoveState = EHeroMoveState.SpawnAoe;
        }
        if (joystickState == EJoystickState.OnClickSkillB)
        {
            HeroMoveState = EHeroMoveState.UseCC;
        }
    }

    private void GetDesireCellpos(EMoveDir dir)
    {
        if (dir == EMoveDir.MoveNone)
            return;

        if (LerpCellPosCompleted == false)
            return;

        Vector2Int checkPos = CellPos + Managers.Map.GetFrontCellPos(dir);
        MoveDir = dir;
        _desirepos = CellPos + Managers.Map.GetFrontCellPos(dir);
        
    }

    #endregion
    void SendMovePacket()
    {
        if (_sendMovePacket)
        {
            _sendMovePacket = false;

            C_Move movePacket = new C_Move() { PosInfo = new PositionInfo() };
            movePacket.PosInfo.MergeFrom(PosInfo);
            movePacket.PosInfo.State = ECreatureState.StateMove;
            movePacket.PosInfo.Posx = CellPos.x;
            movePacket.PosInfo.Posy = CellPos.y;

            Managers.Network.Send(movePacket);
        }
    }
    // 애니메이션 동기화 packet
    void SendAnimPacket()
    {
        C_Move movePacket = new C_Move() { PosInfo = new PositionInfo() };
        movePacket.PosInfo.MergeFrom(PosInfo);
        movePacket.PosInfo.State = ECreatureState.StateIdle;
        movePacket.PosInfo.Posx = CellPos.x;
        movePacket.PosInfo.Posy = CellPos.y;

        Managers.Network.Send(movePacket);
    }

    #region 초기만든것2
    //private void HandleOnJoystickStateChanged(EJoystickState state)
    //{
    //    switch (state)
    //    {
    //        case EJoystickState.PointerDown:
    //            HeroMoveState = EHeroMoveState.ForceMove;
    //            _movepressed = true;
    //            //CreatureState = ECreatureState.StateMove;
    //            break;
    //        case EJoystickState.PointerUp:
    //            HeroMoveState = EHeroMoveState.None;
    //            _movepressed = true;
    //            //CreatureState = ECreatureState.StateIdle;
    //            break;
    //        case EJoystickState.Drag:
    //            HeroMoveState = EHeroMoveState.ForceMove;
    //            _movepressed = true;
    //            //CreatureState = ECreatureState.StateMove;
    //            break;
    //        case EJoystickState.OnClickAttack:
    //            HeroMoveState = EHeroMoveState.TargetMonster;
    //            //CreatureState = ECreatureState.StateIdle;
    //            break;
    //    }
    //}
    #endregion

    #region AI HelPer
    void FindTargetAndSelectState(int id, float searchrange)
    {
        Creature co = FindTargetInRange(id, searchrange);
        Target = co;
        // 범위안에 없을시 그냥 idle로 변경
        if (Target == null)
        {
            CreatureState = ECreatureState.StateIdle;
            HeroMoveState = EHeroMoveState.None;
            return;
        }
        CreatureState = ECreatureState.StateMove;
        return;
    }

    void FindPathAndMoveToTarget(EHeroMoveState state)
    {
        if (Target == null)
        {
            CreatureState = ECreatureState.StateIdle;
            HeroMoveState = EHeroMoveState.None;
            return;
        }
        EFindPathResult result = FindPathAndCellPos(Target.CellPos, HERO_DEFAULT_MOVE_DEPTH, out List<Vector2Int> path, true);
        if (result == EFindPathResult.Success)
        {
            if (DistToTargetSqr <= AttackRange * AttackRange)
            {
                CreatureState = state == EHeroMoveState.TargetMonster ? ECreatureState.StateSkill : ECreatureState.StateSkillelse;
                HeroMoveState = state;
                return;
            }
            else if (DistToTargetSqr > SearchRange * SearchRange)
            {
                CreatureState = ECreatureState.StateIdle;
                HeroMoveState = EHeroMoveState.None;
                Target = null;
                return;
            }
            else
            {
                DestPos = path[1];
                if (Managers.Map.GetObject(DestPos) != null)
                {
                    CreatureState = ECreatureState.StateIdle;
                    return;
                }
                    
                // 클라맵 동기화
                Managers.Map.MoveTo(this, DestPos);
                CellPos = DestPos;
                CreatureState = ECreatureState.StateMove;
                HeroMoveState = state;
                return;
            }
        }
        else
        {
            if (DistToTargetSqr <= AttackRange * AttackRange)
            {
                CreatureState = state == EHeroMoveState.TargetMonster ? ECreatureState.StateSkill : ECreatureState.StateSkillelse;
                HeroMoveState = state;
                return;
            }
            return;
        }
        //return;
    }

    void CheckSkillCoolTime(int id)
    {
        if (id == DefaultSkill.DataId)
        {
            if (_nextDefaultskillTime <= Util.TickCount || _nextDefaultskillTime == 0)
            {
                _nextDefaultskillTime = Util.TickCount + (long)(DefaultSkill.CoolTime * 1000);
                CreatureState = ECreatureState.StateSkill;
                C_Skill skill = new C_Skill();
                skill.SkillId = DefaultSkill.DataId;
                skill.TargetId = Target.Id;
                Managers.Network.Send(skill);
                return;
            }
            return;
            
        }
        else if (id == SkillA.DataId)
        {
            if (_nextASkillTime <= Util.TickCount || _nextASkillTime == 0)
            {
                _nextASkillTime = Util.TickCount + (long)(SkillA.CoolTime * 1000);
                CreatureState = ECreatureState.StateSkillelse;
                C_Skill skill = new C_Skill();
                skill.SkillId = SkillA.DataId;
                skill.TargetId = Target.Id;
                Managers.Network.Send(skill);
                
                return;
            }
            CreatureState = ECreatureState.StateMove;
            HeroMoveState = EHeroMoveState.None;
            return; 
        }
        else if (id == SkillB.DataId)
        {
            if (_nextBSkillTime <= Util.TickCount || _nextBSkillTime == 0)
            {
                _nextBSkillTime = Util.TickCount + (long)(SkillB.CoolTime * 1000);
                CreatureState = ECreatureState.StateSkillelse;
                C_Skill skill = new C_Skill();
                skill.SkillId = SkillB.DataId;
                skill.TargetId = Target.Id;
                Managers.Network.Send(skill);
                return;
            }
            CreatureState = ECreatureState.StateMove;
            HeroMoveState = EHeroMoveState.None;
            return;
        }
        return;
        
    }
    void CheckSkillOrAttackTarget(int skillid ,float skillcooltime)
    {
        // 타겟이 이미 죽은 경우(leave 패킷은 애니메이션 재생후에 도착)
        if (Target == null)
        {
            //Target = null;
            CreatureState = ECreatureState.StateIdle;
            HeroMoveState = EHeroMoveState.None;
            //anim 동기화 위한 movepakcket
            SendAnimPacket();
            return;
        }
        // 타겟이 범위 내인 경우
        if (DistToTargetSqr <= AttackRange * AttackRange)
        {
            if (DataTemplateID == HERO_ALCHEMIST_ID)
            {
                // 스킬쿨타임 여부 확인
                CheckSkillCoolTime(skillid);
                return;
            }
            else if (DataTemplateID == HERO_OWEN_ID)
            {
                // 스킬쿨타임 여부 확인
                CheckSkillCoolTime(skillid);
                return;
            }
        }
        // 타겟이 범위 밖으로 나간 경우
        else if (DistToTargetSqr > AttackRange * AttackRange)
        {
            CreatureState = ECreatureState.StateMove;
            return;
        }
    }


    #endregion

    #region Ai
    protected override void UpdateIdle()
    {
       
        base.UpdateIdle();
        // 0. 피없을때 죽은 애니메이션 틀어주기
        if (Hp == 0)
        {
            CreatureState = ECreatureState.StateDie;
            HeroMoveState = EHeroMoveState.None;
            return;
        }
        // 1. 이동상태에서 강제변경
        if (HeroMoveState == EHeroMoveState.ForceMove)
        {
            CreatureState = ECreatureState.StateMove;
            return;
        }
        // 2. 조이스틱 조작x idle 복귀
        if (HeroMoveState == EHeroMoveState.None)
        {
            CreatureState = ECreatureState.StateIdle;
            return;
        }
        // 3. 몬스터 혹은 플레이어 
        if (HeroMoveState == EHeroMoveState.TargetMonster)
        {
            FindTargetAndSelectState(Id, SearchRange);
            return;
        }
        // 4. 장판 스킬 발사
        if (HeroMoveState == EHeroMoveState.SpawnAoe)
        {
            FindTargetAndSelectState(Id, SearchRange);
            return;
        }
        // 5. CC기 사용
        if (HeroMoveState == EHeroMoveState.UseCC)
        {
            FindTargetAndSelectState(Id, SearchRange);
            return;
        }

        return;

        // 2. env 채집
    }
    protected override void UpdateMove()
    {
        if (LerpCellPosCompleted == false)
            return;

        if (Hp == 0)
        {
            CreatureState = ECreatureState.StateDie;
            HeroMoveState = EHeroMoveState.None;
            return;
        }
        // Idle 되돌리기 용
        if (HeroMoveState == EHeroMoveState.None)
        {
            CreatureState = ECreatureState.StateIdle;
        }
        // 조이스틱 강제 무빙용
        if (HeroMoveState == EHeroMoveState.ForceMove)
        {
            if(Managers.Map.CanGo(this, _desirepos) == false)
            {
                CreatureState = ECreatureState.StateIdle;
                return;
            }
            EFindPathResult result = FindPathAndCellPos(_desirepos, HERO_DEFAULT_MOVE_DEPTH, out List<Vector2Int> path, true);
            if (result == EFindPathResult.Success)
            {
                DestPos = path[1];
                // 클라맵 동기화 (장소 선점하고 이동처리하니까 로직상 문제없음)
                Managers.Map.MoveTo(this, DestPos);
                CellPos = DestPos;
                return;
            }
            return;
        }
        //OnClickAttack
        if (HeroMoveState == EHeroMoveState.TargetMonster)
        {
            EHeroMoveState movestate = EHeroMoveState.TargetMonster;
            FindPathAndMoveToTarget(movestate);
        }
        //SkillA
        if (HeroMoveState == EHeroMoveState.SpawnAoe)
        {
            EHeroMoveState movestate = EHeroMoveState.SpawnAoe;
            FindPathAndMoveToTarget(movestate);
        }
        //SkillB
        if (HeroMoveState == EHeroMoveState.UseCC)
        {
            EHeroMoveState movestate = EHeroMoveState.UseCC;
            FindPathAndMoveToTarget(movestate);
        }

        return;
        #region 예전거
        //if (MoveToCellPos(position, 2) == false)
        //{
        //    CreatureState = ECreatureState.Move;
        //}
        //if (HeroMoveState == EHeroMoveState.TargetMonster)
        //{
        //    if (Target != null)
        //    {
        //        //FindPathAndMoveToCellPos(Target.transform.position, HERO_DEFAULT_MOVE_DEPTH, true);
        //        if (DistToTargetSqr < AttackDistance)
        //        {
        //            CreatureState = ECreatureState.StateSkill;
        //            return;
        //        }
        //        //else if (DistToTargetSqr < SearchDistace && DistToTargetSqr > AttackDistance)
        //        //{
        //        //    return;
        //        //}
        //        else if (DistToTargetSqr > SearchDistace)
        //        {
        //            Target = null;
        //            HeroMoveState = EHeroMoveState.None;
        //            return;
        //        }
        //    }
        //}
        //else
        //{
        //    CreatureState = ECreatureState.Move;
        //}
        //
        #endregion
    }

    protected override void UpdateSkill()
    {
        base.UpdateSkill();
        if (Hp == 0)
        {
            CreatureState = ECreatureState.StateDie;
            HeroMoveState = EHeroMoveState.None;
            return;
        }
        // 1. 강제 이동
        if (HeroMoveState == EHeroMoveState.ForceMove)
        {
            CreatureState = ECreatureState.StateMove;
        }
        // 2. OnclickAttack
        else if (HeroMoveState == EHeroMoveState.TargetMonster)
        {
            CheckSkillOrAttackTarget(CreatureData.DefaultSkillId, DefaultSkill.CoolTime);
        }
        // 3. 다른 스킬 사용
        else if (HeroMoveState == EHeroMoveState.SpawnAoe || HeroMoveState == EHeroMoveState.UseCC)
        {
            CreatureState = ECreatureState.StateIdle;
        }
        return;
    }

    protected override void UpdateSkillelse()
    {
        base.UpdateSkillelse();
        if (Hp == 0)
        {
            CreatureState = ECreatureState.StateDie;
            HeroMoveState = EHeroMoveState.None;
            return;
        }
        //1. 강제 이동
        if (HeroMoveState == EHeroMoveState.ForceMove)
        {
            CreatureState = ECreatureState.StateMove;
        }
        else if (HeroMoveState == EHeroMoveState.TargetMonster)
        {
            CreatureState = ECreatureState.StateIdle;
        }
        // SKILL A 사용시
        // 연금술사
        else if (HeroMoveState == EHeroMoveState.SpawnAoe && DataTemplateID == HERO_ALCHEMIST_ID)
        {
            CheckSkillOrAttackTarget(SPAWN_POISION_AOE, SkillA.CoolTime);
        }
        // 오웬
        else if (HeroMoveState == EHeroMoveState.SpawnAoe && DataTemplateID == HERO_OWEN_ID)
        {
            CheckSkillOrAttackTarget(SPAWN_MULTIFIRE_AOE, SkillA.CoolTime);
        }
        // Skill B 사용시
        // 연금술사
        else if (HeroMoveState == EHeroMoveState.UseCC && DataTemplateID == HERO_ALCHEMIST_ID)
        {
            CheckSkillOrAttackTarget(CC_FREEZE, SkillB.CoolTime);
        }
        // 오웬
        else if (HeroMoveState == EHeroMoveState.UseCC && DataTemplateID == HERO_OWEN_ID)
        {
            CheckSkillOrAttackTarget(CC_STUN, SkillB.CoolTime);
        }
        return;
    }

    protected override void UpdateDead()
    {
        base.UpdateDead();
    }

    protected override void UpdateOnDamaged()
    {
        base.UpdateOnDamaged();
        if (Hp == 0)
        {
            CreatureState = ECreatureState.StateDie;
            HeroMoveState = EHeroMoveState.None;
            return;
        }
        CreatureState = ECreatureState.StateOndamaged;
        HeroMoveState = EHeroMoveState.None;
        Target = null;
        return;

    }

    #endregion

    #region Coroutine

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

    #endregion

    #region Skill

    public override void UseSkill(int skillId, int targetId)
    {
        base.UseSkill(skillId, targetId);
        if (skillId == MELE_NORMAL_ATTACK)
        {
           Target = Managers.Object.FindbyIdCreatrue(targetId);
        }
        else if (skillId == RANGE_NORMAL_ATTACK)
        {
            Target = Managers.Object.FindbyIdCreatrue(targetId);
        }

    }

    #endregion

    #region SkillCool 

    public float GetARemainCoolTime()
    {
        return Math.Max(0, (_nextASkillTime - Util.TickCount)) / 1000;
    }
    public float GetBRemainCoolTime()
    {
        return Math.Max(0, (_nextBSkillTime - Util.TickCount)) / 1000;
    }
    public float GetARemainCoolTimeRatio()
    {
        return GetARemainCoolTime() / SkillA.CoolTime;
    }
    public float GetBRemainCoolTimeRatio()
    {
        return GetBRemainCoolTime() / SkillB.CoolTime;
    }

    #endregion
}

