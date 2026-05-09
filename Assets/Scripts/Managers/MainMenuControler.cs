using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MainMenuControler : MonoBehaviour
{
    [SerializeField] private CanvasGroup menuCanvasGroup;
    [SerializeField] private CanvasGroup gameCanvasGroup;
    [SerializeField] private float duration = 0.8f;

    public void ShowGame()
    {
        PopupManager.Instance?.HideAll();
        GameManager.Instance?.ResumeGameplay();

        gameCanvasGroup.DOKill();
        menuCanvasGroup.DOKill();

        gameCanvasGroup.alpha = 0f;
        gameCanvasGroup.interactable = false;
        gameCanvasGroup.blocksRaycasts = false;

        GameManager.Instance.LoadLevel();

        gameCanvasGroup.DOFade(1, duration).SetUpdate(true).OnComplete(() =>
        {
            gameCanvasGroup.interactable = true;
            gameCanvasGroup.blocksRaycasts = true;
        });

        menuCanvasGroup.DOFade(0, duration).SetUpdate(true);
        menuCanvasGroup.interactable = false;
        menuCanvasGroup.blocksRaycasts = false;
    }
    public void ShowMenu()
    {
        PopupManager.Instance?.HideAll();

        gameCanvasGroup.DOKill();
        menuCanvasGroup.DOKill();

        menuCanvasGroup.DOFade(1, duration).SetUpdate(true);
        menuCanvasGroup.interactable = true;
        menuCanvasGroup.blocksRaycasts = true;

        gameCanvasGroup.DOFade(0, duration).SetUpdate(true);
        gameCanvasGroup.interactable = false;
        gameCanvasGroup.blocksRaycasts = false;

        GameManager.Instance?.PauseGameplay();
    }
}
