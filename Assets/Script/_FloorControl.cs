using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _FloorControl : MonoBehaviour {

    public GameObject mainCamera;       // 相机对象
    public static float width;         // 一个背景对象的宽度
    private int floorNum = 3;           // 背景对象数量
    private float totalWidth;           // 背景总宽度
    private float distance;             // 角色与背景初始位置的距离
    private Vector3 startPosition;      // 背景对象初始位置

    private void Start()
    {
        width = transform.Find("map1").GetComponent<Renderer>().bounds.size.x;
        totalWidth = width * floorNum;
        distance = 0.0f;
        startPosition = transform.position;
    }

    void Update () {
        Vector3 targetPosition = startPosition;
        distance = mainCamera.transform.position.x - startPosition.x;
        int n = Mathf.RoundToInt(distance / totalWidth);
        targetPosition.x += n * totalWidth;
        transform.position = targetPosition;
	}
}
