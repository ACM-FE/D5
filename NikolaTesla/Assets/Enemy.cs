using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour {
    private NavMeshAgent agent;
    private Transform player;
    private Animator anim;
    private new Rigidbody rigidbody;
    private new CapsuleCollider collider;
    private new AudioSource audio;
    private Weapon weapon;

    [Header("Stats")]
    public int health;
    public float speed;

    [Header("Animations")]
    public AnimationClip AttackAnim;
    public AnimationClip HitAnim;


    [Header ("Internals")]
    
    [SerializeField]
    private float attackDistance;
    [SerializeField]
    private bool hunting;
    [SerializeField]
    private float aggroTime;
    [SerializeField]
    private float steadyTime;

    // DEBUG INTERNALS
    [HideInInspector]
    public bool attacking = false;
    private bool canMove = true;

    [SerializeField]
    private float aggroClock = 0f;
    private bool alerted = false;
    

    void Start() {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        anim = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();
        collider = GetComponent<CapsuleCollider>();
        weapon = GetComponentInChildren<Weapon>();
        audio = GetComponent<AudioSource>();
    }

    // messy bullshit
    // why the fuck would you write code like this
    public bool PlayerInLOS() {
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
        if ((PlayerInLOS() || alerted )&& agent.enabled) {
            //print("WOOFWOOFBARKBARK");
            canMove=true;
            aggroClock = aggroTime;
            
        } else if (aggroClock > 0f && agent.enabled) {
            alerted = false;
            //print(aggroClock);
            //print("grrrr");
            canMove = true;
            aggroClock -= Time.deltaTime;
        } else {canMove=false; alerted=false;}

        if (Vector3.Distance(transform.position,player.position) < attackDistance && !attacking && PlayerInLOS()) {
            Attack();
        }

        Move();
    }

    void Attack() {
        //StartCoroutine(attack());
        attacking = true;
        anim.SetTrigger("attack");
        print(weapon.makeAttack().name);

        StartCoroutine(finishAttack(AttackAnim.length + steadyTime));
    }

    void Move() {
        if (canMove&&!attacking) {
            agent.destination = player.position;
            anim.SetBool("walking", true);
            agent.speed = speed;
        } else {
            anim.SetBool("walking", false);
            agent.speed = 0;
        }
    }

    // hopefully obsolete...
    IEnumerator attack() {
        attacking = true;

        // fuck you for writing this. 
        // if you keep this in your final build, you deserve the death penalty.
        anim.SetTrigger("attack");
        yield return new WaitUntil(() => anim.GetAnimatorTransitionInfo(0).fullPathHash != 0);
        canMove = false;
        yield return new WaitUntil(() => anim.GetAnimatorTransitionInfo(0).fullPathHash == 1567584094); 
        canMove = true;

        attacking = false;
    }

    void hit(int damage) {
        health -= damage;
        canMove = false;
        attacking = false;
        if (health <= 0) {
            print("owie");
            rigidbody.isKinematic = true;
            canMove = false;
            collider.enabled=false;
            agent.enabled=false;
            audio.enabled = false;
            anim.SetBool("Dead",true);

            // final thing or everything breaks
            this.enabled = false;
        } else {
            anim.SetTrigger("Hit");
            StartCoroutine(moveDelay(HitAnim.length));
        }
    }

    IEnumerator moveDelay(float delay) {
        yield return new WaitForSeconds(delay);
        canMove=true;
        agent.destination = player.position;
        aggroClock = aggroTime;
    }
    IEnumerator finishAttack(float delay) {
        yield return new WaitForSeconds(delay);
        attacking = false;
    }
}