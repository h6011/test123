using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Limiter : MonoBehaviour
{
    //rect bound
    [Header("화면 경계")]//viewport 기준
    [SerializeField] Vector2 viewPortLimitMin;
    [SerializeField] Vector2 viewPortLimitMax;

    [Header("보스용 화면 경계")]//viewport 기준
    [SerializeField] Vector2 viewPortLimitMinBoss;
    [SerializeField] Vector2 viewPortLimitMaxBoss;


    Vector2 worldPosLimitMin; // 실제 데이터는 이 변수가 가지고 있음
    public Vector2 WorldPosLimitMin => worldPosLimitMin;

    Vector2 worldPosLimitMax;
    public Vector2 WorldPosLimitMax => worldPosLimitMax;

    Camera cam;
    GameManager gameManager;

    private void Start()
    {
        cam = Camera.main;
        gameManager = GameManager.Instance;
        gameManager._Limiter = this;

        initWorldPos();
    }

    /// <summary>
    /// 게임 시작시 뷰포인트의 화면 경계 변수들을 월드 포지션으로 초기화 합니다.
    /// </summary>
    private void initWorldPos()
    {
        // 뷰포인트 데이터
        worldPosLimitMin = cam.ViewportToWorldPoint(viewPortLimitMin);
        worldPosLimitMax = cam.ViewportToWorldPoint(viewPortLimitMax);


    }

    /// <summary>
    /// 코드에 의해 플레이어 캐릭터가 카메라 밖으로 이동하지 못하도록 정의
    /// </summary>
    public Vector3 checkMovePosition(Vector3 _pos, bool _isBoss = false)
    {
        Vector3 viewPortPos = cam.WorldToViewportPoint(_pos);

        // 조건 연산자, 삼항 연산자, 다항식

        if (viewPortPos.x < (_isBoss == false ? viewPortLimitMin.x : viewPortLimitMinBoss.x))//0~1
        {
            viewPortPos.x = (_isBoss == false ? viewPortLimitMin.x : viewPortLimitMinBoss.x);
        }
        else if (viewPortPos.x > (_isBoss == false ? viewPortLimitMax.x : viewPortLimitMaxBoss.x))
        {
            viewPortPos.x = (_isBoss == false ? viewPortLimitMax.x : viewPortLimitMaxBoss.x);
        }

        if (viewPortPos.y < viewPortLimitMin.y)
        {
            viewPortPos.y = viewPortLimitMin.y;
        }
        else if (viewPortPos.y > viewPortLimitMax.y)
        {
            viewPortPos.y = viewPortLimitMax.y;
        }

        return cam.ViewportToWorldPoint(viewPortPos);
    }

    public bool checkMovePositionForBoss(Vector3 _pos, bool _isRight = true)
    {
        Vector3 viewPortPos = cam.WorldToViewportPoint(_pos);

        // 조건 연산자, 삼항 연산자, 다항식
        if (_isRight && viewPortPos.x > viewPortLimitMaxBoss.x)
        {
            return true;
        }
        if (_isRight == false && viewPortPos.x < viewPortLimitMinBoss.x)
        {
            return true;
        }



        return false;
    }



    public (bool _x, bool _y) IsReflectItem(Vector3 _pos, Vector3 _dir) // 화면 경계에 닿았거나 화면밖으로 나갔다면 반사 해야한다고 알려줌
    {
        bool rX = false;
        bool rY = false;

        if ((_pos.x < worldPosLimitMin.x && _dir.x < 0) || (_pos.x > worldPosLimitMax.x && _dir.x > 0))
        {
            rX = true;
        }

        if ((_pos.y < worldPosLimitMin.y && _dir.y < 0) || (_pos.y > worldPosLimitMax.y && _dir.y > 0))
        {
            rY = true;
        }

        return (rX, rY);

    }

}
