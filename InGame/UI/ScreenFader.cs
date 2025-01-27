using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenFader : MonoBehaviour
{
    public Image fadeImage; // Canvas 하위의 Image 연결
    public float fadeDuration = 1.5f; // Fade Out이 완료되는 시간

    public void StartFadeOut()
    {
        StartCoroutine(FadeOutRoutine());
    }

    private IEnumerator FadeOutRoutine()
    { 
        float elapsedTime = 0f;
        Color color = fadeImage.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Clamp01(elapsedTime / fadeDuration); // 알파값 증가
            fadeImage.color = color;
            yield return null;
        }

        // Fade Out 완료 후 추가 동작
        GameManager.instance.GameOver(); // 게임 오버 처리
    }
}