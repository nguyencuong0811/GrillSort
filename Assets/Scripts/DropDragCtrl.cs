using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DropDragCtrl : MonoBehaviour
{
    [SerializeField] private Image _imgFoodDrag;
    [SerializeField] private float _timeCheckSuggest;

    private FoodSlots _currentFood, _cacheFood;
    private bool _hasDrag;

    private Vector3 _offset;
    private float _countTime;
    private bool _canClick =true;

    void Update()
    {
        _countTime += Time.deltaTime;
        if(_countTime >= _timeCheckSuggest)
        {
            _countTime = 0;
            GameManager.Instance?.OnCheckAndShake();
        }
        if (Input.GetMouseButtonDown(0) && _canClick) 
        {
            _currentFood = Utils.GetRayCastUI<FoodSlots>(Input.mousePosition); // check xem co nhan vao slot k
            if(_currentFood != null && _currentFood.HasFood)
            {
                _hasDrag = true;
                _cacheFood = _currentFood;
                //gan sprite cho dummyfood
                _imgFoodDrag.gameObject.SetActive(true);
                _imgFoodDrag.sprite = _currentFood.GetSpriteFood;
                _imgFoodDrag.SetNativeSize();
                _imgFoodDrag.transform.position = _currentFood.transform.position;

                //tinh offset
                Vector3 mouseWordPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                _offset = mouseWordPos - _currentFood.transform.position;

                _imgFoodDrag.transform.DOScale(Vector3.one * 1.3f, 0.2f);

                _currentFood.OnActiveFood(false);
            }
        }
        if (_hasDrag)
        {
            _countTime =0;
            Vector3 mouseWordPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 foodPos = mouseWordPos + _offset;
            foodPos.z = 0f;
            _imgFoodDrag.transform.position = foodPos;

            FoodSlots slot = Utils.GetRayCastUI<FoodSlots>(Input.mousePosition);
            if(slot != null)
            {
                if (!slot.HasFood)// vi tri item chua co food
                {
                    if(_cacheFood == null || _cacheFood.GetInstanceID() != slot.GetInstanceID())
                    {
                        _cacheFood?.OnHideFood();
                        _cacheFood = slot;
                        _cacheFood.OnFadeFood();
                        _cacheFood.OnSetFood(_currentFood.GetSpriteFood);
                    }
                }
                else //vi tri chuot da co item
                {
                    FoodSlots slotAvalable = slot.GetSlotNull;

                    if(slotAvalable != null)
                    {
                        _cacheFood?.OnHideFood();
                        // _cacheFood = slotAvalable;
                        // _cacheFood.OnFadeFood();
                        // _cacheFood.OnSetFood(_currentFood.GetSpriteFood);
                        _cacheFood = null;
                    }
                    else
                    {
                        this.OnClearCacheSlot();
                    }

                }
            }
            else
            {
                if(_cacheFood != null)
                {
                    _cacheFood.OnHideFood();
                    _cacheFood = null;
                }
            }

        }
        if(Input.GetMouseButtonUp(0) && _hasDrag)
        {
            if(_cacheFood != null)
            {
                _imgFoodDrag.transform.DOMove(_cacheFood.transform.position, 0.2f).OnComplete(() =>
                {
                    _imgFoodDrag.gameObject.SetActive(false);
                    _cacheFood.OnSetFood(_currentFood.GetSpriteFood);
                    _cacheFood.OnActiveFood(true);
                    _cacheFood.OnCheckMerge();
                    _currentFood.OnCheckPerpareTray();
                    _cacheFood = null;
                    _currentFood = null;
                    
                });
            }
            else
            {
                _imgFoodDrag.transform.DOMove(_currentFood.transform.position, 0.2f).OnComplete(() =>
                {
                    _imgFoodDrag.gameObject.SetActive(false);
                    _currentFood.OnActiveFood(true);
                });
            }
            _hasDrag = false;
            StartCoroutine(TimeForClick());
        }
    }
    private void OnClearCacheSlot()
    {
        if(_cacheFood != null && _cacheFood.GetInstanceID() != _currentFood.GetInstanceID())
        {
            _cacheFood.OnHideFood();
            _cacheFood = null;
        }
    }
    IEnumerator TimeForClick()
    {
        _canClick = false;
        yield return new WaitForSeconds(0.21f);
        _canClick = true;
    }

}
