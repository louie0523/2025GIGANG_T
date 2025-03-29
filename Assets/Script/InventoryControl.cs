using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class InventoryControl : MonoBehaviour
{


    /// <summary>
    /// 인벤토리 자식들을 담아놓은 부모, 자식 넘버에 따라 0: 자물쇠, 1: 아이템이름 , 2번 갯수
    /// </summary>
    public GameObject baseInv;

    public List<Transform> inventory = new List<Transform>();
    public PlayerControl playerControl;
    public GameManager gameManager;

    private void Start()
    {
        playerControl = GetComponent<PlayerControl>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        baseInv = GameObject.Find("Inven");

        for(int i = 0; i < baseInv.transform.childCount; i++)
        {
            inventory.Add(baseInv.transform.GetChild(i));
        }
        for(int i = 0; i < inventory.Count; i++)
        {
            if(i < GameData.iBagSize[GameData.Instance.bagUpgradeNum])
            {
                inventory[i].GetChild(0).gameObject.SetActive(false);
                inventory[i].GetChild(1).GetComponent<Text>().text = "";
                inventory[i].GetChild(2).GetComponent<Text>().text = "";
            } else
            {
                inventory[i].GetChild(0).gameObject.SetActive(true);
                inventory[i].GetChild(1).GetComponent<Text>().text = "";
                inventory[i].GetChild(2).GetComponent<Text>().text = "";
            }
        }
    }

    public void UpgradInven()
    {
        for (int i = 0; i < inventory.Count; i++) {
            if(i < GameData.iBagSize[GameData.Instance.bagUpgradeNum])
            {
                inventory[i].GetChild(0).gameObject.SetActive(false);
            } else
            {
                inventory[i].GetChild(0).gameObject.SetActive(true);
            }
        }
    }

    public void UseItem(int itemNum)
    {
        playerControl.UseItem(itemNum);
    }

    public bool GetInventoryEmpty(Item item)
    {
        int bagSize = GameData.iBagSize[GameData.Instance.bagUpgradeNum];

        if(GameData.Instance.bagData.Count < bagSize)
        {
            if(GameData.Instance.bagData.Count > 0)
            {
                for(int i = 0; i < GameData.Instance.bagData.Count; i++)
                {
                    if (GameData.Instance.bagData[i] != null)
                    {
                        GameData.Instance.bagData[i].num++;
                        GameData.Instance.bagData[i].weight += GameData.iWeight[item.num];
                        inventory[i].GetChild(1).GetComponent<Text>().text = item.itemName;
                        inventory[i].GetChild(2).GetComponent<Text>().text = GameData.Instance.bagData[i].num.ToString();
                    }

                    GameData.Instance.bagData.Add(item);
                    inventory[GameData.Instance.bagData.Count - 1].GetChild(1).GetComponent <Text>().text = item.itemName;
                    inventory[GameData.Instance.bagData.Count - 1].GetChild(2).GetComponent<Text>().text = item.name.ToString();
                }
            } else
            {
                GameData.Instance.bagData.Add (item);
                inventory[0].GetChild(1).GetComponent<Text>().text = item.itemName;
                inventory[0].GetChild(2).GetComponent<Text>().text = item.name.ToString();
            }
            return true;
        } else if(GameData.Instance.bagData.Count == bagSize)
        {
            // 가방 안에 이미 아이템이 있다.
            for (int i = 0; i < GameData.Instance.bagData.Count; i++)
            {
                if (GameData.Instance.bagData[i] != null)
                {
                    GameData.Instance.bagData[i].num++;
                    GameData.Instance.bagData[i].weight += GameData.iWeight[item.num];
                    inventory[i].GetChild(1).GetComponent<Text>().text = GameData.Instance.bagData[i].itemName;
                    inventory[i].GetChild(2).GetComponent<Text>().text = GameData.Instance.bagData[i].num.ToString();
                }
            }
        }
        gameManager.delayText.SetText("아이템이 가득찼습니다.");
        return false;
    }

    public void SetInventory()
    {
        for(int i = 0; i < GameData.iBagSize[GameData.Instance.bagUpgradeNum]; i++)
        {
            if(i < GameData.Instance.bagData.Count)
            {
                inventory[i].GetChild(1).GetComponent<Text>().text = GameData.Instance.bagData[i].itemName;
                inventory[i].GetChild(2).GetComponent<Text>().text = GameData.Instance.bagData[i].num.ToString();
            } else
            {
                inventory[i].GetChild(1).GetComponent<Text>().text = "";
                inventory[i].GetChild(2).GetComponent<Text>().text = "";
            }
        }
    }

    private void Update()
    {
        int invenNum = -1;
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            invenNum = 0;
        } else if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            invenNum = 1;
        }
        else if(Input.GetKeyDown(KeyCode.Alpha3)) { invenNum = 2;}
        else if(Input.GetKeyDown(KeyCode.Alpha4)) { invenNum = 3;}  
        else if(Input.GetKeyDown(KeyCode.Alpha5)) { invenNum = 4;}
        else if(Input.GetKeyDown(KeyCode.Alpha6)) { invenNum = 5;}
        else if(Input.GetKeyDown(KeyCode.Alpha7)) { invenNum = 6;}
        else if(Input.GetKeyDown(KeyCode.Alpha8)) { invenNum = 7;}

        if(invenNum > -1)
        {
            if (GameData.Instance.bagData.Count > invenNum)
            {
                GameData.Instance.bagData[invenNum].num--;
                UseItem(GameData.Instance.bagData[invenNum].itemNum);
                if (GameData.Instance.bagData[invenNum].num <= 0)
                {
                    GameData.Instance.bagData.Remove(GameData.Instance.bagData[invenNum]);
                }
                SetInventory();
            } else
            {
                gameManager.delayText.SetText("아이템이 읎습니다.");
            }
        }
    }
}
