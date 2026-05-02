using UnityEngine;
using System.Collections.Generic;

public class PopupManager : MonoBehaviour
{
    public static PopupManager Instance;

    private bool _hasPopupOpen = false;

    [SerializeField] private PopupBase _popupLoss;
    [SerializeField] private PopupBase _popupWin;
    [SerializeField] private PopupBase _popupPause;

    private Stack<PopupBase> popupStack = new Stack<PopupBase>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void ShowPopup(PopupBase popup)
    {
        _hasPopupOpen = true;
        if (popupStack.Count > 0)
        {
            popupStack.Peek().gameObject.SetActive(false);
        }

        popupStack.Push(popup);
        popup.Show();
        
    }

    public void HidePopup()
    {
        if (popupStack.Count == 0) return;

        PopupBase top = popupStack.Pop();

        top.Hide(() =>
        {
            if (popupStack.Count > 0)
            {
                popupStack.Peek().gameObject.SetActive(true);
            }
            else
            {
                _hasPopupOpen = false;
                Time.timeScale = 1f;
            }
        });
    }

    public void HideAll()
    {
        while (popupStack.Count > 0)
        {
            popupStack.Pop().gameObject.SetActive(false);
        }

        _hasPopupOpen = false;
        Time.timeScale = 1f;
    }

    public void ShowPause() => ShowPopup(_popupPause);
    public void ShowLoss() => ShowPopup(_popupLoss);
    public void ShowWin() => ShowPopup(_popupWin);

    public bool HasPopupOpen => _hasPopupOpen;

}