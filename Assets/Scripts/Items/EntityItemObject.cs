using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Entity Item Object")]
public class EntityItemObject : ItemObject
{
    public GameObject EntityPrefab;

    public object properties;
   
    public override void UseItemStart(ItemOperationInfo info)
    {
        EntityItemController wc = info.entity.GetComponent<EntityItemController>();
        wc.FireDown();
    }

    public override void UseItemEnd(ItemOperationInfo info)
    {
        EntityItemController wc = info.entity.GetComponent<EntityItemController>();
        wc.FireUp();
    }

    public override void SelectItem(ItemOperationInfo info)
    {
        GameObject entity = Instantiate(EntityPrefab);
        EntityItemController eic = entity.GetComponent<EntityItemController>();
        object property = BlockMap.Instance.GetEntityProperty(info.item.entityId);
        eic.SetProperty(property);
        EntityHolder eh;
        if(info.invoker.TryGetComponent(out eh))
        {
            eh.Hold(entity);
        }
    }

    public override void DeselectItem(ItemOperationInfo info)
    {
        Destroy(info.entity);
    }

    public override Item CreateItem(int count)
    {
        EntityItemController eic = EntityPrefab.GetComponent<EntityItemController>();
        object property = eic.CreateProperty();
        int entityId = BlockMap.Instance.CreateEntityProperty(property);

        return new Item(id, count, entityId);
    }
}