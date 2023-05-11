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
}
