using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _LevelControl : MonoBehaviour {
 
    /// <summary>
    /// 怪物模式
    /// </summary>
    public enum GROUPTYPE {
        NORMAL,     // 普通模式
        RAPID,      // 急速模式
        SLOW,       // 缓慢模式
        PASSING,    // 赶超模式
        DECELERATE  // 加速-减速模式
    };

    public GROUPTYPE groupType = GROUPTYPE.NORMAL;      // 怪物模式
    public GameObject Player;                           // 角色对象
    public GameObject OniGroupPrefab;                   // 怪物编队预制体
    public bool dispatch = false;                       // 是否生成怪物
    private int level = 0;                              // 普通模式等级
    private float time = 0.0f;                          // 游戏时间
    private float dispatchTime = 5.0f;                  // 下一次生成怪物的时间

    private float normalSpeed = 5.0f;           // 普通速度
    private float rapidSpeed = 3.0f;            // 快速
    private float slowSpeed = 15.0f;            // 慢速
    private float passingSpeed = 0.0f;          // 赶超速度
    public float minDispatchDampTime = 0.5f;    // 普通模式生成怪物的间隔时间
    public float maxDispatchDampTime = 5.0f;    // 缓慢模式生成怪物的间隔时间

    public GameObject[] OniYamaPrefab;          // 怪物堆预制体
    public GameObject[] OniStillPrefab;         // 发射怪物预制体
    private const float emitTime = 1.0f;        // 发射怪物生成时间
    private const float emitDampTime = 0.2f;    // 发射怪物时间间隔
    private int emitCount = 5;                 //发射数量
    

    public int repeats = 0;     // 同一个模式下生成怪物的波数
    public int curRepeat = 0;   // 当前波数

    public bool isEnd = false;  // 游戏是否结束
	
	void Update () {
        time += Time.deltaTime;
        if (!isEnd)
        {
            if (time > dispatchTime) dispatch = true;

            if (dispatch)
            {
                Vector3 position = Vector3.zero;    // 怪物生成的位置
                position.x = Player.transform.position.x;
                position.x += _FloorControl.width / 2;

                switch (groupType)
                {
                    // 普通模式
                    case GROUPTYPE.NORMAL:
                        dispatch = false;
                        CreateOniGroup(position, normalSpeed, level, groupType);
                        dispatchTime += minDispatchDampTime * (5 - level);
                        if (level < 3)
                            level++;
                        else
                        {
                            curRepeat++;
                            if (curRepeat >= repeats)
                                TurnGroupType();
                        }
                        break;
                    // 急速模式
                    case GROUPTYPE.RAPID:
                        dispatch = false;
                        CreateOniGroup(position, rapidSpeed, level, groupType);
                        dispatchTime += Random.Range(0, minDispatchDampTime * 2);
                        curRepeat++;
                        if (curRepeat >= repeats)
                            TurnGroupType();
                        break;
                    // 缓慢模式
                    case GROUPTYPE.SLOW:
                        dispatch = false;
                        CreateOniGroup(position, slowSpeed, level, groupType);
                        dispatchTime += maxDispatchDampTime;
                        curRepeat++;
                        if (curRepeat >= repeats)
                            TurnGroupType();
                        break;
                    // 赶超模式
                    case GROUPTYPE.PASSING:
                        dispatch = false;
                        CreateOniGroup(position, slowSpeed, level, groupType);
                        position.x += Random.Range(_FloorControl.width * 2 / 3, _FloorControl.width * 4 / 3);
                        CreateOniGroup(position, passingSpeed, level, groupType);
                        dispatchTime += maxDispatchDampTime;
                        curRepeat++;
                        if (curRepeat >= repeats)
                            TurnGroupType();
                        break;
                    // 加速-减速模式
                    case GROUPTYPE.DECELERATE:
                        dispatch = false;
                        CreateOniGroup(position, slowSpeed, level, groupType);
                        dispatchTime += maxDispatchDampTime;
                        curRepeat++;
                        if (curRepeat >= repeats)
                            TurnGroupType();
                        break;
                }

            }
        }
    }

    /// <summary>
    /// 创建怪物编队
    /// </summary>
    /// <param name="position">怪物出现的位置</param>
    /// <param name="speed">怪物的速度</param>
    /// <param name="size">怪物的数量</param>
    /// <param name="type">怪物模式</param>
    private void CreateOniGroup(Vector3 position, float speed, int level,GROUPTYPE type)
    {
        position.y = 1.0f;
        GameObject group = Instantiate(OniGroupPrefab, position, Quaternion.identity);
        if (group == null) return;

        int size = 1;
        if (level < 3)
        {
            size = level + 1;
        }
        else
        {
            size = Random.Range(6, 10);
        }
        _OniGroupControl groupControl = group.GetComponent<_OniGroupControl>();
        if (groupControl == null) return;
        groupControl.CreateOnis(size, position);
        groupControl.runSpeed = speed;
        if (type == GROUPTYPE.DECELERATE)
        {
            groupControl.isDecelerate = true;
        }
    }

    /// <summary>
    /// 切换模式
    /// </summary>
    private void TurnGroupType()
    {
        curRepeat = 0;
        if (groupType == GROUPTYPE.NORMAL)
        {
            int rand = Random.Range(1, 5);
            switch (rand)
            {
                case 1:
                    groupType = GROUPTYPE.RAPID;
                    repeats = Random.Range(3, 10);
                    break;
                case 2:
                    groupType = GROUPTYPE.SLOW;
                    repeats = Random.Range(1, 4);
                    break;
                case 3:
                    groupType = GROUPTYPE.PASSING;
                    repeats = 1;
                    break;
                case 4:
                    groupType = GROUPTYPE.DECELERATE;
                    repeats = 1;
                    break;
            }

        }
        else
        {
            groupType = GROUPTYPE.NORMAL;
            repeats = Random.Range(1, 4);
        }

    }

    /// <summary>
    /// 重置怪物模式
    /// </summary>
    public void ResetLevel()
    {
        level = 0;
        curRepeat = 0;
        repeats = 0;
        groupType = GROUPTYPE.NORMAL;
        dispatchTime += 5.0f;
    }

    /// <summary>
    /// 游戏结束
    /// </summary>
    public void GameEnd(int score)
    {
        isEnd = true;
        // 所有怪物离场
        GameObject[] oniGroups = GameObject.FindGameObjectsWithTag("OniGroup");
        foreach(var group in oniGroups)
        {
            group.GetComponent<_OniGroupControl>().BeginLeave();
        }

        // 生成怪物堆
        Vector3 position = Vector3.zero;
        position.x = Player.transform.position.x;
        position.x += _FloorControl.width / 2;

        int index = 0;
        if (score > 500) index = 0;
        else if (score > 100) index = 1;
        else index = 2;
        Instantiate(OniYamaPrefab[index], position, Quaternion.AngleAxis(180,Vector3.up));

        // 生成坠落怪物
        position.y += 10.0f;
        StartCoroutine(EmitOni(position));
    }

    /// <summary>
    /// 发射坠落怪物
    /// </summary>
    /// <param name="position">发射位置</param>
    /// <returns></returns>
    private IEnumerator EmitOni(Vector3 position)
    {
        Vector3 emitterPosition = position;
        yield return new WaitForSeconds(emitTime);
        for (int i = 0; i < emitCount; i++)
        {
            Vector3 randPosition = emitterPosition;
            randPosition.x += Random.Range(-1.0f, 1.0f);
            randPosition.z += Random.Range(-1.0f, 1.0f);

            int index = Random.Range(0, 2); 
            GameObject oniStill = Instantiate(OniStillPrefab[index], randPosition, Quaternion.identity);
            // 开始时播放坠落声音
            if (i == 0) oniStill.GetComponent<AudioSource>().Play();
            yield return new WaitForSeconds(emitDampTime);
        }
    }
}
