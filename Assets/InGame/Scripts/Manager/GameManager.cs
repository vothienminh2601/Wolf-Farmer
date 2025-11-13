using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private bool isGameOver = false;

    public static event Action<float> OnTimeScaleChanged;
    void Start()
    {
        ResourceManager.OnCoinChanged += CheckWin;
    }

    void OnDestroy()
    {
        ResourceManager.OnCoinChanged -= CheckWin;
    }

    void Update()
    {
        if(isGameOver) return;
        if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            Time.timeScale += 0.5f;
            OnTimeScaleChanged?.Invoke(Time.timeScale);
        }
        if(Input.GetKeyDown(KeyCode.DownArrow))
        {
            Time.timeScale -= 0.5f;
            OnTimeScaleChanged?.Invoke(Time.timeScale);
        }

        if(Input.GetKeyDown(KeyCode.C))
        {
            ResourceManager.Instance.AddCoin(100);
        }
        
    }

    public void SetGameOver(bool isGameOver)
    {
        this.isGameOver = isGameOver;
        Time.timeScale = 0;
        OnTimeScaleChanged?.Invoke(Time.timeScale);
    }
 
    void CheckWin(int coin)
    {
        if(coin >= 1000000)
        {
            UIWin.Instance.ShowWin(true);
            SetGameOver(true);
        }
    }
}
