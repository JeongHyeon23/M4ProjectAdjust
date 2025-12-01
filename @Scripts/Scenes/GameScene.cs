using ServerCore;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using static Define;
using UnityEngine;
using Google.Protobuf.Protocol;

public class GameScene : BaseScene
{
    public int Id;

    protected override void Awake()
    {
        base.Awake();
        // 서버 접속
        Managers.Network.Init();
        // 맵 로드
        Managers.Map.LoadMap("TestMap");

        int charid = Managers.Scene.EnterCharId;
        EnterCharacter(charid);

        GameObject joystick =Managers.Resource.Instantiate("UI_Joystick");
        joystick.GetComponent<UI_Joystick>().Init();

    }

    public void EnterCharacter(int id)
    {
        //int posx = Random.Range(-8, 8);
        //int posy = Random.Range(-8, 8);

        if (id == 1)
        {
            C_Entergame entergame = new C_Entergame();
            {
                entergame.ObjInfo = new ObjectInfo();
                entergame.ObjInfo.ObjectId = HERO_ALCHEMIST_ID;
                entergame.ObjInfo.PosInfo = new PositionInfo()
                {
                    Posx = 25,
                    Posy = -15,
                    State = ECreatureState.StateIdle,
                    Dir = EMoveDir.MoveDown
                };
            }
            Managers.Network.Send(entergame);
        }
        if (id == 2)
        {
            C_Entergame entergame = new C_Entergame();
            {
                entergame.ObjInfo = new ObjectInfo();
                entergame.ObjInfo.ObjectId = HERO_OWEN_ID;
                entergame.ObjInfo.PosInfo = new PositionInfo()
                {
                    Posx = 25,
                    Posy = 30,
                    State = ECreatureState.StateIdle,
                    Dir = EMoveDir.MoveDown
                };
            }
            Managers.Network.Send(entergame);
        }
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    void OnApplicationQuit()
    {
        Managers.Resource.Clear();
        Managers.Network.Disconnect();
    }
}
