using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Yoneticiler")]
    [SerializeField] SaveSystem saveSystem;
    [SerializeField] GemManager gemManager;
    [SerializeField] UIManager uiManager;
    [SerializeField] Transform stackGem;
    [SerializeField] List<GridManager> grids;

    private IEnumerator Start()
    {
        yield return gemManager.GemRarity();
        yield return saveSystem.SaveControl();
        yield return uiManager.GemUICreate();
        foreach (GridManager grid in grids) {
            grid.stackGem = stackGem;
            grid.GridStart();
        }
    }
}
