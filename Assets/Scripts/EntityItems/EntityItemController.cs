using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EntityItemController : MonoBehaviour
{
    public abstract void FireDown(ItemOperationInfo info);

    public abstract void FireUp(ItemOperationInfo info);

    public abstract void Select(ItemOperationInfo info);

    public abstract void Deselect(ItemOperationInfo info);

    public abstract void SetProperty(object properties);

    public abstract object GetProperty();

    public abstract object CreateProperty();

    public abstract System.Type GetPropertyType();
}
