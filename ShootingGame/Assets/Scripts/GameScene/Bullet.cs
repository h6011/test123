using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    #region 총알 관련 수치

    /// <summary>
    /// 총알 이동속도
    /// </summary>
    [SerializeField] float moveSpeed;
    /// <summary>
    /// 총알 데미지
    /// </summary>
    [SerializeField] float damage = 1f;

    /// <summary>
    /// 적이 쏜 총알인지 아닌지
    /// </summary>
    bool isShootEnemy = true;

    #endregion

    #region 메모1
    //적기에 닿았을때 or 플레이어에 닿았을때
    //몇초뒤에 사라진다고 명령했을때
    //화면밖으로 나갔을때
    #endregion

    private void OnBecameInvisible()
    {
        DestoryMe();
    }

    private void OnTriggerEnter2D(Collider2D collision)//collision은 상대 콜리전
    {
        OnHit(collision.transform);
    }

    void Update()
    {
        transform.position += transform.up * moveSpeed * Time.deltaTime;
    }

    /// <summary>
    /// 자기 (자신)을 (삭제) 하는 함수
    /// </summary>
    private void DestoryMe()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// 무언가에 닿았을때
    /// </summary>
    /// <param name="hitTransform">닿은 Transform</param>
    private void OnHit(Transform hitTransform)
    {
        //때릴 대상을 정확히 할 필요가 있음
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
