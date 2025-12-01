using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using static Define;

public class UI_Skillcool : UI_Base
{
    enum Images
    {
        SkillABG,
        SkillAImage,
        SkillBBG,
        SkillBImage
    }

    enum Texts
    {
        SkillAText,
        ANameText,
        SkillBText,
        BNameText
    }

    float ARatio;
    float BRatio;

    float ARemaintime;
    float BRemaintime;
 
    
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindImages(typeof(Images));
        BindTexts(typeof(Texts));

        GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
        GetComponent<Canvas>().worldCamera = Camera.main;

        return true;
    }

    // ui 이미지 및 텍스트 설정
    public void SetInfo(int templateid)
    {
        if (templateid == HERO_OWEN_ID)
        {
            // ASKILL 표시
            GetImage((int)Images.SkillAImage).sprite = Managers.Resource.Load<Sprite>("S_Explosive");
            GetImage((int)Images.SkillAImage).color = new Color(1, 1, 1, 1);
            GetImage((int)Images.SkillABG).sprite = Managers.Resource.Load<Sprite>("S_Explosive");
            GetImage((int)Images.SkillABG).color = new Color(0.3f, 0.3f, 0.3f, 1);
            GetText((int)Texts.ANameText).text = "SkillA";
            // BSKILL 표시
            GetImage((int)Images.SkillBImage).sprite = Managers.Resource.Load<Sprite>("S_pet_hit");
            GetImage((int)Images.SkillBImage).color = new Color(1, 1, 1, 1);
            GetImage((int)Images.SkillBBG).sprite = Managers.Resource.Load<Sprite>("S_pet_hit");
            GetImage((int)Images.SkillBBG).color = new Color(0.3f, 0.3f, 0.3f, 1);
            GetText((int)Texts.BNameText).text = "SkillB";
        }
        if (templateid == HERO_ALCHEMIST_ID)
        {
            // ASKILL 표시
            GetImage((int)Images.SkillAImage).sprite = Managers.Resource.Load<Sprite>("S_Green_invade");
            GetImage((int)Images.SkillAImage).color = new Color(1, 1, 1, 1);
            GetImage((int)Images.SkillABG).sprite = Managers.Resource.Load<Sprite>("S_Green_invade");
            GetImage((int)Images.SkillABG).color = new Color(0.3f, 0.3f, 0.3f, 1);
            GetText((int)Texts.ANameText).text = "SkillA";
            // BSKILL 표시
            GetImage((int)Images.SkillBImage).sprite = Managers.Resource.Load<Sprite>("S_chilly_kick");
            GetImage((int)Images.SkillBImage).color = new Color(1, 1, 1, 1);
            GetImage((int)Images.SkillBBG).sprite = Managers.Resource.Load<Sprite>("S_chilly_kick");
            GetImage((int)Images.SkillBBG).color = new Color(0.3f, 0.3f, 0.3f, 1);
            GetText((int)Texts.BNameText).text = "SkillB";
        }
    }

    public void GetCoolInfo()
    {
        ARatio = Managers.Object.MyHero.GetARemainCoolTimeRatio();
        BRatio = Managers.Object.MyHero.GetBRemainCoolTimeRatio();
        ARemaintime = Managers.Object.MyHero.GetARemainCoolTime();
        BRemaintime = Managers.Object.MyHero.GetBRemainCoolTime();
    }

    // 남은 쿨 텍스트로 표현하기
    // 남은 쿨타임 fillamount로 보여주기
    public void ShowUIcool()
    {
        // 내가죽었을때 오류 방지
        if (Managers.Object.MyHero == null)
            return;

        GetCoolInfo();
        if (Managers.Object.MyHero._nextASkillTime == 0)
        {
            GetImage((int)Images.SkillAImage).fillAmount = 1;
            GetText((int)Texts.SkillAText).text = "";
        }
        if (Managers.Object.MyHero._nextBSkillTime == 0)
        {
            GetImage((int)Images.SkillBImage).fillAmount = 1;
            GetText((int)Texts.SkillBText).text = "";
        }
        if (Managers.Object.MyHero._nextASkillTime != 0)
        {
            if (ARatio == 0)
            {
                GetImage((int)Images.SkillAImage).fillAmount = 1;
                GetText((int)Texts.SkillAText).text = "";
                return;
            }
            
            GetImage((int)Images.SkillAImage).fillAmount = ARatio;
            GetText((int)Texts.SkillAText).text = ARemaintime.ToString("F0");
        }
        if (Managers.Object.MyHero._nextBSkillTime != 0)
        {
            if (BRatio == 0)
            {
                GetImage((int)Images.SkillBImage).fillAmount = 1;
                GetText((int)Texts.SkillBText).text = "";
                return; 
            }

            GetImage((int)Images.SkillBImage).fillAmount = BRatio;
            GetText((int)Texts.SkillBText).text = BRemaintime.ToString("F0");
        }
    }

    public void Update()
    {
        ShowUIcool();
    }

}
