using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public PlayerControl playerControl;

    public MonsterControl monsterControl;

    public Text ChestNum;

    public DelayText delayText;

    public MapControl mapControl;

    public Slider HPBar;

    public Slider O2Bar;

    public MachingCardGame gameCard;

    public GameObject MenuUI;

    public Text totalTime;

    private void Start()
    {
        InitGame();
    }

    public void InitGame()
    {
        if (GameObject.Find("GameData") == null)
        {
            Debug.Log("게임데이터 없음");
            return;
        }
        playerControl = new GameObject("Player").AddComponent<PlayerControl>();
        monsterControl = new GameObject("MonsterControl").AddComponent<MonsterControl>();
        mapControl = new GameObject("MapControl").AddComponent<MapControl>();
        ChestNum = GameObject.Find("ChestNum").GetComponent<Text>();
        Cursor.lockState = CursorLockMode.Locked;
        GameData.Instance.isMouse = false;
        HPBar = GameObject.Find("HPBar").GetComponent<Slider>();
        O2Bar = GameObject.Find("O2Bar").GetComponent<Slider>();
    }

    public void GameModeChange()
    {
        if (!GameData.Instance.isMouse)
        {
            Cursor.lockState = CursorLockMode.None;
            GameData.Instance.isMouse = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            GameData.Instance.isMouse = false;
        }
    }

    public void SetChest(int chestNum)
    {
        ChestNum.text = "X" + chestNum;
    }

    public void SetGameClear(bool isGame)
    {
        //gameCard.gameobject.SetActive(false);
        MenuUI.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        GameData.Instance.isMouse = true;
        if(isGame)
        {
            MenuUI.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "Game Clear";
        } else
        {
            MenuUI.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "Game Over";
        }

    }

    public void BtnMenu(int num)
    {
        switch (num)
        {
            case 0:
                {
                    GameData.Instance.isGame = false;
                    SceneManager.LoadScene("Main");
                }
                break;
            case 1:
                {
                    GameData.Instance.isGame = false;
                    SceneManager.LoadScene("Game");
                }
                break;
            case 2:
                {
                    Application.Quit();
                }
                break;
            
        }
    }
}