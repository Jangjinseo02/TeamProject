using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SceneMgr : SingleTon<SceneMgr>
{
    [SerializeField] TMP_InputField setNameField;
    [SerializeField] Button setName;

    [SerializeField] Button easyScorebtn;
    [SerializeField] Button nomalScorebtn;
    [SerializeField] Button hardScorebtn;
    [SerializeField] Button extraScorebtn;

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
        DataManager.Instance.StartSetting();

        easyScorebtn.onClick.AddListener(() => DataManager.Instance.GetScore("Easy"));
        nomalScorebtn.onClick.AddListener(() => DataManager.Instance.GetScore("Nomal"));
        hardScorebtn.onClick.AddListener(() => DataManager.Instance.GetScore("Hard"));
        extraScorebtn.onClick.AddListener(() => DataManager.Instance.GetScore("Extra"));

        if (setName.gameObject.activeInHierarchy)
        {
            setName.onClick.AddListener(() => DataManager.Instance.CreateEmail(setNameField));
        }
    }
}
