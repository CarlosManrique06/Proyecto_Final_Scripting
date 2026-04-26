using System.Collections.Generic;
using UnityEngine;

public class SwingController : MonoBehaviour
{
    public KeyCode swingKey = KeyCode.E;
    public KeyCode detachKey = KeyCode.Space;

    public float ropeLength = 3f;
    public float detachBoost = 1.15f;

    public bool IsSwinging => distanceJoint != null;
    public bool IsLaunching { get; private set; }

    private List<SwingPoint> nearbyPoints = new List<SwingPoint>();
    private int currentIndex;

    private Yoyo yoyo;
    private DistanceJoint2D distanceJoint;
    private Rigidbody2D rb;

    void Awake() => rb = GetComponent<Rigidbody2D>();

    void Update()
    {
        if (!IsSwinging && !IsLaunching && Input.GetKeyDown(swingKey) && nearbyPoints.Count > 0)
            Launch(nearbyPoints[currentIndex]);

        if (IsSwinging && (Input.GetKeyDown(detachKey) || Input.GetKeyDown(KeyCode.W)))
            Detach();
    }

    void FixedUpdate()
    {
        if (IsSwinging && rb.linearVelocity.magnitude < 15f)
            rb.AddForce(rb.linearVelocity.normalized * 1.5f, ForceMode2D.Force);
    }

    void Launch(SwingPoint target)
    {
        IsLaunching = true;
        Vector2 dir = ((Vector2)target.transform.position - (Vector2)transform.position).normalized;
        Vector2 spawnPos = (Vector2)transform.position + dir * 0.4f;

        // Pool en vez de Instantiate
        GameObject go = YoyoPool.Instance.Get(spawnPos);
        yoyo = go.GetComponent<Yoyo>();
        yoyo.LaunchToAnchor(target.transform.position, transform, () => Attach(target));
    }

    void Attach(SwingPoint target)
    {
        IsLaunching = false;

        distanceJoint = gameObject.AddComponent<DistanceJoint2D>();
        distanceJoint.autoConfigureConnectedAnchor = false;
        distanceJoint.connectedAnchor = target.transform.position;
        distanceJoint.autoConfigureDistance = false;
        distanceJoint.distance = Mathf.Min(
            Vector2.Distance(transform.position, target.transform.position), ropeLength);
        distanceJoint.maxDistanceOnly = true;
        distanceJoint.enableCollision = true;
    }

    void Detach()
    {
        Vector2 vel = rb.linearVelocity;
        Destroy(distanceJoint);
        distanceJoint = null;
        rb.linearVelocity = vel * detachBoost;

        if (yoyo != null) yoyo.ReturnToOwner();
        RefreshHighlights();
    }

    public void RegisterPoint(SwingPoint sp)
    {
        if (nearbyPoints.Contains(sp)) return;
        nearbyPoints.Add(sp);
        currentIndex = nearbyPoints.Count - 1;
        RefreshHighlights();
    }

    public void UnregisterPoint(SwingPoint sp)
    {
        nearbyPoints.Remove(sp);
        currentIndex = Mathf.Clamp(currentIndex, 0, Mathf.Max(0, nearbyPoints.Count - 1));
        sp.SetHighlight(false);
        RefreshHighlights();
    }

    void RefreshHighlights()
    {
        for (int i = 0; i < nearbyPoints.Count; i++)
            nearbyPoints[i].SetHighlight(i == currentIndex);
    }
}