using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    #region �Ѿ� ���� ��ġ

    /// <summary>
    /// �Ѿ� �̵��ӵ�
    /// </summary>
    [SerializeField] float moveSpeed;
    /// <summary>
    /// �Ѿ� ������
    /// </summary>
    [SerializeField] float damage = 1f;

    /// <summary>
    /// ���� �� �Ѿ����� �ƴ���
    /// </summary>
    bool isShootEnemy = true;

    #endregion

    #region �޸�1
    //���⿡ ������� or �÷��̾ �������
    //���ʵڿ� ������ٰ� ���������
    //ȭ������� ��������
    #endregion

    private void OnBecameInvisible()
    {
        DestoryMe();
    }

    private void OnTriggerEnter2D(Collider2D collision)//collision�� ��� �ݸ���
    {
        OnHit(collision.transform);
    }

    void Update()
    {
        transform.position += transform.up * moveSpeed * Time.deltaTime;
    }

    /// <summary>
    /// �ڱ� (�ڽ�)�� (����) �ϴ� �Լ�
    /// </summary>
    private void DestoryMe()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// ���𰡿� �������
    /// </summary>
    /// <param name="hitTransform">���� Transform</param>
    private void OnHit(Transform hitTransform)
    {
        //���� ����� ��Ȯ�� �� �ʿ䰡 ����
        if (isShootEnemy == false && hitTransform.tag == "Enemy")
        {
            DestoryMe();
            Enemy enemy = hitTransform.GetComponent<Enemy>();
            enemy.Hit(damage);
        }
        if (isShootEnemy == true && hitTransform.tag == "Player")
        {
            DestoryMe();
            Player player = hitTransform.GetComponent<Player>();
            player.PlayerGotHit();
        }
    }

    public void ShootPlayer()
    {
        isShootEnemy = false;
    }


}
