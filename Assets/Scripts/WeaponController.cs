using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public Vector3 target;

    public GameObject bullet;

    public Transform Muzzle;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 lookRotation = new Vector3(target.x - transform.position.x, target.y - transform.position.y, 0);
        float angle = Mathf.Atan2(lookRotation.y, lookRotation.x) * Mathf.Rad2Deg;
        transform.localScale = new Vector3(1, lookRotation.x > 0 ? 1 : -1, 1); 
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    public virtual void Fire()
    {
        Instantiate(bullet, Muzzle.position, transform.rotation);
    }
}
