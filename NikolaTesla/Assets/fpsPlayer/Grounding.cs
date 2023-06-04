using UnityEngine;
public class Grounding : MonoBehaviour {
    public float stayTime = 0;
    private void OnTriggerStay(Collider col) { 
        if (stayTime>0.1f) {
            GetComponentInParent<Control>().canJump = true;
        } else {
            print(stayTime);
            stayTime+=Time.deltaTime;
        }
    }
}
