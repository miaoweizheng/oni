using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 场景淡入淡出类
/// 使用委托添加要调用的切换场景的函数
/// </summary>
public class _FadeInOut : MonoBehaviour{

    public RawImage fadeImage;              // 场景淡入淡出遮罩
    public float fadeSpeed = 1.5f;          // 场景淡入淡出速度
    public bool isStart = false;            // 是否进入场景
    public bool isEnd = false;              // 是否结束场景

    public delegate void StartSceneDelegate();
    public delegate void EndSceneDelegate();
    /// <summary>
    /// 进入场景
    /// </summary>
    public StartSceneDelegate StartScene;
    /// <summary>
    /// 结束场景
    /// </summary>
    public StartSceneDelegate EndScene;

    private void Start()
    {
        
        isStart = true;
        fadeImage.color = Color.black;
    }

    void Update () {
        if (isStart) FadeIn();
        if (isEnd) FadeOut();
	}

    /// <summary>
    /// 场景淡入
    /// </summary>
    protected void FadeIn()
    {
        fadeImage.color = Color.Lerp(fadeImage.color, Color.clear, fadeSpeed * Time.deltaTime);
        if (fadeImage.color.a < 0.05f)
        {
            isStart = false;
            StartScene();
        }
    }
    /// <summary>
    /// 场景淡出
    /// </summary>
    protected void FadeOut()
    {
        fadeImage.color = Color.Lerp(fadeImage.color, Color.black, fadeSpeed * Time.deltaTime);
        if (fadeImage.color.a > 0.95f)
        {
            isEnd = false;
            EndScene();
        }
    }

}
