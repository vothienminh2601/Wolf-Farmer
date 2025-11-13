using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIOption : Singleton<UIOption>
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Button bg, continueBtn, restartBtn, saveGameBtn, exitBtn;

    void Start()
    {
        continueBtn.onClick.AddListener(Continue);
        saveGameBtn.onClick.AddListener(SaveGame);
        restartBtn.onClick.AddListener(Restart);
        exitBtn.onClick.AddListener(Exit);
        bg.onClick.AddListener(() => ShowOption(false));
    }

    void OnDestroy()
    {
        continueBtn.onClick.RemoveListener(Continue);
        restartBtn.onClick.RemoveListener(Restart);
        saveGameBtn.onClick.RemoveListener(SaveGame);
        exitBtn.onClick.RemoveListener(Exit);
        bg.onClick.RemoveListener(() => ShowOption(false));
    }

    public void Toggle()
    {
        ShowOption(canvasGroup.alpha == 0);
    }

    public void ShowOption(bool on)
    {
        canvasGroup.DOFade(on ? 1 : 0, 0.5f);
        canvasGroup.interactable = on;
        canvasGroup.blocksRaycasts = on;
    }

    void Continue()
    {
        ShowOption(false);
    }

    void Restart()
    {
        UserData.Instance.DeleteAllData();
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }

    void SaveGame()
    {
        UserData.Instance.SaveGame();
    }

    void Exit()
    {
        
        UserData.Instance.SaveGame();
        Application.Quit();
    }
}
