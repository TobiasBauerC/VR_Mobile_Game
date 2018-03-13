using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverController : MonoBehaviour 
{
    // Update is called once per frame
	void Update () 
    {
#if UNITY_ANDROID || UNITY_IOS
        for (int i = 0; i < Input.touchCount; ++i)
        {
            if (Input.GetTouch(i).phase == TouchPhase.Began)
            {
                SceneManager.instance.GoToMainMenu(0);
            }
        }
#endif
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            SceneManager.instance.GoToMainMenu(0);
        }
#endif
	}
}
