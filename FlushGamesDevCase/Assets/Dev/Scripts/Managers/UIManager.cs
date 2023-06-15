using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    TextMeshProUGUI goldText;
    [SerializeField] GemManager gemManager;
    [SerializeField] GameObject gemUIParent;
    [SerializeField] GameObject gemUI;
    private Dictionary<string, TextMeshProUGUI> gemUITexts = new Dictionary<string, TextMeshProUGUI>();
    private List<Gem> gems;
    public void UIValueUpdate()
    {
        
    }

    public IEnumerator GemUICreate()
    {
        yield return null;
        gems = new List<Gem>(gemManager.gems);
        Transform _gemUI;
        TextMeshProUGUI _gemUIText;
        foreach (Gem gem in gems)
        {
            _gemUI = Instantiate(gemUI, gemUIParent.transform).transform;
            _gemUI.GetChild(0).GetComponent<Image>().sprite = gem.gemSprite;
            _gemUI.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Gem Type: " + gem.gemName;
            _gemUIText = _gemUI.GetChild(2).GetComponent<TextMeshProUGUI>();
            _gemUIText.text = "Collected Count: " + PlayerPrefs.GetInt(gem.gemName).ToString();
            gemUITexts.Add(gem.gemName, _gemUIText);
        }
    }
    public void GemUIUpdate(string _gemName)
    {
        gemUITexts[_gemName].text = "Collected Count: " + PlayerPrefs.GetInt(_gemName).ToString();
        //goldText.text = PlayerPrefs.GetInt("gold").ToString();
    }
}
