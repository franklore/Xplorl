using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class MeleeWeaponController : EntityItemController
{
    private Animator anim;

    private float _cooldown;

    public float cooldown {
        get => _cooldown;
        set
        {
            _cooldown = value;
            anim.speed = 1 / cooldown;
        }
    }

    private float timeAfterLastAttack;

    public bool repeat;

    public float damage;

    public float attackRange;

    private bool fireDown;

    private SpriteRenderer sr;

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

        public float attackRange;
    }

    public override void Select(ItemOperationInfo info)
    {
        MeleeWeaponProperties property = BlockMap.Instance.GetEntityProperty<MeleeWeaponProperties>(info.item.entityId);
        SetProperty(property);
    }

    public override void Deselect(ItemOperationInfo info)
    {
        Destroy(gameObject);
    }

    public void SetProperty(object property)
    {
        MeleeWeaponProperties melee = (MeleeWeaponProperties)property;
        cooldown = melee.coolDown;
        repeat = melee.repeat;
        damage = melee.damage;
        attackRange = melee.attackRange;
    }

    public MeleeWeaponProperties GetProperty()
    {
        MeleeWeaponProperties properties = new MeleeWeaponProperties();
        properties.coolDown = cooldown;
        properties.repeat = repeat;
        properties.damage = damage;
        properties.attackRange = attackRange;
        return properties;
    }

    public override object CreateProperty()
    {
        MeleeWeaponProperties properties = new MeleeWeaponProperties();
        properties.coolDown = Random.Range(0.35f, 0.45f);
        properties.repeat = true;
        properties.damage = 40;
        properties.attackRange = 2;
        return properties;
    }

    public override string GetDescription(Item item)
    {
        MeleeWeaponProperties property = BlockMap.Instance.GetEntityProperty<MeleeWeaponProperties>(item.entityId);

        return "Pickaxe\n" +
            "damage " + property.damage + "\n" +
            "speed " + 1 / property.coolDown + "\n" +
            (property.repeat ? "auto fire\n" : "");
    }

    public virtual void Fire()
    {
        sr.enabled = true;
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
        sr = GetComponent<SpriteRenderer>();
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
    }
}
