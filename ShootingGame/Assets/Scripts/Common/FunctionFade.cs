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
    bool fade = false; // true = 페이드 아웃, false = 페이드 인
    UnityAction action = null; // 어떤 기능이 동작 완료후 실행


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

        imgFade = GetComponentInChildren<Image>(); // 내위치로 부터 자식중 이미지 컴포넌트 가 있는 오브젝트를 찾아 등록해줌
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
