using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Animator anim;

    #region 플레이어 관련 수치
    
    [Header("플레이어 설정"), SerializeField, Tooltip("플레이어의 이동속도")] float moveSpeed;
    private Vector3 moveDir;

    #endregion


    [Header("총알")]
    [SerializeField] GameObject fabBullet;//플레이어가 복제해서 사용할 원본 총알
    [SerializeField] GameObject fabBullet2;//플레이어가 복제해서 사용할 원본 총알
    [SerializeField] GameObject fabBullet3;//플레이어가 복제해서 사용할 원본 총알
    [SerializeField] GameObject fabBullet4;//플레이어가 복제해서 사용할 원본 총알
    [SerializeField] GameObject fabBullet5;//플레이어가 복제해서 사용할 원본 총알
    [SerializeField] Transform dynamicObject;
    SpriteRenderer spriteRenderer;

    #region 사격 관련 수치

    [SerializeField] bool autoFire = false;//자동공격기능
    [SerializeField] float fireRateTime = 0.5f;//이시간이 지나면 총알이 발사됨
    float fireTimer = 0;

    #endregion

    GameManager gameManager;
    GameObject fabExplosion;
    Limiter limiter;

    #region 체력 관련 수치

    [Header("체력")]
    [SerializeField] int maxHp = 3;
    [SerializeField] int curHp;
    int beforeHp;

    /// <summary>
    /// 무적 여부
    /// </summary>
    private bool invincibilty = false;
    /// <summary>
    /// 무적 시간
    /// </summary>
    [SerializeField] private float invincibiltyTime = 1f;
    private float invincibiltyTimer;

    #endregion

    #region 레벨 관련 수치

    [Header("플레이어 레벨")]
    [SerializeField] int minLevel = 1;
    [SerializeField] int maxLevel = 5;
    [SerializeField, Range(1, 5)] int curLevel;

    #region 강사님 스타일

    [Header("강사님 스타일 관련 수치")]
    [SerializeField, Tooltip("2레벨 이상시 총알이 중심으로 부터 벌어지는 거리")] float distanceBullet; // 2레벨 이상시 총알이 중심으로 부터 벌어지는 거리 // 플레이어 전방에서 발사
    [SerializeField, Tooltip("4레벨 이상시 총알이 회전된 값")] float angleBullet; // 4레벨 이상시 총알이 회전된 값
    [SerializeField, Tooltip("기본 총알 발사 위치")] Transform shootTrs;
    [SerializeField, Tooltip("4레벨 이상시 총알이 발사될 위치")] Transform shootTrsLevel4; // 4레벨 이상시 총알이 발사될 위치
    [SerializeField, Tooltip("5레벨 이상시 총알이 발사될 위치")] Transform shootTrsLevel5; // 5레벨 이상시 총알이 발사될 위치

    #endregion

    #region 내 스타일
    /* 메모1
     * 난 그냥 레벨 비례로 점점 많아 지며 펼쳐지는 느낌으로
     */
    [Header("내 스타일 관련 수치")]
    [SerializeField] float anglePerLevel = 10f;

    #endregion


    /// <summary>
    /// 강사님의 샷 스타일을 따를껀지 여부
    /// </summary>
    
    public enum eShootStyle
    {
        Study1,
        My,
        Study2,
    }
    
    [Header("강사님 스타일 따를껀지")]
    [SerializeField] private eShootStyle shootStyle;

    #endregion

    



    private void OnValidate() // 인스펙터에서 어떤값이 변동이 생기면 호출
    {
        if (Application.isPlaying == false) return;

        if (beforeHp != curHp)
        {
            beforeHp = curHp;
            GameManager.Instance.SetHp(maxHp, curHp);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        OnHit(collision.transform);
    }


    #region Awake 보다 앞선 함수 (메모)

    //[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    //private static  void initCode()
    //{
    //    Debug.Log("initCode");
    //}

    #endregion

    private void Awake()
    {
        anim = transform.GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        curHp = maxHp;
    }

    private void Start()
    {
        //cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        gameManager = GameManager.Instance;
        fabExplosion = gameManager.FabExplosion;
        gameManager._Player = this;
        curLevel = 1;
    }

    void Update()
    {
        moving();
        doAnimation();
        checkPlayerPos();

        shoot();
        checkinvincibilty();
    }

    private void checkinvincibilty() // 무적일때만 작동하여 일정시간이 지나고 나면 다시 무적을 풀어줌
    {
        if (invincibilty == false) return;

        if (invincibiltyTimer > 0f)
        {
            invincibiltyTimer -= Time.deltaTime;
            if (invincibiltyTimer < 0f)
            {
                SetSpriteInvincibilty(false);
            }
        }

    }

    private void SetSpriteInvincibilty(bool _value)
    {
        Color color = spriteRenderer.color;
        if (_value == true) // 무적이 된것 처럼 투명도를 줄여 유저에게 무적이라 알려줌
        {
            color.a = 0.5f;
            invincibilty = true;
            invincibiltyTimer = invincibiltyTime;
        }
        else
        {
            color.a = 1.0f;
            invincibilty = false;
            invincibiltyTimer = 0.0f;
        }
        spriteRenderer.color = color;
    }


    /// <summary>
    /// 충돌시 호출
    /// </summary>
    /// <param name="HitTransform">충돌된 Transform</param>
    private void OnHit(Transform HitTransform)
    {
        if (HitTransform.tag == Tool.GetTag(GameTags.Enemy))
        {
            PlayerGotHit();
            // 체력 감소
            /*
             * 체력이 0이 되면 게임이 끝남
             * 점수가 랭크인이 되면 이름 입력하는 기능
             * 메인 메뉴에서 1~10 등 랭크
             * 
             * 짧은 시간 무적
             * 
             * 게이지 변화 코드 실행
             */
        }
        else if (HitTransform.tag == Tool.GetTag(GameTags.Item))
        {
            Item itemScript = HitTransform.GetComponent<Item>();
            Destroy(itemScript.gameObject); // 이 함수는 이 함수가 모든 동작을 마치게 되면 삭제 해달라고 예약하는 기능
            if (itemScript.GetItemType() == Item.eItemType.PowerUp)
            {
                curLevel += 1;
                if (curLevel > maxLevel)
                {
                    curLevel = maxLevel;
                }
            }
            else if (itemScript.GetItemType() == Item.eItemType.HpRecovery)
            {
                // 체력 회복
                curHp += 1;
                if (curHp > maxHp)
                {
                    curHp = maxHp;
                }
                gameManager.SetHp(maxHp, curHp);
            }
        }
    }


    /// <summary>
    /// 플레이어 기체의 기동을 정의합니다. / 플레이어를 움직이게 해주는 함수
    /// </summary>
    private void moving()
    {
        moveDir.x = Input.GetAxisRaw("Horizontal");//왼쪽 혹은 오른쪽 입력// -1 0 1
        moveDir.y = Input.GetAxisRaw("Vertical");//위 혹은 아래 입력 // -1 0 1

        transform.position += moveDir * moveSpeed * Time.deltaTime;
    }

    /// <summary>
    /// 애니메이션에 어떤 애니메이션을 실행할지 파라미터를 전달 합니다.
    /// </summary>
    private void doAnimation()//하나의 함수에는 하나의 기능
    {
        anim.SetInteger("Horizontal", (int)moveDir.x);
    }

    /// <summary>
    /// Limiter 가 없으면 채워주는 함수
    /// </summary>
    private void CheckLimiter()
    {
        if (limiter == null)
        {
            limiter = gameManager._Limiter;
        }
    }

    /// <summary>
    /// 플레이어가 정해진 범위 밖을 나갔다면 다시 안으로 들어오게 하는 함수
    /// </summary>
    private void checkPlayerPos()
    {
        CheckLimiter();
        transform.position = limiter.checkMovePosition(transform.position, false);
    }

    /// <summary>
    /// 무슨 스타일로 쏘게 할껀지, 레벨 확인
    /// </summary>
    private void CheckLevelShoot()
    {
        if (shootStyle == eShootStyle.Study1)
        {
            if (curLevel == 1)
            {
                gameManager.CreateBullet(shootTrs.position, Quaternion.Euler(0, 0, 0));
            }
            else if (curLevel == 2)
            {
                gameManager.CreateBullet(shootTrs.position + new Vector3(distanceBullet, 0, 0), Quaternion.Euler(0, 0, 0));
                gameManager.CreateBullet(shootTrs.position + new Vector3(-distanceBullet, 0, 0), Quaternion.Euler(0, 0, 0));
            }
            else if (curLevel == 3)
            {
                gameManager.CreateBullet(shootTrs.position, Quaternion.Euler(0, 0, 0));
                gameManager.CreateBullet(shootTrs.position + new Vector3(distanceBullet, 0, 0), Quaternion.Euler(0, 0, 0));
                gameManager.CreateBullet(shootTrs.position + new Vector3(-distanceBullet, 0, 0), Quaternion.Euler(0, 0, 0));
            }
            else if (curLevel == 4)
            {
                gameManager.CreateBullet(shootTrs.position, Quaternion.Euler(0, 0, 0));
                gameManager.CreateBullet(shootTrs.position + new Vector3(distanceBullet, 0, 0), Quaternion.Euler(0, 0, 0));
                gameManager.CreateBullet(shootTrs.position + new Vector3(-distanceBullet, 0, 0), Quaternion.Euler(0, 0, 0));
                Vector3 lv4Pos = shootTrsLevel4.position;
                gameManager.CreateBullet(lv4Pos, Quaternion.Euler(0, 0, angleBullet));
            }
            else if (curLevel == 5)
            {
                gameManager.CreateBullet(shootTrs.position, Quaternion.Euler(0, 0, 0));
                gameManager.CreateBullet(shootTrs.position + new Vector3(distanceBullet, 0, 0), Quaternion.Euler(0, 0, 0));
                gameManager.CreateBullet(shootTrs.position + new Vector3(-distanceBullet, 0, 0), Quaternion.Euler(0, 0, 0));
                Vector3 lv4Pos = shootTrsLevel4.position;
                gameManager.CreateBullet(lv4Pos, Quaternion.Euler(0, 0, angleBullet));
                Vector3 lv5Pos = shootTrsLevel5.position;
                gameManager.CreateBullet(lv5Pos, Quaternion.Euler(0, 0, -angleBullet));
            }
        }
        else if(shootStyle == eShootStyle.My)
        {
            if (curLevel % 2 == 0) // 2 4
            {
                for (int iNum = 1; iNum <= curLevel/2; iNum++)
                {
                    gameManager.CreateBullet(transform.position, Quaternion.Euler(0, 0, anglePerLevel/2 * iNum * 1));
                    gameManager.CreateBullet(transform.position, Quaternion.Euler(0, 0, anglePerLevel/2 * iNum * -1));
                }
            }
            else // 1  3  5
            {
                int savecurLevel = curLevel - 1;
                gameManager.CreateBullet(transform.position, Quaternion.Euler(0, 0, 0));
                for (int iNum = 1; iNum <= savecurLevel / 2; iNum++)
                {
                    gameManager.CreateBullet(transform.position, Quaternion.Euler(0, 0, anglePerLevel * iNum * 1));
                    gameManager.CreateBullet(transform.position, Quaternion.Euler(0, 0, anglePerLevel * iNum * -1));
                }
            }
        }
        else if (shootStyle == eShootStyle.Study2)
        {
            if (curLevel == 1)
            {
                GameObject bullet = Instantiate(fabBullet, shootTrs.position, Quaternion.identity, dynamicObject);
                Bullet bulletScript = bullet.GetComponent<Bullet>();
                bulletScript.ShootPlayer();
            }
            else if (curLevel == 2)
            {
                Instantiate(fabBullet2, shootTrs.position, Quaternion.identity, dynamicObject);
            }
            else if (curLevel == 3)
            {
                Instantiate(fabBullet3, shootTrs.position, Quaternion.identity, dynamicObject);
            }
            else if (curLevel == 4)
            {
                Instantiate(fabBullet4, shootTrs.position, Quaternion.identity, dynamicObject);
            }
            else if (curLevel == 5)
            {
                Instantiate(fabBullet5, shootTrs.position, Quaternion.identity, dynamicObject);
            }
        }
    }

    private void shoot()
    {
        if (autoFire == false && Input.GetKeyDown(KeyCode.Space) == true)//유저가 스페이스 키를 누른다면
        {
            CheckLevelShoot();
        }
        else if (autoFire == true)
        {
            //일정시간이 지나면 총알 한발 발사
            fireTimer += Time.deltaTime;//1초가 지나면 1이 될수있도록 소수점들이 fireTimer에 쌓임
            if(fireTimer > fireRateTime) 
            {
                CheckLevelShoot();
                fireTimer = 0;
            }
        }
    }




    /// <summary>
    /// 플레이어가 데미지를 입었을떄 호출
    /// </summary>
    public void PlayerGotHit()
    {
        if (invincibilty == true) return;

        SetSpriteInvincibilty(true);

        

        curHp -= 1;
        if (curHp < 0) { curHp = 0; }
        GameManager.Instance.SetHp(maxHp, curHp);

        curLevel -= 1;

        if (curLevel < minLevel) { curLevel = minLevel; }

        if (curHp == 0)
        {
            Destroy(gameObject);

            float width = spriteRenderer.sprite.rect.width;
            gameManager.CreateExplosionEffect(transform.position, width);
            gameManager.GameOver();

        }


    }














}
