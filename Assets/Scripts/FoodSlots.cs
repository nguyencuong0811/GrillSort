using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FoodSlots : MonoBehaviour
{
    private Image _imgFood;

    private Color _normalColor = new Color(1f,1f, 1f,1f);
    private Color _fadeColor = new Color(1f,1f,1f, 0.7f);

    private GrillStation _grillCtrl;

    private bool _hasSetSlot = false;

    void Awake()
    {
        _grillCtrl = this.transform.parent.parent.GetComponent<GrillStation>();
        _imgFood = this.transform.GetChild(0).GetComponent<Image>();
        _imgFood.gameObject.SetActive(false);

    }
    public void OnSetFood(Sprite sprite)
    {
        _hasSetSlot = true;
        _imgFood.gameObject.SetActive(true);
        _imgFood.sprite = sprite;
        _imgFood.SetNativeSize();
    }
    public void OnActiveFood(bool active)
    {
        _imgFood.gameObject.SetActive(active);
        _imgFood.color = _normalColor;
    }
    public void OnFadeFood()
    {
        this.OnActiveFood(true);
        _imgFood.color = _fadeColor;
    }
    public void OnHideFood()
    {
        this.OnActiveFood(false);
        _imgFood.color = _normalColor;
    }
    public void OnPerPareItem(Image image)
    {
        this.OnSetFood(image.sprite);
        _imgFood.color = _normalColor;
        _imgFood.transform.position = image.transform.position;
        _imgFood.transform.localScale = image.transform.localScale;
        _imgFood.transform.localEulerAngles = image.transform.localEulerAngles;

        _imgFood.transform.DOLocalMove(Vector3.zero, 0.5f);
        _imgFood.transform.DOScale(Vector3.one, 0.5f);
        _imgFood.transform.DORotate(Vector3.zero, 0.5f);
    }
    public void OnFadeOut()
    {
        _imgFood.transform.DOLocalMoveY(100f, 0.6f).OnComplete(() =>
        {
            this.OnActiveFood(false);
            _imgFood.transform.localPosition = Vector3.zero;
        });
        _imgFood.DOColor(new Color(1f,1f,1f,0f), 0.6f);
    }
    public void OnCheckPerpareTray()
    {
        _grillCtrl?.OnCheckPrepareTray();
    }
    public bool HasFood => _imgFood.gameObject.activeInHierarchy && _imgFood.color == _normalColor;

    public Sprite GetSpriteFood => _imgFood.sprite;
    public FoodSlots GetSlotNull => _grillCtrl.GetSlotNull();

    public bool HasSetSlot => _hasSetSlot;

    public void OnCheckMerge()
    {
        _grillCtrl?.OnCheckMerge();
    }
    public void DoShake()
    {
        _imgFood.transform.DOShakePosition(0.5f, 10f, 10, 90f);
    }
}
