using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitscanAttack : Attack {
    public GameObject decal;
    public float range;
    public float kineticEnergy;

    public void fired() {
        Ray ray = Camera.main.ScreenPointToRay(new Vector2(Screen.width/2,Screen.height/2));
        RaycastHit hit;

        Physics.Raycast(ray,out hit,range);

        try {hit.transform.ToString();}
        catch {
            return;
        }

        GameObject objectHit = hit.transform.gameObject;
    
        if (!objectHit.CompareTag("Enemy")) {
            Instantiate(decal,hit.point,Quaternion.FromToRotation(Vector3.forward,hit.normal)).transform.SetParent(hit.transform);
            if (objectHit.CompareTag("prop")) {
                objectHit.GetComponent<Rigidbody>().AddExplosionForce(100f*kineticEnergy,hit.point,5f);
            }
        } else if (objectHit.CompareTag("Enemy")) {
            objectHit.SendMessage("hit",this.damage);
        }
    }
}
