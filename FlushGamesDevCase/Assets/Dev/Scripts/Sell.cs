using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using DG.Tweening;
using TMPro.EditorUtilities;

public class Sell : MonoBehaviour
{
    [SerializeField] UIManager uiManager;
    public bool status = false;
    public void SellGemsStart(List<InventoryInfo> _inventoryInfo)
    {
        StopSellGems();

        status = true;
        StartCoroutine(SellGems(_inventoryInfo));
    }
    public void StopSellGems()
    {

        status = false;
    }
    #region satis islemi
    private IEnumerator SellGems(List<InventoryInfo> _inventoryInfo)
    {
        GameObject _gemObject;
        while (_inventoryInfo.Count != 0 && status)
        {
            _gemObject = _inventoryInfo[^1].gemObje;
            PlayerPrefs.SetInt(_gemObject.name, PlayerPrefs.GetInt(_gemObject.name) + 1);
            _gemObject.transform.DOMove(transform.position, .18f).OnComplete(() =>
            {
                Destroy(_gemObject);
                _inventoryInfo.RemoveAt(_inventoryInfo.Count - 1);
            });
            uiManager.GemUIUpdate(_gemObject.name);
            yield return new WaitForSeconds(.2f);
        }
        status = false;
    }
    #endregion
}
