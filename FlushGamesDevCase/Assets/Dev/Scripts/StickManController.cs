using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Progress;

public class StickManController : MonoBehaviour
{
    //hareket
    [Header("Oyuncu ayarlari")]
    [SerializeField] Animator stickManAnimator;
    [SerializeField] FloatingJoystick joystick;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotateSpeed;
    private Vector3 moveVector;
    [Header("Collider genislemesi icin ayarlar")]
    public float maxAngle = .25f;
    public float minAngle = 0f;
    public float updateSpeed = .2f;
    [SerializeField] private SphereCollider sphereCollider; // SphereCollider bileseni
    //Envanter
    [SerializeField] List<InventoryInfo> myInventoryInfo = new List<InventoryInfo>();
    private void OnTriggerEnter(Collider other)
    {
        
        GameObject _otherGameObject = other.gameObject;
        switch (_otherGameObject.tag)
        {
            case "gemCenter":
                other.enabled = false;
                Transform _gemTransform = other.transform.GetChild(0).transform;
                InventoryInfo _inInfo = new InventoryInfo();
                _inInfo.gemObje = _gemTransform.gameObject;
                _inInfo.gemScale = _gemTransform.localScale.x;
                myInventoryInfo.Add(_inInfo);

                other.transform.parent.GetComponent<GridManager>().TriggerChild(other.transform, transform);

                break;
            case "sellCenter":
                Debug.Log("sell carp");
                other.GetComponent<Sell>().SellGemsStart(myInventoryInfo);
                break;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        switch (other.tag)
        {
            case "sellCenter":
                other.GetComponent<Sell>().StopSellGems();
                break;
        }
    }
    private void Start()
    {
        StartCoroutine(Move());
    }
    #region Oyuncu hareket
    IEnumerator Move()
    {
        float joystickHorizontal;
        float joystickVertical;
        float _moveSpeed;
        Vector3 direction = new Vector3();
        while (true)
        {
            yield return null;
            moveVector = Vector3.zero;
            joystickHorizontal = joystick.Horizontal;
            joystickVertical = joystick.Vertical;

            if (Mathf.Approximately(joystickHorizontal, 0) || Mathf.Approximately(joystickVertical, 0))
            {
                stickManAnimator.SetBool("idle", true);
            }
            else
            {
                _moveSpeed = moveSpeed * Mathf.Abs(joystickVertical);
                moveVector.x = joystickHorizontal * moveSpeed * Time.deltaTime;
                moveVector.z = joystickVertical * moveSpeed * Time.deltaTime;
                stickManAnimator.SetBool("idle", false);
                stickManAnimator.SetFloat("dikey", Mathf.Abs(joystickVertical) + Mathf.Abs(joystickHorizontal));
                transform.Translate(0, 0, 1 * Time.deltaTime * moveSpeed);
            }
            direction = Vector3.RotateTowards(transform.forward, moveVector, rotateSpeed * Time.deltaTime, 0.0f);
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }
    #endregion


}
