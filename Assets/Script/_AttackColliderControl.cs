using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _AttackColliderControl : MonoBehaviour {

    public GameObject Player;               // 角色对象
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
                Player.GetComponent<_PlayerControl>().AttackTheTarget(oniGroup.transform.position);
                oniGroup.OnAttackedFromPlayer();
            }
        }
    }

    public void OnAttacked(bool bValue)
    {
        isAttacking = bValue;
    }
}
