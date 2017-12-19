using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _AnimatedTextureExtendedUV : MonoBehaviour {

    private float AFXTimer = 0.0f;              // 斩击特效计时
    private const float AFXPLAYTIME = 0.4f;     // 斩击特效时间
    // 纹理参数
    public int rowCount = 2;
    public int colCount = 4;
    public int totalcount = 8;
    public int rowIndex = 0;
    public int colIndex = 0;
    public int fps = 30;
    public int frame = 18;

	void Start () {
        GetComponent<Renderer>().enabled = false;
	}
	
	void Update () {
        // 根据动画帧计算当前纹理索引
        int index = (int)((AFXTimer * fps) / frame * totalcount);
        if (index < totalcount)
        {
            SetSprite(index);
            AFXTimer += Time.deltaTime;
        }
        else
            GetComponent<Renderer>().enabled = false;
    }

    /// <summary>
    /// 设置纹理贴图
    /// </summary>
    /// <param name="index">纹理索引</param>
    private void SetSprite(int index)
    {
        // 设置纹理尺寸
        float sizeX = 1.0f / colCount;
        float sizeY = 1.0f / rowCount;
        Vector2 size = new Vector2(sizeX, sizeY);
        GetComponent<Renderer>().material.SetTextureScale("_MainTex", size);
        // 设置纹理坐标
        colIndex = index % colCount;
        rowIndex = index / colCount;
        float offsetX = sizeX * colIndex;
        float offsetY = sizeY * (rowCount - 1 - rowIndex);  // 纹理坐标原点在左下角，因此需要转换以下坐标值
        Vector2 offset = new Vector2(offsetX, offsetY);
        GetComponent<Renderer>().material.SetTextureOffset("_MainTex", offset);
    }

    /// <summary>
    /// 攻击特效
    /// </summary>
    public void OnAttackAFX()
    {
        GetComponent<Renderer>().enabled = true;
        AFXTimer = 0;
    }
}
