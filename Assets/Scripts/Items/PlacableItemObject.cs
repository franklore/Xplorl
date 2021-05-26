using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Placable Item")]

public class PlacableItemObject : ItemObject
{
    public int placedBlockId;


    public override void UseItemStart(ItemOperationInfo info)
    {
        Pack pack = info.invoker.GetComponent<Pack>();
        BlockObject bo = BlockFactory.Instance.GetBlockObject(placedBlockId);
        Vector3Int pos = new Vector3Int(
            Mathf.FloorToInt(info.operationPosition.x),
            Mathf.FloorToInt(info.operationPosition.y),
            0);
        if (pack.SelectedItem.count > 0 && bo.SetBlock(pos))
        {
            pack.ConsumeItemAtIndex(pack.SelectedItemIndex, 1);
        }
    }
}
