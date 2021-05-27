using System;
using System.Collections;
using UnityEngine;

public class GunController : EntityItemController
{
    public Vector3 target;

    public GameObject bullet;

    public Transform Muzzle;

    public GunController()
    {
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 lookRotation = new Vector3(target.x - transform.position.x, target.y - transform.position.y, 0);
        float angle = Mathf.Atan2(lookRotation.y, lookRotation.x) * Mathf.Rad2Deg;
        transform.localScale = new Vector3(1, lookRotation.x > 0 ? 1 : -1, 1);
        transform.rotation = Quaternion.Euler(0, 0, angle);

        Vector3 weaponPositionOffset = new Vector3(lookRotation.x, lookRotation.y, 0).normalized * 0.1f;
        transform.localPosition = weaponPositionOffset;

    }

    public override void FireDown(ItemOperationInfo info)
    {
        Instantiate(bullet, Muzzle.position, transform.rotation);
    }

    public override void FireUp(ItemOperationInfo info)
    {
        
    }

    public override object CreateProperty()
    {
        return 0;
    }

    public override void Select(ItemOperationInfo info)
    {
        
    }

    public override void Deselect(ItemOperationInfo info)
    {
        
    }
}
