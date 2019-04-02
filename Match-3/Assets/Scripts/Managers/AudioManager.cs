using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AudioManager : Manager
{

    [System.Serializable]
    public struct AudioData
    {
        public string clipName;
        public AudioClip clip;
    }


    public List<AudioData> audioData;

    [SerializeField]
    private AudioSource m_audioSourceSFX;

    [SerializeField]
    private AudioSource m_audioSourceBG;


    private void Start()
    {
        PlayBG("BG");
    }




    public override void ManagedUpdate()
    {
        
    }


    public void PlaySFX(string clipName)
    {
        AudioClip clip = audioData.Single(x => x.clipName == clipName).clip;
        m_audioSourceSFX.PlayOneShot(clip);
             
    }

    

    public void PlayBG(string clipName)
    {
        AudioClip clip = audioData.Single(x => x.clipName == clipName).clip;

        m_audioSourceBG.clip = clip;
        m_audioSourceBG.Play();
    }

    
    public void VolumeControl(float value, bool isSFX)
    {

        if(isSFX)
        {
            m_audioSourceSFX.volume =  Mathf.Clamp01(value);
        }

        else
        {
            m_audioSourceBG.volume = Mathf.Clamp01(value);
        }

    }



}
