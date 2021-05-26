using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EntityItemController : MonoBehaviour
{
    public abstract void FireDown();

    public abstract void FireUp();

    public abstract void SetProperty(object properties);

    public abstract object GetProperty();

    public abstract object CreateProperty();
}
