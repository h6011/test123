using System;
using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;

public class EnemyBoss : Enemy
{
    public float Hp => hp;

    Transform trsBossPosition;

    bool isMovingTrsBossPosition = false; // 보스가 원 위치까지 이동을 완료 했는지
    bool patternChange = false; // 패턴을 바꾸고 그동안 유저가 공격할 타이밍을 만들어줌

    Vector3 createPos = Vector3.zero;
    float timer = 0.0f;

    bool isSwayRight = false;
    

    [Header("현재 위치에서 전방으로")]
    [SerializeField] private int pattern1Count = 10;
    [SerializeField] private float pattern1Reload = 0.5f;
    [SerializeField] private GameObject pattern1Fab;

    [Header("샷건")]
    [SerializeField] private int pattern2Count = 5;
    [SerializeField] private float pattern2Reload = 0.3f;
    [SerializeField] private GameObject pattern2Fab;

    [Header("조준 발사")]
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

    [Header("발사위치")]
    [SerializeField] List<Transform> trsShootPos; // public 으로 선언 하거나 시리얼라이즈 필드로 

    
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
        if (isMovingTrsBossPosition == false) // 위치까지 이동
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
        if (isDied) { return; } // 죽었을때 실행 무시
        hp -= _damage; // 데미지 처리
        gameManager.modifyBossHp(hp);

        if (hp <= 0) // 체력이 0 이하라면 (죽었다면)
        {
            isDied = true;
            Destroy(gameObject);
            //매니저로부터 받아온 폭발 연출을 내 위치에 생성하고 부모로 사용중인 레이어에 만들어줌
            float width = spriteRenderer.sprite.rect.width;
            gameManager.CreateExplosionEffect(transform.position, width);

            gameManager.createItem(transform.position, Item.eItemType.PowerUp);
            gameManager.createItem(transform.position, Item.eItemType.HpRecovery);

            gameManager.KillBoss();

            //gameManager.AddKillCount(); // 보스가 죽었다고 전달 // 다시 적들이 출동하도록 설계

        }
        else // 피가 줄어들었을때
        {
            // 이 친구는 스프라이트만 활용 하는것이 아니라 스프라이트 애니매이션을 활용함으로 애니매이션에서 히트 애님을 실행
            animator.SetTrigger("bossHit");
        }
    }


}
