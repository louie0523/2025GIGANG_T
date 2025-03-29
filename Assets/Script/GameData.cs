using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class BagData
{
    /// <summary>
    /// ������ �̸�
    /// </summary>
    public string name;
    /// <summary>
    /// ������ ����
    /// </summary>
    public int num;
    /// <summary>
    /// ������ ���� ����
    /// </summary>
    public int weight;
    /// <summary>
    /// ���濡 ����ִ� ����
    /// </summary>
    public int bagNum;
}

public enum AttackType { Melee = 0, Ranged = 1 };
public class GameData : MonoBehaviour
{
    #region ���� ���ο��� ���
    /// <summary>
    /// ���� �������� ��ȣ
    /// </summary>
    public int statgeNum = 0;
    /// <summary>
    /// 0 : 4ĭ(�⺻), 1: 6ĭ(����), 8(�ʴ���)
    /// </summary>
    public int bagUpgradeNum = 0;
    /// <summary>
    /// ���������� Ȯ��, ���� ���̶�� ���� ���
    /// </summary>
    public bool isGame = true;
    /// <summary>
    /// ���� �������� ������ ���
    /// </summary>
    public int stageSize = 0;
    /// <summary>
    /// ���� ������ ���
    /// </summary>
    public List<Item> bagData = new List<Item>();
    /// <summary>
    /// ���� ����
    /// </summary>
    public List<bool> chestData = new List<bool>();
    /// <summary>
    /// ��������ð�
    /// </summary>
    public float gameTime = 0f;
    /// <summary>
    /// ���� ���� ��
    /// </summary>
    public int cost = 0;
    /// <summary>
    /// ������ ���� ���ɿ���
    /// </summary>
    public bool[] isBuyItem = { true, true, true, true, true };
    /// <summary>
    /// ���콺 ����(true:�Ϲ�, false:���)
    /// </summary>
    public bool isMouse = true;
    /// <summary>
    /// ����� �ѹ�
    /// </summary>
    public int bagO2 = 0;

    #endregion

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    #region ���� �� ������ �ʴ� ����
    /// <summary>
    /// �̱���
    /// </summary>
    public static GameData Instance;
    /// <summary>
    /// ���������� ó�� ���� ��ġ
    /// </summary>
    public static List<Vector3> StagePos = new List<Vector3>
    {
        new Vector3 (1, 0, 1),
        new Vector3 (1, 0, 1),
        new Vector3 (1, 0, 1),
        new Vector3 (1, 0, 1),
        new Vector3 (1, 0, 1)
    };

    /// <summary>
    /// �ؽ��� ������ width
    /// </summary>
    public static int textureWidth = 256;
    /// <summary>
    /// �ؽ��� ������ height
    /// </summary>
    public static int textureHeight = 256;

    /// <summary>
    /// ���������� ��� ���ҷ�
    /// </summary>
    public static int[] stageDeO2 = { 1, 1, 2, 2, 4 };
    /// <summary>
    /// ������� ���̽�
    /// </summary>
    public static Color baseSandColor = new Color(0.85f, 0.7f, 0.4f);
    /// <summary>
    /// ������� �ͽ�
    /// </summary>
    public static Color mixSandColor = new Color(0.98f, 0.85f, 0.66f);

    /// <summary>
    /// �ܵ���� ���̽�
    /// </summary>
    public static Color baseGlassColor = new Color(0.30f, 0.55f, 0.16f);
    /// <summary>
    /// �ܵ���� �ͽ�
    /// </summary>
    public static Color mixGlassColor = new Color(0.16f, 0.36f, 0.03f);
    /// <summary>
    /// �μ��� ���� ���(�ٴڸ� ����,��)
    /// </summary>
    public static Color ColorNoneSandBlock = Color.white;
    /// <summary>
    /// �μ��� ���� ���(�ٴڸ� ����, �ܵ�)
    /// </summary>
    public static Color ColorNoneGlassBlock = new Color(34f / 255f, 177f / 255f, 76 / 255f);
    /// <summary>
    /// ��ֹ� ���
    /// </summary>
    public static Color ColorBlock = Color.red;
    /// <summary>
    /// �������� ��Ʈ�� �̵��ϴ� ��
    /// </summary>
    public static Color ColorPatrolEnemyTop = Color.blue;
    /// <summary>
    /// Ư�� �Ÿ����� ������� �� 
    /// </summary>
    public static Color ColorEnemy = Color.green;
    /// <summary>
    /// ����
    /// </summary>
    public static Color ColorTreasure = Color.black;
    /// <summary>
    /// ����
    /// </summary>
    public static Color ColorTrap = Color.magenta;
    /// <summary>
    /// �Ƕ�̵� ����(���� ������* ������ �ʿ�)
    /// </summary>
    public static Color ColorPyramid = Color.cyan;
    /// <summary>
    /// ���� ������ ����
    /// </summary>
    public static Color colorRandom = new Color(185f / 255f, 122f / 255f, 87f / 255f);
    /// <summary>
    /// �κ� ���ư��� 
    /// </summary>
    public static Color colorRobby = new Color(255f / 255f, 174f / 255f, 201f / 255f);
    /// <summary>
    /// ��������
    /// </summary>
    public static Color colorStart = new Color(127f / 255f, 127f / 255f, 127f / 255f);
    /// <summary>
    /// ��������
    /// </summary>
    public static Color colorEND = new Color(0f / 255f, 162f / 255f, 232f / 255f);
    /// <summary>
    /// ���� ������ ����
    /// </summary>
    public static int[] itemCost = { 10, 100, 1000, 100, 1000 };
    /// <summary>
    /// ���� ������ ����
    /// </summary>
    public static string[] itemText =
    {
        "���п� �����\n$10",
        "�߾п� �����\n$100",
        "��п� �����\n$1000",
        "���� ����\n$100",
        "�ʴ��� ����\n$1000"
    };
    public static string buyItemText = "���� �Ϸ�";
    public static string buyFileText = "���� �Ұ���";

    /// <summary>
    /// ������ �̸���
    /// </summary>
    public static string[] sName = { "FIND", "HP", "O2", "NOTRECOG", "DOUBLEFAST", "FAST", };
    /// <summary>
    /// ������ ���Ե�
    /// </summary>
    public static int[] iWeight = { 10, 10, 10, 10, 10, 10 };
    public static int[] iBagSize = { 4, 6, 8 };
    /// <summary>
    /// ���� ����
    /// </summary>
    public static int[] iBagWeight = { 100, 250, 400 };
    /// <summary>
    /// ü�� ��
    /// </summary>
    public static int MaxHp = 10;
    /// <summary>
    /// ��� ��
    /// </summary>
    public static int[] MaxO2 = { 60, 70, 80, 100 };
    /// <summary>
    /// ���������� �������� ����
    /// </summary>
    public static int[] chestCost = { 100, 10, 100, 500, 1000 };
    #endregion

}
