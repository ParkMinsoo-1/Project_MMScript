using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundSlider : MonoBehaviour
{
    public AudioMixer MainMixer;
    public Slider BGMslider;
    public Slider SFXslider;

    public TMP_Text BGMAmountTXT;
    public TMP_Text SFXAmountText;



    private void Awake()
    {
        // 슬라이더의 값이 변경될 때 AddListener를 통해 이벤트 구독
        BGMslider.onValueChanged.AddListener(SetBGMVolume);
        SFXslider.onValueChanged.AddListener(SetSFXVolume);
    }

    // Start is called before the first frame update
    void Start()
    {
        // PlayerPrefs에 Volume 값이 저장되어 있을 경우,
        if (PlayerPrefs.HasKey("Volume"))
        {
            // Slider의 값을 저장해 놓은 값으로 변경.
            BGMslider.value = PlayerPrefs.GetFloat("Volume");
            SFXslider.value = PlayerPrefs.GetFloat("Volume");
        }
        else
        {
            BGMslider.value = 0.5f;     // PlayerPrefs에 Volume이 없을 경우
            SFXslider.value = 0.5f;
        }

        // audioMixer.SetFloat("audioMixer에 설정해놓은 Parameter", float 값)
        // audioMixer에 미리 설정해놓은 parameter 값을 변경하는 코드.
        // Mathf.Log10(BGMSlider.value) * 20 : 데시벨이 비선형적이기 때문에 해당 방식으로 값을 계산.
        MainMixer.SetFloat("BGM", Mathf.Log10(BGMslider.value) * 20);
        MainMixer.SetFloat("SFX", Mathf.Log10(SFXslider.value) * 20);
    }

    // Slider를 통해 걸어놓은 이벤트
    public void SetBGMVolume(float volume)
    {
        // 변경된 Slider의 값 volume으로 audioMixer의 Volume 변경하기
        MainMixer.SetFloat("BGM", Mathf.Log10(volume) * 20);
        // 변경된 Volume 값 저장하기
        PlayerPrefs.SetFloat("Volume", BGMslider.value);
        BGMAmountTXT.text = (BGMslider.value*100f).ToString("F0");
    }

    public void SetSFXVolume(float volume)
    {
        // 변경된 Slider의 값 volume으로 audioMixer의 Volume 변경하기
        MainMixer.SetFloat("SFX", Mathf.Log10(volume) * 20);
        // 변경된 Volume 값 저장하기
        PlayerPrefs.SetFloat("Volume", SFXslider.value);
        SFXAmountText.text = (SFXslider.value*100f).ToString("F0");
    }
}