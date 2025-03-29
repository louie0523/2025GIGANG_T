using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterControl : MonoBehaviour
{
    public GameObject patroMonstor;

    public GameObject AIMonster;

    private void Start()
    {
        patroMonstor = Resources.Load<GameObject>("Prefabs/PatrolMonster");
        AIMonster = Resources.Load<GameObject>("Prefabs/AIMonster");

        Monster monster = this.GetComponent<Monster>();
    }

    public void CreatePatrolMonsters(Vector3 pos)
    {
        GameObject Pm = Instantiate(patroMonstor, pos, patroMonstor.transform.rotation);
        Pm.AddComponent<Monster>();

    }

    public void CreateAIMonsters(Vector3 pos)
    {
        GameObject Am = Instantiate(AIMonster, pos, AIMonster.transform.rotation);
        Am.AddComponent<Monster>().isPatrol = false;
    }
}
