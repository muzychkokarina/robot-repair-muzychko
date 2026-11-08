using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public PlayerController player;
    private EnemyController[] enemies;
    public UIHandler uiHandler;

    private bool gameEnded = false;

    void Start()
    {
        enemies = FindObjectsByType<EnemyController>(FindObjectsSortMode.None);
    }

    void Update()
    {
        if (gameEnded)
            return;

        // Поразка
        if (player != null && player.health <= 0)
        {
            gameEnded = true;

            if (uiHandler != null)
            {
                uiHandler.DisplayLoseScreen();
            }

            Invoke(nameof(ReloadScene), 3f);
            return;
        }

        // Перемога
        if (AllEnemiesFixed())
        {
            gameEnded = true;

            if (uiHandler != null)
            {
                uiHandler.DisplayWinScreen();
            }

            Invoke(nameof(ReloadScene), 3f);
        }
    }

    bool AllEnemiesFixed()
    {
        if (enemies == null || enemies.Length == 0)
            return false;

        foreach (EnemyController enemy in enemies)
        {
            // Якщо хоч один робот ДОСІ зламаний — перемога ще не настала
            if (enemy != null && enemy.isBroken)
                return false;
        }

        return true;
    }

    void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}