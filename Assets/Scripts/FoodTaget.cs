using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Unity.VisualScripting;

public class FoodTaget : MonoBehaviour
{
    private Image _imgFoodTarget, _imgNote;
    void Awake()
    {
        _imgNote = this.GetComponent<Image>();
        _imgFoodTarget = this.transform.GetChild(0).GetComponent<Image>();
        this.enabled = false;
    }

    public void OnSetFood(Sprite sprite)
    {
        _imgFoodTarget.sprite = sprite;
        _imgFoodTarget.SetNativeSize();
    }
    public void SetActive(bool active) 
    {
        this.gameObject.SetActive(active);
    }
    public Sprite GetSpriteFood => _imgFoodTarget.sprite;

    public void DoShakeAndFadeOut()
    {
        this.transform.DOShakePosition(0.5f, 10f, 10, 90f);
        _imgNote.DOFade(0f, 1.5f).OnComplete(() =>
        {
            this.SetActive(false);
        });
        _imgFoodTarget.DOFade(0f, 1.5f);
    }
}
