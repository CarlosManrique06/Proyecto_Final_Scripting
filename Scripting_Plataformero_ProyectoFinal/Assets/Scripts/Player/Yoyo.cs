using System;
using UnityEngine;

public class Yoyo : MonoBehaviour
{
    public float speed = 14f;
    public float maxDistance = 5f;
    public float returnDistance = 0.35f;

    public float oscillationAmplitude = 0.35f;
    public float oscillationFrequency = 9f;

    public int damage = 1;
    public string enemyTag = "enemy";

    public LineRenderer lineRenderer;

    public bool isActive;
    public bool isAnchored;

    private enum State { Going, Returning, GoingToAnchor, Anchored }
    private State currentState;

    private Transform owner;
    private Vector2 direction;
    private Vector2 startPosition;
    private Vector2 anchorPosition;
    private float oscillationTime;
    private Action onAnchorReached;

    void Awake()
    {
        if (lineRenderer == null)
            lineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        if (!isActive) return;
        Move();
        DrawString();
    }

    

    public void Launch(Vector2 dir, Transform player)
    {
        direction = dir.normalized;
        owner = player;
        startPosition = transform.position;
        oscillationTime = 0f;
        currentState = State.Going;
        isActive = true;
    }

    public void LaunchToAnchor(Vector2 target, Transform player, Action callback)
    {
        anchorPosition = target;
        owner = player;
        direction = (target - (Vector2)transform.position).normalized;
        onAnchorReached = callback;
        currentState = State.GoingToAnchor;
        isActive = true;
    }

    public void ReturnToOwner()
    {
        isAnchored = false;
        currentState = State.Returning;
    }

    //Llamado por YoyoPool al sacar del pool 
    public void ResetState()
    {
        isActive = false;
        isAnchored = false;
        owner = null;
        onAnchorReached = null;
        oscillationTime = 0f;
        if (lineRenderer != null) lineRenderer.positionCount = 0;
    }


    void Move()
    {
        if (currentState == State.Going)
        {
            oscillationTime += Time.deltaTime;
            Vector2 perp = Vector2.Perpendicular(direction);
            float osc = Mathf.Cos(oscillationTime * oscillationFrequency)
                        * oscillationAmplitude * oscillationFrequency;

            transform.Translate((direction * speed + perp * osc) * Time.deltaTime, Space.World);

            float projected = Vector2.Dot((Vector2)transform.position - (Vector2)owner.position, direction);
            if (projected >= maxDistance) currentState = State.Returning;
        }
        else if (currentState == State.Returning)
        {
            Vector2 toOwner = ((Vector2)owner.position - (Vector2)transform.position).normalized;
            transform.Translate(toOwner * speed * Time.deltaTime, Space.World);

            if (Vector2.Distance(transform.position, owner.position) <= returnDistance)
                Deactivate();
        }
        else if (currentState == State.GoingToAnchor)
        {
            transform.Translate(direction * speed * Time.deltaTime, Space.World);

            if (Vector2.Distance(transform.position, anchorPosition) <= 0.18f)
            {
                transform.position = anchorPosition;
                currentState = State.Anchored;
                isAnchored = true;
                onAnchorReached?.Invoke();
            }
        }
        else if (currentState == State.Anchored)
        {
            transform.position = anchorPosition;
        }
    }

    void DrawString()
    {
        if (lineRenderer == null || owner == null) return;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, owner.position);
        lineRenderer.SetPosition(1, transform.position);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isActive || isAnchored) return;
        if (other.CompareTag(enemyTag))
        {
            other.GetComponent<IDamage>()?.TakeDamage(damage);
            ReturnToOwner();
        }
    }

    void Deactivate()
    {
        isActive = false;
        isAnchored = false;
        if (lineRenderer != null) lineRenderer.positionCount = 0;

        // Devolver al pool en vez de Destroy
        if (YoyoPool.Instance != null)
            YoyoPool.Instance.Return(gameObject);
        else
            Destroy(gameObject);
    }
}