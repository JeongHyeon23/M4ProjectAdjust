using Google.Protobuf.Protocol;
using ServerCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using static Define;

public class MapManager
{
    public GameObject Map { get; private set; }
    public string MapName { get; private set; }
    public Grid CellGrid { get; private set; }

    // (CellPos, BaseObject)
    Dictionary<Vector2Int, BaseObject> _cells = new Dictionary<Vector2Int, BaseObject>();

    private int MinX;
    private int MaxX;
    private int MinY;
    private int MaxY;

    public Vector2Int World2Cell(Vector3 worldPos) { return (Vector2Int)CellGrid.WorldToCell(worldPos); }
    public Vector3 Cell2World(Vector2Int cellPos) { return (Vector3.up *0.5f) + CellGrid.CellToWorld((Vector3Int)cellPos); }

    bool[,] _collision;

    public void LoadMap(string mapName)
    {
        //DestroyMap();

        GameObject map = Managers.Resource.Instantiate(mapName);
        map.transform.position = Vector3.zero;
        map.name = $"Map_{mapName}";

        Map = map;
        MapName = mapName;
        CellGrid = map.GetComponent<Grid>();

        ParseCollisionData(map, mapName);
    }

    public void DestroyMap()
    {
        ClearObjects();

        if (Map != null)
            Managers.Resource.Destroy(Map);
    }

    public void ParseCollisionData(GameObject map, string mapName, string tilemap = "Tilemap_Collision")
    {
        GameObject collision = Util.FindChild(map, tilemap, true);
        if (collision != null)
            collision.SetActive(false);

        // collision 파일 관련
        TextAsset txt = Managers.Resource.Load<TextAsset>($"{mapName}Collision");
        StringReader reader = new StringReader(txt.text);

        MinX = int.Parse(reader.ReadLine());
        MaxX = int.Parse(reader.ReadLine());
        MinY = int.Parse(reader.ReadLine());
        MaxY = int.Parse(reader.ReadLine());

        int xCount = MaxX - MinX - 1;
        int yCount = MaxY - MinY - 1;
        _collision = new bool[xCount, yCount];

        for (int y = 0; y < yCount; y++)
        {
            string line = reader.ReadLine();
            for (int x = 0; x < xCount; x++)
            {
                _collision[x, y] = line[x] == '1' ? true : false;       
            }
        }
    }

    public bool MoveTo(BaseObject obj, Vector2Int cellPos, bool forceMove = false)
    {
        if (CanGo(obj, cellPos) == false)
            return false;

        RemoveObject(obj);

        AddObject(obj, cellPos);

        obj.SetCellPos(cellPos, forceMove);

        return true;
    }

