using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using DG.Tweening;
using TMPro.EditorUtilities;

public class Sell : MonoBehaviour
{
    [SerializeField] UIManager uiManager;
    [SerializeField] GemManager gemManager;
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
        string _gemName;
        float _gold;
        while (_inventoryInfo.Count != 0 && status)
        {

            _gemObject = _inventoryInfo[^1].gemObje;
            _gemName = _gemObject.name;
            PlayerPrefs.SetInt(_gemName, PlayerPrefs.GetInt(_gemName) + 1);
            _gold = gemManager.gemInfo[_gemName].gemMoney + (_inventoryInfo[^1].gemScale * 100f);
            PlayerPrefs.SetFloat("gold", _gold + PlayerPrefs.GetFloat("gold"));
            _gemObject.transform.DOMove(transform.position, .18f).OnComplete(() =>
            {
                Destroy(_gemObject);
                _inventoryInfo.RemoveAt(_inventoryInfo.Count - 1);


            });

            uiManager.GemUIUpdate(_gemName);
            yield return new WaitForSeconds(.2f);
        }
        status = false;
    }
    #endregion
}
