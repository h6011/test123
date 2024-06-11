using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// 플레이어의 HP가 변동이 생기면 HP게이지를 즉시 변경하고, Effect가 해당 게이지로 초당 변동하게 만들어 줍니다.
/// </summary>

public class FunctionHP : MonoBehaviour
{
    GameManager gameManager;

    [SerializeField] Image imgHp;
    [SerializeField] Image imgEffect;

    /// <summary>
    /// 몇초 동안 줄어들게 할건지
    /// </summary>
    [Range(0.1f, 10f)]
    [SerializeField] float effectTime = 1f;

    /// <summary>
    /// Hp UI 따라다닐때 Offset
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
    /// Hp UI fillamount 수정 함수
    /// </summary>
    /// <param name="_maxHp">최대 체력</param>
    /// <param name="_curHp">현재 체력</param>
    public void SetHp(float _maxHp, float _curHp)
    {
        imgHp.fillAmount = _curHp / _maxHp;
    }


    /// <summary>
    /// Hp, Effect UI 초기화 함수
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
