using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BaseScene : MonoBehaviour
{
    protected virtual void Awake()
    {
        Object obj = GameObject.FindObjectOfType(typeof(EventSystem));
        if (obj == null)
            Managers.Resource.Instantiate("EventSystem").name = "@EventSystem";
    }

    protected virtual void Start() { }
    
    protected virtual void Update() { }

    
}
