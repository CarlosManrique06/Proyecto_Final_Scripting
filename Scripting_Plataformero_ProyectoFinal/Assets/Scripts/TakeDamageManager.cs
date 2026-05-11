using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeDamageManager : MonoBehaviour
{
    // Start is called before the first frame update
    private HealthManager healthManager;

    private Animator playerAnimator;
    private float invincibilityClock;
    [SerializeField] public  float  time;
   
    private SpriteRenderer spriteRenderer;
    [SerializeField] public ParticleSystem ParticlesCaller;
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
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
                spriteRenderer.color = Color.red;
        }
        else if(invincibilityClock <= 0)
        {
            spriteRenderer.color = Color.white;
        }
    }

    public void TakeDamage(float damage)
    {
        ParticlesCaller  = GetComponentInChildren<ParticleSystem>();
        var main = ParticlesCaller.main;
       
        

        main.startColor = Color.red;
        ParticlesCaller.Play();
        
            healthManager.playerHealth -= damage;
            playerAnimator.SetTrigger("Hit");
            invincibilityClock = time;
        
        

    }


}
