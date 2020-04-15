using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundManager : MonoBehaviour
{
    [SerializeField]
    AudioSource[] audioSources;
    [SerializeField]
    AudioClip[] audioClips;

    public AudioClip PagePickupSound; 
    public AudioClip PlantHitYouSound;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    public bool AudioSourceIsPlaying(int sourceIndex)
    {
        return audioSources[sourceIndex].isPlaying;
    }

    public void PlaySound(int index)
    {
        audioSources[0].clip = audioClips[index];
        //Debug.Log("PlayerSoundManager::PlaySound(int)::" + audioSources[0].clip.name);
        if(index < audioClips.Length)
            audioSources[0].Play();
    }

    public void PlaySound(int clipIndex, int sourceIndex)
    {
        audioSources[sourceIndex].clip = audioClips[clipIndex];
        //Debug.Log("PlayerSoundManager::PlaySound(int)::" + audioSources[sourceIndex].clip.name);
        if (clipIndex < audioClips.Length && !audioSources[sourceIndex].isPlaying)
            audioSources[sourceIndex].Play();
    }

    public void PlaySound(int clipIndex, int sourceIndex, bool resetIfAlreadyPlaying)
    {
        if(resetIfAlreadyPlaying || !audioSources[sourceIndex].isPlaying)
        {
            PlaySound(clipIndex, sourceIndex);
        }
    }

    public void SpellPickup()
    {
        gameObject.GetComponent<AudioSource>().PlayOneShot(PagePickupSound, 1);
        Debug.Log("Page Received");
    }

    public void BulletHit()
    {
        gameObject.GetComponent<AudioSource>().PlayOneShot(PlantHitYouSound, 1);
    }

    public void StopSound(int clipIndex)
    {
    }

    public void StopSound(int clipIndex, int sourceIndex)
    {
        for(int i = 0; i < audioClips.Length; i++)
        {
            if(audioClips[i] == audioSources[sourceIndex].clip && audioClips[clipIndex] == audioClips[i])
                audioSources[sourceIndex].Stop();
        }
    }
}
