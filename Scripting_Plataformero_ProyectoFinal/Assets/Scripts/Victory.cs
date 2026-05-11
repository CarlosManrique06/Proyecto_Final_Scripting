using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class VictoryManager : MonoBehaviour
{
    
    [SerializeField] private GameObject victoryPanel; 

  
    [SerializeField] private string menuSceneName = "Menu";
    [SerializeField] private float timeBeforeExit = 3f;

   
    [SerializeField] private string playerTag = "Player";

    private bool isLevelFinished = false;

    private void Awake()
    {
        // Aseguramos que el panel esté oculto al empezar
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Si el jugador entra y el nivel no ha terminado aún
        if (!isLevelFinished && other.CompareTag(playerTag))
        {
            StartCoroutine(VictorySequence());
        }
    }

    private IEnumerator VictorySequence()
    {
        isLevelFinished = true;

        // Mostramos la pantalla gris con el texto
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
        }

        // Esperamos el tiempo definido
        yield return new WaitForSeconds(timeBeforeExit);

        // Cargamos el menú principal
        SceneManager.LoadScene(menuSceneName);
    }
}