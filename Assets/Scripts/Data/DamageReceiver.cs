using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageReceiver : MonoBehaviour
{
    public void ApplyDamage(Damage damage)
    {
        SendMessage("OnReceiveDamage", damage);
    }
}
