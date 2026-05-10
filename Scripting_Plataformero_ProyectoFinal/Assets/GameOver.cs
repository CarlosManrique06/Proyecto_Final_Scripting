using UnityEngine;

public class GameOver : MonoBehaviour
{
    // referencia al panel de Game Over
    public GameObject gameOverPanel;

    void Start()
    {
        // aseguramos que el panel esté oculto al inicio
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    // método público para mostrar el panel
    public void ShowGameOver()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }
}
