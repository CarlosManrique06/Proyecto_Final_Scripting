using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    public Slider healthSlider;
    public float playerHealth;
    public float maxHealth = 100;

    public TextMeshProUGUI coinsText;
    public float coinsNumber;


    // Start is called before the first frame update
    void Start()
    {
        
        coinsText.text = "  ";
        playerHealth = maxHealth;

        healthSlider.maxValue = maxHealth;

    }

    // Update is called once per frame
    void Update()
    {

        coinsText.text = $" {coinsNumber} ";

        if (playerHealth > maxHealth)
        {
            playerHealth = maxHealth;
        }

        
        else if (playerHealth > 0)
        {

            healthSlider.value = playerHealth;
        }

        else if (playerHealth <= 0)
        {

            SceneManager.LoadScene(0);
        }

       
    }
   
}
