using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _OniControl : MonoBehaviour {

    public Animation charaAnimation;                   // 动画

    /// <summary>
    /// 怪物状态
    /// </summary>
    enum STATE
    {
        NONE,       // 空
        WAVE,       // 左右移动
        DEFEATED    // 被击飞
    }
    STATE state = STATE.NONE;

    private float waveAmplitude;             // 左右移动振幅
    private float waveSpeed;                 // 左右移动速度

	void Start () {
        charaAnimation = transform.GetChild(0).GetComponent<Animation>();
        charaAnimation["oni_run1"].speed = 1.0f;
        charaAnimation["oni_run2"].speed = 2.0f;
        transform.rotation = Quaternion.AngleAxis(180, Vector3.up);
        GetComponent<Rigidbody>().maxAngularVelocity = float.PositiveInfinity;      // 修改最大角速度限制
		GetComponent<Rigidbody>().centerOfMass = new Vector3(0.0f, 0.5f, 0.0f);     // 设置中心点为质心
    }

    void Update () {
        switch (state)
        {
            case STATE.WAVE:
                Wave();
                break;
            case STATE.DEFEATED:
                GetComponent<Rigidbody>().AddForce(2 * 9.8f * Vector3.down);    // 施加双倍重力，防止怪物的下落看起来轻飘飘的
                break;
        }
    }

    private void FixedUpdate()
    {
        
    }

    /// <summary>
    /// 设置奔跑动画
    /// </summary>
    /// <param name="speed"></param>
    public void SetMotionState(int motionIndex)
    {
        switch (motionIndex)
        {
            case 1:
                charaAnimation.Play("oni_run1");
                break;
            case 2:
                charaAnimation.Play("oni_run2");
                break;
        }
    }

    /// <summary>
    /// 设置左右移动
    /// </summary>
    /// <param name="amplitude">振幅</param>
    /// <param name="speed">速度</param>
    /// <param name="startLR">初始方向，ture向左，false向右</param>
    public void SetWave(float amplitude, float speed,bool startLR)
    {
        state = STATE.WAVE;
        waveAmplitude = amplitude;
        waveSpeed = speed;
        if (startLR)
            GetComponent<Rigidbody>().velocity = waveSpeed * Vector3.back;
        else
            GetComponent<Rigidbody>().velocity = waveSpeed * Vector3.forward;
    }

    /// <summary>
    /// 左右移动
    /// </summary>
    private void Wave()
    {
        if (transform.position.z > waveAmplitude)
            GetComponent<Rigidbody>().velocity = waveSpeed * Vector3.back;
        else if (transform.position.z < -waveAmplitude)
            GetComponent<Rigidbody>().velocity = waveSpeed * Vector3.forward;
    }

    /// <summary>
    /// 被击飞
    /// </summary>
    /// <param name="speed">击飞的速度</param>
    public void OnAttack(Vector3 speed)
    {
        state = STATE.DEFEATED;
        charaAnimation.Play("oni_yarare");
        GetComponent<Rigidbody>().velocity = speed;
        GetComponent<Rigidbody>().rotation = Quaternion.AngleAxis(90, Vector3.up) * Quaternion.LookRotation(speed); // 让怪物先朝向z轴，再旋转至速度方向
        GetComponent<Rigidbody>().angularVelocity = Vector3.Cross(Vector3.up, speed).normalized * Mathf.PI * 10.0f;
    }
}
