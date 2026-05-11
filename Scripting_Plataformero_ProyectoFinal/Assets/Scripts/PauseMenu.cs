using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] public GameObject pauseMenuPanel;
    private bool isPaused = false;
    void Start()
    {
        Time.timeScale = 1f;
        pauseMenuPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            
            isPaused = !isPaused;

            if (isPaused)
            {
                PauseGame();
            }
            else
            {
                ResumeGame();
            }
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        isPaused = false;
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
    }
}
