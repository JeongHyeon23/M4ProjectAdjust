using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using static Define;

public static class Extension 
{
    public static T GetOrAddComponent<T>(this GameObject go) where T : UnityEngine.Component
    {
        return Util.GetOrAddComponent<T>(go);
    }
    public static void BindEvent(this GameObject go, Action<PointerEventData> action = null, EUIEvent type = EUIEvent.Click)
    {
        UI_Base.BindEvent(go, action, type);
    }
}
