using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerEX
{
    //public BaseScene CurrentScene { get { return GameObject.FindObjectOfType<BaseScene>(); } }
    public int EnterCharId {  get; set; }
    public void LoadScene(Define.EScene type)
    {
        SceneManager.LoadScene(GetSceneName(type));
    }

    public string GetSceneName(Define.EScene type)
    {
        string name = System.Enum.GetName(typeof(Define.EScene), type);
        return name;
    }

    public void Clear()
    {
        //CurrentScene.Clear();
    }
}