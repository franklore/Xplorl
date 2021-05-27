using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Entity Item Object")]
public class EntityItemObject : ItemObject
{
    public GameObject EntityPrefab;

    public object properties;
   
    public override void UseItemStart(ItemOperationInfo info)
    {
        EntityItemController eic = info.entity.GetComponent<EntityItemController>();
        eic.FireDown(info);
    }

    public override void UseItemEnd(ItemOperationInfo info)
    {
        EntityItemController eic = info.entity.GetComponent<EntityItemController>();
        eic.FireUp(info);
    }

    public override void SelectItem(ItemOperationInfo info)
    {
        GameObject entity = Instantiate(EntityPrefab);
        EntityItemController eic = entity.GetComponent<EntityItemController>();
        eic.Select(info);
        EntityHolder eh;
        if (info.invoker.TryGetComponent(out eh))
        {
            eh.Hold(entity);
        }
    }

    public override void DeselectItem(ItemOperationInfo info)
    {
        EntityItemController eic = info.entity.GetComponent<EntityItemController>();
        eic.Deselect(info);
    }

    public override Item CreateItem(int count)
    {
        EntityItemController eic = EntityPrefab.GetComponent<EntityItemController>();
        object property = eic.CreateProperty();
        int entityId = BlockMap.Instance.CreateEntityProperty(property);

        return new Item(id, count, entityId);
    }
}