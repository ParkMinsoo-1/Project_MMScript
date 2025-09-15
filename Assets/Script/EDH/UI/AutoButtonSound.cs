using UnityEngine;
using UnityEngine.UI;


public class ButtonSoundPlayer : MonoBehaviour
{
    public AudioClip clickSound;       // 클릭 사운드
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayClickSound()
    {
        if (clickSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }
    public ButtonSoundPlayer soundPlayer;

    private void Start()
    {
        Button[] buttons = FindObjectsOfType<Button>();

        foreach (Button btn in buttons)
        {
            btn.onClick.AddListener(soundPlayer.PlayClickSound);
        }
    }
}