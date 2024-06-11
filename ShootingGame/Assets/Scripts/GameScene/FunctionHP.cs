using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// �÷��̾��� HP�� ������ ����� HP�������� ��� �����ϰ�, Effect�� �ش� �������� �ʴ� �����ϰ� ����� �ݴϴ�.
/// </summary>

public class FunctionHP : MonoBehaviour
{
    GameManager gameManager;

    [SerializeField] Image imgHp;
    [SerializeField] Image imgEffect;

    /// <summary>
    /// ���� ���� �پ��� �Ұ���
    /// </summary>
    [Range(0.1f, 10f)]
    [SerializeField] float effectTime = 1f;

    /// <summary>
    /// Hp UI ����ٴҶ� Offset
    /// </summary>
    [SerializeField] Vector3 chaseOffset = new Vector3(0, -0.7f, 0);

    private void Awake()
    {
        initHp();
    }

    private void Start()
    {
        gameManager = GameManager.Instance;
    }

    private void Update()
    {
        checkFillAmount();
        chasePlayer();
        checkPlayerDestroy();
    }

    private void checkFillAmount()
    {
        float HpFillAmount = imgHp.fillAmount;
        float EffectFillAmount = imgEffect.fillAmount;

        if (HpFillAmount == EffectFillAmount) { return; };

        if (HpFillAmount < EffectFillAmount)
        {
            imgEffect.fillAmount -= (Time.deltaTime / effectTime);
            if (HpFillAmount > EffectFillAmount)
            {
                imgEffect.fillAmount = imgHp.fillAmount;
            }
        }
        else if (HpFillAmount > EffectFillAmount)
        {
            imgEffect.fillAmount = imgHp.fillAmount;
        }
    }

    private void chasePlayer()
    {
        if (gameManager.GetPlayerPosition(out Vector3 pos))
        {
            transform.position = pos + chaseOffset;
        }
    }


    /// <summary>
    /// Hp UI fillamount ���� �Լ�
    /// </summary>
    /// <param name="_maxHp">�ִ� ü��</param>
    /// <param name="_curHp">���� ü��</param>
    public void SetHp(float _maxHp, float _curHp)
    {
        imgHp.fillAmount = _curHp / _maxHp;
    }


    /// <summary>
    /// Hp, Effect UI �ʱ�ȭ �Լ�
    /// </summary>
    private void initHp()
    {
        imgHp.fillAmount = 1;
        imgEffect.fillAmount = 1;
    }

    private void checkPlayerDestroy()
    {
        //if (isEnded == false && gameManager._Player == null)
        //{
        //    isEnded = true;

        //    Destroy(gameObject, 0.5f);

        //}

        if (imgEffect.fillAmount == 0.0f)
        {
            Destroy(gameObject);
        }
    }

}
