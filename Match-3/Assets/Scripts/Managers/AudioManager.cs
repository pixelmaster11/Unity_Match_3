using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AudioManager : Manager
{

    [SerializeField]
    private UIManager m_uiManager;

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


    private bool m_SFX_Enabled = true;
    private bool m_Music_Enabled = true;

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

    
    public void VolumeControlSFX()
    {
        m_SFX_Enabled = !m_SFX_Enabled;

        if (m_SFX_Enabled)
            m_audioSourceSFX.volume = 1;
        else
            m_audioSourceSFX.volume = 0;

        m_uiManager.SetSFX(m_SFX_Enabled);
        
    }
    public void VolumeControlMusic()
    {
        m_Music_Enabled = !m_Music_Enabled;

        if (m_Music_Enabled)
            m_audioSourceBG.volume = 1;
        else
            m_audioSourceBG.volume = 0;

        m_uiManager.SetMusic(m_Music_Enabled);
       
    }

}
