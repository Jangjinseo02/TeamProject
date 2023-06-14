using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class SoundManager : SingleTon<SoundManager>
{
    [Header("---------------------Audio")]
    public AudioMixer mixer;
    public AudioSource BGM_Player;
    public AudioSource[] SFX_Player;
    public AudioClip[] bgm;
    public AudioClip[] sfx;
    public AudioClip[] uiSfx;
    public AudioClip[] introSfx;

    public float bgmValue = 1f;
    public float sfxValue = 1f;

    float fadeSpeed = 2f;

    private void Awake()
    {
        if (FindObjectOfType<SoundManager>() != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
    }

    public enum BGM
    {
        Title, Intro, TutorialAndInGame_1, InGame_2, InGame_3, Result, Ending
    }

    public enum Sfx
    {
        CloseDeath, Attack, AttackHard, AttackMonster,
        BreakNomal, BreakHard, BreakGlass, BreakMeteor, LoseAir,
        Restore, Recovery, Bigbang, Meteor, Crystal, Barrier, Ufo,
        UfoMove, UfoStop, UfoBeam, BeamInPlayer,
        Down, Jump, MonsterDamaged, BlockDamaged, Stun, Loose,
        WorrierAttack, MagicianAttack, GetMonster,
        LevelClear, GameClear
    };

    public enum UISfx
    {
        GameStart, OtherButton,
        Pause, GiveUpSelect, GiveUpYes, GiveUpNo, PauseClose,
    }

    public enum IntroSfx
    {
        PlayerText, DoctorText, ExtraText, DropSound, BeforeTutorial
    }
    int sfxCursor;

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        switch (arg0.name)
        {
            case "TitleScene":
                BgmPlay(BGM.Title);
                break;
            case "SampleScene":
                BgmPlay(BGM.TutorialAndInGame_1);
                break;
            case "IntroScene":
                BgmPlay(BGM.Intro);
                break;
            case "OuttroScene":
                BgmPlay(BGM.Ending);
                break;
        }
    }

    public void BgmPlay(BGM cliptype)
    {
        BGM_Player.clip = bgm[(int)cliptype];
        BGM_Player.loop = true;
        BGM_Player.volume = 0.1f;

        BGM_Player.Play();
    }

    public bool SfxPlay(Sfx cliptype, bool loop)
    {
        //for (int i = 0; i < SFX_Player.Length; i++)
        //{
        //    if (SFX_Player[i].isPlaying && SFX_Player[i].clip.Equals(sfx[(int)Sfx.Down]))
        //        return false;
        //}

        if (SFX_Player[sfxCursor].loop)
            sfxCursor = (sfxCursor + 1) % SFX_Player.Length; //Player를 간섭하면 play중인 clip이 강제 종료됨

        SFX_Player[sfxCursor].clip = sfx[(int)cliptype];

        SFX_Player[sfxCursor].loop = loop;
        SFX_Player[sfxCursor].Play();

        sfxCursor = (sfxCursor + 1) % SFX_Player.Length;

        return true;
    }

    public void SfxPlay(UISfx cliptype)
    {
        if (SFX_Player[sfxCursor].loop)
            sfxCursor = (sfxCursor + 1) % SFX_Player.Length; //Player를 간섭하면 play중인 clip이 강제 종료됨

        SFX_Player[sfxCursor].clip = uiSfx[(int)cliptype];

        SFX_Player[sfxCursor].Play();
        sfxCursor = (sfxCursor + 1) % SFX_Player.Length;
    }

    public void SfxStop(Sfx cliptype)
    {
        for(int i = 0; i < SFX_Player.Length; i++)
        {
            if (SFX_Player[i].clip == sfx[(int)cliptype])
                SFX_Player[i].Stop();
        }
    }

    public void SfxAllStop()
    {
        for (int i = 0; i < SFX_Player.Length; i++)
        {
            SFX_Player[i].Stop();
            SFX_Player[i].clip = null;
        }
    }

    //일시정지
    public void SoundPause()
    {
        for (int i = 0; i < SFX_Player.Length; i++)
        {
            for (int j = 0; j < sfx.Length; j++)
            {
                if (SFX_Player[i].clip == sfx[j])
                    SFX_Player[i].Pause();
            }
        }
    }
    //다시 재생
    public void SoundPlay()
    {
        for (int i = 0; i < SFX_Player.Length; i++)
        {
            for (int j = 0; j < sfx.Length; j++)
            {
                if (SFX_Player[i].clip == sfx[j])
                    SFX_Player[i].Play();
            }
        }
    }

    public void SpeedUpSound(int value)
    {
        for (int i = 0; i < SFX_Player.Length; i++)
        {
            if (SFX_Player[i].clip == sfx[value] && SFX_Player[i].pitch <= 1)   
                SFX_Player[i].pitch += 0.5f;
        }
    }

    public void SpeedDownSound(int value)
    {
        for (int i = 0; i < SFX_Player.Length; i++)
        {
            if (SFX_Player[i].clip == sfx[value] && SFX_Player[i].pitch >= 1.3)
                SFX_Player[i].pitch -= 0.5f;
        }
    }

    public void BGMVolume(float val)
    {
        bgmValue = val;
        mixer.SetFloat("BGM", Mathf.Log10(val) * 20);
    }

    public void SfxVolume(float val)
    {
        sfxValue = val;
        mixer.SetFloat("SFX", Mathf.Log10(val) * 20);
    }

    public void Fade()
    {
        StartCoroutine(FadeRoutine());
    }

    IEnumerator FadeRoutine()
    {
        float value = SoundManager.Instance.bgmValue;
        float curvalue = SoundManager.Instance.bgmValue;

        while (curvalue > 0)
        {
            curvalue -= fadeSpeed * Time.deltaTime;
            SoundManager.Instance.BGMVolume(curvalue);

            yield return null;
        }

        yield return new WaitForSeconds(0.1f);

        SoundManager.Instance.BgmPlay(SoundManager.BGM.Result);
        while (curvalue < value)
        {
            curvalue += fadeSpeed * Time.deltaTime;
            SoundManager.Instance.BGMVolume(curvalue);
            yield return null;
        }

        SoundManager.Instance.BGMVolume(value);
    }
}
