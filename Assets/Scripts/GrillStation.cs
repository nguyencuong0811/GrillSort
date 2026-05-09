using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Unity.VisualScripting;
using System.Linq;

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
    public bool LidGrill => _imgGrill.sprite == _grill ? false : true;
    
    void Awake()
    {
        _imgGrill = GetComponentInChildren<Image>();
        _totalTrays = Utils.GetListInChild<TrayItems>(_trayContainer);
        _totalSlots = Utils.GetListInChild<FoodSlots>(_slotContainer);
    }

    public void OnInitGrill(int totalTray, List<Sprite> listFood)
    {
        _stackTrays.Clear();

        // lấy food lên bếp remove thẳng từ listFood gốc
        int foodCount = Random.Range(1, _totalSlots.Count + 1);
        // Giới hạn không lấy quá số food còn lại
        foodCount = Mathf.Min(foodCount, listFood.Count);

        List<Sprite> listSlot = Utils.TakeAndRemoveRandom<Sprite>(listFood, foodCount);
        for (int i = 0; i < listSlot.Count; i++)
        {
            FoodSlots slot = this.RandomSlot();
            if (slot == null)
            {
                Debug.LogWarning($"[GrillStation] OnInitGrill: hết slot trống tại food thứ {i}.");
                break;
            }
            slot.OnSetFood(listSlot[i]);
        }

        //Thoát sớm nếu không cần tray
        if (totalTray <= 1 || listFood.Count == 0)
        {
            _trayHasFood.Clear();
            foreach (var tray in _totalTrays)
            {
                tray.gameObject.SetActive(false);
                _trayHasFood.Add(false);
            }
            return;
        }

        // khởi tạo remainFood — mỗi tray được ít nhất 1 food
        List<List<Sprite>> remainFood = new List<List<Sprite>>();
        for (int i = 0; i < totalTray - 1; i++)
        {
            if (listFood.Count > 0)
            {
                remainFood.Add(new List<Sprite> { listFood[0] });
                listFood.RemoveAt(0);
            }
        }

        // thoát sớm nếu remainFood rỗng hoàn toàn
        if (remainFood.Count == 0)
        {
            Debug.LogWarning("[GrillStation] OnInitGrill: remainFood rỗng sau khi khởi tạo — kiểm tra lại config level (totalTray, allFood).");
            _trayHasFood.Clear();
            foreach (var tray in _totalTrays)
            {
                tray.gameObject.SetActive(false);
                _trayHasFood.Add(false);
            }
            return;
        }

        // thay while bằng loop có cờ didAssign — không bao giờ freeze
        while (listFood.Count > 0)
        {
            bool didAssign = false;

            for (int i = 0; i < remainFood.Count; i++)
            {
                if (listFood.Count == 0) break;
                if (remainFood[i].Count < 3)
                {
                    remainFood[i].Add(listFood[0]);
                    listFood.RemoveAt(0);
                    didAssign = true;
                }
            }

            if (!didAssign)
            {
                Debug.LogWarning($"[GrillStation] OnInitGrill: còn {listFood.Count} food nhưng tất cả tray đã đầy (max 3). Bỏ food thừa.");
                break;
            }
        }

        // Gán tray vào bếp
        _trayHasFood.Clear();
        for (int i = 0; i < _totalTrays.Count; i++)
        {
            bool active = i < remainFood.Count && remainFood[i].Count > 0;
            _totalTrays[i].gameObject.SetActive(active);
            _trayHasFood.Add(active);
            if (active)
            {
                _totalTrays[i].OnSetFood(remainFood[i]);
                _stackTrays.Push(_totalTrays[i]);
            }
        }
    }
    private FoodSlots RandomSlot()
    {
        List<FoodSlots> available = _totalSlots.FindAll(s => !s.HasFood);
        if (available.Count == 0) return null;
        return available[Random.Range(0, available.Count)];
    }
    // private FoodSlots RandomSlot()
    // {
    //     reRand: int n = Random.Range(0, _totalSlots.Count);
    //     if(_totalSlots[n].HasFood)
    //     {
    //         goto reRand;
    //     }
    //     return _totalSlots[n];
    // }
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

            if (CanMerge())
            {
                Sprite mergedFood = _totalSlots[0].GetSpriteFood;
                Debug.Log("Complete Grill");
                StartCoroutine(IEMerge());

                GameManager.Instance?.OnMinusFood();

                //kiem tra mo lid
                GameManager.Instance?.TryUnlockLidGrill(mergedFood);

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
        RebuildStackTrays();
        if(this.HasGrillEmpty())
        {
            this.OnPerpareTray(true);
        }
        CheckTray();
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

                        slot.OnPrepareItem(img);

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
            if(_totalSlots[i].GetSpriteFood.name != name || !_totalSlots[i].gameObject.activeInHierarchy)
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
    public void HideAllFood()
    {
        _stackTrays.Clear();
        foreach(var slot in _totalSlots)
            slot.OnHideFood();
        foreach(var tray in _totalTrays)
            tray.HideFoodInTray();

        _imgGrill.sprite = _grill;
        _imgFoodTarget.SetActive(false);
        foreach(var slot in _totalSlots)
            slot.gameObject.SetActive(true);
        
        foreach(var tray in _totalTrays)
            tray.gameObject.SetActive(true);

        
    }
    public bool CheckMerge()
    {
        if(CanMerge() == true)
            return true;

        foreach (var tray in _totalTrays)
        {
            if (!tray.gameObject.activeInHierarchy) 
                continue;

            // Chỉ lấy food thật sự đang hiện trên khay
            List<Image> activeFoods = tray.FoodList
                .Where(f => f.gameObject.activeInHierarchy && f.sprite != null)
                .ToList();

            // Rule gameplay: phải đúng 3 món
            if (activeFoods.Count != 3)
                continue;

            string name = activeFoods[0].sprite.name;
            if (activeFoods.All(f => f.sprite.name == name))
                return true;
        }

        return false;
    }
    public void CheckTray()
    {
        foreach(var tray in _totalTrays)
        {
            if (tray.gameObject.activeInHierarchy && tray.CheckTray() == false)
            {
                tray.gameObject.SetActive(false);
                CanvasGroup group = tray.GetComponent<CanvasGroup>();
                group.DOFade(0, 0.5f).OnComplete(() =>
                {
                    
                    group.alpha = 1f;
                });
            }
        }
    }

    public TrayItems GetFirstTray()
    {
        for(int i = _totalTrays.Count - 1; i >= 0; i--)
        {
            if (_totalTrays[i].gameObject.activeInHierarchy)
                return _totalTrays[i];
        }
        return null;
    }
    public void RebuildStackTrays()
    {
        _stackTrays.Clear();
    
        for (int i = 0; i < _totalTrays.Count; i++)
        {
            if (_totalTrays[i].gameObject.activeInHierarchy && _totalTrays[i].CheckTray())
            {
                _stackTrays.Push(_totalTrays[i]);
            }
        }
    }
    public void OnCheckTrayOnly()
    {
        RebuildStackTrays();
        CheckTray();
    }
    public void HideTray()
    {
        foreach(var tray in _totalTrays)
        {
            tray.gameObject.SetActive(false);
        }
    }

    public bool CheckFullSlot()
    {
        foreach(var slot in _totalSlots)
        {
            if(!slot.HasFood)
                return false;
        }
        return true;
    }
}
