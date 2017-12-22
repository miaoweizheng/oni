using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class _TitleGUIControl : MonoBehaviour {

    public _TitleSceneControl sceneControl; // 场景管理脚本
    public Image titleImage;                // 标题
    public Button startButton;              // 开始按钮

    public RawImage fadeImage;              // 场景淡入淡出遮罩
    private float fadeSpeed = 1.5f;         // 场景淡入淡出速度
    private float delayStartTime = 0.05f;   // 延迟开始时间
    private float startTimer = 0.0f;        // 开始计时
    private bool isStart = false;

	void Start () {
        
	}
	
	void Update () {
        if (isStart)
        {
            FadeOut();
            
        }
        if (startTimer > 0.0f)
        {
            startTimer -= Time.deltaTime;
            if (startTimer <= 0.0f)
                startButton.GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 1.0f);
        }
    }

    /// <summary>
    /// 点击开始按钮
    /// </summary>
    public void OnStart()
    {
        isStart = true;
        startTimer = delayStartTime;
        GetComponent<CanvasGroup>().interactable = false;   // 禁止控件交互
        startButton.GetComponent<RectTransform>().localScale = new Vector3(2.0f, 2.0f, 2.0f);   // 放大按钮
        GetComponent<AudioSource>().Play();
        //sceneControl.StartGame();
    }

    /// <summary>
    /// 场景淡出
    /// </summary>
    public void FadeOut()
    {
        fadeImage.color = Color.Lerp(fadeImage.color, Color.black, fadeSpeed * Time.deltaTime);
        if (fadeImage.color.a > 0.95f)
            sceneControl.StartGame();
    }
}
