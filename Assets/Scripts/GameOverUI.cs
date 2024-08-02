using TMPro;
using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private GameObject gameOverUIObject;
    public static GameOverUI Instance;
    private TextMeshProUGUI gameOverText;
    public ParticleSystem wonParticle; 

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        gameOverText = gameOverUIObject.GetComponentInChildren<TextMeshProUGUI>(true);

    }
    public void ShowGameOver(bool timeIsUp)
    {
        gameOverUIObject.SetActive(true);
        if (timeIsUp)
        {
            gameOverText.text = "Time is up!";
        }
        else
        {
            gameOverText.text = "Puzzle complete!";
            wonParticle.Play(true);
            if (GameManager.Instance.timerBlinkCoroutine !=null)
            {
                StopCoroutine(GameManager.Instance.timerBlinkCoroutine);

            }
        }
    }
    public void HideGameOver()
    {
        gameOverUIObject.SetActive(false);
        wonParticle.Play(false);
        wonParticle.Stop();

    }
}
