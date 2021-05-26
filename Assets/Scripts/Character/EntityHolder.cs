using System.Collections;
using UnityEngine;

public class EntityHolder : MonoBehaviour
{
    public GameObject entity;

    public void Hold(GameObject entity)
    {
        SendMessage("OnHold", entity);
    }
}
