using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class MeleeWeaponController : EntityItemController
{
    private Animator anim;

    public float cooldown;

    private float timeAfterLastAttack;

    public bool repeat;

    public float damage;

    private bool fireDown;

    private SpriteRenderer sr;

    private Collider2D co;

    public GameObject hitFx;

    public override void FireDown(ItemOperationInfo info)
    {

        if (repeat)
        {
            fireDown = true;
        }
        else
        {
            if (timeAfterLastAttack >= cooldown)
            {
                Fire();
            }
        }
    }

    public override void FireUp(ItemOperationInfo info)
    {
        fireDown = false;
    }

    [System.Serializable]
    public struct MeleeWeaponProperties
    {
        public float coolDown;

        public bool repeat;

        public float damage;
    }

    public override System.Type GetPropertyType()
    {
        return typeof(MeleeWeaponProperties);
    }

    public override void Select(ItemOperationInfo info)
    {
        object property = BlockMap.Instance.GetEntityProperty<MeleeWeaponProperties>(info.item.entityId);
        SetProperty(property);
    }

    public override void Deselect(ItemOperationInfo info)
    {
        Destroy(gameObject);
    }

    public override void SetProperty(object property)
    {
        MeleeWeaponProperties melee = (MeleeWeaponProperties)property;
        cooldown = melee.coolDown;
        repeat = melee.repeat;
        damage = melee.damage;
    }

    public override object GetProperty()
    {
        MeleeWeaponProperties properties = new MeleeWeaponProperties();
        properties.coolDown = cooldown;
        properties.repeat = repeat;
        properties.damage = damage;
        return properties;
    }

    public override object CreateProperty()
    {
        MeleeWeaponProperties properties = new MeleeWeaponProperties();
        properties.coolDown = 0.4f;
        properties.repeat = true;
        properties.damage = 40;
        return properties;
    }

    public void Fire()
    {
        sr.enabled = true;
        co.enabled = true;
        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 lookRotation = mouse - transform.position;
        float angle = Mathf.Atan2(lookRotation.y, lookRotation.x) * Mathf.Rad2Deg;
        anim.SetFloat("facex", lookRotation.x);
        anim.SetFloat("facey", lookRotation.y);
        if (angle > -45 && angle <= 45)
        {
            anim.Play("AttackRight");
        }
        else if (angle > 45 && angle <= 135)
        {
            anim.Play("AttackUp");
        }
        else if (angle > 135 || angle <= -135)
        {
            anim.Play("AttackLeft");
        }
        else
        {
            anim.Play("AttackDown");
        }
        timeAfterLastAttack = 0;
    }

    private void Awake()
    {
        anim = GetComponent<Animator>();
        anim.speed = 1 / cooldown;
        sr = GetComponent<SpriteRenderer>();
        co = GetComponent<Collider2D>();
    }

    // Use this for initialization
    void Start()
    {
        sr.enabled = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 lookRotation = new Vector3(target.x - transform.position.x, target.y - transform.position.y, 0);
        Vector3 weaponPositionOffset = new Vector3(lookRotation.x, lookRotation.y, 0).normalized * 0.3f;
        transform.localPosition = weaponPositionOffset;

        timeAfterLastAttack += Time.deltaTime;
        if (repeat && fireDown && timeAfterLastAttack >= cooldown)
        {
            Fire();
        }
    }

    private void FireAnimationEnd()
    {
        Debug.Log("animation end");
        sr.enabled = false;
        co.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Attack");
        Damage damage;
        damage.value = this.damage;
        DamageReceiver dr;
        if (collision.gameObject.TryGetComponent(out dr))
        {
            dr.ApplyDamage(damage);
            Vector3 randomOffset = Random.insideUnitCircle;
            Instantiate(hitFx, collision.transform.position + randomOffset, Quaternion.identity);
        }
    }
}
