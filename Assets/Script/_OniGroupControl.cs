using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _OniGroupControl : MonoBehaviour {
    public GameObject Player;                   // 角色对象
    //public _GameControl gameControl;            // 游戏管理脚本
    public GameObject[] OniPrefab;              // 怪物预制体
    public float runSpeed = 2.0f;               // 移动速度
    public float groupRange = 1.0f;             // 怪物生成的范围
    private _OniControl[] Onis;                 // 编队的所有怪物数组
    public bool isAlive = true;                 // 编队是否活着

    public bool isDecelerate;                   // 是否加速-减速模式
    public float minSpeed = 5.0f;               // 最小速度
    public float maxSpeed = 30.0f;              // 最大速度
    private float leaveSpeed = 15.0f;           // 离场速度
    public float speedUpDamp = 5.0f;            // 加速速度
    public float speedCutDamp = 3.0f;           // 减速速度
    public float distanceSpeedUp=4.0f;          // 加速距离
    public float distanceSpeedCut = 7.0f;      // 减速距离

    public float waveSpeed = 3.0f;              // 左右移动速度

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
        //gameControl = GameObject.FindGameObjectWithTag("GameController").GetComponent<_GameControl>();
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
                runSpeed = Mathf.Lerp(runSpeed, maxSpeed, speedUpDamp * Time.deltaTime);
                break;
            // 减速
            case STATE.SPEEDCUT:
                runSpeed = Mathf.Lerp(runSpeed, minSpeed, speedCutDamp * Time.deltaTime);
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
        basePosition.y--;
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
        else if(state == STATE.SPEEDUP && transform.position.x > Player.transform.position.x + distanceSpeedCut)
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
    /// <param name="evaluation">击飞程度</param>
    public void OnAttackedFromPlayer(_GameControl.EVALUATION evaluation)
    {
        isAlive = false;
        state = STATE.DEFEATED;
        GetComponent<BoxCollider>().enabled = false;
        _PlayerControl playerControl = Player.GetComponent<_PlayerControl>();

        runSpeed = playerControl.runSpeed;  // 固定编队在屏幕的位置

        // 设置击飞速度,以圆锥型状发散发射出去
        float groupSpeed = 0.0f;    // 整体速度
        float groupAngle = 0.0f;    // 整体发射角度
        float randSpeed = 0.0f;     // 散开速度
        float randAngle = 0.0f;     // 散开角度
        if (evaluation== _GameControl.EVALUATION.OKAY)
        {
            groupSpeed = 10.0f;
            groupAngle = 20.0f;
            randSpeed = 2.0f;
        }
        else if (evaluation == _GameControl.EVALUATION.GOOD)
        {
            groupSpeed = 10.0f;
            groupAngle = -20.0f;
            randSpeed = 3.0f;
        }
        else if (evaluation == _GameControl.EVALUATION.GREAT)
        {
            groupSpeed = 15.0f;
            groupAngle = -40.0f;
            randSpeed = 4.0f;
        }
        groupSpeed *= Random.Range(0.8f, 1.2f);
        for (int i = 0; i < Onis.Length; i++)
        {
            Vector3 speed = Vector3.forward * randSpeed;
            randAngle = Random.Range(0.0f, 360.0f);
            speed = Quaternion.AngleAxis(randAngle, Vector3.up) * speed;
            speed += groupSpeed * Vector3.up;
            speed= Quaternion.AngleAxis(groupAngle, Vector3.forward) * speed;
            Onis[i].OnAttack(speed);
        }
    }

    /// <summary>
    /// 怪物离场
    /// </summary>
    public void BeginLeave()
    {
        isAlive = false;
        GetComponent<BoxCollider>().enabled = false;
        state = STATE.LEAVE;
    }

    /// <summary>
    /// 获取怪物数量
    /// </summary>
    /// <returns></returns>
    public int GetCount()
    {
        return Onis.Length;
    }
}
