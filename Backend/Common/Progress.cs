using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class Progress : MonoBehaviour
{
    [SerializeField]
    private Slider sliderProgress;
    [SerializeField]
    private TextMeshProUGUI textProgressData;
    [SerializeField]
    private float progressTime;  //로딩바 재생시간

    public void Play(UnityAction action = null)
    {
        StartCoroutine(OnProgress(action));
    }

    private IEnumerator OnProgress(UnityAction action)
    {
        float current = 0;
        float percent = 0;

        while (percent < 1)
        {
            current += Time.deltaTime;
            percent = current / progressTime;

            // Text 값 설정
            textProgressData.text = $"로딩 중입니다....{sliderProgress.value * 100:F0}%";
            // Slider 값 설정
            sliderProgress.value = Mathf.Lerp(0, 1, percent);

            yield return null;
        }

        // action이 null 이 아니면 action 메소드 실행
        action?.Invoke();
    }
}
