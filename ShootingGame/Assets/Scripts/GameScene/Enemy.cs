using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.ReorderableList;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum eEnemyType
    {
        EnemyA,
        EnemyB,
        EnemyC,
        EnemyBoss,
    }


    [SerializeField] protected eEnemyType enemyType;


    #region Enemy Stats

    /// <summary>
    /// 적 이동속도
    /// </summary>
    [SerializeField] protected float moveSpeed;
    /// <summary>
    /// 적 체력
    /// </summary>
    [SerializeField] protected float hp;

    #endregion

    #region Sprite Vars

    protected Sprite defaultSprite;
    [SerializeField] protected Sprite hitSprite;
    protected SpriteRenderer spriteRenderer;

    #endregion

    /// <summary>
    /// Explosion Prefab 폭발 프리팹
    /// </summary>
    protected GameObject fabExplosion;
    protected GameManager gameManager;

    #region 아이템 관련

    /// <summary>
    /// 죽을때 아이템을 드랍여부
    /// </summary>
    private bool haveItem = false;
    /// <summary>
    /// 죽었는지 여부
    /// </summary>
    protected bool isDied = false; // 적기가 죽고나면 더이상 기능을 
    
    /// <summary>
    /// 아이템 보유시 sprite 색깔
    /// </summary>
    [Header("아이템 보유시 컬러")]
    [SerializeField] protected Color colorHaveItem;


    [Header("파괴시 점수")]
    [SerializeField] protected int score; // 자신이 파괴 되었을때 점수

    #endregion


    protected virtual void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    //protected virtual void OnDestroy()
    //{
    //    if (gameManager == null) return;
    //    gameManager.AddScore(score);
    //}


    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    protected virtual void Start()
    {
        initVars();
    }

    protected virtual void Update()
    {
        moving();
    }


    /// <summary>
    /// 보통 Start 에서의 행동 함수
    /// </summary>
    private void initVars()
    {
        
        defaultSprite = spriteRenderer.sprite;

        if (haveItem == true)
        {
            spriteRenderer.color = colorHaveItem;
        }

        gameManager = GameManager.Instance;
        fabExplosion = gameManager.FabExplosion;
    }

    /// <summary>
    /// 적 이동 함수
    /// </summary>
    protected virtual void moving()
    {
        transform.position -= transform.up * moveSpeed * Time.deltaTime;
    }

    

    protected virtual void shooting()
    {

    }

    /// <summary>
    /// 적에에 데미지를 주기 위해 만든 함수
    /// </summary>
    /// <param name="_damage">데미지 양</param>
    public virtual void Hit(float _damage)
    {
        if (isDied) { return; } // 죽었을때 실행 무시
        hp -= _damage; // 데미지 처리

        if (hp <= 0) // 체력이 0 이하라면 (죽었다면)
        {
            isDied = true;
            Destroy(gameObject); // 삭제를 예약
            //매니저로부터 받아온 폭발 연출을 내 위치에 생성하고 부모로 사용중인 레이어에 만들어줌
            float width = spriteRenderer.sprite.rect.width;
            gameManager.CreateExplosionEffect(transform.position, width);
            

            // 매니저를 호출후 현재 내위치를 전달하면 매니저가 아이템을 그 위치에 만들어줌
            if (haveItem)
            {
                gameManager.createItem(transform.position);
            }

            gameManager.AddKillCount();
            gameManager.AddScore(score);

            //int score = 0;

            //if (enemyType == eEnemyType.EnemyA)
            //{
            //    score = 100;
            //}



        }
        else // 피가 줄어들었을때
        {
            //hit 연출 스프라이트 변경기능
            spriteRenderer.sprite = hitSprite;
            //약간의 시간이 지난뒤에 어떤 함수를 실행하고 싶을때
            Invoke("setDefaultSprite", 0.04f);
        }
    }

    /// <summary>
    /// 기본 스프라이트로 설정
    /// </summary>
    private void setDefaultSprite()
    {
        spriteRenderer.sprite = defaultSprite;
    }

    /// <summary>
    /// 자신이 아이템을 드랍해야 한다고 신호 주는용, haveItem 이 true 로 설정됨
    /// </summary>
    public void SetItem()
    {
        haveItem = true;
    }


}
