using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Unity.VisualScripting;

public class GrillStation : MonoBehaviour
{
    [SerializeField] private Sprite _lidGrill;
    [SerializeField] private Sprite _grill;
    [SerializeField] private Transform _trayContainer;
    [SerializeField] private Transform _slotContainer;

    private Stack<TrayItems> _stackTrays = new Stack<TrayItems>();
    public List<FoodSlots> TotalSlot => _totalSlots;

    private List<TrayItems> _totalTrays;
    private List<FoodSlots> _totalSlots;
    [SerializeField] private FoodTaget _imgFoodTarget;

    private List<bool> _trayHasFood = new List<bool>();
    private Image _imgGrill;
    
    void Awake()
    {
        _imgGrill = GetComponentInChildren<Image>();
        _totalTrays = Utils.GetListInChild<TrayItems>(_trayContainer);
        _totalSlots = Utils.GetListInChild<FoodSlots>(_slotContainer);
    }

    public void OnInitGrill(int totalTray, List<Sprite> listFood)
    {
        //xu ly gia tri cho bep 
    
        List<Sprite> list = listFood;
        int foodCount = Random.Range(1, _totalSlots.Count + 1);
        List<Sprite> listSlot = Utils.TakeAndRemoveRandom<Sprite>(list, foodCount);
        for(int i=0; i< listSlot.Count; i++)
        {
            FoodSlots slots = this.RandomSlot();
            slots.OnSetFood(listSlot[i]);
        }

        // xu ly dia
        List<List<Sprite>> remainFood = new List<List<Sprite>>();
        for(int i=0; i<totalTray-1; i++)
        {
            if(listFood.Count > 0)
            {
                remainFood.Add(new List<Sprite>());
                remainFood[i].Add(listFood[0]);
                listFood.RemoveAt(0);
            }
        }   
        while(listFood.Count > 0)
        {
            int rand = Random.Range(0, remainFood.Count);
            if(remainFood[rand].Count < 3)
            {
                remainFood[rand].Add(listFood[0]);
                listFood.RemoveAt(0);
            }
        }
        _trayHasFood.Clear();
        for (int i = 0; i < _totalTrays.Count; i++)
        {
            bool active = i < remainFood.Count && remainFood[i].Count > 0;
            _totalTrays[i].gameObject.SetActive(active);
            _trayHasFood.Add(active);
            if (active)
            {
                _totalTrays[i].OnSetFood(remainFood[i]);
                TrayItems item = _totalTrays[i];
                _stackTrays.Push(item);
            }
        }
        Debug.Log("So thuc an chua set: "+listFood.Count);
    }
    private FoodSlots RandomSlot()
    {
        reRand: int n = Random.Range(0, _totalSlots.Count);
        if(_totalSlots[n].HasFood)
        {
            goto reRand;
        }
        return _totalSlots[n];
    }
    public FoodSlots GetSlotNull()
    {
        for(int i = 0; i < _totalSlots.Count; i++)
        {
            if (!_totalSlots[i].HasFood)
            {
                return _totalSlots[i];
            }
        }
        return null;
    }
    private bool HasGrillEmpty()
    {
        for(int i = 0; i < _totalSlots.Count; i++)
        {
            if(_totalSlots[i].HasFood) 
                return false;
        }
        return true;
    }

    public void OnCheckMerge()
    {
        if(this.GetSlotNull() == null)
        {
            if (this.CanMerge())
            {
                Sprite mergedFood = _totalSlots[0].GetSpriteFood;
                Debug.Log("Complete Grill");
                StartCoroutine(IEMerge());

                GameManager.Instance?.OnMinusFood();

                //kiem tra mo lid
                GameManager.Instance.TryUnlockLidGrill(mergedFood);

                this.OnPerpareTray(false);
            }

            IEnumerator IEMerge()
            {
                for(int i = 0; i < _totalSlots.Count; i++)
                {
                    _totalSlots[i].OnFadeOut();
                    yield return new WaitForSeconds(0.1f);
                }
            }
        }
    }
    public void OnCheckPrepareTray()
    {
        if(this.HasGrillEmpty())
        {
            this.OnPerpareTray(true);
        }
    }
    private void OnPerpareTray(bool isNow)
    {
        StartCoroutine(IEPrepare());
        IEnumerator IEPrepare()
        {
            if(!isNow) yield return new WaitForSeconds(0.95f);
            if(_stackTrays.Count > 0)
            {
                TrayItems item = _stackTrays.Pop();
                for(int i = 0; i < item.FoodList.Count; i++)
                {
                    Image img = item.FoodList[i];
                    if (img.gameObject.activeInHierarchy)
                    {
                        // _totalSlots[i].OnPerPareItem(img);
                        // img.gameObject.SetActive(false);
                        FoodSlots slot = GetSlotNull();
                        if (slot == null) break;

                        slot.OnPerPareItem(img);
                        img.gameObject.SetActive(false);
                        yield return new WaitForSeconds(0.1f);
                    }

                }
                CanvasGroup canvas = item.GetComponent<CanvasGroup>();
                canvas.DOFade(0f, 0.5f).OnComplete(() =>
                {
                    item.gameObject.SetActive(false);
                    canvas.alpha = 1f;
                });
            }
        }
    }
    public bool CanMerge()
    {
        string name = _totalSlots[0].GetSpriteFood.name;
        for (int i = 0;i < _totalSlots.Count; i++)
        {
            if(_totalSlots[i].GetSpriteFood.name != name)
                return false;
        }
        return true;
    }
    private bool _isHiddenByLid = false;
    public void HideGrillWithLid(string foodName) 
    {
        _isHiddenByLid = true;
        
        // Ẩn slots và trays
        foreach(var slot in _totalSlots)
            slot.gameObject.SetActive(false);
        
        foreach(var tray in _totalTrays)
            tray.gameObject.SetActive(false);
        
        Sprite targetSprite = GameManager.Instance.GetSpriteFoodName(foodName);


        _imgFoodTarget.SetActive(true);
        _imgFoodTarget.OnSetFood(targetSprite);
        _imgGrill.sprite = _lidGrill;
    }
    public void ShowGrill()
    {
        _isHiddenByLid = false;
        
        _imgFoodTarget.DoShakeAndFadeOut();

        _imgGrill.DOFade(0.8f, 1.3f).OnComplete(() =>
        {
            _imgGrill.sprite = _grill;
            _imgGrill.DOFade(1f, 0.2f);
        });
        
        foreach(var slot in _totalSlots)
            slot.gameObject.SetActive(true);

        for(int i = 0; i < _totalTrays.Count; i++)
            _totalTrays[i].gameObject.SetActive(_trayHasFood[i]);
    }

    public bool HasLidWithFood(Sprite completedFood)
    {
        if (!_isHiddenByLid) return false;
        return _imgFoodTarget.GetSpriteFood.name == completedFood.name;
    }
    public List<Sprite> GetAllFoodInTray()
    {
        List<Sprite> food = new List<Sprite>();
        foreach(var tray in _totalTrays)
        {
            if(!tray.gameObject.activeInHierarchy) 
                continue;
            foreach(var foodImg in tray.FoodList)
            {
                if(foodImg.gameObject.activeInHierarchy && foodImg.sprite != null)
                    food.Add(foodImg.sprite);
            }
        }

        return food;
    }
}
