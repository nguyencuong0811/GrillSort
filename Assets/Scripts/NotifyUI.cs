using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class NotifyUI : MonoBehaviour
{
    public static NotifyUI Instance;
    [SerializeField] private TextMeshProUGUI notifytxt;
    [SerializeField] CanvasGroup canvas;

    void Start()
    {
        canvas.alpha = 0;
        BoosterSystem.OnNotify += ShowNotify;
    }
    private void Awake()
    {
        Instance = this;
    }
    public void ShowNotify(string message)
    {
        notifytxt.rectTransform.DOKill();
        canvas.DOKill();

        notifytxt.text = message;

        canvas.alpha = 0;

        canvas.DOFade(1, 0.2f);

        notifytxt.rectTransform
            .DOShakePosition(
                0.35f,
                new Vector2(25f, 0f),
                18,
                90,
                false,
                true)
            .OnComplete(() =>
            {
                canvas.DOFade(0, 0.2f)
                    .SetDelay(0.5f);
            });
    }
}
