using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _AttackColliderControl : MonoBehaviour {

    public GameObject Player;               // 角色对象
    public _GameControl gameControl;        // 游戏管理脚本
    private bool isAttacking = false;       // 攻击是否生效
	void Start () {
		
	}
	
	void Update () {
		
	}

    private void OnTriggerStay(Collider other)
    {
        if (!isAttacking) return;

        if (other.gameObject.tag == "OniGroup")
        {
            _OniGroupControl oniGroup = other.GetComponent<_OniGroupControl>();
            if (oniGroup.isAlive)
            {
                float attackTime = Player.GetComponent<_PlayerControl>().GetAttackTimer();  //  攻击时机
                int count = oniGroup.GetCount();    // 怪物数量
                _GameControl.EVALUATION evaluation = gameControl.Scored(count, attackTime); // 评估得分
                Player.GetComponent<_PlayerControl>().AttackTheTarget(oniGroup.transform.position); // 角色击中目标
                oniGroup.OnAttackedFromPlayer(evaluation);  // 怪物被击飞
            }
        }
    }

    /// <summary>
    /// 攻击是否生效
    /// </summary>
    /// <param name="bValue"></param>
    public void OnAttacked(bool bValue)
    {
        isAttacking = bValue;
    }
}
