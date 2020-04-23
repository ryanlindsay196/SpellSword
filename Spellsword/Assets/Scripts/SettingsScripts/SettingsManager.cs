using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SettingsManager : MonoBehaviour
{
    static float soundEffectsVolume, musicVolume, brightness;
    public enum SettingTypes { soundEffectsVolume, musicVolume, brightness}
    [SerializeField]
    AudioMixer sfxMixer, musicMixer;
    static AudioMixer s_sfxMixer, s_musicMixer;

    private void Start()
    {
        s_musicMixer = musicMixer;
        s_sfxMixer = sfxMixer;
    }

    public static void UpdateSetting(SettingTypes in_settingType, float in_settingValue)
    {
        switch(in_settingType)
        {
            case SettingTypes.soundEffectsVolume:
                soundEffectsVolume = in_settingValue;
                s_sfxMixer.SetFloat("vol", Mathf.Log10(Mathf.Clamp(in_settingValue, 0.01f, 1)) * 40);
                break;
            case SettingTypes.musicVolume:
                musicVolume = in_settingValue;
                s_musicMixer.SetFloat("vol", Mathf.Log10(Mathf.Clamp(in_settingValue, 0.01f, 1)) * 40);
                break;
            case SettingTypes.brightness:
                brightness = in_settingValue;
                RenderSettings.ambientLight = new Color(in_settingValue, in_settingValue, in_settingValue);
                break;
        }
    }
}
