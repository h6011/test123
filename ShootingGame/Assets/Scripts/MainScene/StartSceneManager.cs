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

    [Header("��ũ ������")]
    [SerializeField] GameObject fabRank;
    [SerializeField] Transform contents;


    private void Awake()
    {
        Tool.isStartingMainScene = true;
        #region �޸� 1
        //UnityEngine.Events.UnityAction<float, float> call = (float _value, float _value2) => { };
        //call.Invoke(1, 2);
        #endregion

        btnStart.onClick.AddListener(() => { gameStart(); });
        btnRanking.onClick.AddListener(() => { ShowRanking(); });
        btnExit.onClick.AddListener(() => { GameExit(); });

        btnExitRanking.onClick.AddListener(() => { viewRank.SetActive(false); });

        #region �޸� 2
        #region �޸� 2
        // json
        // string ���ڿ�, Ű�� ����
        // {key:value};

        // save���, ���� ���� �̵� �Ҷ� ������ �����ϴ� �����Ͱ� �ִٸ�

        // PlayerPrefs // ����Ƽ�� ������ �����͸� ���� �ϵ��� ����Ƽ ���ο� ����

        // PlayerPrefs.SetInt("test", 999);
        // �����͸� ���� ���� �ʴ��� // test 999, ������ �����ϸ� �̵����Ͱ� �����ǰ� �ҷ��ü� ����
        //int value = PlayerPrefs.GetInt("test");
        //Debug.Log(value);
        //PlayerPrefs.DeleteKey("test");

        //PlayerPrefs.HasKey();
        #endregion


        #region json ���� �޸�
        //string path = Application.streamingAssetsPath; // os�� ���� �б��������� ����
        // C:/Users/User/2dplatform_jiho/Assets/StreamingAssets

        //File.WriteAllText(path + "/abc.json", "�׽�Ʈ");
        //File.Delete(path + "/abc.json");

        //string result = File.ReadAllText(path + "/abc.json");
        //Debug.Log(result);

        //Debug.Log(path);
        //path = Application.persistentDataPath; // R/W�� ������ ������ġ
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
        //    Name = "������",
        //    Score = 100,
        //    UserPos = new Vector3(0, 1, 2)
        //};
        //cUserData cUserData2 = new cUserData()
        //{
        //    Name = "�󸶹�",
        //    Score = 200,
        //};

        //List<cUserData> listUserData = new List<cUserData>() {
        //cUserData,
        //cUserData2
        //};





        ///*
        // * JsonUtility
        // * 
        // * list �� json���� �����ϸ� Ʈ������ ������
        // * 
        // * 
        // * 
        // * 
        // * 
        // * ����
        // * - ������
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
    /// ��ũ�� ����Ǿ� �ִٸ� ����� ��ũ �����͸� �̿��ؼ� ��ũ�並 ������ְ�,
    /// ��ũ�� ����Ǿ� ���� �ʴٸ� ����ִ� ��ũ�� ����� ��ũ�並 �������
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
        else // ��ũ �����Ͱ� ����Ǿ� ���� �ʾҴٸ�
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
        #region �޸� 3
        // �����Ϳ��� �÷��̸� ���� ���, ������ ���� ���
        // ���带 ���ؼ� ������ ������ �������� �ȵ�
        // ��ó��, �ڵ尡 ���ǿ� ���ؼ� ������ ���°�ó�� Ȥ�� �ִ°� ó��
        // �����ϰ� ����
        #endregion

#if UNITY_EDITOR

        UnityEditor.EditorApplication.isPlaying = false;

#else // ����Ƽ ������ �������� �ʾ�����

        // ���� ������ ���� ����
        Application.Quit();

#endif






    }


}
