using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenFader : MonoBehaviour
{
    public Image fadeImage; // Canvas ������ Image ����
    public float fadeDuration = 1.5f; // Fade Out�� �Ϸ�Ǵ� �ð�

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
            color.a = Mathf.Clamp01(elapsedTime / fadeDuration); // ���İ� ����
            fadeImage.color = color;
            yield return null;
        }

        // Fade Out �Ϸ� �� �߰� ����
        GameManager.instance.GameOver(); // ���� ���� ó��
    }
}