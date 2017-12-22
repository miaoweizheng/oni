using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _PlayerControl : MonoBehaviour {

    public _GameControl gameControl;                // 游戏管理脚本
    private Animation charaAnimation;               // 动画控件
    private bool isPlaying = true;                  // 是否在游戏中

    /// <summary>
    /// 攻击动作
    /// </summary>
    private enum ATTACKMOTION { LEFT,RIGHT};
    ATTACKMOTION                        attackMotion = ATTACKMOTION.LEFT;   // 攻击动作
    private _AttackColliderControl      attackColliderControl;              // 攻击触发器
    private bool isCombo = false;                                           // 是否连击

    private AudioSource attackAudioSource;      // 角色攻击音频源
    public AudioClip[]  attackAudioClip;        // 角色攻击音频
    private int         attackAudioIndex = 0;   // 角色攻击音频索引
    private AudioSource missAudioScource;       // 失误音频源
    public AudioClip    missAudioClip;          // 失误音频

    private AudioSource                 swordAudioSource;   // 剑斩击音频源
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

    public ParticleSystem runAFX;               // 奔跑特效
    private AudioSource runAudioSource;         // 奔跑音频源
    public AudioClip    runAudioClip;           // 奔跑音频
    private const float runRateMin = 0.2f;      // 奔跑最小音调
    private const float runRateMax = 0.5f;      // 奔跑最大音调
    public float        runSpeed;               // 速度
    public const float  maxSpeed = 20.0f;       // 最大速度
    public const float  speedAdd = 5.0f;        // 加速值 
    public const float  speedSub = 4.0f;        // 减速值

    // 奔跑状态
    public enum RUNSTATE
    {
        STOP,       // 停止
        RUN,        // 奔跑
    }
    public RUNSTATE runState = RUNSTATE.RUN;               // 奔跑状态

    private void Awake()
    {
        charaAnimation = transform.GetComponentInChildren<Animation>();
        attackColliderControl = transform.GetComponentInChildren<_AttackColliderControl>();
    }

    void Start () {
        // 添加并初始化音频源组件
        attackAudioSource = gameObject.AddComponent<AudioSource>();

        runAudioSource = gameObject.AddComponent<AudioSource>();
        runAudioSource.loop = true;
        runAudioSource.clip = runAudioClip;
        //runAudioSource.volume = 1.0f;
        runAudioSource.Play();

        missAudioScource = gameObject.AddComponent<AudioSource>();
        missAudioScource.clip = missAudioClip;

        swordAudioSource = gameObject.AddComponent<AudioSource>();

    }

    void Update () {
        // 设置速度
        Vector3 velocity = Vector3.zero;
        if (runState == RUNSTATE.RUN)
            runSpeed += speedAdd * Time.deltaTime;  // 加速
        else if (runState == RUNSTATE.STOP)
        {
            runSpeed -= speedSub * Time.deltaTime;  // 减速
            if (runSpeed < 0.0f && charaAnimation.IsPlaying("P_run"))
            {
                runAFX.Stop();
                charaAnimation.Play("P_stop");
            }
        }
        runSpeed = Mathf.Clamp(runSpeed, 0.0f, maxSpeed);
        velocity.x = runSpeed;
        GetComponent<Rigidbody>().velocity = velocity;
        // 根据速度设置奔跑音调
        float runRate = runSpeed / maxSpeed;
        runAudioSource.pitch = Mathf.Lerp(runRateMin, runRateMax, runRate);

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
                missAudioScource.Play();
                charaAnimation.Play("P_yarare");
                charaAnimation.CrossFadeQueued("P_run");
            }


        }
    }

    /// <summary>
    /// 攻击
    /// </summary>
    private void AttackControl()
    {
        if (!isPlaying) return;

        if (canAttack && Input.GetMouseButtonDown(0))
        {
            // 连击
            if (isCombo)
            {
                charaAnimation.Play("P_attack_Rot");
                attackRightAFX.OnAttackAFX();
            }
            else
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
            }
            charaAnimation.CrossFadeQueued("P_run");

            swordAudioSource.PlayOneShot(swordAudioClip);   // 剑斩击音效

            // 切换角色攻击音效
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
            {
                canAttack = true;
                isCombo = false;
            }
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
    /// <param name="position">击中位置</param>
    public void AttackTheTarget(Vector3 position)
    {
        canAttack = true;
        isCombo = true;
        swordAudioSource.PlayOneShot(swordHitAudioClip);
        hitAFX.transform.position = position;
        hitAFX.Play();
    }

    /// <summary>
    /// 游戏结束
    /// </summary>
    public void GameEnd()
    {
        isPlaying = false;
        runSpeed = 10.0f;
        runState = _PlayerControl.RUNSTATE.STOP;
    }
}
