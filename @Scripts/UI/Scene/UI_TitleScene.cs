using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Define;

public class UI_TitleScene : UI_Scene
{
    enum Images
    {
        Background,
        Icon,
        StartImage
    }

    enum Texts
    {
        Titletext,
    }

    private enum TitleState
    {
        None,
        AssetLoading,
        AssetLoaded,
        ConnectingToServer,
        ConnectedToServer
    }

    TitleState _state = TitleState.None;
    TitleState State
    {
        get { return _state; }
        set
        {
            _state = value;
            switch(value)
            {
                case TitleState.None:
                    break;
                case TitleState.AssetLoading:
                    GetText((int)Texts.Titletext).text = "에셋로드중~~";
                    break;
                case TitleState.AssetLoaded:
                    GetText((int)Texts.Titletext).text = "에셋 로드 완료~~";
                    break;
                case TitleState.ConnectingToServer:
                    break;
                case TitleState.ConnectedToServer:
                    break;

            }
        }
    }

    public override bool Init()
    {
        if( base.Init() == false )
            return false;

        BindImages(typeof(Images));
        BindTexts(typeof(Texts));
        GetImage((int)Images.StartImage).gameObject.BindEvent(evt =>
        {
            Debug.Log("EnterGameScene");
            Managers.Scene.LoadScene(EScene.LobbyScene);
        });

        return true;
    }

    protected override void Start()
    {
        base.Start();

        State = TitleState.AssetLoading;
        Managers.Resource.LoadAllAsync<Object>("PreLoad", (key, count, totalcount) =>
        {
            Debug.Log($"{key} {count} / {totalcount}");
            if (count == totalcount)
            {
                OnAssetLoaded();
            }
        });


    }

    private void OnAssetLoaded()
    {
        State = TitleState.AssetLoaded;
        Managers.Data.Init();
    }

    #region 어드레서블 타이밍때문에 바꿈
    // 플레이 모드와 달리 빌드시 에셋 로드못하고 넘어가는 문제 발생
    //public void StartLoadAsset()
    //{
    //    Managers.Resource.LoadAllAsync<Object>("PreLoad", (key, count, totalcount) =>
    //    {
    //        Debug.Log($"{key} {count} / {totalcount}");
    //        if (count == totalcount)
    //        {
    //            Managers.Data.Init();

    //            //// 데이터 있는지 확인
    //            //if (Managers.Game.LoadGame() == false)
    //            //{
    //            //    Managers.Game.InitGame();
    //            //    Managers.Game.SaveGame();
    //            //}
    //            GetText((int)Texts.Titletext).text = "Touch To Start!";
    //        }
    //    });
    //}
    #endregion




}
