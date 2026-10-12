using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(-1)]
public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    [SerializeField] private Sword blade;
    [SerializeField] private FruitSp spawner;
    [SerializeField] private Text scoreText;
    [SerializeField] private Image fadeImage;

    public int score { get; private set; } = 0;

    public int lives = 3;
    public Image[] hearts;

    private void Awake()
    {
        if (Instance != null)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void Start()
    {
        NewGame();
    }

    private void NewGame()
    {
        Time.timeScale = 1f;

        ClearScene();

        blade.enabled = true;
        spawner.enabled = true;

        score = 0;
        scoreText.text = score.ToString();

        lives = 3;
        UpdateHearts();
    }

    private void ClearScene()
    {
        Sliceable[] fruits = FindObjectsOfType<Sliceable>();
        foreach (Sliceable fruit in fruits)
        {
            Destroy(fruit.gameObject);
        }

        Bomb[] bombs = FindObjectsOfType<Bomb>();
        foreach (Bomb bomb in bombs)
        {
            Destroy(bomb.gameObject);
        }
    }

    public void IncreaseScore(int points)
    {
        score += points;
        scoreText.text = score.ToString();

        float hiscore = PlayerPrefs.GetFloat("hiscore", 0);
        if (score > hiscore)
        {
            hiscore = score;
            PlayerPrefs.SetFloat("hiscore", hiscore);
        }
    }

    public void LoseLife()
    {
        lives--;

        // Обнови отображение сердец (твой существующий код)
        UpdateHearts(); // или как у тебя называется метод

        // ПРОВЕРКА: если жизни закончились — покажи экран проигрыша
        if (lives <= 0)
        {
            GameOverManager.Instance.ShowGameOver();
        }
    }

    void UpdateHearts()
    {
        if (hearts == null) return;

        for (int i = 0; i < hearts.Length; i++)
        {
            if (hearts[i] != null)
                hearts[i].enabled = (i < lives);
        }
    }

    public void Explode()
    {
        blade.enabled = false;
        spawner.enabled = false;

        StartCoroutine(ExplodeSequence());
    }

    private IEnumerator ExplodeSequence()
    {
        float diam = 0f;
        float duration = 0.5f;

        while (diam < duration)
        {
            float t = Mathf.Clamp01(diam / duration);
            fadeImage.color = Color.Lerp(Color.clear, Color.white, t);
            Time.timeScale = 1f - t;
            diam += Time.unscaledDeltaTime;
            yield return null;
        }

        yield return new WaitForSecondsRealtime(1f);

        NewGame();

        diam = 0f;

        while (diam < duration)
        {
            float t = Mathf.Clamp01(diam / duration);
            fadeImage.color = Color.Lerp(Color.white, Color.clear, t);
            diam += Time.unscaledDeltaTime;
            yield return null;
        }
    }
}