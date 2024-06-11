using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public enum eItemType // �ڷ������� ����
    {
        None,
        PowerUp,
        HpRecovery,
    }

    [SerializeField] private eItemType ItemType;

    #region ������ ���� ��ġ

    /// <summary>
    /// ������ ���� �̵��ӵ�
    /// </summary>
    float moveSpeed;//�����̴� �ӵ�

    /// <summary>
    /// ������ ���� ����
    /// </summary>
    Vector3 moveDirection;//������ ����

    [SerializeField] float minSpeed = 1;
    [SerializeField] float maxSpeed = 3;

    #endregion

    private Limiter limiter;

    private void Awake()
    {
        moveSpeed = Random.Range(minSpeed, maxSpeed); //1~3���� � ���̰��� �ӵ�
        float directionX = Random.Range(-1.0f, 1.0f);
        float directionY = Random.Range(-1.0f, 1.0f);
        moveDirection.x = directionX;
        moveDirection.y = directionY;

        moveDirection.Normalize();

        //100.ToString();
        //int.Parse("100");
        // ���ڸ� ���ڷ� Ȥ�� ���ڸ� ���ڷ� ����ÿ��� �Լ��� �̿��ؾ߸� ������ ����

        //int value = (int)eItemType.None;
        //string sValue = eItemType.HpRecovery.ToString();

        // ���ڸ� enum �ڷ������� ����
        //eItemType eValue1 = (eItemType)1;
        // ���ڸ� enum �ڷ������� ����
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
    /// ������ �̵� �Լ�
    /// </summary>
    private void moveItem()
    {
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }

    /// <summary>
    /// �����Ͱ� ����ִ��� ���� Ȯ��, ����ִٸ� ä���ִ� �Լ�
    /// </summary>
    private void LimiterCheck()
    {
        if (limiter == null)
        {
            limiter = GameManager.Instance._Limiter;
        }
    }

    /// <summary>
    /// �������� ���� ƨ��� ���ִ� �Լ�
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
    /// �� �������� ItemType�� ����
    /// </summary>
    /// <returns>�� �������� ItemType</returns>
    public eItemType GetItemType()
    {
        return ItemType; 
    }


}
