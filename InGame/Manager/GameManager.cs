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

        uiLevelUp.Select(playerId % 2); // ���� ������������ ����Ͽ� �⺻���� ������ ���� % 2 �س���, �ϴ� ������ ��(0) ��(1) �ø� %3 �ϸ� �ǰٳ�?
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
        isLive = false; // ���� �ߴ� ���� ����
        yield return new WaitForSeconds(3f); // ������ ���� ��� �ð�
        player.anim.SetTrigger("Win"); // �÷��̾� �¸� �ִϸ��̼� ����
        yield return new WaitForSeconds(1.5f);
        uiResult.gameObject.SetActive(true); // ���â Ȱ��ȭ
        uiResult.Win(); // �¸� ó�� ȣ��
        levelUpUI.SetActive(false);
        IsBossing = false;
        Stop(); // ���� ����

        AudioManager.instance.PlayBgm(false); // ��� ���� �ߴ�
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Win); // �¸� ȿ���� ���
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


    public void TogglePause() // ����� �Ͻ����� ��ư
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
            StartCoroutine(ResumeGameWithDelay(3)); // 3�� �� �����
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0; // �ð� ����
        pauseButton.GetComponent<Button>().interactable = false;
        // �Ͻ����� UI ǥ��
        pausePanel.SetActive(true);
    }


    private IEnumerator ResumeGameWithDelay(float delay)
    {
        pausePanel.SetActive(false);
        countdownText.gameObject.SetActive(true);

        // 3�� ī��Ʈ�ٿ� �ִϸ��̼�
        for (int i = (int)delay; i > 0; i--)
        {
            yield return StartCoroutine(AnimateCountdown(i));
        }

        // ���� �����
        pauseButton.GetComponent<Image>().sprite = pauseSprites[0];
        pauseButton.GetComponent<Button>().interactable = true;
        countdownText.gameObject.SetActive(false);
        Time.timeScale = 1;
        IsPaused = false;
    }

    private IEnumerator AnimateCountdown(int number)
    {
        countdownText.text = number.ToString(); // ���� ǥ��
        countdownText.color = new Color(1, 1, 1, 1); // �ؽ�Ʈ ���� �ʱ�ȭ (��� ������)
        countdownText.transform.localScale = Vector3.one; // ũ�� �ʱ�ȭ

        // ũ��� ������ ��ȭ
        float duration = 1f; // �ִϸ��̼� ���� �ð� (1��)
        for (float t = 0; t < duration; t += Time.unscaledDeltaTime)
        {
            float progress = t / duration;

            // ũ�� ���� Ű��� (1�� -> 1.5��)
            countdownText.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 1.5f, progress);

            // �ؽ�Ʈ ���� �����ϰ� (���� �� ����)
            countdownText.color = new Color(1, 1, 1, 1 - progress);

            yield return null;
        }

        // ������ ���� ���� (������ �����ϰ�)
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

        if (gameTime > 480f) // 8�� �ʰ� ��
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
        if(exp == nextExp[Mathf.Min(level, nextExp.Length-1)]) // Min -> �ְ� ����ġ�ٸ� �״�� ����ϱ� ����
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


    public void Stop() // �׾��� ��
    {
        isLive = false;
        Time.timeScale = 0; // timeScale : ����Ƽ�� �ð� �ӵ�(����)
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
