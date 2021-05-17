using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LRUCache<K, T>
{
    private Dictionary<K, LinkedListNode<KTPair>> dict;

    private LinkedList<KTPair> list;

    private Queue<T> pool;

    private T fetch;

    public int capacity;

    public delegate void OnRemove(K k, T t);

    private event OnRemove onRemove;

    //public delegate T OnUnhit();

    //private OnUnhit onUnhit;

    public LRUCache(int capacity) : this(capacity, false, null)
    {

    }

    public LRUCache(int capacity, bool enablePool, T[] initValue)
    {
        this.capacity = capacity;
        dict = new Dictionary<K, LinkedListNode<KTPair>>(capacity);
        list = new LinkedList<KTPair>();
        if (enablePool)
        {
            pool = new Queue<T>();
            registerOnRemoveMethod(OnRemoveT);
            for (int i = 0; i < capacity; i++)
            {
                pool.Enqueue(initValue[i]);
            }
        }
    }

    private struct KTPair
    {
        public K key;

        public T value;

        public KTPair(K key, T value)
        {
            this.key = key;
            this.value = value;
        }
    }

    public bool TryGetValue(K key, out T value)
    {
        LinkedListNode<KTPair> node;
        if (dict.TryGetValue(key, out node))
        {
            if (node != list.First)
            {
                //Debug.Log("move up " + key);
                list.Remove(node);
                list.AddFirst(node);
            }

            value = node.Value.value;
            return true;
        }
        else
        {
            value = default;
            return false;
        }
    }

    public bool ContainsKey(K key)
    {
        return dict.ContainsKey(key);
    }

    public void Refresh(K key)
    {
        LinkedListNode<KTPair> node;
        if (dict.TryGetValue(key, out node))
        {
            if (node != list.First)
            {
                //Debug.Log("move up " + key);
                list.Remove(node);
                list.AddFirst(node);
            }
        }
    }

    public void Add(K key, T value)
    {
        KTPair kt = new KTPair(key, value);
        list.AddFirst(kt);
        dict.Add(key, list.First);
        if (list.Count > capacity)
        {
            dict.Remove(list.Last.Value.key);
            KTPair p = list.Last.Value;
            list.RemoveLast();
            onRemove(p.key, p.value);
        }
    }

    public void Set(K key, T value)
    {
        LinkedListNode<KTPair> node;
        if (dict.TryGetValue(key, out node))
        {
            if (node != list.First)
            {
                list.Remove(node);
                list.AddFirst(node);
            }
            node.Value = new KTPair(key, value);
        }
    }

    public void Remove(K key)
    {
        LinkedListNode<KTPair> node;
        if (dict.TryGetValue(key, out node))
        {
            dict.Remove(key);
            list.Remove(node);
            onRemove(node.Value.key, node.Value.value);
        }
    }

    public void RemoveLast()
    {
        if (list.Count == 0)
        {
            return;
        }
        LinkedListNode<KTPair> node = list.Last;
        dict.Remove(node.Value.key);
        list.Remove(node);
        onRemove(node.Value.key, node.Value.value);
    }

    public void RemoveLastIfFull()
    {
        if (list.Count == capacity)
        {
            RemoveLast();
        }
    }

    public void Clear()
    {
        List<K> keys = new List<K>(dict.Keys);
        foreach (K k in keys)
        {
            Remove(k);
        }
    }

    public T Fetch()
    {
        RemoveLastIfFull();
        fetch = pool.Dequeue();
        return fetch;
    }

    public void DiscardFetch()
    {
        pool.Enqueue(fetch);
        fetch = default(T);
    }

    public void SubmitFetch(K k)
    {
        Add(k, fetch);
        fetch = default(T);
    }

    public void registerOnRemoveMethod(OnRemove func)
    {
        onRemove += func;
    }

    private void OnRemoveT(K key, T value)
    {
        pool.Enqueue(value);
    }
}