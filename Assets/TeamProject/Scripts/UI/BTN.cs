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
    Extra_Mode,
    Title,
    Title_Option_panel,
    Yes,
    No,
    Restart,
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
        BtnSound();

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
                if (string.IsNullOrEmpty(PlayerPrefs.GetString("Intro")))
                {
                    PlayerPrefs.SetString("Intro", "play");
                    SceneManager.LoadScene("IntroScene");
                    DataManager.Instance.level = DataManager.Level.Tutorial;
                    return;
                }
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
            case BTNType.Extra_Mode:
                DataManager.Instance.level = DataManager.Level.Extra;
                SceneManager.LoadScene("SampleScene");
                break;
            case BTNType.Title:
                if (!DataManager.Instance.replay)
                {
                    if (GameManager.Instance != null && GameManager.Instance.clear && !GameManager.Instance.level.Equals(GameManager.Level.Tutorial))
                    {
                        SceneManager.LoadScene("OuttroScene");
                        Time.timeScale = 1f;
                        break;
                    }
                    else if (DataManager.Instance != null && DataManager.Instance.level.Equals(DataManager.Level.Extra))
                    {
                        //extra모드 클리어 시 레벨 데이터를 변환해서 넘깁니다.
                        //레벨에 맞는 아웃트로를 플레이 하시면 됩니다.
                        if (GameManager.Instance != null && GameManager.Instance.curCatch >= 50)
                        {
                            DataManager.Instance.level = DataManager.Level.Hard;
                            SceneManager.LoadScene("OuttroScene");
                            Time.timeScale = 1f;
                            break;
                        }
                        else if (GameManager.Instance != null && GameManager.Instance.curCatch >= 25)
                        {
                            DataManager.Instance.level = DataManager.Level.Nomal;
                            SceneManager.LoadScene("OuttroScene");
                            Time.timeScale = 1f;
                            break;
                        }
                        else if (GameManager.Instance != null && GameManager.Instance.curCatch >= 10)
                        {
                            DataManager.Instance.level = DataManager.Level.Easy;
                            SceneManager.LoadScene("OuttroScene");
                            Time.timeScale = 1f;
                            break;
                        }
                    }
                    else if (GameManager.Instance != null && GameManager.Instance.level.Equals(GameManager.Level.Tutorial))
                    {
                        SceneManager.LoadScene("SampleScene");
                        DataManager.Instance.level = DataManager.Level.Easy;
                        Time.timeScale = 1;
                        break;
                    }
                }
                DataManager.Instance.replay = false;
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

                    if (SoundManager.Instance != null)
                        SoundManager.Instance.SfxPlay(SoundManager.UISfx.GiveUpYes);

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
            case BTNType.Restart:
                Option_pannel.SetActive(false);
                SceneManager.LoadScene("SampleScene");
                Time.timeScale = 1;
                break;
        }
    }

    void BtnSound()
    {
        switch (currentType)
        {
            case BTNType.Start:
                if (SoundManager.Instance != null)
                    SoundManager.Instance.SfxPlay(SoundManager.UISfx.GameStart);
                break;
            case BTNType.Option:
                if (SoundManager.Instance != null)
                    SoundManager.Instance.SfxPlay(SoundManager.UISfx.Pause);
                break;
            case BTNType.Title:
                if (SoundManager.Instance != null)
                    SoundManager.Instance.SfxPlay(SoundManager.UISfx.GiveUpSelect);
                break;
            //case BTNType.Yes:
            //    if (SoundManager.Instance != null)
            //        SoundManager.Instance.SfxPlay(SoundManager.UISfx.GiveUpYes);
            //    break;
            case BTNType.No:
                if (SoundManager.Instance != null)
                    SoundManager.Instance.SfxPlay(SoundManager.UISfx.GiveUpNo);
                break;
            case BTNType.Option_Back:
                if (SoundManager.Instance != null)
                    SoundManager.Instance.SfxPlay(SoundManager.UISfx.PauseClose);
                break;
            default:
                if (SoundManager.Instance != null)
                    SoundManager.Instance.SfxPlay(SoundManager.UISfx.OtherButton);
                break;
        }
    }
}