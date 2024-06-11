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
    /// �� �̵��ӵ�
    /// </summary>
    [SerializeField] protected float moveSpeed;
    /// <summary>
    /// �� ü��
    /// </summary>
    [SerializeField] protected float hp;

    #endregion

    #region Sprite Vars

    protected Sprite defaultSprite;
    [SerializeField] protected Sprite hitSprite;
    protected SpriteRenderer spriteRenderer;

    #endregion

    /// <summary>
    /// Explosion Prefab ���� ������
    /// </summary>
    protected GameObject fabExplosion;
    protected GameManager gameManager;

    #region ������ ����

    /// <summary>
    /// ������ �������� �������
    /// </summary>
    private bool haveItem = false;
    /// <summary>
    /// �׾����� ����
    /// </summary>
    protected bool isDied = false; // ���Ⱑ �װ��� ���̻� ����� 
    
    /// <summary>
    /// ������ ������ sprite ����
    /// </summary>
    [Header("������ ������ �÷�")]
    [SerializeField] protected Color colorHaveItem;


    [Header("�ı��� ����")]
    [SerializeField] protected int score; // �ڽ��� �ı� �Ǿ����� ����

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
    /// ���� Start ������ �ൿ �Լ�
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
    /// �� �̵� �Լ�
    /// </summary>
    protected virtual void moving()
    {
        transform.position -= transform.up * moveSpeed * Time.deltaTime;
    }

    

    protected virtual void shooting()
    {

    }

    /// <summary>
    /// ������ �������� �ֱ� ���� ���� �Լ�
    /// </summary>
    /// <param name="_damage">������ ��</param>
    public virtual void Hit(float _damage)
    {
        if (isDied) { return; } // �׾����� ���� ����
        hp -= _damage; // ������ ó��

        if (hp <= 0) // ü���� 0 ���϶�� (�׾��ٸ�)
        {
            isDied = true;
            Destroy(gameObject); // ������ ����
            //�Ŵ����κ��� �޾ƿ� ���� ������ �� ��ġ�� �����ϰ� �θ�� ������� ���̾ �������
            float width = spriteRenderer.sprite.rect.width;
            gameManager.CreateExplosionEffect(transform.position, width);
            

            // �Ŵ����� ȣ���� ���� ����ġ�� �����ϸ� �Ŵ����� �������� �� ��ġ�� �������
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
        else // �ǰ� �پ�������
        {
            //hit ���� ��������Ʈ ������
            spriteRenderer.sprite = hitSprite;
            //�ణ�� �ð��� �����ڿ� � �Լ��� �����ϰ� ������
            Invoke("setDefaultSprite", 0.04f);
        }
    }

    /// <summary>
    /// �⺻ ��������Ʈ�� ����
    /// </summary>
    private void setDefaultSprite()
    {
        spriteRenderer.sprite = defaultSprite;
    }

    /// <summary>
    /// �ڽ��� �������� ����ؾ� �Ѵٰ� ��ȣ �ִ¿�, haveItem �� true �� ������
    /// </summary>
    public void SetItem()
    {
        haveItem = true;
    }


}
