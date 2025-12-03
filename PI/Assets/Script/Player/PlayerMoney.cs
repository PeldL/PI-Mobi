using UnityEngine;

public class PlayerMoney : MonoBehaviour
{
    public static PlayerMoney Instance { get; private set; }

    [SerializeField] private int _money = 100;
    public int money
    {
        get => _money;
        set
        {
            _money = Mathf.Max(0, value);
            OnMoneyChanged?.Invoke(_money);
        }
    }

    public event System.Action<int> OnMoneyChanged;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadMoney();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void LoadMoney()
    {
        money = PlayerPrefs.GetInt("PlayerMoney", 100);
    }

    public void SaveMoney()
    {
        PlayerPrefs.SetInt("PlayerMoney", money);
    }
}