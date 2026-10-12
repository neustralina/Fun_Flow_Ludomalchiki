using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance;

    public GameObject gameOverCanvas; // Сюда перетащишь GameOverCanvas

    void Awake()
    {
        Instance = this;
    }

    public void ShowGameOver()
    {
        gameOverCanvas.SetActive(true); // Показываем экран проигрыша
        Time.timeScale = 0; // Останавливаем игру
    }

    public void RestartGame()
    {
        Time.timeScale = 1; // Возвращаем время
        SceneManager.LoadScene("FruitNinja"); // Перезагружаем сцену
    }
}
