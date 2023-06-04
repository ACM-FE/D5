using UnityEngine;
using TMPro;
using System.Collections;


public class HUD : MonoBehaviour{
    public TMP_Text notification;

    public void notify(string text) {
        IEnumerator inNout(float duration) {
            notification.gameObject.SetActive(true);
            yield return new WaitForSeconds(duration);
            notification.gameObject.SetActive(false);
        }
        notification.text = text;
        StartCoroutine(inNout(2f));
    }
}
