using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerControl : MonoBehaviour
{
    public float moveSpeed = 5.0f;

    public float originalSpeed;

    public float speedTimer = 5.0f;
    public float speedTiming = 0;
    public bool isSpeedUp = false;

    /// <summary>
    /// 더블 스피드업 관련
    /// </summary>
    public float dbSpeedTimer = 5.0f;
    public float dbSpeedTiming = 0;
    public bool isDbSpeedUp = false;

    public GameObject ChestArrow;
    public GameObject tempChest;

    public float rotSpeed = 2.0f;

    public Transform trCamera;

    private CharacterController characterController;

    private float rotationX = 0f;

    private float rotationY = 0f;

    public AttackType attackType = AttackType.Melee;

    public GameObject prefabKnife;

    public GameObject knife;

    private Quaternion knifeOriginalRot;

    private Vector3 knifeOriginalPos;

    public int O2 = 60;
    public int HP = 10;

    public GameManager gameManager;

    public InventoryControl inventoryControl;

    public float O2Timer = 10f;
    public float O2Timing = 0;
    public bool isO2 = true;

    public bool isRecog = false;
    public float recogTimimg = 5.0f;
    public float recogTimer = 0;


    public void SetPlayer()
    {
        originalSpeed = moveSpeed;

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        this.transform.position = GameData.StagePos[GameData.Instance.statgeNum];

        trCamera = GameObject.Find("Main Camera").transform;

        ChestArrow = Resources.Load<GameObject>("Prefabs/ChestArrow");

        characterController = this.AddComponent<CharacterController>();

        inventoryControl = this.AddComponent<InventoryControl>();

        CreateKnife();
    }

    void CreateKnife()
    {
        prefabKnife = Resources.Load<GameObject>("Prefabs/Knfie");
        knife = Instantiate(prefabKnife);

        knife.transform.SetParent(trCamera);
        knife.transform.localPosition = new Vector3(0.3f, -0.2f, 0.5f);
        knifeOriginalRot = knife.transform.localRotation;
        knifeOriginalPos = knife.transform.localPosition;
    }

    IEnumerator AnimateKnifeAttack()
    {
        Vector3 attackPos = knifeOriginalPos + new Vector3(-0.2f, 0, 0.2f);
        Quaternion attackRot = Quaternion.Euler(knifeOriginalRot.eulerAngles + new Vector3(0, -20, 0));

        float attackTime = 0.1f;
        float elapsedTIme = 0f;

        while (elapsedTIme < attackTime)
        {
            knife.transform.localPosition = Vector3.Lerp(knifeOriginalPos, attackPos,  elapsedTIme / attackTime);
            // 시작점 , 목적지, 선형보간형식으로 부드럽게 움직임
            knife.transform.localRotation = Quaternion.Lerp(knifeOriginalRot, attackRot, elapsedTIme / attackTime);
            elapsedTIme += Time.deltaTime;
            yield return null;
        }
        elapsedTIme = 0f;
        while (elapsedTIme < attackTime)
        {
            knife.transform.localPosition = Vector3.Lerp(attackPos, knifeOriginalPos, elapsedTIme / attackTime);
            knife.transform.localRotation = Quaternion.Lerp(attackRot, knifeOriginalRot, elapsedTIme / attackTime);
            elapsedTIme += Time.deltaTime;
            yield return null;
        }
    }

    void Move()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = transform.right * moveX + transform.forward * moveZ;

        characterController.Move(move.normalized * moveSpeed * Time.deltaTime);
        trCamera.position = transform.position + new Vector3(0, 0.5f, 0);
    }

    void LookAround( )
    {
        float mouseX = Input.GetAxis("Mouse X") * rotSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * rotSpeed;
        // 마우스의 상하 적용
        rotationX -= mouseY;
        // 카메라가 90도 이상 회전하지 못하도록
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);
        // 마우스의 상하
        rotationY += mouseX;
        // 캐릭터 회전
        transform.rotation = Quaternion.Euler(0f, rotationY, 0f);
        // 카메라 회전
        trCamera.localRotation = Quaternion.Euler(rotationX, rotationY, 0f);

    }

    void DetectObjectInFront()
    {
        RaycastHit hit;
        Vector3 forward = this.transform.forward;
        Vector3 rayOrigin = this.transform.position;

        if(Physics.BoxCast(rayOrigin, new Vector3(0.5f, 1.0f, 0.5f), forward, out hit, Quaternion.identity, 2.0f)) {
            Debug.Log("Detected Object : " +  hit.collider.gameObject.name);
            if (hit.collider.gameObject.CompareTag("Chest"))
            {
                int num = gameManager.mapControl.chests.FindIndex(a => a == hit.collider.gameObject);
                Debug.Log(num);
                if(num > -1)
                {
                    Debug.Log("상자 비활성화");
                    GameData.Instance.chestData[num] = false;
                    GameData.Instance.cost += GameData.chestCost[GameData.Instance.statgeNum];
                    hit.collider.gameObject.SetActive(false);
                }
                gameManager.SetChest(GameData.Instance.chestData.FindAll(a => a).Count);
            } else if(hit.collider.gameObject.CompareTag("Obstacle"))
            {
                hit.collider.gameObject.SetActive(false);   
            } else if(hit.collider.gameObject.CompareTag("Start"))
            {
                SceneManager.LoadScene("Robby");
                gameManager.GameModeChange();
            }
            else if (hit.collider.gameObject.CompareTag("End"))
            {
                if(GameData.Instance.chestData.FindAll(a => a).Count <= 0)
                {
                    gameManager.GameModeChange();
                    Debug.Log("퍼즐 오픈");
                    gameManager.gameCard.gameObject.SetActive(true);
                } else
                {
                    gameManager.delayText.SetText("보물을 다 찾아야 이동가능합니디");
                }
            } else if(hit.collider.gameObject.CompareTag("Item"))
            {
                //Item its = hit.collider.gameObject.GetComponent<Item>();
                //inventoryControl.GetInventoryEmpty(its);
            }
        }   
    }

    void Attack()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            DetectObjectInFront();
            StartCoroutine(AnimateKnifeAttack());
        }
    }


    private void Start()
    {
        SetPlayer();
    }
    private void Update()
    {
        if(!GameData.Instance.isMouse)
        {
            Move();
            LookAround();
            Attack();
            SetO2();
        }

        if(isSpeedUp)
        {
            if(speedTiming > speedTimer)
            {
                isSpeedUp = false;
                moveSpeed = originalSpeed;
                speedTiming = 0;
            } else
            {
                speedTiming += Time.deltaTime;
            }
        }
        if (isDbSpeedUp)
        {
            if (dbSpeedTiming > dbSpeedTimer)
            {
                isDbSpeedUp = false;
                moveSpeed = originalSpeed;
                dbSpeedTiming = 0;
            }
            else
            {
                dbSpeedTiming += Time.deltaTime;
            }
        }
        if(isRecog)
        {
            if(recogTimimg > recogTimer)
            {
               isRecog = false;
                recogTimimg = 0;
            } else
            {
                recogTimimg += Time.deltaTime;
            }
        }
        gameManager.totalTime.text = GameData.Instance.gameTime.ToString("#");
        GameData.Instance.gameTime += Time.deltaTime;
    }

    public void UseItem(int itemnum)
    {
        Debug.Log("ItemNum : " + itemnum + "사용");
        switch (itemnum)
        {
            case 0:
                {
                    Vector3 pos = FindChest().position;
                    tempChest = Instantiate(ChestArrow, pos, ChestArrow.transform.rotation);
                }
                break;
            case 1:
                {
                    HP = GameData.MaxHp;
                    gameManager.HPBar.value = (float)HP / (float)GameData.MaxHp;
                }
                break;
            case 2:
                {
                    O2 = GameData.MaxO2[GameData.Instance.bagO2];
                    gameManager.O2Bar.value = (float)O2 / (float)GameData.MaxO2[GameData.Instance.bagO2];
                }
                break;
            case 3:
                {
                    isRecog = true;
                    recogTimimg = 0;
                }
                break;
            case 4:
                {
                    isDbSpeedUp = true;
                    moveSpeed = originalSpeed * 1.4f;
                    speedTiming = 0;
                }
                break ;
            case 5:
                {
                    isSpeedUp = true;
                    moveSpeed = originalSpeed * 1.2f;
                    speedTiming = 0;
                }
                break;
        }
    }

    public Transform FindChest()
    {
        float distance = 10000;
        Transform returnTr = null;
        for(int i = 0; i < gameManager.mapControl.chests.Count; i++)
        {
            if(gameManager.mapControl.chests[i].activeInHierarchy)
            {
                float tempDistance = Vector3.Distance(this.transform.position, gameManager.mapControl.chests[i].transform.position);
                if(distance > tempDistance)
                {
                    distance = tempDistance;
                    returnTr = gameManager.mapControl.chests[i].transform;
                }
            }
        }
        return returnTr;
    }

    public void SetO2()
    {
        if(isO2) { 
            if(O2Timing > O2Timer)
            {
                O2Timing = 0;
                O2 -= 1;
                if(O2 < 0)
                {
                    gameManager.O2Bar.value = 0;
                    gameManager.SetGameClear(false);
                } else
                {
                    gameManager.O2Bar.value = (float)O2 / (float)GameData.MaxO2[GameData.Instance.bagO2];
                }
            } else
            {
                O2Timing += Time.deltaTime;
            }
        }
    }

    public void SetDamage(int damage)
    {
        if(!isRecog)
        {
            HP -= damage;
            Debug.Log("체력 감소");
            if(HP < 0)
            {
                
            } else
            {

            }
            gameManager.HPBar.value = (float)HP / (float)GameData.MaxHp;
        }
    }











}
