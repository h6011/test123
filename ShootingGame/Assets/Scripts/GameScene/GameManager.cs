using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;//null 채워줘야함, Singleton


    [Header("적기들")]
    [SerializeField] List<GameObject> listEnemy;

    GameObject fabExplosion;//실제 데이터를 가지고 있는 변수는 private를 유지하고
    [SerializeField] GameObject fabBoss;
    [SerializeField] GameObject fabBullet;

    #region 적 생성 관련

    [Header("적 생성 여부")]
    [SerializeField] bool isSpawn = false;//보스가 등장하거나 원하는 사유가 있을때 이값을
    [SerializeField] Color sliderDefaultColor;
    [SerializeField] Color sliderBossSpawnColor;
    WaitForSeconds halfTime = new WaitForSeconds(0.5f);

    

    /// <summary>
    /// 적 스폰을 하기위한 타이머
    /// </summary>
    [Header("적 생성 시간")]
    float enemySpawnTimer = 0.0f;//0초에서 시작되는 타이머

    /// <summary>
    /// 적 스폰 딜레이
    /// </summary>
    [SerializeField, Range(0.1f,5f), Tooltip("적 스폰 딜레이")] float spawnTime = 1.0f;

    [Header("적 생성 위치")]
    [SerializeField] Transform trsSpawnPosition;
    [SerializeField] Transform trsDynamicObject;

    #endregion

    #region 아이템, 드롭 관련

    [Header("드롭아이템")]
    [SerializeField] List<GameObject> listItem;

    [Header("드롭 확률")]
    [SerializeField, Range(0f, 100f)] float itemDropRate;

    #endregion

    #region 체력 관련

    [Header("체력 게이지")]
    [SerializeField] FunctionHP functionHP;
    [SerializeField] Slider slider;
    [SerializeField] TMP_Text sliderText;
    [SerializeField] Image imgSliderFill;

    #endregion

    #region get set 관련
    Limiter limiter;
    public Limiter _Limiter
    {
        get { return limiter; }
        set { limiter = value; }
    }

    Player player;
    public Player _Player
    {
        get { return player; }
        set { player = value; }
    }

    public GameObject FabExplosion//정보를 전달 혹은 가져와야할때만 함수로서 사용가능
    {
        get
        {
            return fabExplosion;
        }
        //set { fabExplosion = value; }
    }

    #endregion

    #region 보스 관련

    private bool isSpawnBoss = false;
    bool IsSpawnBoss
    {
        set 
        { 
            isSpawnBoss = value;
            StartCoroutine(sliderColorChange(isSpawnBoss));
        }
    }

    [Header("보스 출현 조건")]
    [SerializeField] int killCount = 100;
    [SerializeField] int curKillCount = 0;

    [Header("보스 포지션")]
    [SerializeField] Transform trsBossPosition;
    public Transform TrsBossPosition => trsBossPosition; // get

    [SerializeField] float bossSpawnTime = 60f;
    [SerializeField] float bossSpawnTimer = 0f;

    IEnumerator sliderColorChange(bool _spawnBoss)
    {
        float timer = 0.0f;

        while (timer <= 1.0f)
        {
            timer += Time.deltaTime;
            if (timer > 1.0f)
            {
                timer = 1.0f;
            }

            if (_spawnBoss)
            {
                imgSliderFill.color = Color.Lerp(sliderDefaultColor, sliderBossSpawnColor, timer);
            }
            else
            {
                imgSliderFill.color = Color.Lerp(sliderBossSpawnColor, sliderDefaultColor, timer);
            }
            yield return null;
        }
        // yield return new WaitForSeconds(0.5f) // new Ram 메모리 할당 받음
        //yield return halfTime; // yield 양보하다
    }

    #endregion


    [Header("점수")]
    [SerializeField] TMP_Text textScore;
    private int score;

    private bool gameStart = false;

    [Header("스타트텍스트")]
    [SerializeField] TMP_Text textStart;

    [Header("게임 오버 메뉴")]
    [SerializeField] GameObject objGameOverMenu;
    [SerializeField] TMP_Text textGameOverMenuScore;
    [SerializeField] TMP_Text textGameOverMenuRank;
    [SerializeField] TMP_Text textGameOverMenuBtn;
    [SerializeField] TMP_InputField IFGameOverMenuRank;
    [SerializeField] Button btnGameOverMenu;

    public bool GetPlayerPosition(out Vector3 _pos)
    {
        _pos = default;
        if (player == null)
        {
            return false;
        }
        else
        {
            _pos = player.transform.position;
            return true;
        }
    }

    //인스펙터의 값이 변동이 있을때 이함수가 강제 호출
    //private void OnValidate()
    //{
    //    if (Application.isPlaying == false) return;

    //    if (spawnTime < 0.1f)
    //    {
    //        spawnTime = 0.1f;
    //    }
    //}

    private void Awake()
    {
        if (Tool.isStartingMainScene == false)
        {
            SceneManager.LoadScene(0);
        }
        //1개만 존재해야함
        if (Instance == null)
        {
            Instance = this;
        }
        else//인스턴스가 이미 존재한다면 나는 지워져야함
        {
            //Destroy(this);//이러면 컴포넌트만 삭제됨
            Destroy(gameObject);//오브젝트가 지워지면서 스크립트도 같이 지워짐
        }

        fabExplosion = Resources.Load<GameObject>("Effect/Test/fabExplosion");
        initSlider();
    }

    private void initSlider()
    {
        // 킬카운트 버전
        //slider.minValue = 0;
        //slider.maxValue = killCount;
        //slider.value = 0;
        //sliderText.text = $"{curKillCount.ToString("d4")} / {killCount.ToString("d4")}";
        slider.minValue = 0;
        slider.maxValue = bossSpawnTime;

        modifySlider();
    }

    private void Start()
    {
        StartCoroutine(doStartText());
    }

    IEnumerator doStartText()
    {
        Color color = textStart.color;
        color.a = 0f;

        textStart.color = color;

        while (true)
        {
            color = textStart.color;
            color.a += Time.deltaTime;

            if (color.a > 1.0f)
            {
                color.a = 1.0f;
            }

            textStart.color = color;

            if (color.a == 1.0f)
            {
                break;
            }

            yield return null;
        }


        while (true)
        {
            color = textStart.color;
            color.a -= Time.deltaTime;

            if (color.a < 0.0f)
            {
                color.a = 0.0f;
            }

            textStart.color = color;

            if (color.a == 0.0f)
            {
                break;
            }

            yield return null;
        }

        Destroy(textStart.gameObject);

        gameStart = true;
        isSpawn = true;


    }



    void Update()//프레임당 한번 실행되는 함수
    {
        if (gameStart == false) { return; }
        createEnemy();
        checkTimer();
    }

    private void checkTimer()
    {
        if (isSpawnBoss == false)
        {
            bossSpawnTimer += Time.deltaTime;
            modifySlider();
            if (bossSpawnTimer >= bossSpawnTime)
            {
                checkSpawnBoss();
            }
        }
    }

    //public void 

    private void createEnemy()
    {
        if (isSpawn == false) return;

        enemySpawnTimer += Time.deltaTime;
        if (enemySpawnTimer > spawnTime)
        {
            //적을 생성
            int count = listEnemy.Count; //개의 적기 0 ~ 2
            int iRand = Random.Range(0, count);//0, 3

            Vector3 defulatPos = trsSpawnPosition.position;//y => 7 
            float x = Random.Range(limiter.WorldPosLimitMin.x, limiter.WorldPosLimitMax.x);//x => -2.4 ~ 2.4
            defulatPos.x = x;

            GameObject enemyObj = Instantiate(listEnemy[iRand], defulatPos, Quaternion.identity, trsDynamicObject);
            //생성할 위치, 다이나믹 오브젝트 위치가 필요

            // 주사위를 굴림

            float rate = Random.Range(0.0f, 100.0f);
            if (rate <= itemDropRate)
            {
                // 적기가 아이템을 가지고 있음
                Enemy enemyScript = enemyObj.GetComponent<Enemy>();
                enemyScript.SetItem();

            }



            enemySpawnTimer = 0.0f;
        }
    }

    /// <summary>
    /// 아이템 생성 함수
    /// </summary>
    /// <param name="_pos">생성될 위치 Vector3</param>
    public void createItem(Vector3 _pos)
    {
        int count = listItem.Count;
        int iRand = Random.Range(0, count);
        GameObject NewItem = Instantiate(listItem[iRand], _pos, Quaternion.identity, trsDynamicObject);
        Item NewItemScript = NewItem.GetComponent<Item>();

    }

    public void createItem(Vector3 _pos, Item.eItemType _type) // 0은 없음, 1은 파워업, 2는 체력 회복
    {
        if (_type == Item.eItemType.None) { return; }
        GameObject NewItem = Instantiate(listItem[(int)_type - 1], _pos, Quaternion.identity, trsDynamicObject);



    }


    /// <summary>
    /// functionHP.SetHP() 호출 함수
    /// </summary>
    /// <param name="_maxHp">최대 체력</param>
    /// <param name="_curHp">현재 체력</param>
    public void SetHp(float _maxHp, float _curHp)
    {
        // FunctionHP 에게 알려줘야함
        functionHP.SetHp(_maxHp, _curHp);
    }

    /// <summary>
    /// 킬카운트 추가 함수!
    /// </summary>
    public void AddKillCount()
    {
        curKillCount += 1;
        //modifySlider();
        //checkSpawnBoss();
    }

    public void AddScore(int _value)
    {
        score += _value;
        textScore.text = $"{score.ToString("d8")}";
    }

    private void modifySlider()
    {
        // 킬 카운트 버전
        //slider.value = curKillCount;
        //sliderText.text = $"{curKillCount.ToString("d4")} / {killCount.ToString("d4")}";
        slider.value = bossSpawnTimer;
        sliderText.text = $"{((int)bossSpawnTimer).ToString("d4")} / {((int)bossSpawnTime).ToString("d4")}";
    }

    private void checkSpawnBoss()
    {
        // 킬카운트 버전
        //if (isSpawnBoss == false && curKillCount >= killCount)
        //{
        //    isSpawn = false;
        //    isSpawnBoss = true;

        //    curKillCount = 0;

        //    Instantiate(fabBoss, trsSpawnPosition.position, Quaternion.identity, trsDynamicObject);

        //}
        if (isSpawnBoss == false && bossSpawnTimer >= bossSpawnTime)
        {
            isSpawn = false;
            IsSpawnBoss = true;

            curKillCount = 0;

            GameObject BossObject = Instantiate(fabBoss, trsSpawnPosition.position, Quaternion.identity, trsDynamicObject);
            EnemyBoss BossScript = BossObject.GetComponent<EnemyBoss>();
            setSliderBossType(BossScript.Hp);


        }
    }

    private void setSliderBossType(float _maxHp)
    {
        slider.maxValue = _maxHp;
        slider.value = _maxHp;
        sliderText.text = $"{(int)_maxHp} / {(int)_maxHp}";
    }

    public void modifyBossHp(float _hp)
    {
        slider.value = _hp;
        sliderText.text = $"{(int)_hp} / {(int)slider.maxValue}";
    }

    public void KillBoss()
    {
        bossSpawnTimer = 0f;
        bossSpawnTime += 10;
        isSpawn = true;
        initSlider();
        IsSpawnBoss = false;
    }







    public void GameOver()
    {

        List<cUserData> listUserData = JsonConvert.DeserializeObject<List<cUserData>>(PlayerPrefs.GetString(Tool.rankKey));

        int rank = -1;
        int count = listUserData.Count;
        for (int iNum = 0; iNum < count; iNum++)
        {
            cUserData userData = listUserData[iNum];
            if (userData.Score < score)
            {
                rank = iNum;

                break;
            }
        }

        textGameOverMenuScore.text = $"점수 : {score.ToString("d8")}";

        // 플레이어가 랭크에 들었는지 확인, 몇등인지 데이터 필요
        if (rank != -1)
        {
            textGameOverMenuRank.text = $"랭킹 : {rank + 1}등";
            IFGameOverMenuRank.gameObject.SetActive(true);
            textGameOverMenuBtn.text = "등록";
        }
        else // 랭크 안에 들지 못했다면 이름을 적을 필요가 없음
        {
            textGameOverMenuRank.text = $"랭크인 하지 못했습니다";
            IFGameOverMenuRank.gameObject.SetActive(false);
            textGameOverMenuBtn.text = "메인 메뉴로";
        }

        

        btnGameOverMenu.onClick.AddListener(() => 
        {
            if (rank == -1)
            {
                string name = IFGameOverMenuRank.text;

                if (name == string.Empty)
                {
                    name = "-empty-";
                }

                cUserData newRank = new cUserData();
                newRank.Score = score;
                newRank.Name = name;

                listUserData.Insert(rank, newRank);
                listUserData.RemoveAt(listUserData.Count - 1);

                string value = JsonConvert.SerializeObject(listUserData);

                PlayerPrefs.SetString(Tool.rankKey, value);
            }

            FunctionFade.Instance.ActiveFade(true, () =>
            {
                SceneManager.LoadScene(0);
                FunctionFade.Instance.ActiveFade(false);
            });
        });

        objGameOverMenu.SetActive(true);
    }






    /// <summary>
    /// 폭발 이펙트 생성 함수
    /// </summary>
    /// <param name="Position">World 위치 Vector3</param>
    /// <param name="width">이펙트 사이즈를 위한 width (float)</param>
    public void CreateExplosionEffect(Vector3 Position, float width = 1f)
    {
        GameObject ExplosionObject = Instantiate(fabExplosion, Position, Quaternion.identity, trsDynamicObject);
        Explosion ExplosionScript = ExplosionObject.GetComponent<Explosion>();
        ExplosionScript.setImageSize(width);//현재 기체의 이미지 길이를 넣어줌
    }

    /// <summary>
    /// 총알 생성 함수
    /// </summary>
    /// <param name="position">생성될 위치 (World)</param>
    /// <param name="quaternion">생성될 각도</param>
    /// <param name="isOwnerPlayer">플레이어가 쏜 총알인지</param>
    public void CreateBullet(Vector3 position, Quaternion quaternion, bool isOwnerPlayer = true)//총알을 생성한다
    {
        GameObject NewBullet = Instantiate(fabBullet, position, quaternion, trsDynamicObject);
        Bullet NewBulletScript = NewBullet.GetComponent<Bullet>();
        if (isOwnerPlayer) { NewBulletScript.ShootPlayer(); }
    }

    public void CreateBullet(GameObject fab, Vector3 position, Quaternion quaternion, bool isOwnerPlayer = true)//총알을 생성한다
    {
        GameObject NewBullet = Instantiate(fab, position, quaternion, trsDynamicObject);
        Bullet NewBulletScript = NewBullet.GetComponent<Bullet>();
        if (isOwnerPlayer) { NewBulletScript.ShootPlayer(); }
    }


}
