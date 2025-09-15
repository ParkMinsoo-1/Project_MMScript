using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    [Header("메인, 스테이지")]
    public AudioSource bgmSource;  //  메인로비, 스테이지 선택

    [Header("BGM")]
    public List<AudioClip> bgm;

    public void PlayBGM(int i)
    {
        AudioClip clip = bgm[i];
        bgmSource.Stop();
        bgmSource.clip = clip;
        bgmSource.loop = true;
        bgmSource.Play();
    }
    public void StopBGM()
    {
        bgmSource.Stop();
    }
}
