using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapFileTest : MonoBehaviour
{
    public Tilemap map;

    // Start is called before the first frame update
    void Start()
    {
        //map.BoxFill(Vector3Int.zero, null, 0, 0, 5, 5);

        //MapFileManager manager = new MapFileManager("test");
        //manager.CreateMap();
        //manager.LoadMap();
        //Chunk chunk = new Chunk(new Vector3Int(0, 0, 0));
        //for (int col = 0; col < Chunk.chunkSize; col++)
        //{
        //    for (int row = 0; row < Chunk.chunkSize; row++)
        //    {
        //        chunk[row, col] = new Block((byte)row, 0);
        //    }
        //}
        //manager.WriteChunk(chunk);

        //chunk = new Chunk(new Vector3Int(0, 1, 0));
        //for (int col = 0; col < Chunk.chunkSize; col++)
        //{
        //    for (int row = 0; row < Chunk.chunkSize; row++)
        //    {
        //        chunk[row, col] = new Block((byte)1, 0);
        //    }
        //}
        //manager.WriteChunk(chunk);

        //chunk = new Chunk(new Vector3Int(1, 0, 0));
        //for (int col = 0; col < Chunk.chunkSize; col++)
        //{
        //    for (int row = 0; row < Chunk.chunkSize; row++)
        //    {
        //        chunk[row, col] = new Block((byte)2, 0);
        //    }
        //}
        //manager.WriteChunk(chunk);

        //Chunk readChunk = manager.ReadChunk(new Vector3Int(1, 0, 0));
        //readChunk[5, 5].Id = 6;
        //manager.WriteChunk(readChunk);
        
        //manager.Flush();
        //manager.Dispose();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
