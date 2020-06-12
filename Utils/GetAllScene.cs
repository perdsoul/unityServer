using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetAllScene : MonoBehaviour {

    public UnityEngine.UI.Dropdown dropdownList = null;
	// Use this for initialization
	void Start ()
    {
        int sceneCount = UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings;
        List<string> scenes = new List<string>();
        for (int i = 0; i < sceneCount; i++)
        {
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(UnityEngine.SceneManagement.SceneUtility.GetScenePathByBuildIndex(i));
            if (sceneName != "Launcher")
            {
                scenes.Add(sceneName);
            }
        }

        dropdownList.AddOptions(scenes);

       //Debug.LogError("Scnee: " + sceneCount);

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
