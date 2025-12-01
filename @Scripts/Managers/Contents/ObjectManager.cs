using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Define;

public class ObjectManager
{
    public MyHero MyHero { get; set; }
    public Dictionary<int, GameObject> _objects = new Dictionary<int, GameObject>();

    #region Roots
    public Transform GetRootTrasnform(string name)
    {
        GameObject root = GameObject.Find(name);
        if (root == null)
            root = new GameObject() { name = name };

        return root.transform;
    }

    public Transform HeroRoot { get { return GetRootTrasnform("@Heroes"); } }
    public Transform MonsterRoot { get { return GetRootTrasnform("@Monsters"); } }
    public Transform ProjectileRoot { get { return GetRootTrasnform("@Projectiles"); } }
    public Transform DamageFont { get { return GetRootTrasnform("@DamageFonts"); } }
    public Transform AoeRoot { get { return GetRootTrasnform("@Aoes"); } }
    public Transform EffectRoot { get { return GetRootTrasnform("@Effects"); } }
    #endregion

    public EObjectType GetObjectTypeById(int id)
    {
        int type = (id >> 28) & 0x7F;
        return (EObjectType)type;
    }

    
    public int GetTemplateIdById(int id)
    {
        int templateid = (id >> 20) & 0xFF;
        return templateid;
    }

    public void SpawnDamageFont(Vector2 position ,Transform parent, float damage)
    {
        if (parent == null)
            return;

        GameObject go = Managers.Resource.Instantiate("DamageFont", parent);
        DamageFont txt = go.GetComponent<DamageFont>();
        txt.SetInfo(position, damage, parent);

    }

    public T Spawn<T>(ObjectInfo objinfo, bool myplayer = false) where T : BaseObject
    {
        if (objinfo.ObjectId == 0)
            return null;

        EObjectType type = GetObjectTypeById(objinfo.ObjectId);
        int templateId = GetTemplateIdById(objinfo.ObjectId);
        string prefabName = "";
        if (myplayer)
        {
            prefabName = "MyHero";
        }
        else
        {
            prefabName = typeof(T).Name;
        }
            
        GameObject go = Managers.Resource.Instantiate(prefabName);
        go.name = prefabName + '_' + objinfo.ObjectId;

        if (type == EObjectType.Hero)
        {
            if (myplayer)
            {
                go.transform.parent = HeroRoot;
                MyHero = go.GetComponent<MyHero>();
                MyHero.Id = objinfo.ObjectId;
                _objects.Add(objinfo.ObjectId, go);
                MyHero.Stat = objinfo.StatInfo;
                MyHero.SetInfo(templateId);
                MyHero.CreatureState = objinfo.PosInfo.State;
                MyHero.PosInfo = objinfo.PosInfo;
                MyHero.SetCellPos(MyHero.CellPos, true);
               
            }
            else
            {
                go.transform.parent = HeroRoot;
                Hero hero = go.GetComponent<Hero>();
                hero.Id = objinfo.ObjectId;
                _objects.Add(objinfo.ObjectId ,go);
                hero.Stat = objinfo.StatInfo;
                hero.SetInfo(templateId);
                hero.CreatureState = objinfo.PosInfo.State;
                hero.PosInfo = objinfo.PosInfo;
                hero.SetCellPos(hero.CellPos, true);
            }
           
        }
        else if (type == EObjectType.Monster)
        {
            go.transform.parent = MonsterRoot;
            Monster monster = go.GetComponent<Monster>();
            monster.Id = objinfo.ObjectId;
            _objects.Add(objinfo.ObjectId, go);
            monster.Stat = objinfo.StatInfo;
            monster.SetInfo(templateId);
            monster.PosInfo = objinfo.PosInfo;
            monster.SetCellPos(monster.CellPos, true);
        }
        else if (type == EObjectType.Projectile)
        {
            go.transform.parent = ProjectileRoot;
            Projectile projectile = go.GetComponent<Projectile>();
            projectile.Id = objinfo.ObjectId;
            _objects.Add(objinfo.ObjectId, go);
            projectile.SetInfo(objinfo.TargetId);
            projectile.PosInfo = objinfo.PosInfo;
            projectile.SetCellPos(projectile.CellPos, true);

        }
        else if (type == EObjectType.Aoe)
        {
            go.transform.parent = AoeRoot;
            AoE aoe = go.GetComponent<AoE>();
            aoe.Id = objinfo.ObjectId;
            _objects.Add(objinfo.ObjectId, go);
            aoe.SetInfo(objinfo.ObjectId);
            aoe.PosInfo = objinfo.PosInfo;
            aoe.SetCellPos(aoe.CellPos, true);
        }
        else if (type == EObjectType.Effect)
        {
            go.transform.parent = EffectRoot;
            Effect eft = go.GetComponent<Effect>();
            eft.Id = objinfo.ObjectId;
            _objects.Add(objinfo.ObjectId, go);
            eft.SetInfo(objinfo.TargetId);
            eft.PosInfo = objinfo.PosInfo;
            eft.SetCellPos(eft.CellPos, true);
        }

        return go as T;
    }

    public void Remove(int id, bool mapremove = true)
    {
        GameObject go = FindById(id);
        if (go == null)
            return;

        // 맵매니저에 저장된 오브젝트 제거
        if (mapremove)
        {
            BaseObject bo = go.GetComponent<BaseObject>();
            Managers.Map.RemoveObject(bo);
        }
        
        _objects.Remove(id);
        Managers.Resource.Destroy(go);
    }
    public Creature FindbyIdCreatrue(int id)
    {
        return FindById(id).GetComponent<Creature>();
    }

    public GameObject FindById (int id)
    {
        GameObject go = null;
        if (_objects.TryGetValue(id, out go))
            return go;
        return null;
    }

    public GameObject Find(Func<GameObject, bool> condition)
    {
        foreach (GameObject go in _objects.Values)
        {
            if (condition.Invoke(go))
                return go;
        }

        return null;
    }

    public void RestartGame()
    {
        Managers.Resource.Clear();
        foreach (GameObject go in _objects.Values)
            Managers.Resource.Destroy(go);
        _objects.Clear();
        MyHero = null;
        Managers.Map.DestroyMap();
        //Managers.Network.Disconnect();
        Managers.Scene.LoadScene(EScene.TitleScene);
    }
}

