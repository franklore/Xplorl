using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityItemController : MonoBehaviour
{
    public virtual void FireDown(ItemOperationInfo info)
    {

    }

    public virtual void FireUp(ItemOperationInfo info)
    {

    }

    public virtual void Select(ItemOperationInfo info)
    {

    }

    public virtual void Deselect(ItemOperationInfo info)
    {

    }

    public virtual object CreateProperty()
    {
        return null;
    }

    public virtual string GetDescription(Item item)
    {
        return "";
    }
}
