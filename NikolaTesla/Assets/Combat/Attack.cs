using UnityEngine;
using UnityEngine.Events;

public class Attack : MonoBehaviour {
    public new string name;
    public float damage;
    public AnimationClip anim;
    public UnityEvent action;

    private Weapon weapon;

    void Awake() {
        weapon = GetComponent<Weapon>();
        this.action.AddListener(() => {this.SendMessage("fired");});
    }
}