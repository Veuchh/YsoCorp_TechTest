using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class GameCanvas : MonoBehaviour
{
    [SerializeField] CardDispenser cardDispenserPrefab;
    [SerializeField] Transform cardDispensersParent;

    private async void Awake()
    {
        while (LevelHandler.Instance == null)
        {
            await UniTask.NextFrame();
        }

        LevelHandler.Instance.OnLevelStarted.AddListener(OnLevelStarted);
    }

    void TryClearUI()
    {
        for (int i = cardDispensersParent.childCount - 1; i >= 0; i--)
        {
            Destroy(cardDispensersParent.GetChild(i).gameObject);
        }
    }

    private void OnLevelStarted(LevelData leveldata, CardSelectionService cardSelectionService)
    {
        TryClearUI();

        foreach (CardAttributes cardDispenserAttributes in leveldata.cardDispenserAttributes)
        {
            CardDispenser newCardDispenser = Instantiate(cardDispenserPrefab, cardDispensersParent);
            newCardDispenser.Initialize(cardDispenserAttributes, cardSelectionService);
        }
    }
}
