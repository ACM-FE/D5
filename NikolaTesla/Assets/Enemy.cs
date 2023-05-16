using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform player;
    private Animator anim;

    [Header("Stats")]
    public int health;
    public float speed;


    [Header ("Internals")]
    
    [SerializeField]
    private float attackDistance;
    [SerializeField]
    private bool hunting;
    [SerializeField]
    private float aggroTime;

    // DEBUG INTERNALS
    private bool attacking = false;
    private bool canMove = true;

    private float aggroClock = 0f;
    

    void Start() {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        anim = GetComponent<Animator>();
    }

    // messy bullshit
    // why the fuck would you write code like this
    bool PlayerInLOS() {
        /*
        RaycastHit hit;
        if (Physics.CapsuleCast(
            transform.position,
            transform.position + transform.forward * 20,
            1,
            transform.forward,
            out hit,
            50
        )) {print ("true");}
        return false;
        */
        RaycastHit hit;
        for (int i = -45; i <= 45; i += 5) {
            if (Physics.Raycast(
                transform.position + transform.up,
                Quaternion.AngleAxis(i,transform.up) * transform.forward,
                out hit,
                50
            )) {
                //print(hit.transform.gameObject.name);
                if (hit.transform.gameObject.CompareTag("Player")) {
                    return true;
                }
            }
        }
        return false;
    }

    void Update() {
        if (PlayerInLOS()) {
            //print("WOOFWOOFBARKBARK");
            aggroClock = aggroTime;
            Move();
        } else if (aggroClock > 0f) {
            print(aggroClock);
            print("grrrr");
            aggroClock -= Time.deltaTime;
            Move();
        } else {
            anim.SetBool("walking", false);
        }


        if (Vector3.Distance(transform.position,player.position) < attackDistance && !attacking) {
            StartCoroutine(attack());
        }
    }

    void Move() {
        if (canMove) {
            agent.destination = player.position;
            anim.SetBool("walking", true);
            agent.speed = speed;
        } else {
            anim.SetBool("walking", false);
            agent.speed = 0;
        }
    }

    IEnumerator attack() {
        attacking = true;

        anim.SetTrigger("attack");
        yield return new WaitUntil(() => anim.GetAnimatorTransitionInfo(0).fullPathHash != 0);
        canMove = false;
        yield return new WaitUntil(() => anim.GetAnimatorTransitionInfo(0).fullPathHash == 1567584094); 
        canMove = true;

        attacking = false;
    }
}