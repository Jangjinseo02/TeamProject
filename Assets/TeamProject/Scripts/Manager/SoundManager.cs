using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : SingleTon<SoundManager>
{
    [Header("---------------------Audio")]
    public AudioSource BGM_Player;
    public AudioSource[] SFX_Player;
    public AudioClip[] sfx;

    public enum Sfx { CloseDeath };
    int sfxCursor;

    private void Awake()
    {
        //SFX_Player = GetComponents<AudioSource>();
    }

    public void SfxPlay(Sfx cliptype)
    {
        switch (cliptype)
        {
            case Sfx.CloseDeath:
                SFX_Player[sfxCursor].clip = sfx[(int)Sfx.CloseDeath];
                break;
        }

        if (SFX_Player[sfxCursor].clip == sfx[(int)Sfx.CloseDeath])
            SFX_Player[sfxCursor].loop = true;
        else
            SFX_Player[sfxCursor].loop = false;

        SFX_Player[sfxCursor].Play();
        sfxCursor = (sfxCursor + 1) % SFX_Player.Length;
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
}
