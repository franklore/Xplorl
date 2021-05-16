using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCollector : MonoBehaviour
{
    public float collectRange;

    [SerializeField]
    private float magnetRange;

    public float collectSpeed;

    private CircleCollider2D circle;

    public float MagnetRange
    {
        get { return magnetRange; }
        set
        {
            magnetRange = value;
            circle.radius = magnetRange;
        }
    }

    private void Start()
    {
        circle = gameObject.AddComponent<CircleCollider2D>();
        circle.radius = magnetRange;
        circle.isTrigger = true;
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        Pack pack = GetComponentInParent<Pack>();
        DroppedItem di;
        if (collider.gameObject.TryGetComponent(out di) && di.canPickUp)
        {
            if (Vector3.Distance(collider.transform.position, transform.position) <= collectRange)
            {
                int remain = pack.AddItem(di.Item.id, di.Item.count);
                if (remain > 0)
                {
                    di.Item = new Item(di.Item.id, di.Item.count - remain);
                }
                else
                {
                    Destroy(di.gameObject);
                }
            }
            else
            {
                collider.transform.position += Vector3.Normalize(transform.position - collider.transform.position) * Time.deltaTime * collectSpeed;
            }
        }
    }

}
