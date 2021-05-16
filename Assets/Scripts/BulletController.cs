using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BulletController : MonoBehaviour
{
    public float speed;

    public float lifetime;

    private float life = 0;

    public GameObject owner;

    public GameObject effect;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        life += Time.deltaTime;
        if (life > lifetime)
        {
            Destroy(gameObject);
        }
        transform.Translate(transform.right * speed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        DamageReceiver damageReceiver;
        if (collision.TryGetComponent(out damageReceiver) && damageReceiver.enabled)
        {
            Damage damage = new Damage();
            damage.value = 100;
            damageReceiver.ApplyDamage(damage);
            Instantiate(effect, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
