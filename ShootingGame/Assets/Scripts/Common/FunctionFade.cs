using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class FunctionFade : MonoBehaviour
{
    public static FunctionFade Instance;


    private Image imgFade;


    [SerializeField] float fadeTime = 1.0f;
    bool fade = false; // true = ���̵� �ƿ�, false = ���̵� ��
    UnityAction action = null; // � ����� ���� �Ϸ��� ����


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        imgFade = GetComponentInChildren<Image>(); // ����ġ�� ���� �ڽ��� �̹��� ������Ʈ �� �ִ� ������Ʈ�� ã�� �������
    }

    private void Update()
    {
        if (fade == true && imgFade.color.a < 1)
        {
            Color color = imgFade.color;
            color.a += Time.deltaTime / fadeTime;
            if (color.a > 1)
            {
                color.a = 1;

                if (action != null)
                {
                    action.Invoke();
                    action = null;
                }
            }
            imgFade.color = color;
        }
        else if (fade == false && imgFade.color.a > 0)
        {
            Color color = imgFade.color;
            color.a -= Time.deltaTime / fadeTime;
            if (color.a < 0)
            {
                color.a = 0;
            }
            imgFade.color = color;
        }
        imgFade.raycastTarget = imgFade.color.a != 0.0f;
    }

    public void ActiveFade(bool _fade, UnityAction _action = null)
    {
        fade = _fade;
        action = _action;
        
    }


}
