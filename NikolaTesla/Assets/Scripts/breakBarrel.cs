using UnityEngine.Events;
using UnityEngine;
using UnityEngine.VFX;

public class breakBarrel : MonoBehaviour {
    public UnityEvent onBreak;
    void Awake() {
        // lambdas are cringe :P
        onBreak.AddListener(() => {
            GetComponent<MeshRenderer>().enabled = false;
            GetComponentInChildren<VisualEffect>().Play();
            GetComponent<BoxCollider>().enabled=false;
        });
    }

    void broken() {
        onBreak.Invoke();
    }
}
