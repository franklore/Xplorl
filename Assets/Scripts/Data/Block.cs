using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Runtime.InteropServices;
using Xplorl.Grid;

public class Block
{
    [StructLayout(LayoutKind.Explicit, Size = 4)]
    struct FourByte
    {
        [FieldOffset(0)]
        public byte b0;
        [FieldOffset(1)]
        public byte b1;
        [FieldOffset(2)]
        public byte b2;
        [FieldOffset(3)]
        public byte b3;

        [FieldOffset(0)]
        public int asInt;

        [FieldOffset(0)]
        public float asFloat;

        [FieldOffset(0)]
        public ushort us0;
        [FieldOffset(2)]
        public ushort us1;
    }

    private FourByte[] properties = new FourByte[propertyNumber];

    public static int propertyNumber = 4;

    public static int ClassSize
    {
        get => propertyNumber * 4;
    }

    private Chunk chunk;

    public Chunk Chunk { get => chunk; set => chunk = value; }


    private Vector3Int blockPosition = -Vector3Int.one;

    public Vector3Int BlockPosition
    {
        get => blockPosition;
        set
        {
            if (blockPosition == -Vector3Int.one)
            {
                blockPosition = value;
            }
            else
            {
                Debug.LogError("setting block's property BlockPosition is forbidden");
            }
        }
    }

    private static Block emptyBlock = new Block();

    public static Block EmptyBlock { get => emptyBlock; }

    public float Health
    {
        get => getPropertyAsFloat(1);
        set => SetPropertyAsFloat(1, value);
    }

    public int Id
    {
        get => properties[0].us0;
        set 
        {
            int oldValid = properties[0].us0 == 0 ? 0 : 1;
            int newValid = value == 0 ? 0 : 1;
            properties[0].us0 = (ushort)value;
            Chunk.UpdateValue(BlockPosition.x, BlockPosition.y, newValid - oldValid);
        }
    }
    public int State
    {
        get => GetPropertyAsUnsignedShort(0, 1);
        set => SetPropertyAsUnsignedShort(0, 1, (ushort)value);
    }

    public Block()
    {

    }

    public Block(int id, int state)
    {
        this.Id = id;
        this.State = state;
    }

    public void SetEmpty()
    {
        Id = 0;
        State = 0;
    }

    public void SetPropertyAsInt(int index, int value)
    {
        properties[index].asInt = value;
        Chunk?.UpdateValue(BlockPosition.x, BlockPosition.y, 0);
    }

    public void SetPropertyAsFloat(int index, float value)
    {
        properties[index].asFloat = value;
        Chunk?.UpdateValue(BlockPosition.x, BlockPosition.y, 0);
    }

    public void SetPropertyAsUnsignedShort(int index, int subIndex, ushort value)
    {
        switch (subIndex)
        {
            case 0: 
                properties[index].us0 = value; 
                break;
            case 1: 
                properties[index].us1 = value; 
                break;
        }
        Chunk?.UpdateValue(BlockPosition.x, BlockPosition.y, 0);
    }

    public int getPropertyAsInt(int index)
    {
        return properties[index].asInt;
    }

    public float getPropertyAsFloat(int index)
    {
        return properties[index].asFloat;
    }

    public ushort GetPropertyAsUnsignedShort(int index, int subIndex)
    {
        switch (subIndex)
        {
            case 0: return properties[index].us0;
            case 1: return properties[index].us1;
            default: return 0;
        }
    }

    public bool IsEmpty()
    {
        return Id == 0;
    }

    public void FromByteArray(byte[] bytes, int offset)
    {
        for (int i = 0; i < propertyNumber; i++)
        {
            properties[i].b0 = bytes[offset + i * 4];
            properties[i].b1 = bytes[offset + 1 + i * 4];
            properties[i].b2 = bytes[offset + 2 + i * 4];
            properties[i].b3 = bytes[offset + 3 + i * 4];
        }
    }

    public void FillByteArray(ref byte[] bytes, int offset)
    {
        for (int i = 0; i < propertyNumber; i++)
        {
            bytes[offset + i * 4] = properties[i].b0;
            bytes[offset + 1 + i * 4] = properties[i].b1;
            bytes[offset + 2 + i * 4] = properties[i].b2;
            bytes[offset + 3 + i * 4] = properties[i].b3;
        }
    }

    public override string ToString()
    {
        return "(" + Id + "," + State + ")";
    }
}
