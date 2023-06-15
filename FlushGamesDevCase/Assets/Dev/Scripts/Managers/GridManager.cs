using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEditor;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;
using System;
using Unity.VisualScripting.FullSerializer;

public class GridManager : MonoBehaviour
{

    [Header("Grid Ayarlari")]
    [Tooltip("Bu degerler olusturulan alanýn boyutunu ayarlamanýza yarar")]
    [Range(1, 10)]
    [SerializeField] public int gridX; // Grid boyutunu belirleyen deðiþken
    [Range(1, 10)]
    [SerializeField] private int gridY; // Grid boyutunu belirleyen deðiþken
    [Tooltip("Gridler icerisinde olusacak obje")]
    [SerializeField] private GameObject objectPrefab; // Oluþturulacak GameObject prefabý
    [Tooltip("Olusacak objelerin arasýndaki bosluk")]
    [SerializeField] private float spacing = 0.1f; // Küpler arasýndaki boþluk
    [Tooltip("Grid icerisinde olusacak nesnenin boyutunu belirtir")]

    //Gem
    [SerializeField] private GemManager gemManager;
    private List<GameObject> grids = new List<GameObject>();
    private Coroutine[] gridsCoroutine;
    private Dictionary<RarityLevel, List<Gem>> rarityGems;
    [SerializeField] public Transform stackGem;


    private void OnValidate()
    {
        StartCoroutine(ClearChildObjects());
    }
    public void GridStart()
    {
        rarityGems = new Dictionary<RarityLevel, List<Gem>>(gemManager.rarityGems);
        gridsCoroutine = new Coroutine[grids.Count];
        GenerateGem();
    }
    #region Grid Olusturma Islemi
    private void GenerateGrid()
    {

        byte objCount = 0;
        Vector3 _scale;
        for (int x = 1; x <= gridY; x++)
        {
            for (int y = 1; y <= gridX; y++)
            {

                Vector3 spawnPosition = transform.TransformPoint(new Vector3(x, 0, y));// Pozisyon al

                // Hocreye objeyi yerlestir
                GameObject obj = Instantiate(objectPrefab, spawnPosition, Quaternion.identity);
                _scale = obj.transform.localScale;
                obj.transform.SetParent(transform);
                obj.name = objCount.ToString();//dictionary icerisinde gridin id sini bulabilmek icin.
                ++objCount;
                spawnPosition += new Vector3((_scale.x + spacing) * x, 0f, (_scale.z + spacing) * y); // Bosluk ekle

                obj.transform.position = spawnPosition;
                grids.Add(obj);
            }
        }
    }
    #endregion
    #region Gem Olusturma
    private void GenerateGem(byte _status = 0, Transform _childTransform = null)
    {

        //gem olusturma
        Func<GameObject, Tuple<Gem, Transform>> createGem = (_grid) =>
        {
            Gem _gem;
            Transform gemTransform;
            Vector3 _up = Vector3.up * 1.5f;
            _gem = RandomGem();
            gemTransform = Instantiate(_gem.gemObject, _grid.transform.position + _up, _grid.transform.rotation).transform;
            gemTransform.name = _gem.gemName;
            gemTransform.SetParent(_grid.transform);
            return Tuple.Create(_gem, gemTransform);
        };
        switch (_status)
        {
            case 0:
                foreach (GameObject grid in grids)
                {
                    createGem(grid);
                }
                break;
            case 1:
                if(_childTransform.childCount == 0)
                {
                    Tuple<Gem, Transform> _tp = createGem(_childTransform.gameObject);
                    gridsCoroutine[Convert.ToInt32(_childTransform.name)] = StartCoroutine(GemScale(_tp.Item1, _childTransform.gameObject, _tp.Item2));
                }
                break;
        }



    }
    #endregion

    #region Nadirlige gore gem uretme
    private Gem RandomGem()
    {
        float randomValue = UnityEngine.Random.value; //0<= x <1
        RarityLevel _rarity;
        Gem _gem;
        List<Gem> _gems;
        if (randomValue < 0.6f)
        {
            _rarity = RarityLevel.Common;
        }
        else if (randomValue < 0.8f)
        {
            _rarity = RarityLevel.Rare;
        }

        else if (randomValue < 0.9f)
        {
            _rarity = RarityLevel.Epic;
        }

        else if (randomValue < 0.95f)
        {
            _rarity = RarityLevel.Legendary;
        }
        else
        {
            _rarity = RarityLevel.Mythical;
        }
        if (!rarityGems.ContainsKey(_rarity))
        {
            RarityLevel[] _control = new RarityLevel[rarityGems.Keys.Count];
            rarityGems.Keys.CopyTo(_control, 0);
            Debug.Log(_control.Length);
            _rarity = _control[UnityEngine.Random.Range(0, _control.Length)];
        }
        _gems = new List<Gem>(rarityGems[_rarity]);
        _gem = _gems[UnityEngine.Random.Range(0, _gems.Count)];
        return _gem;
    }
    #endregion

    #region Var olan Gridleri temizleme ve tekrar olustur
    private IEnumerator ClearChildObjects()
    {
        yield return null;
        GameObject[] _gm = GetComponentsInChildren<Transform>(true)
            .Where(t => t != transform)
            .Select(t => t.gameObject)
            .ToArray();
        foreach (GameObject gameObject in _gm)
        {
            DestroyImmediate(gameObject);
        }
        grids.Clear();
        GenerateGrid();
    }
    #endregion

    #region oyuncu alt objedeki bir yuvaya carptiginda
    public void TriggerChild(Transform _childTransform, Transform _playerTransform)
    {
        int _gridCoroutineIndex = Convert.ToInt32(_childTransform.name);
        //gem buyume kontrolu
        if (gridsCoroutine[_gridCoroutineIndex] !=null)
        {
            StopCoroutine(gridsCoroutine[_gridCoroutineIndex]);
            gridsCoroutine[_gridCoroutineIndex] = null;
        }
        Transform _centerChildTransform = _childTransform.GetChild(0);
        
        _centerChildTransform.DOMove(_playerTransform.position, .1f).OnComplete(() =>
        {
            int _stackChildCount = stackGem.childCount;
            
            _centerChildTransform.localScale = new Vector3(.6f, .6f, .6f);
            if (stackGem.childCount == 0)
            {
                _centerChildTransform.transform.position = stackGem.position + Vector3.up;
            }
            else
            {
                _centerChildTransform.transform.position = stackGem.GetChild(_stackChildCount - 1).position + Vector3.up;
            }
            _centerChildTransform.SetParent(stackGem);
            GenerateGem(1, _childTransform);
        }
        );
    }
    #endregion

    #region Gem buyutme
    private IEnumerator GemScale(Gem _gem, GameObject _gemParentObject, Transform _gemTransform)
    {
        string _gemObjectName = _gemParentObject.name;
        float _waitSeconds = .2f;
        float _collectionRate = _gem.collectionRate;
        float _scale = 1f / (_gem.growthRate / _waitSeconds);
        BoxCollider _gridCollider = _gemParentObject.GetComponent<BoxCollider>();
        Vector3 _po = new Vector3(_scale, _scale, _scale);
        _gemTransform.localScale = Vector3.zero;
        while (_gemTransform.localScale.x < 1f && _gemTransform.parent.name == _gemObjectName)
        {
            _gemTransform.localScale += _po;
            if (_gemTransform.localScale.x >= _collectionRate && !_gridCollider.enabled)
            {
                _gridCollider.enabled = true;
            }
            yield return new WaitForSeconds(_waitSeconds);
        }
    }
    #endregion
}

