using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum BTNType
{
    Start,
    Quit,
    Option,
    Option_Back,
    Back,
    Tutorial,
    Easy_Mode,
    Hard_Mode,
    Infinite_Mode,
    Title,
    Title_Option_panel,
    Yes,
    No,
}

public class BTN : MonoBehaviour
{
    public GameObject Option_pannel;
    public GameObject Main_UI;
    public GameObject Second_UI;
    public GameObject Give_UP;
    public BTNType currentType;
    public void OnBtnClick()
    {
        switch (currentType)
        {
            case BTNType.Start:
                Main_UI.SetActive(false);
                Second_UI.SetActive(true);
                break;
            case BTNType.Quit:
                Application.Quit();
                break;
            case BTNType.Option:
                Option_pannel.SetActive(true);
                Time.timeScale = 0;              
                break;
            case BTNType.Option_Back:
                Option_pannel.SetActive(false);
                Time.timeScale = 1;
                break;
            case BTNType.Back:
                Main_UI.SetActive(true);
                Second_UI.SetActive(false);
                break;
            case BTNType.Easy_Mode:
                SceneManager.LoadScene("SampleScene");
                break;
            case BTNType.Hard_Mode:
                break;
            case BTNType.Infinite_Mode:
                break;
            case BTNType.Title:
                SceneManager.LoadScene("TitleScene");
                Time.timeScale = 1;
                break;
            case BTNType.Title_Option_panel:
                Give_UP.SetActive(true);
                Time.timeScale = 0;
                break;
            case BTNType.Yes:
                Give_UP.SetActive(false);
                SceneManager.LoadScene("TitleScene");
                Time.timeScale = 1;
                break;
            case BTNType.No:
                Give_UP.SetActive(false);
                Time.timeScale = 0;
                break;
        }
    }
}