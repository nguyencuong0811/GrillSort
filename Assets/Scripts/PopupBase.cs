using UnityEngine;
using DG.Tweening;

public class PopupBase : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public Transform content;

    public virtual void Show()
    {
        gameObject.SetActive(true);

        canvasGroup.alpha = 0;
        content.localScale = Vector3.zero;

        canvasGroup.DOFade(1, 0.25f);
        content.DOScale(1f, 0.35f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            Time.timeScale = 0;
        });
    }

    public virtual void Hide(System.Action onComplete = null)
    {
        canvasGroup.DOFade(0, 0.2f).SetUpdate(true);
        content.DOScale(0f, 0.2f)
            .SetEase(Ease.InBack)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                gameObject.SetActive(false);
                onComplete?.Invoke();
            });
    }
}