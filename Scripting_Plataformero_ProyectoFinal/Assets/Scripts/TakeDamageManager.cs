using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeDamageManager : MonoBehaviour
{
    // Start is called before the first frame update
    private HealthManager healthManager;

    private Animator playerAnimator;
    private float invincibilityClock;
    public  float  time;
    public GameObject gameoverMenu;
   
    [SerializeField] public ParticleSystem ParticlesCaller;
    void Start()
    {
      
        healthManager = FindAnyObjectByType<HealthManager>();
        playerAnimator = GetComponent<Animator>();
        TakeDamageManager invincibility = GetComponent<TakeDamageManager>();

    }

    // Update is called once per frame
    void Update()
    {
        if(invincibilityClock > 0)
        {
            invincibilityClock -= Time.deltaTime;
        }
        else if(invincibilityClock <= 0)
        {

        }
    }

    public void TakeDamage(float damage)
    {
        ParticlesCaller  = GetComponentInChildren<ParticleSystem>();
        var main = ParticlesCaller.main;
       
        gameoverMenu.SetActive(true);

        main.startColor = Color.white;
        ParticlesCaller.Play();
        
            healthManager.playerHealth -= 110;
            playerAnimator.SetTrigger("Hit");
            invincibilityClock = time;
        
        

    }


}
