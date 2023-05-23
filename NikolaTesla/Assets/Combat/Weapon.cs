using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {
    // clashes with Object.name - fix at some point
    public string name;
    BoxCollider hurtbox;
    public GameObject decal;
    Attack[] attacks;

    void Awake() {
        attacks = GetComponents<Attack>();
        hurtbox = GetComponent<BoxCollider>();
        if (attacks.Length == 0){throw new System.Exception("put a fuckin attack on the weapon moron");}
    }

    public Attack makeAttack(Attack attack = null) {
        // dont fucking do this??? like why
        if (attacks.Length == 0){throw new System.Exception("put a fuckin attack on the weapon moron");}
        // dont do this either please...
        if (attack==null) {attack = attacks[0];}
        //print(attack.name);

        return attack;
    }
}
