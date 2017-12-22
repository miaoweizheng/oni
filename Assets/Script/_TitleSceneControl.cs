using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class _TitleSceneControl : MonoBehaviour {

	void Start () {
		
	}
	
	void Update () {
		
	}


    /// <summary>
    /// 开始游戏
    /// </summary>
    public void StartGame()
    {
        SceneManager.LoadScene("_GameScene");
    }
}
