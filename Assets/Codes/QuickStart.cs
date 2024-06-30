using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class QuickStart : MonoBehaviour
{
    // Change this to the name of the scene you want to load
    public string sceneToLoad;
    public int time;

    void Start()
    {
        StartCoroutine(ChangeSceneAfterDelay(time));
    }

    IEnumerator ChangeSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneToLoad);
    }
}
