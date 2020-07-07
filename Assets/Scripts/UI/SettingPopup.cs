using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
public class SettingPopup : MonoBehaviour
{
    public AudioMixer mixer;

    public Slider musicSlider;

    protected float m_MusicVolume;

    protected const float k_MinVolume = -80f;
    protected const string k_MusicVolumeFloatName = "MusicVolume";

    public void Open()
    {
        gameObject.SetActive(true);
        UpdateUI();
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    void UpdateUI()
    {
        mixer.GetFloat(k_MusicVolumeFloatName, out m_MusicVolume);

        musicSlider.value = 1.0f - (m_MusicVolume / k_MinVolume);
    }

    public void MusicVolumeChangeValue(float value)
    {
        m_MusicVolume = k_MinVolume * (1.0f - value);
        mixer.SetFloat(k_MusicVolumeFloatName, m_MusicVolume);
    }
}
