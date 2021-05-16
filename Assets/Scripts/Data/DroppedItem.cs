using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedItem : MonoBehaviour
{
    private Item item;

    public SpriteRenderer sr;

    public GameObject go;

    public bool canPickUp = false;

    public float throwAmplitude = 5;

    public Item Item { get => item; 
        set
        { 
            item = value;
            ItemObject io = ItemObjectFactory.Instance.GetItemObject(Item.id);
            sr.sprite = io.sprite;
        }
    }

    private void Start()
    {
        float value = Random.Range(60, 120) * Mathf.Deg2Rad;
        StartCoroutine(ThrowUp(new Vector2(Mathf.Cos(value), Mathf.Sin(value)) * throwAmplitude));
    }

    private IEnumerator ThrowUp(Vector2 initSpeed) 
    {
        float t = 0;
        float x0 = transform.position.x, y0 = go.transform.localPosition.y;
        while (go.transform.localPosition.y >= y0)
        {
            t += Time.deltaTime;
            transform.position += new Vector3(initSpeed.x * Time.deltaTime, 0, 0);
            go.transform.localPosition = new Vector3(0, y0 + initSpeed.y * t - 10 * t * t, 0);
            yield return null;
        }
        go.transform.localPosition = new Vector3(0, y0, 0);
        canPickUp = true;
    }
        
}
