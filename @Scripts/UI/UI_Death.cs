using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;
using TMPro;

public class UI_Death : UI_Base
{
    enum Images
    {
        Background,
        UpBarImage
    }

    enum GameObjects
    {
        CloseImageBG,
        ButtonImage
    }

    enum Texts
    {
        CloseText,
        TitleText,
        ButtonText,
    }

    enum Skeletons
    {
        CharImage
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindImages(typeof(Images));
        BindTexts(typeof(Texts));
        BindSkeletons(typeof(Skeletons));
        BindObjects(typeof(GameObjects));

        GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
        GetComponent<Canvas>().worldCamera = Camera.main;
        GetComponent<Canvas>().sortingOrder = 1200;

        return true;
    }

    public void SetInfo(int templateid)
    {
        if (templateid == HERO_OWEN_ID)
        {
            // 죽은 캐릭터 이미지 가져오기 
            SkeletonGraphic skeleton = GetSkeleton((int)Skeletons.CharImage);
            skeleton.skeletonDataAsset = Managers.Resource.Load<SkeletonDataAsset>("020_SkeletonData");
            skeleton.startingAnimation = "dead";
            skeleton.startingLoop = false;
            skeleton.Initialize(true);
            // 죽은 캐릭터 이름써서 텍스트 작성
            GetText((int)Texts.TitleText).text = "오웬 님이 사망했습니다.";
        }
        if (templateid == HERO_ALCHEMIST_ID)
        {
            // 죽인 캐릭터 이미지 가져오기 
            SkeletonGraphic skeleton = GetSkeleton((int)Skeletons.CharImage);
            skeleton.skeletonDataAsset = Managers.Resource.Load<SkeletonDataAsset>("003_SkeletonData");
            skeleton.startingAnimation = "dead";
            skeleton.startingLoop = false;
            skeleton.Initialize(true);
            // 죽인 캐릭터 이름써서 텍스트 작성
            GetText((int)Texts.TitleText).text = "연금술사 님이 사망했습니다.";
        }

        GetObject((int)GameObjects.ButtonImage).BindEvent(evt => Managers.Object.RestartGame());
        GetObject((int)GameObjects.CloseImageBG).BindEvent(evt => Managers.Object.RestartGame());
        
    }


}
