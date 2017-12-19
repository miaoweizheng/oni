using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _OniGroupControl : MonoBehaviour {
    public GameObject Player;                   // 角色对象
    public _GameControl gameControl;            // 游戏管理脚本
    public GameObject[] OniPrefab;              // 怪物预制体
    public float runSpeed = 2.0f;               // 移动速度
    public float groupRange = 1.0f;             // 怪物生成的范围
    private _OniControl[] Onis;                 // 编队的所有怪物数组
    public bool isAlive = true;                 // 编队是否活着

    public bool isDecelerate;                   // 是否加速-减速模式
    public float minSpeed = 5.0f;               // 最小速度
    public float maxSpeed = 30.0f;              // 最大速度
    private float leaveSpeed = 15.0f;           // 离场速度
    public float speedDamp = 5.0f;              // 加速度
    public float distanceSpeedUp=4.0f;          // 加速距离
    public float distanceSpeedDown = 7.0f;      // 减速距离

    public float waveSpeed = 2.0f;              // 左右移动速度

    // 状态
    public enum STATE {
        NORMAL,     // 正常
        SPEEDUP,    // 加速
        SPEEDCUT,   // 减速
        LEAVE,      // 离场
        DEFEATED    // 被击中
    };
    public STATE state = STATE.NORMAL;         // 状态

    private void Awake()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        gameControl = GameObject.FindGameObjectWithTag("GameController").GetComponent<_GameControl>();
    }

	
	void Update () {
        // 被超越后移除怪物
        //if (transform.position.x < Player.transform.position.x) isAlive = false;

        // 如果是加速-减速模式，则加速
        if (isDecelerate) SpeedUpDown();

        switch (state)
        {
            // 加速
            case STATE.SPEEDUP:
                runSpeed = Mathf.Lerp(runSpeed, maxSpeed, speedDamp * Time.deltaTime);
                break;
            // 减速
            case STATE.SPEEDCUT:
                runSpeed = Mathf.Lerp(runSpeed, minSpeed, speedDamp * Time.deltaTime);
                break;
            // 离场
            case STATE.LEAVE:
                runSpeed = leaveSpeed;
                break;
            // 被击中
            case STATE.DEFEATED:

                break;
        }
        
        // 如果所有怪物都消失则销毁
        if (!isAlive)
        {
            bool leave = true;
            foreach (var oni in Onis)
            {
                if (oni.GetComponent<Renderer>().isVisible)
                {
                    leave = false;
                    break;
                }

            }
            if (leave) Destroy(gameObject);
        }

	}


    private void FixedUpdate()
    {
        // 向前奔跑
        Vector3 position = transform.position;
        position.x += runSpeed * Time.deltaTime;
        transform.position = position;
    }


    /// <summary>
    /// 创建怪物
    /// </summary>
    /// <param name="num">怪物数量</param>
    /// <param name="basePosition">怪物初始位置</param>
    public void CreateOnis(int num,Vector3 basePosition)
    {
        if (OniPrefab == null) return;
        Onis = new _OniControl[num];
        for (int i = 0; i < num; i++)
        {
            int index = 0;
            Vector3 position = basePosition;
            position.x += Random.Range(-groupRange, groupRange);
            position.z += Random.Range(-groupRange, groupRange);
            index = Random.Range(0, 2);
            GameObject Oni = Instantiate(OniPrefab[index], position, Quaternion.identity);
            Oni.transform.parent = this.transform;
            Onis[i] = Oni.GetComponent<_OniControl>();
            // 设置左右移动
            index = Random.Range(0, 2);
            if (index == 0) Onis[i].SetWave(groupRange, waveSpeed, true);
            else Onis[i].SetWave(groupRange, waveSpeed, false);


        }
    }

    /// <summary>
    /// 加速-减速
    /// </summary>
    private void SpeedUpDown()
    {
        // 当接近角色时加速
        if (state == STATE.NORMAL && transform.position.x < Player.transform.position.x + distanceSpeedUp)
        {
            state = STATE.SPEEDUP;
            SetOnisMotionState(2);
        }
        // 当远离角色时减速
        else if(state == STATE.SPEEDUP && transform.position.x > Player.transform.position.x + distanceSpeedDown)
        {
            state = STATE.SPEEDCUT;
            SetOnisMotionState(1);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            //GetComponent<BoxCollider>().enabled = false;
        }
    }

    /// <summary>
    /// 设置奔跑动画
    /// </summary>
    /// <param name="index"></param>
    private void SetOnisMotionState(int index)
    {
        for (int i = 0; i < Onis.Length; i++)
            Onis[i].SetMotionState(index);
    }

    /// <summary>
    /// 被击中
    /// </summary>
    public void OnAttackedFromPlayer()
    {
        // 计算得分
        gameControl.Scored(Onis.Length);

        isAlive = false;
        state = STATE.DEFEATED;
        GetComponent<BoxCollider>().enabled = false;
        _PlayerControl playerControl = Player.GetComponent<_PlayerControl>();

        runSpeed = playerControl.runSpeed;  // 固定编队在屏幕的位置

        // 设置整体击飞速度
        Vector3 baseSpeed=Vector3.zero; // 基础速读
        if (gameControl.evaluation== _GameControl.EVALUATION.OKAY)
        {
            baseSpeed = new Vector3(-4, 8, 0);
        }
        else if (gameControl.evaluation == _GameControl.EVALUATION.GOOD)
        {
            baseSpeed = new Vector3(4, 10, 0);
        }
        else if (gameControl.evaluation == _GameControl.EVALUATION.GREAT)
        {
            baseSpeed = new Vector3(8, 15, 0);
        }
        baseSpeed.z += Random.Range(-10.0f, 10.0f);

        for (int i = 0; i < Onis.Length; i++)
        {
            // 怪物击飞方向随机波动
            Vector3 speed = baseSpeed;
            speed *= Random.Range(0.8f, 1.2f);
            speed += Onis[i].GetComponent<Rigidbody>().velocity;
            Onis[i].OnAttack(speed);
        }
    }

    /// <summary>
    /// 碰撞后离场
    /// </summary>
    public void BeginLeave()
    {
        isAlive = false;
        GetComponent<BoxCollider>().enabled = false;
        state = STATE.LEAVE;
    }
}
