using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

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

    //public InventoryControl inventoryControl;

    public float O2Timer = 10f;
    public float O2Timing = 0;
    public bool isO2 = true;

    public void SetPlayer()
    {
        originalSpeed = moveSpeed;

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        this.transform.position = GameData.StagePos[GameData.Instance.statgeNum];

        trCamera = GameObject.Find("Main Camera").transform;

        ChestArrow = Resources.Load<GameObject>("Prefabs/ChestArrow");

        characterController = this.AddComponent<CharacterController>();

        //inventoryControl = this.AddComponent<InventoryControl>();

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

        if(Physics.BoxCast(rayOrigin, new Vector3(0.5f, 1.0f, 0.5f), forward, out hit, Quaternion.identity, 1.0f)) {
            Debug.Log("Detected Object : " +  hit.collider.gameObject.name);
            if (hit.collider.gameObject.CompareTag("Chest"))
            {
                int num = gameManager.mapControl.chests.FindIndex(a => a == hit.collider.gameObject);
                if(num > 1)
                {
                    GameData.Instance.chestData[num] = false;
                    GameData.Instance.cost += GameData.chestCost[GameData.Instance.statgeNum];
                    hit.collider.gameObject.SetActive(false);
                }
                //gameManager.SetChest(GameData.Instance.chestData.FindAll(a => a).Count);
            } else if(hit.collider.gameObject.CompareTag("Obstacle"))
            {
                hit.collider.gameObject.SetActive(false);   
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

            //SetO2();
        }
    }











}
