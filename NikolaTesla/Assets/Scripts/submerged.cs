using UnityEngine;

public class submerged : MonoBehaviour {
    public float depth=1;
    public float speed=2;
    float y;
    float phase = 0;

    void Awake(){y=transform.position.y;phase=Random.Range(-100f,100f);}


    void Update() {
        // please dont do maths like this...
        // this is not how maths works
        transform.position = new Vector3(transform.position.x,y+(Mathf.Sin((Time.time+phase)*speed)*depth),transform.position.z);
    }
}
