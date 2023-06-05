using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class customSplash : MonoBehaviour {
    void Start() {
        GetComponent<UnityEngine.Video.VideoPlayer>().loopPointReached += videoFinishedDelegate;
        GetComponent<UnityEngine.Video.VideoPlayer>().Pause();
        StartCoroutine("startVideo");
    }

    IEnumerator startVideo() {
        yield return new WaitForSeconds(1.0f);
        GetComponent<UnityEngine.Video.VideoPlayer>().Play();
    }

    void videoFinishedDelegate(object sender) {
        StartCoroutine("videoFinished");
    }
    IEnumerator videoFinished() {
        GameObject.Find("FadeOut").GetComponentInChildren<Animation>().Play("fadeOut");
        yield return new WaitForSeconds(2.5f); // this is yucky, and it will only get worse the more i am forced to use it
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
    }
}
