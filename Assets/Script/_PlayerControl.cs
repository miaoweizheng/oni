using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _PlayerControl : MonoBehaviour {

    public _GameControl gameControl;                // 游戏管理脚本
    private Animation charaAnimation;               // 动画控件

    /// <summary>
    /// 攻击动作
    /// </summary>
    private enum ATTACKMOTION { LEFT,RIGHT};
    ATTACKMOTION                        attackMotion = ATTACKMOTION.LEFT;   // 攻击动作
    private _AttackColliderControl      attackColliderControl;              // 攻击触发器

    public AudioSource  attackAudioSource;      // 角色攻击音频源
    public AudioClip[]  attackAudioClip;        // 角色攻击音频
    private int attackAudioIndex = 0;           // 角色攻击音频索引

    public AudioSource                  swordAudioSource;   // 剑斩击音频源
    public AudioClip                    swordAudioClip;     // 剑斩击音频
    public AudioClip                    swordHitAudioClip;  // 剑斩击命中音频
    public _AnimatedTextureExtendedUV   attackLeftAFX;      // 剑左手斩击特效
    public _AnimatedTextureExtendedUV   attackRightAFX;     // 剑右手斩击特效
    public ParticleSystem               hitAFX;             // 剑斩击命中特效

    private bool        canAttack = true;                   // 是否可以攻击
    private bool        isAttacking = false;                // 是否正在攻击
    public const float  ATTACK_DISABLE_TIME = 0.3f;         // 攻击失效时间
    public float        attackDisableTimer = 0.0f;          // 攻击失效计时
    public const float  ATTACK_TIME = 1.0f;                 // 攻击间隔时间
    public float        attackTimer = 0.0f;                 // 攻击间隔计时

    public float       runSpeed;            // 速度
    public const float maxSpeed = 20.0f;    // 最大速度
    public const float speedAdd = 5.0f;     // 加速值 
    public const float speedSub = 20.0f;    // 减速值

    // 奔跑状态
    enum RUNSTATE
    {
        STOP,       // 停止
        RUN,        // 奔跑
    }
    RUNSTATE runState = RUNSTATE.RUN;               // 奔跑状态

    private void Awake()
    {
        charaAnimation = transform.GetComponentInChildren<Animation>();
        attackColliderControl = transform.GetComponentInChildren<_AttackColliderControl>();
    }

    void Start () {
        attackAudioSource = gameObject.AddComponent<AudioSource>();
        swordAudioSource = gameObject.AddComponent<AudioSource>();

	}
	
	void Update () {
        // 设置速度
        if (runState == RUNSTATE.RUN)
            runSpeed += speedAdd * Time.deltaTime;  // 加速
        else if(runState==RUNSTATE.STOP)
            runSpeed -= speedSub * Time.deltaTime;  // 减速
        runSpeed = Mathf.Clamp(runSpeed, 0.0f, maxSpeed);
        GetComponent<Rigidbody>().velocity = runSpeed * Vector3.right;


        // 攻击
        AttackControl();

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "OniGroup")
        {
            _OniGroupControl oniGroup = collision.gameObject.GetComponent<_OniGroupControl>();
            if (oniGroup.isAlive)
            {
                runSpeed = 0.0f;
                gameControl.OnPlayerMissed();
            }
        }
    }

    /// <summary>
    /// 攻击
    /// </summary>
    private void AttackControl()
    {
        if (canAttack && Input.GetMouseButtonDown(0))
        {
            // 左手斩击
            if (attackMotion == ATTACKMOTION.LEFT)
            {
                charaAnimation.Play("P_attack_L");
                attackLeftAFX.OnAttackAFX();
                attackMotion = ATTACKMOTION.RIGHT;
            }
            // 右手斩击
            else if (attackMotion == ATTACKMOTION.RIGHT)
            {
                charaAnimation.Play("P_attack_R");
                attackRightAFX.OnAttackAFX();
                attackMotion = ATTACKMOTION.LEFT;
            }
            charaAnimation.CrossFadeQueued("P_run");

            // 切换斩击声音
            attackAudioSource.PlayOneShot(attackAudioClip[attackAudioIndex]);
            attackAudioIndex = (attackAudioIndex + 1) % attackAudioClip.Length;

            attackDisableTimer = ATTACK_DISABLE_TIME;
            attackTimer = ATTACK_TIME;

            canAttack = false;
            attackColliderControl.OnAttacked(true);
        }
        if (attackDisableTimer > 0)
        {
            attackDisableTimer -= Time.deltaTime;
            if (attackDisableTimer <= 0)
                attackColliderControl.OnAttacked(false);
        }
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0)
                canAttack = true;
        }
    }

    /// <summary>
    /// 获取攻击时机
    /// </summary>
    /// <returns></returns>
    public float GetAttackTimer()
    {
        return ATTACK_DISABLE_TIME - attackDisableTimer;
    }

    /// <summary>
    /// 击中目标
    /// </summary>
    public void AttackTheTarget()
    {
        canAttack = true;
    }
}
