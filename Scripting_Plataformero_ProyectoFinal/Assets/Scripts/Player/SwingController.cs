using System.Collections.Generic;
using UnityEngine;

public class SwingController : MonoBehaviour
{
    public GameObject yoyoPrefab;

    public KeyCode swingKey = KeyCode.E;
    
    public KeyCode detachKey = KeyCode.Space;

    // Lista de puntos 
    private List<SwingPoint> nearbyPoints = new List<SwingPoint>();
    private int currentIndex;

    private Yoyo yoyo;
    private DistanceJoint2D distanceJoint;
    private Rigidbody2D rigidBody;

    public float attachImpulseForce = 5f;
    public float detachImpulseMultiplier = 0.5f;
    public float swingSpeedBoost = 1.2f;
    public float maxSwingSpeed = 20f;
    public float ropeLength = 3f;

    void Awake()
    {
        
        rigidBody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
      

        // Lanzar yoyo
        if (distanceJoint == null && yoyo == null && Input.GetKeyDown(swingKey) && nearbyPoints.Count > 0)
        {
            Launch(nearbyPoints[currentIndex]);
        }

        // Soltar yoyo
        if (distanceJoint != null && Input.GetKeyDown(detachKey))
        {
            Detach();
        }


    }
    void FixedUpdate()
    {
        if (distanceJoint != null)
        {
            Vector2 velocity = rigidBody.linearVelocity;

            float speed = velocity.magnitude;

            if (speed < maxSwingSpeed)
            {
                Vector2 direction = velocity.normalized;

               
                rigidBody.AddForce(direction * 2f, ForceMode2D.Force);
            }
        }
    }

    // Lanzar hacia un punto
    void Launch(SwingPoint target)
    {
        Vector2 direction = (target.transform.position - transform.position).normalized;
        Vector2 spawnPosition = (Vector2)transform.position + direction * 0.4f;

        GameObject newYoyoObject = Instantiate(yoyoPrefab, spawnPosition, Quaternion.identity);

        yoyo = newYoyoObject.GetComponent<Yoyo>();

        yoyo.LaunchToAnchor(target.transform.position, transform, OnYoyoReachedTarget(target));
    }

   
    System.Action OnYoyoReachedTarget(SwingPoint target)
    {
        return delegate
        {
            Attach(target);
        };
    }

    // Crear la cuerda 
    void Attach(SwingPoint target)
    {
        distanceJoint = gameObject.AddComponent<DistanceJoint2D>();

        distanceJoint.autoConfigureConnectedAnchor = false;
        distanceJoint.connectedAnchor = target.transform.position;
        distanceJoint.autoConfigureDistance = false;


        float distance = Vector2.Distance(transform.position, target.transform.position);
        distanceJoint.distance = Mathf.Min(distance, ropeLength);

        distanceJoint.maxDistanceOnly = true;
        distanceJoint.enableCollision = true;

        Vector2 direction = (target.transform.position - transform.position).normalized;
        rigidBody.AddForce(direction * attachImpulseForce, ForceMode2D.Impulse);
    }

    // Soltar
    void Detach()
    {
        
        Vector2 currentVelocity = rigidBody.linearVelocity;

        Destroy(distanceJoint);
        distanceJoint = null;

        if (yoyo != null)
        {
            yoyo.ReturnToOwner();
        }

       
        rigidBody.AddForce(currentVelocity *detachImpulseMultiplier, ForceMode2D.Impulse);

        RefreshHighlights();
    }

   
    public void RegisterPoint(SwingPoint swingPoint)
    {
        if (nearbyPoints.Contains(swingPoint))
        {
            return;
        }

        nearbyPoints.Add(swingPoint);

       
        currentIndex = nearbyPoints.Count - 1;

        RefreshHighlights();
    }

    // Quitar punto cercano
    public void UnregisterPoint(SwingPoint swingPoint)
    {
        nearbyPoints.Remove(swingPoint);

        if (currentIndex >= nearbyPoints.Count)
        {
            currentIndex = nearbyPoints.Count - 1;
        }

        if (currentIndex < 0)
        {
            currentIndex = 0;
        }

        swingPoint.SetHighlight(false);

        RefreshHighlights();
    }

    // Actualizar qué punto está seleccionado
    void RefreshHighlights()
    {
        for (int i = 0; i < nearbyPoints.Count; i++)
        {
            if (i == currentIndex)
            {
                nearbyPoints[i].SetHighlight(true);
            }
            else
            {
                nearbyPoints[i].SetHighlight(false);
            }
        }
    }
}