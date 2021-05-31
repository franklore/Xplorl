using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class XCharacterController : MonoBehaviour
{
    public float MoveSpeed;

    private float health;

    private Vector3 faceDirection;

    private Animator anim;

    private Rigidbody2D rb;

    private Pack pack;

    public Transform WeaponSlot;

    private GameObject entity;

    // Start is called before the first frame update
    void Awake()
    {
        anim = GetComponentInChildren<Animator>();

        rb = GetComponent<Rigidbody2D>();

        pack = GetComponent<Pack>();
        pack.registerUpdateSelectedItem(onUpdatePackSelectedItem);
        Camera.main.transform.parent = transform;
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
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Item pickaxe = ItemObjectFactory.Instance.GetItemObject(4).CreateItem(1);

            pack.AddItem(pickaxe);
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                UseItem(true);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                UseItem(false);
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            CostMap costs = Navigation.CreateCostMapFromBlockMap(new Vector3Int(0, 0, 0), new Vector3Int(8, 8, 0), 1);
            Vector3Int[] path = Navigation.FindPath(new Vector3Int(0, 0, 0), new Vector3Int(8, 8, 0), costs);
            for (int i = 0; i < path.Length; i++)
            {
                GameObject go = BlockMap.Instance.GetBlockGameObject(path[i]);
                SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
                sr.color = Color.blue;
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (PackUIController.Instance.Expanded)
            {
                PackUIController.Instance.Collapse();
                RecipeListUIController.Instance.gameObject.SetActive(false);
            }
            else
            {
                PackUIController.Instance.Expand();
                RecipeListUIController.Instance.gameObject.SetActive(true);
                RecipeListUIController.Instance.UpdateUI(RecipeFactory.Instance.getRecipes());
            }
        }

        SelectPackSlot();
    }
    private void UseItem(bool start)
    {
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (!pack[pack.SelectedItemIndex].IsEmpty())
        {
            ItemObject io = ItemObjectFactory.Instance.GetItemObject(pack[pack.SelectedItemIndex].id);
            ItemOperationInfo info;
            info.invoker = gameObject;
            info.entity = entity;
            info.operationPosition = mouseWorldPosition;
            info.item = pack.SelectedItem;
            if (start)
            {
                io.UseItemStart(info);
            }
            else
            {
                io.UseItemEnd(info);
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

    public void onUpdatePackSelectedItem(int oldSelect)
    {
        Debug.Log("select " + pack.SelectedItemIndex + " replace " + oldSelect);
        ItemOperationInfo info;
        info.invoker = gameObject;
        info.operationPosition = Vector3.zero;
        info.entity = entity;
        info.item = pack.SelectedItem;
        ItemObject io1 = ItemObjectFactory.Instance.GetItemObject(pack[oldSelect].id);
        io1.DeselectItem(info);

        ItemObject io2 = ItemObjectFactory.Instance.GetItemObject(pack.SelectedItem.id);
        io2.SelectItem(info);
    }

    public struct PlayerInfo
    {
        public float health;

        public Vector3 position;

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
        transform.position = playerInfo.position;
        pack.InitPack(playerInfo.items);
    }

    public void Save()
    {
        PlayerInfo playerInfo;
        playerInfo.health = health;
        playerInfo.position = transform.position;
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

    public void OnHold(GameObject entity)
    {
        this.entity = entity;
        entity.transform.parent = WeaponSlot;
    }
}
