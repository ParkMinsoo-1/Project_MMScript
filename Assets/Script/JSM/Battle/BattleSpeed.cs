using UnityEngine;
using TMPro;

public class BattleSpeed : MonoBehaviour
{
    public TextMeshProUGUI speedLabel;

    public int gameSpeed = 1;
    public void Awake()
    {
        Time.timeScale = 1f;
    }
    public void ToggleSpeed()
    {
        switch (gameSpeed)
        {
            case 1: gameSpeed = 2;break;
            case 2: gameSpeed = 3;break;
            case 3: gameSpeed = 1;break;
        }
        Time.timeScale = gameSpeed;

        if (speedLabel != null)
            speedLabel.text = $"X{gameSpeed}";
    }
}
