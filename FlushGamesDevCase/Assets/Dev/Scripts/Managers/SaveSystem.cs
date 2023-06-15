using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SaveSystem : MonoBehaviour
{
    [SerializeField] bool restartSaveVaribles;
    [SerializeField] List<SaveVaribles> saveVaribles = new List<SaveVaribles>();
    [SerializeField] public List<Varibles> varibles = new List<Varibles>();
    [SerializeField] private GemManager gemManager;
    private void Load()
    {
        string value = null;
        Varibles sad = new Varibles();
        for (sbyte i1 = 0; i1 < saveVaribles.Count; ++i1)
        {
            Debug.Log(saveVaribles[i1].variblesName);
            sad.varibleName = saveVaribles[i1].variblesName;
            switch (saveVaribles[i1].type)
            {

                case SaveVaribles.Type.string_:
                    value = PlayerPrefs.GetString(saveVaribles[i1].variblesName);
                    break;
                case SaveVaribles.Type.int_:
                    value = PlayerPrefs.GetInt(saveVaribles[i1].variblesName).ToString();
                    break;

                case SaveVaribles.Type.float_:
                    Debug.LogError(PlayerPrefs.GetFloat(saveVaribles[i1].variblesName).ToString() + "  " + PlayerPrefs.GetFloat(saveVaribles[i1].variblesName));
                    value = PlayerPrefs.GetFloat(saveVaribles[i1].variblesName).ToString();
                    break;
            }
            sad.value = value;
            varibles.Add(sad);
            sad = new Varibles();
        }
    }
    //kayitli olmayan verileri kayit eder
    public IEnumerator SaveControl()
    {
        if (restartSaveVaribles)
            PlayerPrefs.DeleteAll();
        yield return null;
        //gem verilerini kontrol et
        foreach (Gem gem in gemManager.gems)
        {
            if (!PlayerPrefs.HasKey(gem.gemName))
            {
                PlayerPrefs.SetInt(gem.gemName,0);

                Debug.Log("added: " + gem.gemName);
            }
        }
        //diger verileri kontrol et
        foreach (SaveVaribles saveVarible in saveVaribles)
        {
            if (!PlayerPrefs.HasKey(saveVarible.variblesName))
            {

                switch (saveVarible.type)
                {
                    case SaveVaribles.Type.string_:
                        PlayerPrefs.SetString(saveVarible.variblesName, saveVarible.defaultValue);
                        break;
                    case SaveVaribles.Type.int_:
                        PlayerPrefs.SetInt(saveVarible.variblesName, System.Convert.ToInt32(saveVarible.defaultValue));
                        break;

                    case SaveVaribles.Type.float_:
                        PlayerPrefs.SetFloat(saveVarible.variblesName, float.Parse(saveVarible.defaultValue));
                        break;
                }

                Debug.Log("added: " + saveVarible.variblesName);
            }
        }
       //load devre disi Load();
    }

    [System.Serializable]
    class SaveVaribles
    {
        public enum Type { string_, int_, float_ };
        public string variblesName;
        public Type type;
        public string defaultValue;
    }
    [System.Serializable]
    public class Varibles
    {
        public string varibleName;
        public string value;
    }
}
