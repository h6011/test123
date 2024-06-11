using System;
using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;

public class EnemyBoss : Enemy
{
    public float Hp => hp;

    Transform trsBossPosition;

    bool isMovingTrsBossPosition = false; // ������ �� ��ġ���� �̵��� �Ϸ� �ߴ���
    bool patternChange = false; // ������ �ٲٰ� �׵��� ������ ������ Ÿ�̹��� �������

    Vector3 createPos = Vector3.zero;
    float timer = 0.0f;

    bool isSwayRight = false;
    

    [Header("���� ��ġ���� ��������")]
    [SerializeField] private int pattern1Count = 10;
    [SerializeField] private float pattern1Reload = 0.5f;
    [SerializeField] private GameObject pattern1Fab;

    [Header("����")]
    [SerializeField] private int pattern2Count = 5;
    [SerializeField] private float pattern2Reload = 0.3f;
    [SerializeField] private GameObject pattern2Fab;

    [Header("���� �߻�")]
    [SerializeField] private int pattern3Count = 30;
    [SerializeField] private float pattern3Reload = 0.1f;
    [SerializeField] private GameObject pattern3Fab;

    //[System.Serializable]
    //public class cPattern
    //{
    //    [TextArea] public string explain;
    //    public int patternCount = 1;
    //    public float patternReload = 0.1f;
    //    public GameObject patternFab;
    //}

    //[SerializeField] List<cPattern> listPattern;

    private Limiter limiter;

    private int curPattern = 1;
    private int curPatternShootCount = 0;

    Animator animator;

    [Header("�߻���ġ")]
    [SerializeField] List<Transform> trsShootPos; // public ���� ���� �ϰų� �ø�������� �ʵ�� 

    
    //protected override void OnDestroy()
    //{
    //    base.OnDestroy();
    //    if (gameManager == null) return;
    //    gameManager.createItem(transform.position, Item.eItemType.PowerUp);
    //    gameManager.createItem(transform.position, Item.eItemType.HpRecovery);
    //}

    protected override void Start()
    {
        gameManager = GameManager.Instance;
        trsBossPosition = gameManager.TrsBossPosition;
        fabExplosion = gameManager.FabExplosion;
        createPos = transform.position;
        animator = GetComponent<Animator>();
    }


    protected override void moving()
    {
        if (isMovingTrsBossPosition == false) // ��ġ���� �̵�
        {
            if (timer < 1.0f)
            {
                timer += Time.deltaTime;
                //transform.position = Vector3.Lerp(createPos, trsBossPosition.position, timer);
                float posX = Mathf.SmoothStep(createPos.x, trsBossPosition.position.x, timer);
                float posY = Mathf.SmoothStep(createPos.y, trsBossPosition.position.y, timer);
                transform.position = new Vector3(posX, posY, 0);

                if (timer >= 1.0f)
                {
                    isMovingTrsBossPosition = true;
                    timer = 0.0f;
                }

            }
            return;
        }

        if (isSwayRight == true)
        {
            transform.position += Vector3.right * Time.deltaTime * moveSpeed;
        }
        else
        {
            transform.position += Vector3.left * Time.deltaTime * moveSpeed;
        }
        checkMovingLimit();



        shooting();



    }


    protected override void shooting()
    {
        if (isMovingTrsBossPosition == false)
        {
            return;
        }
        timer += Time.deltaTime;
        if (patternChange == true)
        {
            if (timer >= 1.0f)
            {
                timer = 0f;
                patternChange = false;
            }
            return;
        }

        switch (curPattern)
        {
            case 1:
                if (timer >= pattern1Reload)
                {
                    timer = 0;
                    shootStraight();
                    if (curPatternShootCount >= pattern1Count)
                    {
                        curPattern += 1;
                        patternChange = true;
                        curPatternShootCount = 0;
                        
                    }
                }
                break;
            case 2:
                if (timer >= pattern2Reload)
                {
                    timer = 0;
                    shootShotgun();
                    if (curPatternShootCount >= pattern2Count)
                    {
                        curPattern += 1;
                        patternChange = true;
                        curPatternShootCount = 0;
                    }
                }
                break;
            case 3:
                if (timer >= pattern3Reload)
                {
                    timer = 0;
                    shootgatling();
                    if (curPatternShootCount >= pattern3Count)
                    {
                        curPattern = 1;
                        patternChange = true;
                        curPatternShootCount = 0;
                    }
                }
                break;
            default:
                break;
        }

        //if (curPattern == 1)
        //{

        //}
        //else if (curPattern == 2)
        //{

        //}
        //else if (curPattern == 3)
        //{

        //}


    }

