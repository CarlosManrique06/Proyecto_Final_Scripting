using UnityEngine;

public class GameOver : MonoBehaviour
{
    // referencia al panel de Game Over
    [SerializeField] public GameObject gameOverPanel;

    void Start()
    {
        // aseguramos que el panel esté oculto al inicio
        Time.timeScale = 1f;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    // método público para mostrar el panel
    public void ShowGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            Time.timeScale = 0f; 
         }

    }

 


}
