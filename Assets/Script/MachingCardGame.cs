using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MachingCardGame : MonoBehaviour
{
    public Sprite CardBack;

    public Sprite[] cardFaces;

    public Button[] cardButton;

    private int[] cardValue;

    private bool[] matchedCards;

    private int firstSelected = -1;

    private int secondSelected = -1;

    private bool isCheckingMatch = false;

    public int totalPairs = 4;

    public float timeLimit = 60f;

    public float remainingTime;

    public Text timerText;

    private int successNum = 0;

    public GameObject objPuzzle;

    public GameObject prefabBackCard;

    public GameManager gameManager;

    private void Start()
    {
        InitializeGame();
    }


    void InitializeGame()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        cardFaces = Resources.LoadAll<Sprite>("Puzzle");
        objPuzzle = GameObject.Find("Puzzle");
        prefabBackCard = Resources.Load<GameObject>("Prefabs/BackCard");
        cardButton = new Button[totalPairs * 2];
        for (int i = 0; i < totalPairs * 2; i++)
        {
            cardButton[i] = Instantiate(prefabBackCard, objPuzzle.transform).GetComponent<Button>();
        }
        cardValue = new int[totalPairs * 2];

        matchedCards = new bool[totalPairs * 2];

        List<int> values = new List<int>();

        for (int i = 0; i < totalPairs; i++)
        {
            values.Add(i);
            values.Add(i);
        }
        // 섞기
        Shuffle(values);

        for (int i = 0; i < totalPairs * 2; i++)
        {
            int index = i;
            cardValue[i] = values[i];

            cardButton[i].onClick.AddListener(() => OnCardSelected(index));
            cardButton[i].image.sprite = CardBack;
        }
    }

    void Shuffle(List<int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int temp = list[i];
            int randomindex = Random.Range(i, list.Count);
            list[i] = list[randomindex];
            list[randomindex] = temp;
        }
    }

    void OnCardSelected(int index)
    {
        if (matchedCards[index] || index == firstSelected || isCheckingMatch)
        {
            return;
        }
        cardButton[index].image.sprite = cardFaces[cardValue[index]];
        if(firstSelected == -1)
        {
            firstSelected = index;
        } else
        {
            secondSelected = index;
            isCheckingMatch = true;
            StartCoroutine(CheckMatch());
        }

    }

    IEnumerator CheckMatch()
    {
        yield return new WaitForSeconds(1f);

        if (cardValue[firstSelected] == cardValue[secondSelected])
        {
            matchedCards[firstSelected] = true;
            matchedCards[secondSelected] = true;
            successNum++;
            if(successNum == totalPairs)
            {
                ShowMenu(true);
            }
        } else
        {
            cardButton[firstSelected].image.sprite = CardBack;
            cardButton[secondSelected].image.sprite = CardBack;
        }
        firstSelected = -1;
        secondSelected = -1;
        isCheckingMatch = false;
    }

    IEnumerator Timer()
    {
        if(!(successNum == totalPairs))
        {
            while(remainingTime > 0)
            {
                remainingTime -= Time.deltaTime;
                timerText.text = "Tiem: " + Mathf.Ceil(remainingTime);
                yield return null;
            }
        }
        ShowMenu(false);
    }

    void ShowMenu(bool isWin)
    {
        gameManager.SetGameClear(isWin);
        this.gameObject.SetActive(false);
    }

}
