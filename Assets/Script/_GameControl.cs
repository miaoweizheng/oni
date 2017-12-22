using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _GameControl : MonoBehaviour {

    public GameObject Player;                       // 角色对象
    public _LevelControl levelControl;              // 模式控制脚本
    public _GUIControl guiControl;                  // GUI脚本

    private int score = 0;          // 分数
    public int okayCount = 0;       // okay斩击次数
    public int greatCount = 0;      // great斩击次数
    public float timer = 60.0f;     // 计时器
    public float addTime = 2.0f;    // 斩击增加游戏时间
    private bool isEnd = false;     // 游戏是否结束

    /// <summary>
    /// 斩击评估
    /// </summary>
    public enum EVALUATION
    {
        OKAY,
        GOOD,
        GREAT
    }
    public EVALUATION evaluation = EVALUATION.OKAY;     // 斩击评估
    private const float ATTACK_TIME_GOOD = 0.1f;        // 不错的攻击时机
    private const float ATTACK_TIME_GREAT = 0.05f;      // 精准的攻击时机

	void Start () {

	}
	
	void Update () {
        if (!isEnd)
        {
            timer -= Time.deltaTime;
            timer = Mathf.Clamp(timer, 0.0f, 60.0f);
            if (timer <= 0)
                GameEnd();
        }
	}

    /// <summary>
    /// 玩家失误
    /// </summary>
    public void OnPlayerMissed()
    {
        GameObject[] oniGroups = GameObject.FindGameObjectsWithTag("OniGroup");
        foreach(var oniGroup in oniGroups)
        {
            oniGroup.GetComponent<_OniGroupControl>().BeginLeave();
        }
        levelControl.ResetLevel();
    }

    /// <summary>
    /// 评估斩击得分
    /// </summary>
    /// <param name="count">怪物数量</param>
    /// <param name="attackTime">攻击时机</param>
    public EVALUATION Scored(int count,float attackTime)
    {
        if (attackTime < ATTACK_TIME_GREAT)
        {
            evaluation = EVALUATION.GREAT;
            greatCount++;
            timer += addTime;    // great斩击可以增加时间
        }
        else if (attackTime < ATTACK_TIME_GOOD)
        {
            evaluation = EVALUATION.GOOD;
            okayCount++;
        }
        else
        {
            evaluation = EVALUATION.OKAY;
            okayCount++;
        }
        score += count;

        guiControl.DrawEvaluation(evaluation);
        guiControl.SetScore(score);
        return evaluation;
    }

    /// <summary>
    /// 游戏结束
    /// </summary>
    public void GameEnd()
    {
        isEnd = true;
        levelControl.GameEnd(score);
        guiControl.OnResult(score, okayCount, greatCount);
        Player.GetComponent<_PlayerControl>().GameEnd();
    }
}
