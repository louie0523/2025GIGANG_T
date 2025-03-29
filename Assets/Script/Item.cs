using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    /// <summary>
    /// 아이템 넘버 : 0 : Hp, 1 : O2 ,  2 : FIND , 3 : 이속1, 4 : 이속2, 5:인식불가
    /// </summary>
    public int itemNum;

    GameManager gameManager;

    public string itemName;

    public int num;

    public int weight;

    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    /// <summary>
    /// 초기값
    /// </summary>
    /// <param name="itemNum"></param>
    public void SetItem(int itemNum)
    {
        this.itemNum = itemNum;
        itemName = GameData.sName[this.itemNum];
        num = 1;
        weight = GameData.iWeight[this.itemNum];
    }







}
