using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



public class GemManager : MonoBehaviour
{
    [Header("Gemler")]
    public List<Gem> gems = new List<Gem>();
    [HideInInspector] public Dictionary<string, Gem> gemInfo = new Dictionary<string, Gem>();//isime göre geme ulasmak icin
    // [HideInInspector] 
    public Dictionary<RarityLevel, List<Gem>> rarityGems = new Dictionary<RarityLevel, List<Gem>>();
    #region Gem nadirliklerini ayarla
    public IEnumerator GemRarity()
    {
        yield return null;
        //gecici listeler
        List<Gem> _gems = new List<Gem>(gems);
        List<Gem> _rarityGems = new List<Gem>();
        //RarityLevel enumunun elemanlarý
        foreach (RarityLevel _rarity in Enum.GetValues(typeof(RarityLevel)))
        {
            foreach (Gem gem in _gems)// gecici listedeki gemleri gez
            {
                //gem info 
                if (!gemInfo.ContainsKey(gem.gemName))
                    gemInfo.Add(gem.gemName, gem);
                if (gem.rarityLevel == _rarity)//donen rarityLevel ile donen gemin rarity leveli ayniysa gecici listeye al
                {
                    _rarityGems.Add(gem);
                }
            }
            //dicitonarya gecici listeyi yazir ve alinan gemleri gecici listeden cikar
            if (_rarityGems.Count != 0)
                rarityGems.Add(_rarity, new List<Gem>(_rarityGems));
            _gems = _gems.Except(_rarityGems).ToList();
            _rarityGems.Clear();
        }

        Debug.Log(rarityGems.Count);
    }
    #endregion
}


[Serializable]
public class Gem
{

    [Tooltip("adi")]
    public string gemName; //gemin adi
    [Tooltip("fiyati")]
    public float gemMoney; //gemin fiyati
    [Tooltip("sprite")]
    public Sprite gemSprite; //uý icin gemin resmi
    [Tooltip("olusacak obje")]
    public GameObject gemObject; //gem objesi
    [Tooltip("nadirlik")]
    public RarityLevel rarityLevel = RarityLevel.Common; //nadirlik
    [Tooltip("kac saniyede maksimum buyukluge ulasacagini belirtir")]
    public float growthRate = 5f; //buyume hizi
    [Range(.2f, 1f)]
    [Tooltip("toplanabilir buyukluk")]
    public float collectionRate = .2f; //toplanabilir buyukluk
}
//gemlerin nadirligi
public enum RarityLevel
{
    Common, //yaygin
    Rare, //nadir
    Epic, //epic
    Legendary, //efsanevi
    Mythical //mitolojik

}