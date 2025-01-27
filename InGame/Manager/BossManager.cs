using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class BossManager : MonoBehaviour
{
    public Image warning;
    public GameObject bossArea;
    public GameObject boss;
    public Animator anim;

    public CinemachineVirtualCamera virtualCamera;
    private CinemachineBasicMultiChannelPerlin noise;

    private Coroutine currentShakeCoroutine;


    private void Start()
    {
        noise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        anim = boss.GetComponent<Animator>();
    }

    public void bossUI()
    {
        GameManager.instance.IsBossing = true;

        warning.gameObject.SetActive(true);
    }

    public void Onboss()
    {
        boss.gameObject.SetActive(true);
    }

    public void CameraShake()
    {
        gameObject.transform.localScale = Vector3.one;
        if (currentShakeCoroutine != null)
        {
            StopCoroutine(currentShakeCoroutine);
        }
        currentShakeCoroutine = StartCoroutine(CameraMix(3f, 3f));
    }

    public void Camerareset()
    {
        if (currentShakeCoroutine != null)
        {
            StopCoroutine(currentShakeCoroutine);
            currentShakeCoroutine = null;
        }

        StartCoroutine(Camerare());
        
    }

    public IEnumerator CameraMix(float amplitude, float frequency)
    {

        // 흔들림 시작
        noise.m_AmplitudeGain = amplitude;
        noise.m_FrequencyGain = frequency;

        // n초 동안 흔들림 유지
        yield return new WaitForSecondsRealtime(3f);

    }

    public IEnumerator Camerare()
    {
        //흔들림 종료
        noise.m_AmplitudeGain = 0f;
        noise.m_FrequencyGain = 0f;

        GameManager.instance.IsPaused = false;

        yield return new WaitForFixedUpdate();

        anim.SetTrigger("Run");
        gameObject.GetComponent<Boss>().Think();
        gameObject.GetComponent<Collider2D>().enabled = true;
    }

    public void EnemyCleaner()
    {
        StartCoroutine(BossArea());
        Onboss();
    }

    private IEnumerator BossArea()
    { 
        warning.gameObject.SetActive(false);
        GameManager.instance.player.anim.SetFloat("Speed", 0);
        GameManager.instance.IsPaused = true;
        bossArea.SetActive(true);
        yield return new WaitForFixedUpdate();
        bossArea.SetActive(false);

    }
}
