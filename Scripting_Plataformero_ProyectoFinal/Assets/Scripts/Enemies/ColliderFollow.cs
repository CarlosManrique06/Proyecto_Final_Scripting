using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderFollow : MonoBehaviour
{
    private BoxCollider2D col;
    EnemyPatrol enemy;
    Fly_Follow_Shoot shoot;
    // Start is called before the first frame update
    void Start()
    {
       enemy = GetComponent<EnemyPatrol>();
        shoot = GetComponent<Fly_Follow_Shoot>();
        col = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (shoot.enabled == true && enemy.enabled == true)
        {
            col.enabled = false;
        }
        else if (shoot.enabled == true && enemy.enabled == false)
        {
            col.enabled = true;
        }
    }
}
