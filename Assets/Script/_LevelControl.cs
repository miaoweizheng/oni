using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _LevelControl : MonoBehaviour {

    // 怪物模式
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



    private float normalSpeed = 5.0f;                   // 普通速度
    private float rapidSpeed = 3.0f;                    // 快速
    private float slowSpeed = 15.0f;                    // 慢速
    private float passingSpeed = 0.0f;                  // 赶超速度
    public float minDispatchDampTime = 0.5f;            // 普通模式生成怪物的间隔时间
    public float maxDispatchDampTime = 5.0f;            // 缓慢模式生成怪物的间隔时间


    public int repeats = 0;     // 同一个模式下生成怪物的波数
    public int curRepeat = 0;   // 当前波数
	
	void Update () {
        time += Time.deltaTime;
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
                    CreateOniGroup(position, normalSpeed, level,groupType);
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
                    dispatchTime += Random.Range(0, maxDispatchDampTime / 2);
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

    /// <summary>
    /// 创建怪物编队
    /// </summary>
    /// <param name="position">怪物出现的位置</param>
    /// <param name="speed">怪物的速度</param>
    /// <param name="size">怪物的数量</param>
    /// <param name="type">怪物模式</param>
    private void CreateOniGroup(Vector3 position, float speed, int level,GROUPTYPE type)
    {
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
                    repeats = Random.Range(3, 8);
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

    public void ResetLevel()
    {
        level = 0;
        curRepeat = 0;
        repeats = 0;
        groupType = GROUPTYPE.NORMAL;
        dispatchTime += 5.0f;
    }
}
