using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieAttack : Attack
{
    public Enemy zombie;
    public float damage;
    Transform player;

    public void Start() {
        player = GameObject.FindWithTag("Player").transform;
    }

    public void fired() {
        IEnumerator delayedCheck() {
            yield return new WaitForSeconds(this.anim.length-1.5f);
            if (zombie.PlayerInLOS() && Vector3.Distance(transform.position,player.position) < 3f && zombie.attacking) {
                player.GetComponent<Control>().Hurt(damage);
            }
        }
        StartCoroutine(delayedCheck());
    }
}
