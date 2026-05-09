using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private LevelDatabase levelDatabase;
    private LevelData _currentLevel;
    [SerializeField]private TextMeshProUGUI _leveltxt;

    [SerializeField] private int _allFood; // tong so thuc an
    [SerializeField] private int _totalFoods; //tong so loai thuc an
    [SerializeField] private int _totalGrill; //tong so bep
    [SerializeField] private Transform _gridGrill; //container chua cac bep
    [SerializeField] private int _totalLidGrill; // tong so bep bi an

    [SerializeField] private GameTimer _gameTimer;

    private List<GrillStation> _listGrills; //danh sach cac bep
    private float _avgTray; //gia tri trung binh thuc an tren 1 dia

    private List<Sprite> _totalSpriteFood; //danh sach tat ca sprite thuc an 
    private List<int> _randLidGrill = new List<int>();

    private int testInitCount = 0;

    [SerializeField] private List<Image> _magnetLists = new();
    [SerializeField] private Transform _magnetFx;

    [Header("Audio")]
    [SerializeField] private AudioClip _bgAudioInGame;
    [SerializeField] private AudioClip _bgAudioInMenu;
    [SerializeField] private AudioClip _sfxCollect;
    [SerializeField] private AudioClip _OpenGrill;
    
    void Awake()
    {
        _gameTimer = GetComponent<GameTimer>();
        _listGrills = Utils.GetListInChild<GrillStation>(_gridGrill);
        Sprite[] sprites = Resources.LoadAll<Sprite>("itemstest");
        _totalSpriteFood = sprites.ToList();
        if(Instance == null)
        {
            Instance = this;

        }
        else
        {
            Destroy(gameObject);
        }
        
    }
    void Start()
    {
        //LoadLevel();
    }
    
    public void LoadLevel()
    {
        AudioManager.Instance.PlayMusic(_bgAudioInGame);
        PopupManager.Instance.HideAll();
        int levelIndex = SaveManager.GetCurrentLevel();
        if(levelIndex >= levelDatabase.Levels.Count)
            levelIndex = 0;
        
        _currentLevel = levelDatabase.Levels[levelIndex];
        _leveltxt.text = $"Level: {_currentLevel.Level+1}";

        InitLevel(_currentLevel);

        _gameTimer.StartTimer(_currentLevel.TimeLimit);
    }

    public void PauseGameplay()
    {
        _gameTimer?.PauseTimer();
    }

    public void ResumeGameplay()
    {
        _gameTimer?.ResumeTimer();
    }
    private void InitLevel(LevelData levelData)
    {

        _allFood = levelData.AllFood;
        _totalFoods = levelData.TotalFood;
        _totalGrill = levelData.TotalGrill;
        _totalLidGrill = levelData.TotalLidGrill;

        _avgTray = (Random.Range(levelData.MinAvgTray, levelData.MaxAvgTray));

        Debug.Log("Level: "+ levelData.Level);
        // random bep bi an
        this.RandGrillHide();

        CheckAndInit();
        //OnInitLevel();
        this.SetActiveLidGrill();

        foreach(var grill in _listGrills)
        {
            grill.transform.localScale = Vector3.zero;
            grill.transform.DOScale(1, 0.5f).SetEase(Ease.OutBack);
        }
    }
    private void CheckAndInit()
    {
        bool needReset;
        do
        {
            needReset = false;
            testInitCount++;
            Debug.Log("Tao lai: "+ testInitCount +" lan");
            
            RemoveAllFood();
            OnInitLevel();
            int grillFull = 0;
            foreach(var grill in _listGrills)
            {
                if(grill.CheckMerge() == true)
                {
                    needReset = true;
                    break;
                }
                if(grill.CheckFullSlot()) grillFull++;
                
            }
            if(grillFull == _totalGrill) needReset = true;
        }while(needReset && testInitCount < 100);

        testInitCount = 0;
    }
    private void RandGrillHide()
    {
        _randLidGrill.Clear();
        List<int> pool = new List<int>();

        for(int i = 0; i < _totalGrill; i++)
        {
            pool.Add(i);
        }

        for(int i = 0; i < pool.Count; i++)
        {
            int rand = Random.Range(i, pool.Count);
            (pool[i], pool[rand]) = (pool[rand], pool[i]);
        }
        for(int i = 0; i < _totalLidGrill; i++)
        {
            _randLidGrill.Add(pool[i]);
        }
    }
    private void OnInitLevel()
    {
        List<Sprite> takeFood = _totalSpriteFood.OrderBy(x => Random.value).Take(_totalFoods).ToList();
        List<Sprite> useFood = new List<Sprite>();
        for( int i=0; i< _allFood; i++)
        {
            int n = i % takeFood.Count;
            for(int j = 0; j < 3; j++)
            {
                useFood.Add(takeFood[n]);
            }
        }
        //random, trao vi tri cua cac item
        for(int i =0; i<useFood.Count; i++)
        {
            int rand = Random.Range(i, useFood.Count);
            (useFood[i], useFood[rand]) = (useFood[rand], useFood[i]); // ham spawp
        }

        int totalTray = 0;

        totalTray = Mathf.CeilToInt((float)useFood.Count / _avgTray); // tinh tong so luong dia can


        List<int> trayPerGrill = this.DistributeEveLyn(_totalGrill, totalTray);
        List<int> foodPerGrill = this.DistributeEveLyn(_totalGrill, useFood.Count);


        for(int i=0; i< _listGrills.Count; i++)
        {
            bool activeGrill = i< _totalGrill;
            _listGrills[i].gameObject.SetActive(activeGrill);
            if(activeGrill)
            {
                List<Sprite> listFood = Utils.TakeAndRemoveRandom<Sprite>(useFood, foodPerGrill[i]);
                _listGrills[i].OnInitGrill(trayPerGrill[i], listFood);
            }
             
        }

    }

    private List<int> DistributeEveLyn(int grillCount, int totalTrays)
    {
        List<int> result = new List<int>();
        //tinh trung binh so luong dia
        float avg = (float)totalTrays / grillCount;
        int low = Mathf.FloorToInt(avg); // lam tron nho
        int high = Mathf.CeilToInt(avg); // lam tron lon

        int highCount = totalTrays - low * grillCount; // tinh so bep nhieu khay
        int lowCount = grillCount - highCount; // tinh so bep it khay


        for(int i=0; i< lowCount; i++)
        {
            result.Add(low);
        }         
        for(int i=0; i< highCount; i++)
        {
            result.Add(high);
        }

        // dao vi tri 
        for(int i=0; i< result.Count; i++)
        {
            int rand = Random.Range(i, result.Count);
            (result[i], result[rand]) = (result[rand], result[i]);
        }

        return result;
    }
    public void OnMinusFood()
    {
        AudioManager.Instance.PlaySFX(_sfxCollect);
        --_allFood;
        if(_allFood <= 0)
        {   
            int currentLevel = SaveManager.GetCurrentLevel();
            
            int nextLevel = (currentLevel + 1) % levelDatabase.Levels.Count;

            SaveManager.SetCurrentLevel(nextLevel);
            Debug.Log(_currentLevel+"______Mission Completee");
            StartCoroutine(WaitComplete());

            IEnumerator WaitComplete()
            {
                yield return new WaitForSeconds(1f);
                //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                LoadLevel();
            }
            Debug.Log("Lv hien tai "+ nextLevel);
        }
    }
    public void OnCheckAndShake()
    {
        Dictionary<string, List<FoodSlots>> groups = new Dictionary<string, List<FoodSlots>>();

        foreach(var grill in _listGrills)
        {
            if (grill.gameObject.activeInHierarchy)
            {
                for(int i = 0; i < grill.TotalSlot.Count; i++)
                {
                    FoodSlots slot = grill.TotalSlot[i];
                    if (slot.HasFood)
                    {
                        string name = slot.GetSpriteFood.name;
                        if (!groups.ContainsKey(name))
                        {
                            groups.Add(name, new List<FoodSlots>());
                        }
                        groups[name].Add(slot);
                    }
                }
            }
        }

        foreach(var kvp in groups)
        {
            if(kvp.Value.Count >= 3)
            {
               for(int i = 0; i < 3; i++)
                {
                    kvp.Value[i].DoShake();
                } 
                return;
            }
            
        }
    }
    public float AvgTray => _avgTray;

    public void SetActiveLidGrill()
    {
        List<string> foodTarget = this.GetFoodWithMinCount(3);
        for(int i = 0; i < _totalLidGrill; i++)
        {
            _listGrills[_randLidGrill[i]].HideGrillWithLid(foodTarget[0]);
            foodTarget.RemoveAt(0);
        }
    }
    
    public void TryUnlockLidGrill(Sprite completedFood)
    {
        for(int i = 0; i < _randLidGrill.Count; i++)
        {
            GrillStation grill = _listGrills[_randLidGrill[i]];
            if (grill.HasLidWithFood(completedFood))
            {
                grill.ShowGrill();
                AudioManager.Instance.PlaySFX(_OpenGrill);
                _randLidGrill.RemoveAt(i);
                break;
            }
        }
    }
    public Sprite GetSpriteFoodName(string nameFood)
    {
        for(int i =0; i < _totalSpriteFood.Count; i++)
        {
            if(_totalSpriteFood[i].name == nameFood)
            {
                return _totalSpriteFood[i];
            }
        }
        return null;
    }
    
    public List<string> GetFoodWithMinCount(int minCount = 3)
    {
        Dictionary<string, int> foodCount = new Dictionary<string, int>();

        foreach(var grill in _listGrills)
        {
            if(!grill.gameObject.activeInHierarchy ) 
                continue;
            // duyet tren bep
            foreach(var slot in grill.TotalSlot)
            {
                if (slot.HasFood)
                {
                    string foodName = slot.GetSpriteFood.name;
                    if (!foodCount.ContainsKey(foodName))
                        foodCount[foodName] = 0;
                    foodCount[foodName]++;
                }
            }
            //duyet tren tray
            foreach(var food in grill.GetAllFoodInTray())
            {
                string foodName = food.name;
                if(!foodCount.ContainsKey(foodName))
                    foodCount[foodName] = 0;
                foodCount[foodName]++;
            }
        }
        List<string> listFoodCanSetTarget = new List<string>();

        foreach(var kvp in foodCount)
        {
            if(kvp.Value >= minCount)
                listFoodCanSetTarget.Add(kvp.Key);

        }
        return listFoodCanSetTarget;
    }

    public void RemoveAllFood()
    {
        foreach(var grill in _listGrills)
        {
            grill.HideAllFood();
        }
    }

    private bool _isBoosterRuning = false;

    public void OnMagnet()
    {
        if(_isBoosterRuning) return;

        if(!BoosterSystem.UseBooster(BoosterType.Magnet)) return;

        Dictionary<string, List<Image>> foods = new Dictionary<string, List<Image>>();
        foreach(var grill in _listGrills)
        {
            if (grill.gameObject.activeInHierarchy && grill.LidGrill == false)
            {
                for(int i = 0; i< grill.TotalSlot.Count; i++)
                {
                    FoodSlots slot = grill.TotalSlot[i];

                    if (slot.HasFood)
                    {
                        string name = slot.GetSpriteFood.name;

                        if(!foods.ContainsKey(name))
                            foods.Add(name, new List<Image>());
                        
                        foods[name].Add(slot.ImgFood);                    }
                }

                TrayItems  tray = grill.GetFirstTray();

                if(tray != null)
                {
                    for(int i = 0; i < tray.FoodList.Count; i++)
                    {
                        Image img = tray.FoodList[i];
                        if (img.gameObject.activeInHierarchy)
                        {
                            string name = img.sprite.name;
                            if(!foods.ContainsKey(name))
                                foods.Add(name, new List<Image>());

                            foods[name].Add(img);
                        }

                    }
                }
            }
        }

        StartCoroutine(IECollect());

        IEnumerator IECollect()
        {
            _isBoosterRuning = true;

            
            foreach(var kvp in foods)
            {
                if(kvp.Value.Count >= 3)
                {
                    _magnetFx.DOScale(Vector3.one, 0.4f);
                    List<Image> food = kvp.Value;
                    
                    FoodSlots foodSlots;
                    Sprite compeletedSprite = kvp.Value[0].sprite;

                    for(int i = 0; i < 3; i++)
                    {
                
                        foodSlots = kvp.Value[i].gameObject.GetComponentInParent<FoodSlots>();

                        Image imgDummy = _magnetLists[i];
                        Image imgFood = kvp.Value[i];

                        imgDummy.sprite = imgFood.sprite;
                        imgDummy.SetNativeSize();
                        imgDummy.transform.position = imgFood.transform.position;
                        imgDummy.gameObject.SetActive(true);
                        if(foodSlots != null)
                            foodSlots.OnHideFood();
                        else
                            imgFood.gameObject.SetActive(false);
                        //imgFood.gameObject.SetActive(false);
                        imgDummy.color = new Color(1f, 1f, 1f, 1f);

                        Vector3 mid = (imgDummy.transform.position + _magnetFx.position) / 2f;
                        mid += new Vector3(Random.Range(-2, 2), Random.Range(-2, 2), 0);
                        Vector3[] path = new Vector3[] {imgDummy.transform.position, mid, _magnetFx.position};

                        Sequence seq = DOTween.Sequence();
                        seq.Join(imgDummy.transform.DOPath(path, 1.5f, PathType.CatmullRom))
                            .Join(imgDummy.DOColor(new Color(1f, 1f, 1f, 0.1f), 1.5f))
                            .SetEase(Ease.InQuad)
                            .OnComplete(() =>
                            {
                                imgDummy.gameObject.SetActive(false);
                                imgDummy.transform.localScale = Vector3.one;

                                imgFood.SendMessageUpwards("OnCheckPrepareTray");
                                
                            });
                        
                        yield return new WaitForSeconds(0.1f);
                    }

                    yield return new WaitForSeconds(1f);
                    _magnetFx.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack);

                    TryUnlockLidGrill(compeletedSprite);
                    OnMinusFood();
                    break;
                }
            }
            yield return new WaitForSeconds(0.8f);
            _isBoosterRuning = false;

        }
        
    }

    public void OnSwap()
    {
        if(_isBoosterRuning) return;

        if(!BoosterSystem.UseBooster(BoosterType.Swap)) return;

        _isBoosterRuning = true;

        List<Image> foods = new();

        foreach(var grill in _listGrills)
        {
            if (grill.gameObject.activeInHierarchy)
            {
                for(int i = 0; i< grill.TotalSlot.Count; i++)
                {
                    FoodSlots slot = grill.TotalSlot[i];

                    if (slot.HasFood)
                    {
                        foods.Add(slot.ImgFood);
                    }
                }
            }

            
        }

        if (foods.Count == 0)
        {
            _isBoosterRuning = false;
            return;
        }

        Sequence seq = DOTween.Sequence();

        foreach(var item in foods)
        {
            seq.Join(item.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack));
        }
        seq.AppendCallback(() =>
        {
           for(int i = foods.Count - 1; i > 0; i--)
            {
                int n = Random.Range(0, foods.Count);
                Sprite tmp = foods[i].sprite;
                foods[i].sprite = foods[n].sprite;
                foods[n].sprite = tmp;
            } 
        });
        foreach(var item in foods)
        {
            seq.Join(item.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack));
        }
        seq.OnComplete(() =>
        {
            _isBoosterRuning = false;
        });
    }
    public void OnAddGrill()
    {
        if(!BoosterSystem.UseBooster(BoosterType.AddGrill)) return;

        int grills = _listGrills.Count;

        foreach(var grill in _listGrills)
        {
            if (!grill.gameObject.activeInHierarchy)
            {
                grill.HideTray();
                grill.gameObject.SetActive(true);
                grill.transform.localScale = Vector3.zero;
                grill.transform.DOScale(1, 0.5f).SetEase(Ease.OutBack);
                return;
            }

            grills--;
        }
        if(grills <= 0)
        {
            NotifyUI.Instance?.ShowNotify("Không còn bếp khả dụng!");
        }
    }
    public void BackToHome() => LoadingScene.Instance.BackToHome();

}
