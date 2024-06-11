using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public enum eItemType // 자료형으로 정의
    {
        None,
        PowerUp,
        HpRecovery,
    }

    [SerializeField] private eItemType ItemType;

    #region 아이템 관련 수치

    /// <summary>
    /// 아이템 현재 이동속도
    /// </summary>
    float moveSpeed;//움직이는 속도

    /// <summary>
    /// 아이템 현재 방향
    /// </summary>
    Vector3 moveDirection;//움직일 방향

    [SerializeField] float minSpeed = 1;
    [SerializeField] float maxSpeed = 3;

    #endregion

    private Limiter limiter;

    private void Awake()
    {
        moveSpeed = Random.Range(minSpeed, maxSpeed); //1~3까지 어떤 사이값의 속도
        float directionX = Random.Range(-1.0f, 1.0f);
        float directionY = Random.Range(-1.0f, 1.0f);
        moveDirection.x = directionX;
        moveDirection.y = directionY;

        moveDirection.Normalize();

        //100.ToString();
        //int.Parse("100");
        // 글자를 숫자로 혹은 숫자를 글자로 변경시에는 함수를 이용해야만 변경이 가능

        //int value = (int)eItemType.None;
        //string sValue = eItemType.HpRecovery.ToString();

        // 숫자를 enum 자료형으로 변경
        //eItemType eValue1 = (eItemType)1;
        // 글자를 enum 자료형으로 변경
        //eItemType eValue2 = (eItemType)System.Enum.Parse(typeof(eItemType), "PowerUp");


    }

    void Start()
    {
        LimiterCheck();
    }

    void Update()
    {
        moveItem();
        checkItemPos();
    }

    /// <summary>
    /// 아이템 이동 함수
    /// </summary>
    private void moveItem()
    {
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }

    /// <summary>
    /// 리미터가 비어있는지 여부 확인, 비어있다면 채워주는 함수
    /// </summary>
    private void LimiterCheck()
    {
        if (limiter == null)
        {
            limiter = GameManager.Instance._Limiter;
        }
    }

    /// <summary>
    /// 아이템이 벽에 튕기게 해주는 함수
    /// </summary>
    private void checkItemPos()
    {
        LimiterCheck();
        (bool _x, bool _y) rData = limiter.IsReflectItem(transform.position, moveDirection);
        if (rData._x == true)
        {
            moveDirection = Vector3.Reflect(moveDirection, Vector3.right);
        }
        if (rData._y == true)
        {
            moveDirection = Vector3.Reflect(moveDirection, Vector3.up);
        }

    }

    /// <summary>
    /// 이 아이템의 ItemType을 구함
    /// </summary>
    /// <returns>이 아이템의 ItemType</returns>
    public eItemType GetItemType()
    {
        return ItemType; 
    }


}
