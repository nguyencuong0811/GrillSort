using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;

public class UITabController : MonoBehaviour
{
    [System.Serializable]
    public class Tab
    {
        public Button button;
        public RectTransform content;  
        public CanvasGroup panel;
        public Image imgTab;
    }

    public List<Tab> tabs;

    public float moveY = 60f;
    public float scaleSelected = 1.15f;
    public float scaleNormal = 1f;
    public float duration = 0.25f;

    private Tab currentTab;

    void Start()
    {
        foreach (var tab in tabs)
        {
            tab.button.onClick.AddListener(() => SelectTab(tab));
        }

        SelectTab(tabs[0]);
    }

    void SelectTab(Tab selected)
    {
        if (currentTab == selected) return;

        foreach (var tab in tabs)
        {
            bool isSelected = tab == selected;


            //panel
            tab.panel.DOFade(isSelected ? 1 : 0, duration);
            tab.panel.interactable = isSelected;
            tab.panel.blocksRaycasts = isSelected;

            // animate
            tab.imgTab.DOColor(isSelected ? new Color32(234, 185, 139, 255) : new Color32(234, 185, 139, 0), 0.2f);
            tab.content.DOAnchorPosY(isSelected ? moveY : 0, duration)
                .SetEase(Ease.OutBack);


            tab.content.DOScale(isSelected ? scaleSelected : scaleNormal, duration);
        }

        currentTab = selected;
    }
}