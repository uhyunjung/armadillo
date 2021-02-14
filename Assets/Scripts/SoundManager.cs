using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;

/*
 *  사운드 매니저 코드
 */

public class SoundManager : MonoBehaviour
{
    public AudioMixer mixer;
    public AudioSource bgsound;
    public AudioClip[] bglist;
    public static SoundManager instance;

    // 싱글톤 패턴
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);

            SceneManager.sceneLoaded += OnSceneLoaded;      // 씬 이동시 배경음악 재생 메서드 호출 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 씬 이동 시 배경음악 호출 메서드
    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        for (int i = 0; i < bglist.Length; i++)
        {
            if (arg0.name == bglist[i].name) BgSoundPlay(bglist[i]);
        }
    }
    // 사운드 조절 slider
    public void BGSoundVolume(float bgvol)
    {
        mixer.SetFloat("BGsoundVolume", Mathf.Log10(bgvol) * 40);
    }


    // 효과음 메서드
    public void SFXPlay(string soundName, AudioClip clip)
    {
        GameObject sfx = new GameObject(soundName + "Sound");
        AudioSource audiosource = sfx.AddComponent<AudioSource>();
        audiosource.outputAudioMixerGroup = mixer.FindMatchingGroups("SFXsound")[0];
        audiosource.clip = clip;
        audiosource.Play();                                             // Play

        Destroy(sfx, clip.length); // 사운드 재생 후 생성된 오브젝트 파괴
    }


    // 배경음악 메서드
    public void BgSoundPlay(AudioClip clip)
    {
        bgsound.outputAudioMixerGroup = mixer.FindMatchingGroups("BGsound")[0];
        bgsound.clip = clip;
        bgsound.loop = true;                // 배경음악이 loop하도록
        bgsound.volume = 1f;
        bgsound.Play();                     // Play
    }

    // Start is called before the first frame update
    void Start()
    {   
        // 시작 시, 기존에 유저가 저장했던 볼륨값으로 시작
        float initBGvol;
        initBGvol = PlayerPrefs.GetFloat("bgVol");
        mixer.SetFloat("BGsoundVolume", Mathf.Log10(initBGvol) * 40);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
