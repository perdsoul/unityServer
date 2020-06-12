using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class detectCollision : MonoBehaviour
{
    List<string> sceneList = new List<string>();

    void Start()
    {
        sceneList.Add("Launcher");
        sceneList.Add(SceneManager.GetActiveScene().name);
    }

    void OnTriggerEnter(Collider collider)
    {
        if (!sceneList.Contains(collider.gameObject.name)) {
            Debug.Log("start loading scene:" + collider.gameObject.name);
            sceneList.Add(collider.gameObject.name);
            SceneManager.LoadSceneAsync(collider.gameObject.name, LoadSceneMode.Additive);
        }
    }


}
