using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : SingleTon<SoundManager>
{
    [Header("---------------------Audio")]
    public AudioSource BGM_Player;
    public AudioSource[] SFX_Player;
    public AudioClip[] sfx;

    public enum Sfx
    {
        CloseDeath, Attack, AttackHard, AttackMonster,
        BreakNomal, BreakHard, BreakGlass, BreakMeteor, LoseAir,
        Restore, Recovery, Bigbang, Meteor, Crystal, Barrier, Ufo,
        UfoMove, UfoStop, UfoBeam, BeamInPlayer
    };
    int sfxCursor;

    public void SfxPlay(Sfx cliptype, bool loop)
    {
        if (SFX_Player[sfxCursor].loop)
            sfxCursor = (sfxCursor + 1) % SFX_Player.Length; //Player를 간섭하면 play중인 clip이 강제 종료됨

        SFX_Player[sfxCursor].clip = sfx[(int)cliptype];

        SFX_Player[sfxCursor].loop = loop;
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
            SFX_Player[i].Pause();
        }
    }
    //다시 재생
    public void SoundPlay()
    {
        for (int i = 0; i < SFX_Player.Length; i++)
        {
            SFX_Player[i].Play();
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
}
