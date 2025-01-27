using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;


    [Header("# Game Control")]
    public bool isLive;
    public bool IsPaused { get; set; } = false;
    public bool IsBossing { get; set; } = false;
    public float gameTime;
    [Header("# Player Info")]
    public int playerId;
    public float health;
    public float maxHealth = 100;
    public int level;
    public int totalScore;
    public int kill;
    public int exp;
    public int coffeecnt;
    public int[] nextExp = { 10, 30, 60, 100, 150, 210, 280, 360, 450, 600 };
    [Header("# Game Object")]
    public PoolManager pool; 
    public Player player;
    public LevelUp uiLevelUp;
    public Result uiResult;
    public Transform uiJoy;
    public GameObject enemyCleaner;
    public GameObject pausePanel;
    public GameObject pauseButton;
    public GameObject levelUpUI;
    public GameObject bossManager;
    public Sprite[] pauseSprites;
    public ScreenFader fadeOut;
    public Text countdownText;

    void Awake()
    {
        instance = this;
        Application.targetFrameRate = 60;
    }


    public void GameStart(int id)
    {
        playerId = id;
        health = maxHealth;

        player.gameObject.SetActive(true);

        levelUpUI.SetActive(true);

        uiLevelUp.Select(playerId % 2); // 무기 많아졌을때를 대비하여 기본무기 지급을 위해 % 2 해놓음, 일단 지금은 삽(0) 총(1) 늘면 %3 하면 되겟네?
        isLive = true;
        Resume();

        AudioManager.instance.PlayBgm(true);
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
    }

    public void GameOver()
    {
        if (IsPaused)
            return;
        StartCoroutine(GameOverRoutine());
        Debug.Log("GameOver!");
    }
    public void GameWin()
    {
        StartCoroutine(WinRoutine());
        Debug.Log("Game Win!");
    }

    IEnumerator WinRoutine()
    {
        isLive = false; // 게임 중단 상태 설정
        yield return new WaitForSeconds(3f); // 연출을 위한 대기 시간
        player.anim.SetTrigger("Win"); // 플레이어 승리 애니메이션 실행
        yield return new WaitForSeconds(1.5f);
        uiResult.gameObject.SetActive(true); // 결과창 활성화
        uiResult.Win(); // 승리 처리 호출
        levelUpUI.SetActive(false);
        IsBossing = false;
        Stop(); // 게임 정지

        AudioManager.instance.PlayBgm(false); // 배경 음악 중단
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Win); // 승리 효과음 재생
    }

    IEnumerator GameOverRoutine()
    {
        isLive = false;
        yield return new WaitForSeconds(3f);


        uiResult.gameObject.SetActive(true);
        levelUpUI.SetActive(false);
        uiResult.Lose();
        IsBossing = false;
        Stop();


        AudioManager.instance.PlayBgm(false);
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Lose);
    }


    public void TogglePause() // 여기는 일시정지 버튼
    {
        if (IsBossing)
            return;

        IsPaused = !IsPaused;

        if (IsPaused)
        {
            pauseButton.GetComponent<Image>().sprite = pauseSprites[1];
            PauseGame();
        }
        else
        {
            StartCoroutine(ResumeGameWithDelay(3)); // 3초 후 재시작
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0; // 시간 정지
        pauseButton.GetComponent<Button>().interactable = false;
        // 일시정지 UI 표시
        pausePanel.SetActive(true);
    }


    private IEnumerator ResumeGameWithDelay(float delay)
    {
        pausePanel.SetActive(false);
        countdownText.gameObject.SetActive(true);

        // 3초 카운트다운 애니메이션
        for (int i = (int)delay; i > 0; i--)
        {
            yield return StartCoroutine(AnimateCountdown(i));
        }

        // 게임 재시작
        pauseButton.GetComponent<Image>().sprite = pauseSprites[0];
        pauseButton.GetComponent<Button>().interactable = true;
        countdownText.gameObject.SetActive(false);
        Time.timeScale = 1;
        IsPaused = false;
    }

    private IEnumerator AnimateCountdown(int number)
    {
        countdownText.text = number.ToString(); // 숫자 표시
        countdownText.color = new Color(1, 1, 1, 1); // 텍스트 색상 초기화 (흰색 불투명)
        countdownText.transform.localScale = Vector3.one; // 크기 초기화

        // 크기와 투명도를 변화
        float duration = 1f; // 애니메이션 지속 시간 (1초)
        for (float t = 0; t < duration; t += Time.unscaledDeltaTime)
        {
            float progress = t / duration;

            // 크기 점점 키우기 (1배 -> 1.5배)
            countdownText.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 1.5f, progress);

            // 텍스트 점점 투명하게 (알파 값 감소)
            countdownText.color = new Color(1, 1, 1, 1 - progress);

            yield return null;
        }

        // 마지막 상태 정리 (완전히 투명하게)
        countdownText.color = new Color(1, 1, 1, 0);
    }
public void GameRetry()
    {
        Utils.LoadScene("Game");
    }

    public void GameQuit()
    {
        Application.Quit();
    }

    void Update()
    {
        if (GameManager.instance.IsPaused)
            return;
        if (!isLive)
            return;

        gameTime += Time.deltaTime;

        if (gameTime > 480f) // 8분 초과 시
        {
            if (IsBossing)
                return;

            bossManager.GetComponent<BossManager>().bossUI();
        }
    }
    public void GetExp()
    {
        if (GameManager.instance.IsPaused)
            return;
        if (!isLive)
            return;
        if (IsBossing)
            return;

        exp++;
        if(exp == nextExp[Mathf.Min(level, nextExp.Length-1)]) // Min -> 최고 경험치바를 그대로 사용하기 위함
        {
            level++;
            exp = 0;

            uiLevelUp.Show();

        }
    }

    public void AddScore(int score)
    {
        if(!isLive)
            return;
        if (GameManager.instance.IsPaused)
            return;
        totalScore += score;
    }

    public void Heal()
    {
        if (!isLive)
            return;
        if (GameManager.instance.IsPaused)
            return;
        if (coffeecnt <= 0)
            return;

        health = maxHealth;

        coffeecnt--;
        
    }


    public void Stop() // 죽었을 때
    {
        isLive = false;
        Time.timeScale = 0; // timeScale : 유니티의 시간 속도(배율)
        uiJoy.localScale = Vector3.zero;
    }

    public void Resume()
    {
        isLive = true;
        Time.timeScale = 1;
        uiJoy.localScale = Vector3.one;
    }

    public void LoadLobbyScreen()
    {
        Utils.LoadScene("Lobby");
    }
}   
