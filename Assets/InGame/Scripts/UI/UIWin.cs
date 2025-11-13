using DG.Tweening;
using UnityEngine;

public class UIWin : Singleton<UIWin>
{
    [SerializeField] private CanvasGroup canvasGroup;
    public void ShowWin(bool on)
    {
        canvasGroup.DOFade(on ? 1 : 0, 0.5f);
        canvasGroup.interactable = on;
        canvasGroup.blocksRaycasts = on;
    }
}
