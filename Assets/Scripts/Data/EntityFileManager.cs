using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class EntityPropertyManager
{
    public Dictionary<int, object> properties;

    private string filePath;

    private int nextId;

    public EntityPropertyManager(string filePath)
    {
        this.filePath = filePath;
    }

    public int Add(object property)
    {
        properties.Add(nextId, property);
        int r = nextId;
        nextId++;
        return r;
    }

    public void Remove(int entityId)
    {
        properties.Remove(entityId);
    }

    public void Load()
    {
        using (StreamReader reader = new StreamReader(new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Read)))
        {
            string json = reader.ReadToEnd();
            if (json == "")
            {
                properties = new Dictionary<int, object>();
            }
            else
            {
                properties = JsonUtility.FromJson<Dictionary<int, object>>(json);

            }
        }
        foreach (KeyValuePair<int, object> pair in properties)
        {
            if (pair.Key > nextId)
            {
                nextId = pair.Key;
            }
        }
    }

    public void Save()
    {
        using (StreamWriter writer = new StreamWriter(new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write)))
        {
            string json = JsonUtility.ToJson(properties);
            writer.Write(json);
        }
    }
}