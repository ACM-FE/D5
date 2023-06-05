using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class unlockMouse : MonoBehaviour{
    public void unlock() {
        Cursor.lockState = CursorLockMode.None;
    }
}
