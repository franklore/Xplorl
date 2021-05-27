using UnityEngine;
using System.Collections;

public class PickaxeController : MeleeWeaponController
{
    public override void Fire()
    {
        base.Fire();
        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int pos = Vector3Int.FloorToInt(mouse);
        Vector3 centerPos = pos + new Vector3(0.5f, 0.5f, 0);
        if (Vector2.Distance(centerPos, transform.parent.position) < attackRange)
        {
            Debug.Log("Attack");
            GameObject block = BlockMap.Instance.GetTopBlockGameObject(pos);
            if (block != null)
            {
                DamageReceiver dr;
                if (block.TryGetComponent(out dr))
                {
                    Damage damage;
                    damage.value = this.damage;
                    dr.ApplyDamage(damage);
                }
            }
        }

    }
}