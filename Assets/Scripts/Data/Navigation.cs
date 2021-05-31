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

        public float cost { get => fromCost + toCost; }

        public Node parent;

        public Node(Vector3Int pos, float fromCost, float toCost, Node parent)
        {
            this.pos = pos;
            this.fromCost = fromCost;
            this.toCost = toCost;
            this.parent = parent;
        }

        public override string ToString()
        {
            return "" + pos + fromCost + "+" + toCost + "=" + cost;
        }
    }

    public static Vector3Int[] FindPath(Vector3Int from, Vector3Int to, CostMap costs)
    {
        Profiler.BeginSample("FindPath");
        Dictionary<Vector3Int, Node> openList = new Dictionary<Vector3Int, Node>();
        HashSet<Vector3Int> closeList = new HashSet<Vector3Int>();
        Node cur = new Node(from, 0, cost(from, to), null);
        openList.Add(from, cur);
        int maxEpoch = 10000;
        for (int epoch = 0; epoch < maxEpoch; epoch++)
        {
            //DrawImage(new List<Vector3Int>(openList.Keys).ToArray(), new List<Vector3Int>(closeList).ToArray(), costs, "./pathFinding/map" + epoch + ".png");
            if (openList.Count == 0)
            {
                Profiler.EndSample();
                return null;
            }
            Node bestNode = null;
            foreach (KeyValuePair<Vector3Int, Node> node in openList)
            {
                if (bestNode == null || node.Value.cost < bestNode.cost || node.Value.cost == bestNode.cost && node.Value.toCost < bestNode.toCost)
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
                        Profiler.EndSample();
                        return path.ToArray();
                    }
                    if (!closeList.Contains(pos) && canPass(pos, costs))
                    {
                        if (openList.ContainsKey(pos))
                        {
                            openList[pos].fromCost = Mathf.Min(cur.fromCost + cost(Vector3Int.zero, new Vector3Int(x, y, 0)), openList[pos].fromCost);
                        }
                        else
                        {
                            openList.Add(pos, new Node(pos, cur.fromCost + cost(Vector3Int.zero, new Vector3Int(x, y, 0)), cost(pos, to), cur));
                        }
                    }
                }
        }
        Profiler.EndSample();
        return null;
    }

    public static void DrawImage(Vector3Int[] openList, Vector3Int[] closeList, CostMap costs, string path)
    {
        Texture2D texture = new Texture2D(costs.size.x, costs.size.y);
        for (int x = costs.leftDownPos.x; x < costs.leftDownPos.x + costs.size.x; x++)
            for (int y = costs.leftDownPos.y; y < costs.leftDownPos.y + costs.size.y; y++)
            {
                texture.SetPixel(x - costs.leftDownPos.x, y - costs.leftDownPos.y, costs[x, y] > 0.5f ? Color.black : Color.white);
            }
        for (int i = 0; i < openList.Length; i++)
        {
            texture.SetPixel(openList[i].x - costs.leftDownPos.x, openList[i].y - costs.leftDownPos.y, Color.blue);
        }
        for (int i = 0; i < closeList.Length; i++)
        {
            texture.SetPixel(closeList[i].x - costs.leftDownPos.x, closeList[i].y - costs.leftDownPos.y, Color.red);
        }
        Texture2D outTexture = new Texture2D(costs.size.x * 16, costs.size.y * 16);
        for (int x = 0; x < costs.size.x * 16; x++)
            for (int y = 0; y < costs.size.y * 16; y++)
            {
                outTexture.SetPixel(x, y, texture.GetPixel(x / 16, y / 16));
            }
        using (System.IO.BinaryWriter writer = new System.IO.BinaryWriter(new System.IO.FileStream(path, System.IO.FileMode.Create, System.IO.FileAccess.Write)))
        {
            writer.Write(outTexture.EncodeToPNG());
        }
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

    public Vector3Int leftDownPos { get; private set; }

    public Vector3Int size { get; private set; }

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
        size = new Vector3Int(width, height, 0);
        this.leftDownPos = leftDownPos;
    }
}