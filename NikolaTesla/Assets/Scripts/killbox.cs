using UnityEngine;

public class killbox : MonoBehaviour {
    public Transform spawnpoint;
    private void OnTriggerEnter(Collider col) {
        if (col.gameObject.CompareTag("Player")) {
            GameObject.Find("FadeIn").GetComponentInChildren<Animation>().Play("fadeIn");
            col.transform.position = spawnpoint.position;
            col.transform.rotation = spawnpoint.rotation;
            
        } else if (col.gameObject.CompareTag("prop")) {
            Destroy(col.gameObject);
        }
    }
}
