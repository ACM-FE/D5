using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitscanAttack : Attack {
    public GameObject decal;
    public float range;

    public void fired() {
        Ray ray = Camera.main.ScreenPointToRay(new Vector2(Screen.width/2,Screen.height/2));
        RaycastHit hit;

        Physics.Raycast(ray,out hit,range);

        try {hit.transform.ToString();}
        catch {
            return;
        }

        GameObject objectHit = hit.transform.gameObject;
    
        if (objectHit.tag != "Enemy") {
            Instantiate(decal,hit.point,Quaternion.FromToRotation(Vector3.forward,hit.normal));
        } else if (objectHit.tag == "Enemy") {
            objectHit.SendMessage("hit",this.damage);
        }
    }
}
