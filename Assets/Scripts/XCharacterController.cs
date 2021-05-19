using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class XCharacterController : MonoBehaviour
{
    public float MoveSpeed;

    private float health;

    private Vector3 faceDirection;

    private Animator anim;

    private Rigidbody2D rb;

    private Pack pack;

    public WeaponController weapon;

    public BlockMap map;

    // Start is called before the first frame update
    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        pack = GetComponent<Pack>();
    }

    private void FixedUpdate()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        transform.Translate(new Vector2(x, y) * MoveSpeed * Time.deltaTime);

        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        faceDirection = mouseWorldPosition - transform.position;
        anim.SetFloat("facex", faceDirection.x);
        anim.SetFloat("facey", faceDirection.y);

        weapon.target = mouseWorldPosition;
        Vector3 weaponPositionOffset = new Vector3(faceDirection.x, faceDirection.y, 0).normalized * 0.1f;
        weapon.transform.localPosition = weaponPositionOffset;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            weapon.FireDown();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            pack.AddItem(0, 100);
            pack.AddItem(5, 100);
            pack.AddItem(4, 100);
            pack.AddItem(7, 100);
        }

        if (Input.GetMouseButtonDown(1))
        {
            PlaceBlock();
        }

        SelectPackSlot();
    }

    private void PlaceBlock()
    {
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (!pack[pack.SelectedItemIndex].IsEmpty())
        {
            ItemObject io = ItemObjectFactory.Instance.GetItemObject(pack[pack.SelectedItemIndex].id);
            BlockObject bo = BlockFactory.Instance.GetBlockObject(io.placedBlockId);
            if (io.placedBlockId != 0 && bo != null)
            {
                Vector3Int pos = new Vector3Int(
                    Mathf.FloorToInt(mouseWorldPosition.x),
                    Mathf.FloorToInt(mouseWorldPosition.y),
                    bo.layer);
                if (map.GetBlock(pos) == null || map.GetBlock(pos).IsEmpty() || bo.layer == 0)
                {
                    if (pack.ConsumeItem(pack[pack.SelectedItemIndex].id, 1))
                    {
                        Block block = map.GetBlock(pos);
                        bo.CreateBlock(0, ref block);
                    }
                }
            }

        }
    }

    private void SelectPackSlot()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            pack.SelectedItemIndex = 0;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            pack.SelectedItemIndex = 1;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            pack.SelectedItemIndex = 2;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            pack.SelectedItemIndex = 3;
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            pack.SelectedItemIndex = 4;
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            pack.SelectedItemIndex = 5;
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            pack.SelectedItemIndex = 6;
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            pack.SelectedItemIndex = 7;
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            pack.SelectedItemIndex = 8;
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            pack.SelectedItemIndex = 9;
        }
    }

    public struct PlayerInfo
    {
        public float health;

        public Item[] items;

        public static PlayerInfo DefaultInfo
        {
            get
            {
                PlayerInfo info = new PlayerInfo();
                info.health = 100;
                info.items = new Item[10];
                return info;
            }
        }
    }

    public void Load()
    {
        PlayerInfo playerInfo;
        string playerFile = Path.Combine(BlockMap.Instance.MapDir, "player.json");
        if (File.Exists(playerFile))
        {
            using (StreamReader reader = new StreamReader(new FileStream(playerFile, FileMode.Open, FileAccess.Read)))
            {
                string playerInfoJson = reader.ReadToEnd();
                playerInfo = JsonUtility.FromJson<PlayerInfo>(playerInfoJson);
            }
        }
        else
        {
            playerInfo = PlayerInfo.DefaultInfo;
        }

        health = playerInfo.health;
        pack.InitPack(playerInfo.items);
    }

    public void Save()
    {
        PlayerInfo playerInfo;
        playerInfo.health = health;
        playerInfo.items = new Item[pack.packCapacity];
        for (int i = 0; i < pack.packCapacity; i++)
        {
            playerInfo.items[i] = pack[i];
        }
        string playerFile = Path.Combine(BlockMap.Instance.MapDir, "player.json");
        using (StreamWriter writer = new StreamWriter(new FileStream(playerFile, FileMode.Create, FileAccess.Write)))
        {
            string playerInfoJson = JsonUtility.ToJson(playerInfo);
            writer.Write(playerInfoJson);
            Debug.Log("Save player info at " + playerFile);
        }
    }
}
