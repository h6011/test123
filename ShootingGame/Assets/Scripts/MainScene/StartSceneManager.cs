using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using Unity.VisualScripting;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;

public class StartSceneManager : MonoBehaviour
{

    [SerializeField] Button btnStart;
    [SerializeField] Button btnRanking;
    [SerializeField] Button btnExitRanking;
    [SerializeField] Button btnExit;
    [SerializeField] GameObject viewRank;

    [Header("랭크 프리팹")]
    [SerializeField] GameObject fabRank;
    [SerializeField] Transform contents;


    private void Awake()
    {
        Tool.isStartingMainScene = true;
        #region 메모 1
        //UnityEngine.Events.UnityAction<float, float> call = (float _value, float _value2) => { };
        //call.Invoke(1, 2);
        #endregion

        btnStart.onClick.AddListener(() => { gameStart(); });
        btnRanking.onClick.AddListener(() => { ShowRanking(); });
        btnExit.onClick.AddListener(() => { GameExit(); });

        btnExitRanking.onClick.AddListener(() => { viewRank.SetActive(false); });

        #region 메모 2
        #region 메모 2
        // json
        // string 문자열, 키와 벨류
        // {key:value};

        // save기능, 씬과 씬을 이동 할때 가지고 가야하는 데이터가 있다면

        // PlayerPrefs // 유니티가 꺼져도 데이터를 보관 하도록 유니티 내부에 저장

        // PlayerPrefs.SetInt("test", 999);
        // 데이터를 삭제 하지 않는한 // test 999, 게임을 삭제하면 이데이터가 삭제되고 불러올수 없음
        //int value = PlayerPrefs.GetInt("test");
        //Debug.Log(value);
        //PlayerPrefs.DeleteKey("test");

        //PlayerPrefs.HasKey();
        #endregion


        #region json 관련 메모
        //string path = Application.streamingAssetsPath; // os에 따라 읽기전용으로 사용됨
        // C:/Users/User/2dplatform_jiho/Assets/StreamingAssets

        //File.WriteAllText(path + "/abc.json", "테스트");
        //File.Delete(path + "/abc.json");

        //string result = File.ReadAllText(path + "/abc.json");
        //Debug.Log(result);

        //Debug.Log(path);
        //path = Application.persistentDataPath; // R/W가 가능한 폴더위치
        // C:/Users/User/AppData/LocalLow/DefaultCompany/2dplatform_jiho
        //Debug.Log(path);
        // System.IO // C#

        //string path = Application.persistentDataPath + "/Jsons";

        //if (Directory.Exists(path) == false)
        //{
        //    Directory.CreateDirectory(path);
        //}

        //if (File.Exists(path + "/Test/abc.json") == true)
        //{
        //    File.ReadAllText(path + "Test/abc.json");
        //}
        //else
        //{
        //    File.Create(path + "/Test");
        //}

        //cUserData cUserData = new cUserData()
        //{
        //    Name = "가나다",
        //    Score = 100,
        //    UserPos = new Vector3(0, 1, 2)
        //};
        //cUserData cUserData2 = new cUserData()
        //{
        //    Name = "라마바",
        //    Score = 200,
        //};

        //List<cUserData> listUserData = new List<cUserData>() {
        //cUserData,
        //cUserData2
        //};





        ///*
        // * JsonUtility
        // * 
        // * list 를 json으로 변경하면 트러블이 존재함
        // * 
        // * 
        // * 
        // * 
        // * 
        // * 장점
        // * - 빠르다
        // * 
        // * 
        // * 
        // */

        //string jsonData = JsonConvert.SerializeObject(listUserData);
        //List<cUserData> afterData = JsonConvert.DeserializeObject<List<cUserData>>(jsonData);
        #endregion
        #endregion

        initRankView();
        viewRank.SetActive(false);
    }

    /// <summary>
    /// 랭크가 저장되어 있다면 저장된 랭크 데이터를 이용해서 랭크뷰를 만들어주고,
    /// 랭크가 저장되어 있지 않다면 비어있는 랭크를 만들어 랭크뷰를 만들어줌
    /// </summary>
    private void initRankView()
    {
        clearRankView();
        string _rankKey = Tool.rankKey;
        int _rankCount = Tool.rankCount;
        bool hasKey = PlayerPrefs.HasKey(_rankKey);

        List<cUserData> listUserData = null;

        if (hasKey)
        {
            string _key = PlayerPrefs.GetString(_rankKey);
            listUserData = JsonConvert.DeserializeObject<List<cUserData>>(_key);
        }
        else // 랭크 데이터가 저장되어 있지 않았다면
        {
            listUserData = new List<cUserData>();
            for (int iNum = 0; iNum < _rankCount; iNum++)
            {
                cUserData newData = new cUserData() { 
                    Name = "None",
                    Score = 0,
                };
                listUserData.Add(newData);
            }

            for (int i = 0; i < listUserData.Count; i++)
            {
                Debug.Log($"{i} / {listUserData[i]}");
            }

            string value = JsonConvert.SerializeObject(listUserData);
            PlayerPrefs.SetString(_rankKey, value);

        }
        int count = listUserData.Count;
        for (int iNum = 0; iNum < count; iNum++)
        {
            cUserData _data = listUserData[iNum];
            int currentRank = iNum + 1;

            GameObject newObject = Instantiate(fabRank, contents);
            FabRanking newObjectScript = newObject.GetComponent<FabRanking>();
            newObjectScript.SetData(currentRank.ToString(), _data.Name, _data.Score);
        }


    }

    private void clearRankView()
    {
        int count = contents.childCount;

        for (int iNum = count - 1; iNum > -1; --iNum)
        {
            Transform child = contents.GetChild(iNum);
            Destroy(child.gameObject);
        }
    }


    private void gameStart()
    {
        FunctionFade.Instance.ActiveFade(true, () => 
        {
            SceneManager.LoadScene(1);
            FunctionFade.Instance.ActiveFade(false);
        });
    }


    private void ShowRanking()
    {
        viewRank.SetActive(!viewRank.activeSelf);
    }

    private void GameExit()
    {
        #region 메모 3
        // 에디터에서 플레이를 끄는 방법, 에디터 전용 기능
        // 빌드를 통해서 밖으로 가지고 나가서는 안됨
        // 전처리, 코드가 조건에 의해서 본인이 없는것처럼 혹은 있는거 처럼
        // 동작하게 해줌
        #endregion

#if UNITY_EDITOR

        UnityEditor.EditorApplication.isPlaying = false;

#else // 유니티 에디터 실행하지 않았을때

        // 빌드 했을때 게임 종료
        Application.Quit();

#endif






    }


}
