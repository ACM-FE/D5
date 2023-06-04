using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class gimbalCollider : MonoBehaviour
{
    public float x;
    public float y;
    public float z;
    public float turn;

    void OnValidate() {
        transform.rotation = Quaternion.identity * Quaternion.Euler(0,turn,0);
        GetComponent<BoxCollider>().size= new Vector3(x,y,z);
    }
}
