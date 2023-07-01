using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class SceneMgr : SingleTon<SceneMgr>
{
    [SerializeField] TMP_InputField setNameField;
    public TMP_Text outPut;
    [SerializeField] Button setName;

    [SerializeField] Button easyScorebtn;
    [SerializeField] Button nomalScorebtn;
    [SerializeField] Button hardScorebtn;
    [SerializeField] Button extraScorebtn;

    [SerializeField] Slider mainbgmSlider;
    [SerializeField] Slider mainsfxSlider;

    [SerializeField] GameObject OuttroPanel;
    [SerializeField] GameObject leaderBoard;

    //string level = "easy";

    private void Awake()
    {
        //PlayerPrefs.DeleteAll();

        if (string.IsNullOrEmpty(PlayerPrefs.GetString("userID")))
        {
            setNameField.gameObject.SetActive(true);
        }

        Debug.Log(PlayerPrefs.GetString("Easy_Clear"));

        Debug.Log(PlayerPrefs.GetString("Nomal_Clear"));
        Debug.Log(PlayerPrefs.GetString("Hard_Clear"));
        //Debug.Log(DataManager.Instance.level.ToString());
        //Debug.Log(level.ToString());

        //Debug.Log(PlayerPrefs.GetString(DataManager.Instance.level.ToString()));
        //Debug.Log(PlayerPrefs.GetString(level.ToString()));
    }

    private void Start()
    {
        //if (DataManager.Instance.level.Equals(DataManager.Level.Tutorial) && !string.IsNullOrEmpty(PlayerPrefs.GetString("Tuto")))
        //{
        //    //PlayerPrefs.SetString("Tutorial", "play");
        //    DataManager.Instance.level = DataManager.Level.Easy;
        //    SceneManager.LoadScene("sampleScene");
        //    return;
        //}

        //PlayerPrefs.DeleteKey("Intro");
        //PlayerPrefs.DeleteKey("Tuto");


        DataManager.Instance.StartSetting();

        easyScorebtn.onClick.AddListener(() => DataManager.Instance.GetScore("Easy"));
        nomalScorebtn.onClick.AddListener(() => DataManager.Instance.GetScore("Nomal"));
        hardScorebtn.onClick.AddListener(() => DataManager.Instance.GetScore("Hard"));
        extraScorebtn.onClick.AddListener(() => DataManager.Instance.GetScore("Extra"));

        if (setName.gameObject.activeInHierarchy)
        {
            setName.onClick.AddListener(() => DataManager.Instance.CreateEmail(setNameField));
        }

        mainbgmSlider.value = SoundManager.Instance.bgmValue;
        mainsfxSlider.value = SoundManager.Instance.sfxValue;

        mainbgmSlider.onValueChanged.AddListener(SoundManager.Instance.BGMVolume);
        mainsfxSlider.onValueChanged.AddListener(SoundManager.Instance.SfxVolume);
    }

    public void CloseTab()
    {
        //for (int i = 0; i < 10; i++)
        //{
        //    leaderBoard.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(i).GetComponent<Text>().text = null;
        //    leaderBoard.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(i).GetComponent<Text>().text = null;
        //    //if (i < 5)
        //    //{
        //    //    leaderBoard.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(i).GetComponent<Text>().text = null;
        //    //    leaderBoard.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(i).GetComponent<Text>().text = null;
        //    //}
        //    //else
        //    //{
        //    //    leaderBoard.transform.GetChild(1).transform.GetChild(0).transform.GetChild(i).GetComponent<Text>().text = null;
        //    //    leaderBoard.transform.GetChild(1).transform.GetChild(1).transform.GetChild(i).GetComponent<Text>().text = null;
        //    //}
        //}

        //leaderBoard.transform.GetChild(0).gameObject.SetActive(true);
        //leaderBoard.transform.GetChild(1).gameObject.SetActive(false);

        leaderBoard.transform.GetChild(0).GetChild(1).GetComponent<Scrollbar>().value = 1f;
        leaderBoard.SetActive(false);
    }

    //public void NextTab()
    //{
    //    if (leaderBoard.transform.GetChild(1).gameObject.activeInHierarchy)
    //        leaderBoard.transform.GetChild(1).gameObject.SetActive(false);
    //    else if (leaderBoard.transform.GetChild(0).gameObject.activeInHierarchy)
    //        leaderBoard.transform.GetChild(1).gameObject.SetActive(true);
    //}

    public void Replay(int type)
    {
        //type 0 : Tutorial, type 1 : Intro, type 2 : Outtro

        DataManager.Instance.replay = true;

        switch (type)
        {
            case 0:
                DataManager.Instance.level = DataManager.Level.Tutorial;
                SceneManager.LoadScene("SampleScene");
                break;
            case 1:
                SceneManager.LoadScene("IntroScene");
                break;
            case 2:
                //easy outtro
                if(OuttroPanel.transform.GetChild(0).GetChild(1).gameObject.activeInHierarchy)
                {
                    DataManager.Instance.level = DataManager.Level.Easy;
                    SceneManager.LoadScene("OuttroScene");
                }
                break;
            case 3:
                //nomal outtro
                if (OuttroPanel.transform.GetChild(1).GetChild(1).gameObject.activeInHierarchy)
                {
                    DataManager.Instance.level = DataManager.Level.Nomal;
                    SceneManager.LoadScene("OuttroScene");
                }
                break;
            case 4:
                //hard outtro
                if (OuttroPanel.transform.GetChild(2).GetChild(1).gameObject.activeInHierarchy)
                {
                    DataManager.Instance.level = DataManager.Level.Hard;
                    SceneManager.LoadScene("OuttroScene");
                }
                break;
        }
    }

    public void OuttroPanelSet()
    {
        if (!string.IsNullOrEmpty(PlayerPrefs.GetString("Easy_Clear")))
        {
            //이미지 활성화
            OuttroPanel.transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
        }
        if (!string.IsNullOrEmpty(PlayerPrefs.GetString("Nomal_Clear")))
        {
            //이미지 활성화
            OuttroPanel.transform.GetChild(1).GetChild(1).gameObject.SetActive(true);
        }
        if (!string.IsNullOrEmpty(PlayerPrefs.GetString("Hard_Clear")))
        {
            //이미지 활성화
            OuttroPanel.transform.GetChild(2).GetChild(1).gameObject.SetActive(true);
        }
        OuttroPanel.SetActive(true);
        Debug.Log("달성 엔딩 활성화");
    }
}
