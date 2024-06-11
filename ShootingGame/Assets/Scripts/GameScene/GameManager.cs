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
    public static GameManager Instance;//null ä�������, Singleton


    [Header("�����")]
    [SerializeField] List<GameObject> listEnemy;

    GameObject fabExplosion;//���� �����͸� ������ �ִ� ������ private�� �����ϰ�
    [SerializeField] GameObject fabBoss;
    [SerializeField] GameObject fabBullet;

    #region �� ���� ����

    [Header("�� ���� ����")]
    [SerializeField] bool isSpawn = false;//������ �����ϰų� ���ϴ� ������ ������ �̰���
    [SerializeField] Color sliderDefaultColor;
    [SerializeField] Color sliderBossSpawnColor;
    WaitForSeconds halfTime = new WaitForSeconds(0.5f);

    

    /// <summary>
    /// �� ������ �ϱ����� Ÿ�̸�
    /// </summary>
    [Header("�� ���� �ð�")]
    float enemySpawnTimer = 0.0f;//0�ʿ��� ���۵Ǵ� Ÿ�̸�

    /// <summary>
    /// �� ���� ������
    /// </summary>
    [SerializeField, Range(0.1f,5f), Tooltip("�� ���� ������")] float spawnTime = 1.0f;

    [Header("�� ���� ��ġ")]
    [SerializeField] Transform trsSpawnPosition;
    [SerializeField] Transform trsDynamicObject;

    #endregion

    #region ������, ��� ����

    [Header("��Ӿ�����")]
    [SerializeField] List<GameObject> listItem;

    [Header("��� Ȯ��")]
    [SerializeField, Range(0f, 100f)] float itemDropRate;

    #endregion

    #region ü�� ����

    [Header("ü�� ������")]
    [SerializeField] FunctionHP functionHP;
    [SerializeField] Slider slider;
    [SerializeField] TMP_Text sliderText;
    [SerializeField] Image imgSliderFill;

    #endregion

    #region get set ����
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

    public GameObject FabExplosion//������ ���� Ȥ�� �����;��Ҷ��� �Լ��μ� ��밡��
    {
        get
        {
            return fabExplosion;
        }
        //set { fabExplosion = value; }
    }

    #endregion

    #region ���� ����

    private bool isSpawnBoss = false;
    bool IsSpawnBoss
    {
        set 
        { 
            isSpawnBoss = value;
            StartCoroutine(sliderColorChange(isSpawnBoss));
        }
    }

    [Header("���� ���� ����")]
    [SerializeField] int killCount = 100;
    [SerializeField] int curKillCount = 0;

    [Header("���� ������")]
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
        // yield return new WaitForSeconds(0.5f) // new Ram �޸� �Ҵ� ����
        //yield return halfTime; // yield �纸�ϴ�
    }

    #endregion


    [Header("����")]
    [SerializeField] TMP_Text textScore;
    private int score;

    private bool gameStart = false;

    [Header("��ŸƮ�ؽ�Ʈ")]
    [SerializeField] TMP_Text textStart;

    [Header("���� ���� �޴�")]
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

    //�ν������� ���� ������ ������ ���Լ��� ���� ȣ��
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
        //1���� �����ؾ���
        if (Instance == null)
        {
            Instance = this;
        }
        else//�ν��Ͻ��� �̹� �����Ѵٸ� ���� ����������
        {
            //Destroy(this);//�̷��� ������Ʈ�� ������
            Destroy(gameObject);//������Ʈ�� �������鼭 ��ũ��Ʈ�� ���� ������
        }

        fabExplosion = Resources.Load<GameObject>("Effect/Test/fabExplosion");
        initSlider();
    }

    private void initSlider()
    {
        // ųī��Ʈ ����
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



    void Update()//�����Ӵ� �ѹ� ����Ǵ� �Լ�
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
            //���� ����
            int count = listEnemy.Count; //���� ���� 0 ~ 2
            int iRand = Random.Range(0, count);//0, 3

            Vector3 defulatPos = trsSpawnPosition.position;//y => 7 
            float x = Random.Range(limiter.WorldPosLimitMin.x, limiter.WorldPosLimitMax.x);//x => -2.4 ~ 2.4
            defulatPos.x = x;

            GameObject enemyObj = Instantiate(listEnemy[iRand], defulatPos, Quaternion.identity, trsDynamicObject);
            //������ ��ġ, ���̳��� ������Ʈ ��ġ�� �ʿ�

            // �ֻ����� ����

            float rate = Random.Range(0.0f, 100.0f);
            if (rate <= itemDropRate)
            {
                // ���Ⱑ �������� ������ ����
                Enemy enemyScript = enemyObj.GetComponent<Enemy>();
                enemyScript.SetItem();

            }



            enemySpawnTimer = 0.0f;
        }
    }

    /// <summary>
    /// ������ ���� �Լ�
    /// </summary>
    /// <param name="_pos">������ ��ġ Vector3</param>
    public void createItem(Vector3 _pos)
    {
        int count = listItem.Count;
        int iRand = Random.Range(0, count);
        GameObject NewItem = Instantiate(listItem[iRand], _pos, Quaternion.identity, trsDynamicObject);
        Item NewItemScript = NewItem.GetComponent<Item>();

    }

    public void createItem(Vector3 _pos, Item.eItemType _type) // 0�� ����, 1�� �Ŀ���, 2�� ü�� ȸ��
    {
        if (_type == Item.eItemType.None) { return; }
        GameObject NewItem = Instantiate(listItem[(int)_type - 1], _pos, Quaternion.identity, trsDynamicObject);



    }


    /// <summary>
    /// functionHP.SetHP() ȣ�� �Լ�
    /// </summary>
    /// <param name="_maxHp">�ִ� ü��</param>
    /// <param name="_curHp">���� ü��</param>
    public void SetHp(float _maxHp, float _curHp)
    {
        // FunctionHP ���� �˷������
        functionHP.SetHp(_maxHp, _curHp);
    }

    /// <summary>
    /// ųī��Ʈ �߰� �Լ�!
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
        // ų ī��Ʈ ����
        //slider.value = curKillCount;
        //sliderText.text = $"{curKillCount.ToString("d4")} / {killCount.ToString("d4")}";
        slider.value = bossSpawnTimer;
        sliderText.text = $"{((int)bossSpawnTimer).ToString("d4")} / {((int)bossSpawnTime).ToString("d4")}";
    }

    private void checkSpawnBoss()
    {
        // ųī��Ʈ ����
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

        textGameOverMenuScore.text = $"���� : {score.ToString("d8")}";

        // �÷��̾ ��ũ�� ������� Ȯ��, ������� ������ �ʿ�
        if (rank != -1)
        {
            textGameOverMenuRank.text = $"��ŷ : {rank + 1}��";
            IFGameOverMenuRank.gameObject.SetActive(true);
            textGameOverMenuBtn.text = "���";
        }
        else // ��ũ �ȿ� ���� ���ߴٸ� �̸��� ���� �ʿ䰡 ����
        {
            textGameOverMenuRank.text = $"��ũ�� ���� ���߽��ϴ�";
            IFGameOverMenuRank.gameObject.SetActive(false);
            textGameOverMenuBtn.text = "���� �޴���";
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
    /// ���� ����Ʈ ���� �Լ�
    /// </summary>
    /// <param name="Position">World ��ġ Vector3</param>
    /// <param name="width">����Ʈ ����� ���� width (float)</param>
    public void CreateExplosionEffect(Vector3 Position, float width = 1f)
    {
        GameObject ExplosionObject = Instantiate(fabExplosion, Position, Quaternion.identity, trsDynamicObject);
        Explosion ExplosionScript = ExplosionObject.GetComponent<Explosion>();
        ExplosionScript.setImageSize(width);//���� ��ü�� �̹��� ���̸� �־���
    }

    /// <summary>
    /// �Ѿ� ���� �Լ�
    /// </summary>
    /// <param name="position">������ ��ġ (World)</param>
    /// <param name="quaternion">������ ����</param>
    /// <param name="isOwnerPlayer">�÷��̾ �� �Ѿ�����</param>
    public void CreateBullet(Vector3 position, Quaternion quaternion, bool isOwnerPlayer = true)//�Ѿ��� �����Ѵ�
    {
        GameObject NewBullet = Instantiate(fabBullet, position, quaternion, trsDynamicObject);
        Bullet NewBulletScript = NewBullet.GetComponent<Bullet>();
        if (isOwnerPlayer) { NewBulletScript.ShootPlayer(); }
    }

    public void CreateBullet(GameObject fab, Vector3 position, Quaternion quaternion, bool isOwnerPlayer = true)//�Ѿ��� �����Ѵ�
    {
        GameObject NewBullet = Instantiate(fab, position, quaternion, trsDynamicObject);
        Bullet NewBulletScript = NewBullet.GetComponent<Bullet>();
        if (isOwnerPlayer) { NewBulletScript.ShootPlayer(); }
    }


}
