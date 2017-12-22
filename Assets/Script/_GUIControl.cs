using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class _GUIControl : MonoBehaviour {

    public _FadeInOut fadeInOut;        // 场景淡入淡出脚本
    public _GameControl gameControl;    // 游戏管理脚本
    private AudioSource audioSource;    // 音频源
    public Sprite[] numberSprite;       // 数字纹理

    public Image startImage;    // 开始图标

    public Image[] timeImage;   // 时间图像

    public Image scoreView;                         // 分数界面
    public Image[] scoreImage;                      // 分数图片
    public AudioClip[] numberUpAudioClip;           // 数字音频
    public int currentScore = 0;                    // 当前分数
    private int targetScore = 0;                    // 目标分数
    private float scoreTimer = 0.0f;                // 分数动画计时
    private const float ScoreDampTime=0.1f;         // 分数动画间隔时间

    public Image evaluationImage;                           // 斩击评价图片
    public Sprite greatSprite;                              // GREAET斩击评价
    public Sprite okaySprite;                               // OKAY斩击评价
    private float evaluationTimer = 0.0f;                   // 斩击评价动画计时
    private const float evaluationDampTime = 1.0f;          // 斩击评价显示时间
    private const float evaluationInTime = 0.1f;            // 斩击评价出现时间
    private const float evaluationOutTime = 0.9f;           // 斩击评价消失时间

    public GameObject[] resultUI;               // 结束UI元素
    public AudioClip resultAudioClip;           // 结束UI音频
    public Image[] okayNumber;                  // okay斩击次数图像
    public Image[] greatNumber;                 // great斩击次数图像
    public Sprite[] rankSprite;                 // 评价等级精灵
    public Button returnButton;                 // 返回菜单按钮
    public AudioClip buttonAudioClip;           // 按钮音频
    private const float resultTime = 4.0f;      // 结束UI播放时间
    private const float resultDampTime = 0.5f;  // 结束UI播放时间间隔

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Start () {
        // 绑定场景淡入淡出函数
        fadeInOut.StartScene += StartGame;
        fadeInOut.EndScene += EndGame;
	}
	
	void Update () {
        // 显示时间
        DrawNumber(timeImage, (int)gameControl.timer);

        // 显示分数
        if (scoreTimer > 0.0f)
        {
            scoreTimer -= Time.deltaTime;
            ScoreControl();
        }
        if (targetScore > currentScore)
        {
            if (scoreTimer <= 0.0f)
            {
                if (targetScore > currentScore + 10)
                    currentScore += 5;
                else
                    currentScore++;
                scoreTimer = ScoreDampTime;
                DrawNumber(scoreImage, currentScore);
                int index = Random.Range(0, numberUpAudioClip.Length);
                audioSource.PlayOneShot(numberUpAudioClip[index]);
            }
        }

        //显示斩击评价
        if (evaluationTimer > 0.0f)
        {
            evaluationTimer -= Time.deltaTime;
            EvaluationControl();
        }
	}

    /// <summary>
    /// 显示数字
    /// </summary>
    /// <param name="image">数字图像</param>
    /// <param name="number">数字</param>
    private void DrawNumber(Image[] image,int number)
    {
        int digit = 0;
        for (int i = image.Length - 1; i >= 0; i--)
        {
            digit = number % 10;
            number /= 10;
            image[i].sprite = numberSprite[digit];
        }
    }

    /// <summary>
    /// 结束
    /// </summary>
    /// <param name="score">总得分</param>
    /// <param name="okayCount">okay斩击次数</param>
    /// <param name="greatCount">great斩击次数</param>
    public void OnResult(int score,int okayCount,int greatCount)
    {
        Image rankImage = resultUI[resultUI.Length - 1].GetComponent<Image>();
        if (score == 0) rankImage.sprite = rankSprite[3];
        else if (score < 100) rankImage.sprite = rankSprite[2];
        else if (score < 500) rankImage.sprite = rankSprite[1];
        else rankImage.sprite = rankSprite[0];

        DrawNumber(okayNumber, okayCount);
        DrawNumber(greatNumber, greatCount);

        StartCoroutine(DrawResultView());
    }

    /// <summary>
    /// 显示结束界面
    /// </summary>
    /// <returns></returns>
    private IEnumerator DrawResultView()
    {
        yield return new WaitForSeconds(resultTime);
        for(int i = 0; i < resultUI.Length; i++)
        {
            yield return new WaitForSeconds(resultDampTime);
            resultUI[i].SetActive(true);
            audioSource.PlayOneShot(resultAudioClip);
        }
        returnButton.gameObject.SetActive(true);
    }

    /// <summary>
    /// 得分
    /// </summary>
    /// <param name="score"></param>
    public void SetScore(int score)
    {
        targetScore = score;
    }

    /// <summary>
    /// 显示斩击评价
    /// </summary>
    /// <param name="evaluation">斩击评估</param>
    public void DrawEvaluation(_GameControl.EVALUATION evaluation)
    {
        if (evaluation == _GameControl.EVALUATION.GREAT)
            evaluationImage.sprite = greatSprite;
        else
            evaluationImage.sprite = okaySprite;
        evaluationImage.color = Color.white;
        evaluationTimer = evaluationDampTime;
    }

    /// <summary>
    /// 斩击评价动画
    /// </summary>
    private void EvaluationControl()
    {
        float damp = (evaluationTimer - evaluationOutTime )/ evaluationInTime;
        // 先逐渐变大
        Vector3 scale = Vector3.Lerp(Vector3.one, Vector3.zero, damp);
        evaluationImage.rectTransform.localScale = scale;
        // 再逐渐淡去
        damp = evaluationTimer / evaluationOutTime;
        Color color = Color.white;
        color.a = Mathf.Lerp(0.0f, 1.0f, damp);
        evaluationImage.color = color;
    }

    /// <summary>
    /// 分数动画
    /// </summary>
    private void ScoreControl()
    {
        float scale = scoreTimer * 15 + 1.0f;
        scale = Mathf.Max(1.0f, scale);
        for (int i = 0; i < scoreImage.Length; i++)
        {
            scoreImage[i].rectTransform.localScale = scale * Vector3.one;
        }
        
    }

    /// <summary>
    /// 点击返回菜单按钮
    /// </summary>
    public void OnReturn()
    {
        GetComponent<CanvasGroup>().interactable = false;
        audioSource.PlayOneShot(buttonAudioClip);
        StartCoroutine(ReturnControl());
    }

    /// <summary>
    /// 返回菜单
    /// </summary>
    private IEnumerator ReturnControl()
    {
        returnButton.GetComponent<RectTransform>().localScale = Vector3.one * 2;
        yield return new WaitForSeconds(0.05f);
        returnButton.GetComponent<RectTransform>().localScale = Vector3.one;
        OnEnd();
    }

    /// <summary>
    /// 开始进入场景
    /// </summary>
    public void OnStart()
    {
        fadeInOut.isStart = true;
    }

    /// <summary>
    /// 结束场景
    /// </summary>
    public void OnEnd()
    {
        fadeInOut.isEnd = true;
    }

    /// <summary>
    /// 开始游戏
    /// </summary>
    public void StartGame()
    {
        startImage.enabled = false;
    }

    /// <summary>
    /// 结束游戏
    /// </summary>
    public void EndGame()
    {
        SceneManager.LoadScene("_TitleScene");
    }

}
