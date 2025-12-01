using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Managers : MonoBehaviour
{
    public static bool Initialized { get; set; } = false;
    static Managers _manager;
    public static Managers Instance { get { Init(); return _manager; } }

    #region Contents
    ObjectManager _obj = new ObjectManager();
    NetworkManager _net = new NetworkManager();
    GameManager _game = new GameManager();
    MapManager _map = new MapManager();

    public static ObjectManager Object { get { return Instance._obj; } }
    public static NetworkManager Network { get { return Instance._net; } }
    public static GameManager Game { get { return Instance._game; } }
    public static MapManager Map { get { return Instance._map; } }
    #endregion

    #region Core
    UIManager _ui = new UIManager();
    SceneManagerEX _scene = new SceneManagerEX();
    ResourceManager _resource = new ResourceManager();
    PoolManager _pool = new PoolManager();
    DataManager _data = new DataManager();

    public static SceneManagerEX Scene { get { return Instance._scene; } }
    public static ResourceManager Resource { get { return Instance._resource; } }
    public static PoolManager Pool { get { return Instance._pool; } }
    public static UIManager UI { get { return Instance._ui; } }
    public static DataManager Data { get { return Instance._data; } }


    #endregion

    void Update()
    {
        Managers.Network.Update();
    }

    public static void Init()
    {
        if(_manager == null && Initialized == false)
        {
            Initialized = true;
            GameObject go = GameObject.Find("@Managers");
            if (go == null)
            {
                go = new GameObject() { name = "@Managers" };
                go.AddComponent<Managers>();
            }

            DontDestroyOnLoad(go);

            _manager = go.GetComponent<Managers>();
            //_manager._net.Init();
        }
    }


}
