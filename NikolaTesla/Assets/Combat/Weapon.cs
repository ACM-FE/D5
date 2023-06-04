using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Weapon : MonoBehaviour {
    public new string name;
    BoxCollider hurtbox;
    Attack[] attacks;
    public Animation reload;

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

        attack.action.Invoke();
        return attack;
    }
}
