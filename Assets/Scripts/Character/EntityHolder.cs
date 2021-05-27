using System.Collections;
using UnityEngine;

public class EntityHolder : MonoBehaviour
{
    public void Hold(GameObject entity)
    {
        SendMessage("OnHold", entity);
    }
}
