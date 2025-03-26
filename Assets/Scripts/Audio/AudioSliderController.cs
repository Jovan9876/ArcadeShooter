using UnityEngine;
using UnityEngine.UI;
using DarkTonic.MasterAudio;
using static UnityEngine.InputSystem.Controls.AxisControl;
using UnityEngine.Rendering;
using UnityEngine.Audio;

public class AudioSliderController : MonoBehaviour {

    [Header("UI Sliders")]
    public Slider musicSlider;
    public Slider sfxSlider;
    public AudioMixer mixer;

    void Start() {
        if (musicSlider != null) {
            float musicVol = PlayerPrefs.GetFloat("MusicVolume", 1f);
            musicSlider.value = musicVol;
            SetMusicVolume(musicVol);
            musicSlider.onValueChanged.AddListener(SetMusicVolume);
        }
        if (sfxSlider != null) {
            float sfxVol = PlayerPrefs.GetFloat("SFXVolume", 1f);
            sfxSlider.value = sfxVol;
            SetSFXVolume(sfxVol);
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        }

    }

    public void SetMusicVolume(float value) {
        float volume = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
        mixer.SetFloat("MusicVolume", volume);
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    public void SetSFXVolume(float value) {
        MasterAudio.SetBusVolumeByName("SFX", value); // SFX bus 
        PlayerPrefs.SetFloat("SFXVolume", value);
    }
}
