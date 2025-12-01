using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using UnityEngine;

public class Define
{
    public enum EScene
    {
        Unknown,
        TitleScene,
        LobbyScene,
        GameScene,
    }

    public enum EUIEvent
    {
        Click,
        PointerDown,
        PointerUp,
        Drag,
    }

    public enum EJoystickState
    {
        None,
        PointerDown,
        PointerUp,
        Drag,
        OnClickAttack,
        OnClickSkillA,
        OnClickSkillB,
    }
    public enum EFindPathResult
    {
        Fail_LerpCell,
        Fail_NoPath,
        Fail_MoveTo,
        Success,
    }
    public enum EEnvState
    {
        Idle,
        OnDamaged,
        Dead
    }
    public enum EHeroMoveState
    {
        None,
        TargetMonster,
        CollectEnv,
        ForceMove,
        SpawnAoe,
        UseCC,
        //MoveStatechanged,
        //IdleStatechanhed,

    }
    public static class SortingLayers
    {
        public const int SPELL_INDICATOR = 250;
        public const int CREATURE = 300;
        public const int ENV = 300;
        public const int NPC = 310;
        public const int PROJECTILE = 310;
        public const int SKILL_EFFECT = 310;
        public const int DAMAGE_FONT = 410;
    }
    public static class AnimName
    {
        public const string ATTACK_A = "attack";
        public const string ATTACK_B = "attack";
        public const string SKILL_A = "skill";
        public const string SKILL_B = "skill";
        public const string IDLE = "idle";
        public const string MOVE = "move";
        public const string DAMAGED = "hit";
        public const string DEAD = "dead";
        //public const string EVENT_ATTACK_A = "event_attack";
        //public const string EVENT_ATTACK_B = "event_attack";
        //public const string EVENT_SKILL_A = "event_attack";
        //public const string EVENT_SKILL_B = "event_attack";
        public const string FireAoeSmall = "skill_b_small";
        public const string FireAoeNormal = "skill_b_normal";
        public const string FireAoeBig = "skill_b_big";
    }
    // Hero ID
    public const int HERO_WIZARD_ID = 1; 
    public const int HERO_KNIGHT_ID = 2; 
    public const int HERO_ALCHEMIST_ID = 3; 
    public const int HERO_LION_ID = 4; 
    public const int HERO_OWEN_ID = 6;
    // Monster ID
    public const int MONSTER_GOAST_ID = 51;
    public const int MONSTER_SPIDER_ID = 52;
    public const int MONSTER_RARE_SPIDER_ID = 53;
    public const int MONSTER_WOODMAN_ID = 54;
    public const int MONSTER_RARE_GOAST_ID = 55;
    public const int MONSTER_BEAR_ID = 56;
    // Env ID
    public const int ENV_WOOD_ID = 100;
    public const int ENV_BIGWOOD_ID = 101;

    #region Stat
    public const int HERO_DEFAULT_MOVE_DEPTH = 5;
    public const int HERO_JOYSTICK_MOVE_DEPTH = 2;
    public const int MONSTER_DEFAULT_MOVE_DEPTH = 3;
    public const float HERO_SEARCH_DISTANCE = 8.0f;
    public const float MONSTER_SEARCH_DISTANCE = 8.0f;
    public const int PROJECTILE_MOVE_SPEED = 4;
    #endregion

    #region Skill
    public const int MELE_NORMAL_ATTACK = 151;
    public const int RANGE_NORMAL_ATTACK = 152;
    public const int SPAWN_POISION_AOE = 158;
    public const int SPAWN_MULTIFIRE_AOE = 171;
    public const int CC_FREEZE = 161;
    public const int CC_STUN = 157;
    public const int CC_RELEASE = 199;

    #endregion

    #region CoolTime
    public const int MELE_ATTACK_COOLTIME = 1200;
    public const int RANGE_ATTACK_COOLTIME = 1200;
    #endregion

    #region AnimTime
    public const float HERO_DEAD_ANIMTIME = 1.0f;
    public const float MONSTER_DEAD_ANIMTIME = 1.6f;
    #endregion
}
