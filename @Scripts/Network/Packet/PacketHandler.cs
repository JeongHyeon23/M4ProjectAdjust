using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class PacketHandler
{  

    public static void S_EntergameHandler(PacketSession session, IMessage message)
    {
        S_Entergame pkt = message as S_Entergame;
        int objid = pkt.ObjInfo.ObjectId;
        int templateid = Managers.Object.GetTemplateIdById(objid);

        UI_Skillcool skillui = Managers.Resource.Instantiate("UI_Skillcool").GetComponent<UI_Skillcool>();
        skillui.Init();
        skillui.SetInfo(templateid);
        
        Managers.Object.Spawn<Hero>(pkt.ObjInfo, true);
    }
    public static void S_LeavegameHandler(PacketSession session, IMessage message)
    {
        S_Leavegame pkt = message as S_Leavegame;
        // 내용물 다 삭제하는 기능을 ui_death로 이전
        //Managers.Object.Clear();
        int templateid = Managers.Object.MyHero.DataTemplateID;
        UI_Death Deathui = Managers.Resource.Instantiate("UI_Death").GetComponent<UI_Death>();
        Deathui.Init();
        Deathui.SetInfo(templateid);

    }
    public static void S_SpawnHandler(PacketSession session, IMessage message)
    {
        S_Spawn pkt = message as S_Spawn;
        
        foreach (ObjectInfo objinfo in pkt.ObjInfo)
        {
            EObjectType type = Managers.Object.GetObjectTypeById(objinfo.ObjectId);
            switch(type)
            {
                case EObjectType.Hero:
                    Managers.Object.Spawn<Hero>(objinfo);
                    break;
                case EObjectType.Monster:
                    Managers.Object.Spawn<Monster>(objinfo);
                    break;
                case EObjectType.Projectile:
                    Managers.Object.Spawn<Projectile>(objinfo);
                    break;
                case EObjectType.Meleattack:
                    //Managers.Object.Spawn<Meleattack>(objinfo);
                    break;
                case EObjectType.Aoe:
                    Managers.Object.Spawn<AoE>(objinfo);
                    break;
                case EObjectType.Effect:
                    Managers.Object.Spawn<Effect>(objinfo);
                    break;
            }
        }
        

    }
    public static void S_DespawnHandler(PacketSession session, IMessage message)
    {
        S_Despawn pkt = message as S_Despawn;
        
        foreach (int id in pkt.ObjectIds)
        {
            Managers.Object.Remove(id);
        }
    }

    public static void S_MoveHandler(PacketSession session, IMessage message)
    {
        S_Move pkt = message as S_Move;

        GameObject go = Managers.Object.FindById(pkt.ObjectId);
        if (go == null)
            return;

        //if (Managers.Object.MyHero.Id == pkt.ObjectId)
        //    return;

        BaseObject bo = go.GetComponent<BaseObject>();
        if (bo == null)
            return;

        bo.PosInfo = pkt.PosInfo;
    }

    public static void S_SkillHandler(PacketSession session, IMessage message)
    {
        S_Skill pkt = message as S_Skill;

        GameObject go = Managers.Object.FindById(pkt.ObjectId);
        if (go == null)
            return;

        Creature cr = go.GetComponent<Creature>();
        if (cr != null)
        {
            cr.UseSkill(pkt.SkillId, pkt.TargetId);
        }
    }

    public static void S_ChangehpHandler(PacketSession session, IMessage message)
    {
        S_Changehp pkt = message as S_Changehp;
        int id = pkt.ObjectId;
        float damage = pkt.Damage;
        GameObject go = Managers.Object.FindById(id);
        if (go == null)
            return;

        // 타입에 따라 각 객체 stat 설정

        EObjectType type = Managers.Object.GetObjectTypeById(id);
        if (type == EObjectType.Hero)
        {
            if (id == Managers.Object.MyHero.Id)
            {
                MyHero myhero = go.GetComponent<MyHero>();
                myhero.Hp = pkt.Hp;
                Managers.Object.SpawnDamageFont(myhero.UpPosition, myhero.transform, damage);
            }
            else
            {
                Hero hero = go.GetComponent<Hero>();
                hero.Hp = pkt.Hp;
                Managers.Object.SpawnDamageFont(hero.UpPosition, hero.transform, damage);
            }
        }
        else
        {
            Monster monster = go.GetComponent<Monster>();
            monster.Hp = pkt.Hp;
            Managers.Object.SpawnDamageFont(monster.UpPosition, monster.transform, damage);
        }
    }

    public static void S_DieHandler(PacketSession session, IMessage message)
    {
        S_Die pkt = message as S_Die;
      
    }
}
