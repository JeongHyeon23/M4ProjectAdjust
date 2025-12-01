using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class UI_LobbyScene : UI_Scene
{
    enum Images
    {
        Background,
        Charselectbox,
        CharImage1,
        CharImage2,
    }
    enum Skeletons
    {
        AlchImage,
        OwenImage,
    }

    enum Texts
    {
        NameText1,
        NameText2,
        TitleText
    }
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindImages(typeof(Images));
        BindTexts(typeof(Texts));
        BindSkeletons(typeof(Skeletons));
        GetImage((int)Images.CharImage1).gameObject.BindEvent(evt =>
        {
            Debug.Log("Enter Alchemist");
            Managers.Scene.EnterCharId = 1;
            Managers.Scene.LoadScene(EScene.GameScene);

        });
        GetImage((int)Images.CharImage2).gameObject.BindEvent(evt =>
        {
            Debug.Log("Enter Owen");
            Managers.Scene.EnterCharId = 2;
            Managers.Scene.LoadScene(EScene.GameScene);

        });

        return true;
    }


}
