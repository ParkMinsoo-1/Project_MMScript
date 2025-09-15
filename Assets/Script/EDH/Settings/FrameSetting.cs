using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FrameSetting : MonoBehaviour
{
    public Button LowFPSbtn; // 30프레임
    public Button HighFPSbtn;// 60프레임
    
    public Image spriteRendererOne; // 색상1
    public Image SpriteRendererTwo; // 색상2
    // Start is called before the first frame update
    void Start()
    {
        LowFPSbtn.onClick.AddListener(SetLowFPS);
        HighFPSbtn.onClick.AddListener(SetHighFPS);
    }
    private int setframeNumber = 0;

    public void SetLowFPS()
    {
        spriteRendererOne.color = Color.green;
        Debug.Log("나는 초록");
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 30;
        Debug.Log("현재 프레임" + Application.targetFrameRate);
        SpriteRendererTwo.color = Color.white;

        setframeNumber = 30;
        PlayerPrefs.SetInt("Frame",setframeNumber);
    }

    public void SetHighFPS()
    {
        SpriteRendererTwo.color = Color.green; 
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        Debug.Log("현재 프레임" + Application.targetFrameRate);
        spriteRendererOne.color = Color.white;

        setframeNumber = 60;
        PlayerPrefs.SetInt("Frame", setframeNumber);
    }

    public void LoadFrame()
    {
        setframeNumber = PlayerPrefs.GetInt("Frame");
        if(setframeNumber == 30)
        {
            SetLowFPS();
        }
        else
        {
            SetHighFPS();
        }
    }
}
