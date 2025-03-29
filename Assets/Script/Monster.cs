using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Monster : MonoBehaviour
{
    Transform Player;

    public bool isPatrol = true;
    float patrolSpeed = 2.0f;
    float Speed = 2.0f;
    float distance = 10.0f;
    int attackDamage = 1;
    bool isWalk = false;

    public float attackTimer = 1.0f;
    public float atttacTiming = 0;
    public bool isAttack = false;

    private void Start()
    {
        Player = GameObject.Find("Player").transform;

    }

    private void Update()
    {
        if(GameData.Instance.isGame)
        {
            if(isPatrol)
            {
                movePatrolMonster();
            } else
            {
                moveMonster();
            }
            if(isAttack)
            {
                if(atttacTiming > attackTimer)
                {
                    isAttack = false;
                    atttacTiming = 0;
                } else
                {
                    atttacTiming += Time.deltaTime;
                }
            }
        }
    }


    void moveMonster()
    {
        if(Player != null && !Player.GetComponent<PlayerControl>().isRecog)
        {
            Vector3 targetPos = Player.transform.position;
            targetPos.y = this.transform.position.y;

            if(Vector3.Distance(this.transform.position, targetPos ) < distance && Vector3.Distance(this.transform.position, targetPos) > 1.5f)
            {
                Vector3 pos = targetPos - this.transform.position;
                this.transform.rotation = Quaternion.LookRotation(pos);
                if(!isWalk)
                {
                    isWalk = true;
                }
                Vector3 direction = this.transform.forward;
                Vector3 newPostion  = this.transform.position + direction * Speed * Time.deltaTime;
                if(isGround(newPostion))
                {
                    this.transform.position = newPostion;
                }
            } else
            {
                if(isWalk)
                {
                    isWalk = false;
                    if(!isAttack)
                    {
                        isAttack = true;
                        Player.GetComponent<PlayerControl>().SetDamage(attackDamage);
                    }
                }
            }
        }
        {

        }
    }
    
    void movePatrolMonster()
    {
        Vector3 direction = this.transform.forward;
        Vector3 newPostion = this.transform.position + direction * patrolSpeed * Time.deltaTime;
        if(isGround(newPostion))
        {
            this.transform.position = newPostion;

        } else
        {
            this.transform.Rotate(0, 180, 0);
        }
        SetPlayerDamage();
    }

    bool isGround(Vector3 postion)
    {
        //if (postion.x < 0 || postion.x > GameData.Instance.stageSize || postion.z < 0 || postion.z > GameData.Instance.stageSize)
        //{

        //    return false;

        //}
        RaycastHit[] hits;
        Vector3 rayOrigin = this.transform.position + this.transform.forward;
        Vector3 forward = this.transform.forward;
        hits = Physics.BoxCastAll(rayOrigin, new Vector3(0.5f, 2.0f, 0.5f), forward, Quaternion.identity, 1.0f);
        if(hits.Length > 0)
        {
            foreach(RaycastHit hit in hits)
            {
                Debug.Log(hit.transform.name);
                if (hit.transform.CompareTag("Obstacle") || hit.transform.CompareTag("Chest"))
                {
                    return false;
                }
            }
            return true;
        }
        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireCube(this.transform.position + this.transform.forward, new Vector3(0.5f, 2.0f, 0.5f));
    }


    void SetPlayerDamage()
    {
        Vector3 findPos = Player.position;
        findPos.y = this.transform.position.y;
        if(Vector3.Distance(this.transform.position, findPos) < 1.0f)
        {
            if(!isAttack)
            {
                isAttack = true;
                Player.GetComponent<PlayerControl>().SetDamage(attackDamage);
            }
        }
    }



}
