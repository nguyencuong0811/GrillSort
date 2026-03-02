using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using TMPro;

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

    private List<GrillStation> _listGrills; //danh sach cac bep
    private float _avgTray; //gia tri trung binh thuc an tren 1 dia

    private List<Sprite> _totalSpriteFood; //danh sach tat ca sprite thuc an 
    private List<int> _randLidGrill = new List<int>();
    
    void Awake()
    {
        _listGrills = Utils.GetListInChild<GrillStation>(_gridGrill);
        Sprite[] sprites = Resources.LoadAll<Sprite>("items");
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
        LoadLevel();
    }
    public void LoadLevel()
    {
        int levelIndex = SaveManager.GetCurrentLevel();
        if(levelIndex >= levelDatabase.Levels.Count)
            levelIndex = 0;
        
        _currentLevel = levelDatabase.Levels[levelIndex];
        _leveltxt.text = $"Level: {_currentLevel.Level+1}";
        InitLevel(_currentLevel);
    }
    private void InitLevel(LevelData levelData)
    {
        _allFood = levelData.AllFood;
        _totalFoods = levelData.TotalFood;
        _totalGrill = levelData.TotalGrill;
        _totalLidGrill = levelData.TotalLidGrill;

        _avgTray = (Random.Range(levelData.MinAvgTray, levelData.MaxAvgTray));

        // random bep bi an
        this.RandGrillHide();

        OnInitLevel();

        this.SetActiveLidGrill();
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
        Debug.Log("Tong so thuc an: "+ useFood.Count);
        //random, trao vi tri cua cac item
        for(int i =0; i<useFood.Count; i++)
        {
            int rand = Random.Range(i, useFood.Count);
            (useFood[i], useFood[rand]) = (useFood[rand], useFood[i]); // ham spawp
        }

        int totalTray = Mathf.RoundToInt(useFood.Count / _avgTray); // tinh tong so luong dia can

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
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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
        foreach(int grillIndex in _randLidGrill)
        {
            GrillStation grill = _listGrills[grillIndex];
            if (grill.HasLidWithFood(completedFood))
            {
                grill.ShowGrill();
                _randLidGrill.Remove(grillIndex);
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
            if(!grill.gameObject.activeInHierarchy) 
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

    public void BackToHome() => LoadingScene.Instance.BackToHome();
}
