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
                //사운드 일시 정지
                if (SoundManager.Instance != null)
                    SoundManager.Instance.SoundPause();
                Time.timeScale = 0;              
                break;
            case BTNType.Option_Back:
                Option_pannel.SetActive(false);
                //사운드 다시 재생
                if (SoundManager.Instance != null)
                    SoundManager.Instance.SoundPlay();
                Time.timeScale = 1;
                break;
            case BTNType.Back:
                Main_UI.SetActive(true);
                Second_UI.SetActive(false);
                break;
            case BTNType.Easy_Mode:
                DataManager.Instance.level = DataManager.Level.Easy;
                SceneManager.LoadScene("SampleScene");
                break;
            case BTNType.Hard_Mode:
                DataManager.Instance.level = DataManager.Level.Nomal;
                SceneManager.LoadScene("SampleScene");
                break;
            case BTNType.Infinite_Mode:
                DataManager.Instance.level = DataManager.Level.Hard;
                SceneManager.LoadScene("SampleScene");
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

                if (GameManager.Instance != null)
                {
                    GameManager.Instance.giveUp = true;
                    GameManager.Instance.GameExit(); //playerDead실행 이후 loadScene("title")이 실행

                    Option_pannel.SetActive(false); //옵션 창 끈 후 플레이어 애니메이션 출력
                    //사운드 다시 재생
                    if (SoundManager.Instance != null)
                        SoundManager.Instance.SoundPlay();
                    Time.timeScale = 1;
                }
                break;
            case BTNType.No:
                Give_UP.SetActive(false);
                Time.timeScale = 0;
                break;
        }
    }
}