using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _GameControl : MonoBehaviour {

    public GameObject Player;                       // 角色对象
    public _LevelControl levelControl;              // 模式控制脚本
    // 斩击评估
    public enum EVALUATION
    {
        OKAY,
        GOOD,
        GREAT
    }
    public EVALUATION evaluation = EVALUATION.OKAY;
    private const float ATTACK_TIME_GOOD = 0.1f;        // 不错的攻击时机
    private const float ATTACK_TIME_GREAT = 0.05f;      // 精准的攻击时机

	void Start () {
		
	}
	
	void Update () {
		
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
    /// 得分
    /// </summary>
    /// <param name="count">怪物数量</param>
    public void Scored(int count)
    {
        float attackTime = Player.GetComponent<_PlayerControl>().GetAttackTimer();
        if (attackTime < ATTACK_TIME_GREAT)
        {
            evaluation = EVALUATION.GREAT;
        }
        else if (attackTime < ATTACK_TIME_GOOD)
        {
            evaluation = EVALUATION.GOOD;
        }
        else
        {
            evaluation = EVALUATION.OKAY;
        }
    }
}