    #region Helpers
    public List<T> GatherObjects<T>(Vector3 pos, float rangeX, float rangeY) where T : BaseObject
    {
        HashSet<T> objects = new HashSet<T>();

        Vector2Int left = World2Cell(pos + new Vector3(-rangeX, 0));
        Vector2Int right = World2Cell(pos + new Vector3(+rangeX, 0));
        Vector2Int bottom = World2Cell(pos + new Vector3(0, -rangeY));
        Vector2Int top = World2Cell(pos + new Vector3(0, +rangeY));
        int minX = left.x;
        int maxX = right.x;
        int minY = bottom.y;
        int maxY = top.y;

        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                Vector2Int tilePos = new Vector2Int(x, y);

                // 타입에 맞는 리스트 리턴
                T obj = GetObject(tilePos) as T;
                //if (obj.IsValid() == false)
                //    continue;

                objects.Add(obj);
            }
        }

        return objects.ToList();
    }

    public BaseObject GetObject(Vector2Int cellPos)
    {
        _cells.TryGetValue(cellPos, out BaseObject obj);
        return obj;
    }
    public BaseObject GetObject(Vector3 worldPos)
    {
        Vector2Int cellPos = World2Cell(worldPos);
        return GetObject(cellPos);
    }

    public void RemoveObject(BaseObject obj)
    {
        // 기존 좌표 제거
        // 1. 클라로 움직임 제어하는 myHero경우 
        if (Managers.Object.MyHero.Id == obj.Id)
        {
            Vector2Int PrevcellPos = obj.GetComponent<MyHero>().CellPos;
            BaseObject prev = GetObject(PrevcellPos);
            if (prev != null)
                _cells[PrevcellPos] = null;
            return;
        }
        // 2. 그외 다른 히어로와 몬스터의 경우
        if (obj.ObjectType == EObjectType.Monster || obj.ObjectType == EObjectType.Hero)
        {
            Vector2Int PrevcellPos = obj.CellPos;
            BaseObject prev = GetObject(PrevcellPos);
            if (prev != null)
                _cells[PrevcellPos] = null;
        }
            
        
    }

    public void AddObject(BaseObject obj, Vector2Int cellPos)
    {
        // 새 좌표에 오브젝트 없으면 새 좌표 추가
        // 1. 클라로 움직임 제어하는 myHero경우
        if (Managers.Object.MyHero.Id == obj.Id)
        {
            Vector2Int newPos = cellPos;
            BaseObject newobj = GetObject(newPos);
            if (newobj == null)
                _cells[newPos] = obj;
            return;
        }
        if (obj.ObjectType == EObjectType.Monster || obj.ObjectType == EObjectType.Hero)
        {
            Vector2Int newPos = cellPos;
            BaseObject newobj = GetObject(newPos);
            if (newobj == null)
                _cells[newPos] = obj;
        }
    }
    public void ClearObjects()
    {
        _cells.Clear();
    }
    public bool CanGo(BaseObject self, Vector3 worldpos, bool ignoreObjects = false)
    {
        Vector2Int destcellPos = World2Cell(worldpos);
        return CanGo(self, destcellPos, ignoreObjects);
    }


    public bool CanGo(BaseObject self, Vector2Int cellPos, bool ignoreObjects = false)
    {
        if (cellPos.x < MinX || cellPos.x > MaxX)
            return false;
        if (cellPos.y < MinY || cellPos.y > MaxY)
            return false;

        if (ignoreObjects == false)
        {
            BaseObject obj = GetObject(cellPos);
            if (obj != null && obj != self)
                return false;
        }

        int x = cellPos.x - MinX;
        int y = MaxY - cellPos.y;
        bool type = _collision[x, y];
        if (type == true)
            return true;

        return false;
    }

    public Vector2Int GetFrontCellPos(EMoveDir dir)
    {
        if (dir == EMoveDir.MoveNone)
            return Vector2Int.zero;
        else
        {
            return _delta[(int)dir -1];
        }
    }

    public Vector2Int GetSecondCellPos (EMoveDir dir)
    {
        if (dir == EMoveDir.MoveNone)
            return Vector2Int.zero;
        else
        {
            return 2 * _delta[(int)dir -1];
        }
    }

    #endregion

    #region A*PathFinding
    public struct PQNode : IComparable<PQNode>
    {
        public int H;
        public Vector2Int CellPos;
        public int Depth;

        public int CompareTo(PQNode other)
        {
            if (H == other.H)
                return 0;
            return H < other.H ? 1 : -1;
        }
    }

    //public struct CloseNode : IComparable<CloseNode>
    //{
    //    public int H;
    //    public Vector2Int CellPos;
    //    public int CompareTo(CloseNode other)
    //    {
    //        if (H == other.H)
    //            return 0;
    //        return H < other.H ? 1 : -1;
    //    }
    //}



    List<Vector2Int> _delta = new List<Vector2Int>()
    {
        new Vector2Int(0,1),// u
        new Vector2Int(1,1),// ur
        new Vector2Int(1,0),// r
        new Vector2Int(1,-1),// dr
        new Vector2Int(0,-1),// d
        new Vector2Int(-1,-1),// ld
        new Vector2Int(-1,0),// l
        new Vector2Int(-1,1)// lu
    };

    public List<Vector2Int> FindPath(BaseObject self, Vector2Int startCellPos, Vector2Int destCellPos, int maxDepth = 10)
    {

        // 지금까지 제일 좋은 후보 기록.
        Dictionary<Vector2Int, int> best = new Dictionary<Vector2Int, int>();
        // 경로추적
        Dictionary<Vector2Int, Vector2Int> parent = new Dictionary<Vector2Int, Vector2Int>();

        // 현재 발견된 후보중 제일 좋은후보 찾기
        PriorityQueue<PQNode> pq = new PriorityQueue<PQNode>(); //openlist

        Vector2Int pos = startCellPos;
        Vector2Int dest = destCellPos;

        // destCellPos에 도착 못하더라도 제일 가까운 애로
        Vector2Int closestCellPos = startCellPos;
        int closestH = (dest - pos).sqrMagnitude;

        // 시작점 발견
        {
            int h = (dest - pos).sqrMagnitude;
            pq.Push(new PQNode() { H = h, CellPos = pos, Depth = 1 });
            parent[pos] = pos;
            best[pos] = h;
        }

        while (pq.Count > 0)
        {
            // 제일 좋은 후보를 찾는다
            PQNode node = pq.Pop();
            pos = node.CellPos;

            // 목적지에 도착하면 바로 종료
            if (pos == dest)
                break;

            // 깊이 들어가는거 제한
            if (node.Depth >= maxDepth)
                break;

            foreach (Vector2Int delta in _delta)
            {
                Vector2Int next = pos + delta;

                // 갈수 없으면 스킵
                if (CanGo(self, next) == false)
                    continue;

                // 예약진행
                int h = (dest - next).sqrMagnitude;

                // 더좋은 후보 찾았는지
                if (best.ContainsKey(next) == false)
                    best[next] = int.MaxValue;

                if (best[next] <= h)
                    continue;

                best[next] = h;

                pq.Push(new PQNode() { H = h, CellPos = next, Depth = node.Depth + 1 });
                parent[next] = pos;

                // 목적지 까지 가지못해도 가장 좋았던 후보 기록
                if (closestH > h)
                {
                    closestH = h;
                    closestCellPos = next;
                }
            }
        }

        //제일 가까운 애라도 찾음
        if (parent.ContainsKey(dest) == false)
            return CalcCellPathFromParent(parent, closestCellPos);

        return CalcCellPathFromParent(parent, dest);
    }

    List<Vector2Int> CalcCellPathFromParent(Dictionary<Vector2Int, Vector2Int> parent, Vector2Int dest)
    {
        List<Vector2Int> cells = new List<Vector2Int>();

        if (parent.ContainsKey(dest) == false)
            return cells;

        Vector2Int now = dest;
        while (parent[now] != now)
        {
            cells.Add(now);
            now = parent[now];
        }

        cells.Add(now);
        cells.Reverse();

        return cells;
    }

    #endregion

}