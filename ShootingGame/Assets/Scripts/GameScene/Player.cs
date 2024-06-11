using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Animator anim;

    #region �÷��̾� ���� ��ġ
    
    [Header("�÷��̾� ����"), SerializeField, Tooltip("�÷��̾��� �̵��ӵ�")] float moveSpeed;
    private Vector3 moveDir;

    #endregion


    [Header("�Ѿ�")]
    [SerializeField] GameObject fabBullet;//�÷��̾ �����ؼ� ����� ���� �Ѿ�
    [SerializeField] GameObject fabBullet2;//�÷��̾ �����ؼ� ����� ���� �Ѿ�
    [SerializeField] GameObject fabBullet3;//�÷��̾ �����ؼ� ����� ���� �Ѿ�
    [SerializeField] GameObject fabBullet4;//�÷��̾ �����ؼ� ����� ���� �Ѿ�
    [SerializeField] GameObject fabBullet5;//�÷��̾ �����ؼ� ����� ���� �Ѿ�
    [SerializeField] Transform dynamicObject;
    SpriteRenderer spriteRenderer;

    #region ��� ���� ��ġ

    [SerializeField] bool autoFire = false;//�ڵ����ݱ��
    [SerializeField] float fireRateTime = 0.5f;//�̽ð��� ������ �Ѿ��� �߻��
    float fireTimer = 0;

    #endregion

    GameManager gameManager;
    GameObject fabExplosion;
    Limiter limiter;

    #region ü�� ���� ��ġ

    [Header("ü��")]
    [SerializeField] int maxHp = 3;
    [SerializeField] int curHp;
    int beforeHp;

    /// <summary>
    /// ���� ����
    /// </summary>
    private bool invincibilty = false;
    /// <summary>
    /// ���� �ð�
    /// </summary>
    [SerializeField] private float invincibiltyTime = 1f;
    private float invincibiltyTimer;

    #endregion

    #region ���� ���� ��ġ

    [Header("�÷��̾� ����")]
    [SerializeField] int minLevel = 1;
    [SerializeField] int maxLevel = 5;
    [SerializeField, Range(1, 5)] int curLevel;

    #region ����� ��Ÿ��

    [Header("����� ��Ÿ�� ���� ��ġ")]
    [SerializeField, Tooltip("2���� �̻�� �Ѿ��� �߽����� ���� �������� �Ÿ�")] float distanceBullet; // 2���� �̻�� �Ѿ��� �߽����� ���� �������� �Ÿ� // �÷��̾� ���濡�� �߻�
    [SerializeField, Tooltip("4���� �̻�� �Ѿ��� ȸ���� ��")] float angleBullet; // 4���� �̻�� �Ѿ��� ȸ���� ��
    [SerializeField, Tooltip("�⺻ �Ѿ� �߻� ��ġ")] Transform shootTrs;
    [SerializeField, Tooltip("4���� �̻�� �Ѿ��� �߻�� ��ġ")] Transform shootTrsLevel4; // 4���� �̻�� �Ѿ��� �߻�� ��ġ
    [SerializeField, Tooltip("5���� �̻�� �Ѿ��� �߻�� ��ġ")] Transform shootTrsLevel5; // 5���� �̻�� �Ѿ��� �߻�� ��ġ

    #endregion

    #region �� ��Ÿ��
    /* �޸�1
     * �� �׳� ���� ��ʷ� ���� ���� ���� �������� ��������
     */
    [Header("�� ��Ÿ�� ���� ��ġ")]
    [SerializeField] float anglePerLevel = 10f;

    #endregion


    /// <summary>
    /// ������� �� ��Ÿ���� �������� ����
    /// </summary>
    
    public enum eShootStyle
    {
        Study1,
        My,
        Study2,
    }
    
    [Header("����� ��Ÿ�� ��������")]
    [SerializeField] private eShootStyle shootStyle;

    #endregion

    



    private void OnValidate() // �ν����Ϳ��� ����� ������ ����� ȣ��
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


    #region Awake ���� �ռ� �Լ� (�޸�)

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

    private void checkinvincibilty() // �����϶��� �۵��Ͽ� �����ð��� ������ ���� �ٽ� ������ Ǯ����
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
        if (_value == true) // ������ �Ȱ� ó�� ������ �ٿ� �������� �����̶� �˷���
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
    /// �浹�� ȣ��
    /// </summary>
    /// <param name="HitTransform">�浹�� Transform</param>
    private void OnHit(Transform HitTransform)
    {
        if (HitTransform.tag == Tool.GetTag(GameTags.Enemy))
        {
            PlayerGotHit();
            // ü�� ����
            /*
             * ü���� 0�� �Ǹ� ������ ����
             * ������ ��ũ���� �Ǹ� �̸� �Է��ϴ� ���
             * ���� �޴����� 1~10 �� ��ũ
             * 
             * ª�� �ð� ����
             * 
             * ������ ��ȭ �ڵ� ����
             */
        }
        else if (HitTransform.tag == Tool.GetTag(GameTags.Item))
        {
            Item itemScript = HitTransform.GetComponent<Item>();
            Destroy(itemScript.gameObject); // �� �Լ��� �� �Լ��� ��� ������ ��ġ�� �Ǹ� ���� �ش޶�� �����ϴ� ���
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
                // ü�� ȸ��
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
    /// �÷��̾� ��ü�� �⵿�� �����մϴ�. / �÷��̾ �����̰� ���ִ� �Լ�
    /// </summary>
    private void moving()
    {
        moveDir.x = Input.GetAxisRaw("Horizontal");//���� Ȥ�� ������ �Է�// -1 0 1
        moveDir.y = Input.GetAxisRaw("Vertical");//�� Ȥ�� �Ʒ� �Է� // -1 0 1

        transform.position += moveDir * moveSpeed * Time.deltaTime;
    }

    /// <summary>
    /// �ִϸ��̼ǿ� � �ִϸ��̼��� �������� �Ķ���͸� ���� �մϴ�.
    /// </summary>
    private void doAnimation()//�ϳ��� �Լ����� �ϳ��� ���
    {
        anim.SetInteger("Horizontal", (int)moveDir.x);
    }

    /// <summary>
    /// Limiter �� ������ ä���ִ� �Լ�
    /// </summary>
    private void CheckLimiter()
    {
        if (limiter == null)
        {
            limiter = gameManager._Limiter;
        }
    }

    /// <summary>
    /// �÷��̾ ������ ���� ���� �����ٸ� �ٽ� ������ ������ �ϴ� �Լ�
    /// </summary>
    private void checkPlayerPos()
    {
        CheckLimiter();
        transform.position = limiter.checkMovePosition(transform.position, false);
    }

    /// <summary>
    /// ���� ��Ÿ�Ϸ� ��� �Ҳ���, ���� Ȯ��
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
        if (autoFire == false && Input.GetKeyDown(KeyCode.Space) == true)//������ �����̽� Ű�� �����ٸ�
        {
            CheckLevelShoot();
        }
        else if (autoFire == true)
        {
            //�����ð��� ������ �Ѿ� �ѹ� �߻�
            fireTimer += Time.deltaTime;//1�ʰ� ������ 1�� �ɼ��ֵ��� �Ҽ������� fireTimer�� ����
            if(fireTimer > fireRateTime) 
            {
                CheckLevelShoot();
                fireTimer = 0;
            }
        }
    }




    /// <summary>
    /// �÷��̾ �������� �Ծ����� ȣ��
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
