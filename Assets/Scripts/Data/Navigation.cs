using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class Navigation
{
    private class Node
    {
        public Vector3Int pos;

        public float fromCost;

        public float toCost;

        public Node parent;

        public Node(Vector3Int pos, float fromCost, float toCost, Node parent)
        {
            this.pos = pos;
            this.fromCost = fromCost;
            this.toCost = toCost;
            this.parent = parent;
        }
    }

    public static Vector3Int[] FindPath(Vector3Int from, Vector3Int to, CostMap costs)
    {
        Dictionary<Vector3Int, Node> openList = new Dictionary<Vector3Int, Node>();
        HashSet<Vector3Int> closeList = new HashSet<Vector3Int>();
        Node cur = new Node(from, 0, cost(from, to), null);
        openList.Add(from, cur);
        int maxEpoch = 10000;
        for (int epoch = 0; epoch < maxEpoch; epoch++)
        {

            if (openList.Count == 0)
            {
                return null;
            }
            Node bestNode = null;
            foreach (KeyValuePair<Vector3Int, Node> node in openList)
            {
                if (bestNode == null || node.Value.fromCost + node.Value.toCost < bestNode.fromCost + bestNode.toCost)
                {
                    bestNode = node.Value;
                }
            }
            cur = bestNode;
            openList.Remove(bestNode.pos);
            closeList.Add(cur.pos);
            for (int x = -1; x <= 1; x++)
                for (int y = -1; y <= 1; y++)
                {
                    Vector3Int pos = cur.pos + new Vector3Int(x, y, 0);
                    if (pos == to)
                    {
                        cur = new Node(pos, 0, 0, cur);

                        List<Vector3Int> path = new List<Vector3Int>();
                        while (cur != null)
                        {
                            path.Add(cur.pos);
                            cur = cur.parent;
                        }
                        path.Reverse();
                        return path.ToArray();
                    }
                    if (!closeList.Contains(pos) && !openList.ContainsKey(pos) && canPass(pos, costs))
                    {
                        openList.Add(pos, new Node(pos, cur.fromCost + cost(Vector3Int.zero, new Vector3Int(x, y, 0)), cost(pos, to), cur));
                    }
                }
        }


        return null;
    }

    private static float cost(Vector3Int from, Vector3Int to)
    {
        int x = Mathf.Abs(from.x - to.x);
        int y = Mathf.Abs(from.y - to.y);
        int l = Mathf.Max(x, y);
        int s = Mathf.Min(x, y);
        return Mathf.Max(x, y) + 0.5f * Mathf.Min(x, y);
    }

    private static bool canPass(Vector3Int pos, CostMap costs)
    {
        return costs[pos.x, pos.y] < 1;
    }

    public static CostMap CreateCostMapFromBlockMap(Vector3Int from, Vector3Int to, int areaExpand)
    {
        Profiler.BeginSample("CreateCostMap");
        Vector3Int fromChunkPos, fromBlockPos;
        Chunk.SplitPosition(from, out fromChunkPos, out fromBlockPos);
        Vector3Int toChunkPos, toBlockPos;
        Chunk.SplitPosition(to, out toChunkPos, out toBlockPos);
        int chunkxMin = Mathf.Min(fromChunkPos.x, toChunkPos.x) - areaExpand;
        int chunkxMax = Mathf.Max(fromChunkPos.x, toChunkPos.x) + areaExpand;
        int chunkyMin = Mathf.Min(fromChunkPos.y, toChunkPos.y) - areaExpand;
        int chunkyMax = Mathf.Max(fromChunkPos.y, toChunkPos.y) + areaExpand;
        Vector3Int leftDown = Chunk.CombinePosition(new Vector3Int(chunkxMin, chunkyMin, 0), new Vector3Int(0, 0, 0));
        CostMap costs = new CostMap((chunkxMax - chunkxMin + 1) * Chunk.chunkSize, (chunkyMax - chunkyMin + 1) * Chunk.chunkSize, leftDown);
        for (int cx = chunkxMin; cx <= chunkxMax; cx++)
            for (int cy = chunkyMin; cy <= chunkyMax; cy++)
            {
                for (int bx = 0; bx < Chunk.chunkSize; bx++)
                    for (int by = 0; by < Chunk.chunkSize; by++)
                    {
                        Vector3Int pos = Chunk.CombinePosition(new Vector3Int(cx, cy, 0), new Vector3Int(bx, by, 0));
                        Block b = BlockMap.Instance.GetTopBlock(pos);
                        BlockObject bo = BlockFactory.Instance.GetBlockObject(b);
                        if (bo.isCollider)
                        {
                            costs[pos.x, pos.y] = 1;
                        }
                    }
            }
        Profiler.EndSample();
        return costs;
    }
}

public class CostMap
{
    private float[,] costs;

    private Vector3Int leftDownPos;

    public float this[int x, int y]
    {
        get
        {
            return costs[x - leftDownPos.x, y - leftDownPos.y];
        }
        set
        {
            costs[x - leftDownPos.x, y - leftDownPos.y] = value;
        }
    }

    public CostMap(int width, int height, Vector3Int leftDownPos)
    {
        costs = new float[width, height];
        this.leftDownPos = leftDownPos;
    }
}