using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundManager : MonoBehaviour
{
    #region Materials
    Material matTop;
    Material matMid;
    Material matBot;
    #endregion

    #region ��� ���ǵ�

    [SerializeField] private float speedTop;
    [SerializeField] private float speedMid;
    [SerializeField] private float speedBot;

    #endregion

    private GameObject objBackGround;

    void Start()
    {
        objBackGround = transform.Find("BackGround").gameObject;

        Transform trsTop = objBackGround.transform.Find("Top");
        Transform trsMid = objBackGround.transform.Find("Mid");
        Transform trsBot = objBackGround.transform.Find("Bot");

        SpriteRenderer sprTopRenderer = trsTop.GetComponent<SpriteRenderer>();
        SpriteRenderer sprMidRenderer = trsMid.GetComponent<SpriteRenderer>();
        SpriteRenderer sprBotRenderer = trsBot.GetComponent<SpriteRenderer>();

        matTop = sprTopRenderer.material;
        matMid = sprMidRenderer.material;
        matBot = sprBotRenderer.material;
    }

    void Update()//����� ���� ��ǻ�� 1�ʿ� 700��, ����� ������ ��ǻ�� 1�ʿ� 30��
    {
        MoveBackground();
    }


    /// <summary>
    /// Background �� �����̰� �ϴ� �Լ�
    /// </summary>
    private void MoveBackground()
    {
        Vector2 vecTop = matTop.mainTextureOffset;
        Vector2 vecMid = matMid.mainTextureOffset;
        Vector2 vecBot = matBot.mainTextureOffset;

        vecTop += new Vector2(0, speedTop * Time.deltaTime);
        vecMid += new Vector2(0, speedMid * Time.deltaTime);
        vecBot += new Vector2(0, speedBot * Time.deltaTime);

        vecTop.y = Mathf.Repeat(vecTop.y, 1.0f);
        vecMid.y = Mathf.Repeat(vecMid.y, 1.0f);
        vecBot.y = Mathf.Repeat(vecBot.y, 1.0f);

        matTop.mainTextureOffset = vecTop;
        matMid.mainTextureOffset = vecMid;
        matBot.mainTextureOffset = vecBot;
    }
}