    private void shootgatling()
    {
        Vector3 playerPos;
        if (gameManager.GetPlayerPosition(out playerPos) == true)
        {
            Vector3 distance = playerPos - transform.position;

            float angle = Quaternion.FromToRotation(Vector3.up, distance).eulerAngles.z;

            gameManager.CreateBullet(pattern3Fab, trsShootPos[4].position, Quaternion.Euler(0, 0, angle), false);

        }
        curPatternShootCount++;
    }

    private void shootShotgun()
    {
        int bulletCount = 9;
        int anglePerLevel = 15;

        //gameManager.CreateBullet(pattern2Fab, trsShootPos[4].position, Quaternion.Euler(0, 0, 180), false);

        if (bulletCount % 2 == 0) // 2 4
        {
            for (int iNum = 1; iNum <= bulletCount / 2; iNum++)
            {
                gameManager.CreateBullet(pattern2Fab, trsShootPos[4].position, Quaternion.Euler(0, 0, 180 + (anglePerLevel / 2 * iNum * 1)), false);
                gameManager.CreateBullet(pattern2Fab, trsShootPos[4].position, Quaternion.Euler(0, 0, 180 + (anglePerLevel / 2 * iNum * -1)), false);
            }
        }
        else // 1  3  5
        {
            int savecurLevel = bulletCount - 1;
            gameManager.CreateBullet(pattern2Fab, trsShootPos[4].position, Quaternion.Euler(0, 0, 180), false);
            for (int iNum = 1; iNum <= savecurLevel / 2; iNum++)
            {
                gameManager.CreateBullet(pattern2Fab, trsShootPos[4].position, Quaternion.Euler(0, 0, 180 + (anglePerLevel / 2 * iNum * 1)), false);
                gameManager.CreateBullet(pattern2Fab, trsShootPos[4].position, Quaternion.Euler(0, 0, 180 + (anglePerLevel / 2 * iNum * -1)), false);
            }
        }
        curPatternShootCount++;
    }

    private void shootStraight()
    {
        int count = 4;
        for (int iNum = 0; iNum < count; iNum++)
        {
            gameManager.CreateBullet(pattern1Fab, trsShootPos[iNum].position, Quaternion.Euler(0, 0, 180), false);
        }
        curPatternShootCount++;
    }











    private void checkLimiter()
    {
        if (limiter == null)
        {
            limiter = gameManager._Limiter;
        }
    }


    private void checkMovingLimit()
    {
        checkLimiter();
        float posX = transform.position.x;
        float width = spriteRenderer.sprite.rect.width * 0.5f;

        if (limiter.checkMovePositionForBoss(transform.position, isSwayRight) == true)
        {
            isSwayRight = !isSwayRight;
        }


    }



    public override void Hit(float _damage)
    {
        if (isDied) { return; } // �׾����� ���� ����
        hp -= _damage; // ������ ó��
        gameManager.modifyBossHp(hp);

        if (hp <= 0) // ü���� 0 ���϶�� (�׾��ٸ�)
        {
            isDied = true;
            Destroy(gameObject);
            //�Ŵ����κ��� �޾ƿ� ���� ������ �� ��ġ�� �����ϰ� �θ�� ������� ���̾ �������
            float width = spriteRenderer.sprite.rect.width;
            gameManager.CreateExplosionEffect(transform.position, width);

            gameManager.createItem(transform.position, Item.eItemType.PowerUp);
            gameManager.createItem(transform.position, Item.eItemType.HpRecovery);

            gameManager.KillBoss();

            //gameManager.AddKillCount(); // ������ �׾��ٰ� ���� // �ٽ� ������ �⵿�ϵ��� ����

        }
        else // �ǰ� �پ�������
        {
            // �� ģ���� ��������Ʈ�� Ȱ�� �ϴ°��� �ƴ϶� ��������Ʈ �ִϸ��̼��� Ȱ�������� �ִϸ��̼ǿ��� ��Ʈ �ִ��� ����
            animator.SetTrigger("bossHit");
        }
    }


}
