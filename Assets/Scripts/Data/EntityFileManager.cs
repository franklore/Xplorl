using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class EntityPropertyManager
{
    private Dictionary<int, JObject> properties;

    private string filePath;

    private int nextId;

    public EntityPropertyManager(string filePath)
    {
        this.filePath = filePath;
    }

    public int Add(object property)
    {
        JObject j = JObject.FromObject(property);
        properties.Add(nextId, j);
        int r = nextId;
        nextId++;
        return r;
    }

    public void Remove(int entityId)
    {
        properties.Remove(entityId);
    }

    public T GetProperty<T>(int entityId)
    {
        JObject j = (JObject)properties[entityId];
        return j.ToObject<T>();
    }

    [System.Serializable]
    private struct KVPair
    {
        public int id;

        public object property;

        public KVPair(int id, object property)
        {
            this.id = id;
            this.property = property;
        }
    }

    public void Load()
    {
        using (StreamReader reader = new StreamReader(new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Read)))
        {
            string json = reader.ReadToEnd();
            if (json == "")
            {
                properties = new Dictionary<int, JObject>();
            }
            else
            {
                properties = JsonConvert.DeserializeObject<Dictionary<int, JObject>>(json);
            }
        }
        foreach (KeyValuePair<int, JObject> pair in properties)
        {
            if (pair.Key >= nextId)
            {
                nextId = pair.Key + 1;
            }
        }
    }

    public void Save()
    {
        using (StreamWriter writer = new StreamWriter(new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write)))
        {
            
            string json = JsonConvert.SerializeObject(properties);
            Debug.Log(json);
            writer.Write(json);
        }
    }
}