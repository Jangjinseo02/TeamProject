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

    [SerializeField] GameObject leaderBoard;

    string level = "easy";

    private void Awake()
    {
        if (string.IsNullOrEmpty(PlayerPrefs.GetString("userID")))
        {
            setNameField.gameObject.SetActive(true);
        }
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

    public void NextTab()
    {
        if (leaderBoard.transform.GetChild(1).gameObject.activeInHierarchy)
            leaderBoard.transform.GetChild(1).gameObject.SetActive(false);
        else if (leaderBoard.transform.GetChild(0).gameObject.activeInHierarchy)
            leaderBoard.transform.GetChild(1).gameObject.SetActive(true);
    }
}
