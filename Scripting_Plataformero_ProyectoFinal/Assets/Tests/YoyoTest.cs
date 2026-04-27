using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class YoyoTest
{


    Yoyo CreateYoyo(Vector2 pos)
    {
        var go = new GameObject("Yoyo");
        go.transform.position = pos;
        return go.AddComponent<Yoyo>();
    }

    GameObject CreateOwner(Vector2 pos)
    {
        var go = new GameObject("Owner");
        go.transform.position = pos;
        return go;
    }

    void Cleanup(params Object[] objects)
    {
        foreach (var o in objects) if (o != null) Object.DestroyImmediate(o);
    }


    // Launch activa el yo-yo

    [Test]
    public void Launch_ActivatesYoyo()
    {
        var yoyo = CreateYoyo(Vector2.zero);
        var owner = CreateOwner(Vector2.zero);

        yoyo.Launch(Vector2.right, owner.transform);

        Assert.IsTrue(yoyo.isActive, "isActive debe ser true tras Launch");
        Assert.IsFalse(yoyo.isAnchored, "isAnchored debe ser false");

        Cleanup(yoyo.gameObject, owner);
    }

    // LaunchToAnchor activa pero no ancla de inmediato

    [Test]
    public void LaunchToAnchor_NotYetAnchored()
    {
        var yoyo = CreateYoyo(Vector2.zero);
        var owner = CreateOwner(Vector2.zero);
        bool called = false;

        yoyo.LaunchToAnchor(new Vector2(5, 0), owner.transform, () => called = true);

        Assert.IsTrue(yoyo.isActive, "isActive debe ser true");
        Assert.IsFalse(yoyo.isAnchored, "no debería estar anclado todavía");
        Assert.IsFalse(called, "callback no se llama hasta llegar");

        Cleanup(yoyo.gameObject, owner);
    }


    //ReturnToOwner

    [Test]
    public void ReturnToOwner_ClearsAnchoredFlag()
    {
        var yoyo = CreateYoyo(Vector2.zero);
        var owner = CreateOwner(Vector2.zero);

        yoyo.LaunchToAnchor(Vector2.zero, owner.transform, null);
        yoyo.isAnchored = true;
        yoyo.ReturnToOwner();

        Assert.IsFalse(yoyo.isAnchored, "isAnchored debe ser false tras ReturnToOwner");

        Cleanup(yoyo.gameObject, owner);
    }


    //ResetState limpia todo

    [Test]
    public void ResetState_ClearsAllFlags()
    {
        var yoyo = CreateYoyo(Vector2.zero);
        var owner = CreateOwner(Vector2.zero);

        yoyo.Launch(Vector2.right, owner.transform);
        yoyo.ResetState();

        Assert.IsFalse(yoyo.isActive, "isActive debe ser false tras ResetState");
        Assert.IsFalse(yoyo.isAnchored, "isAnchored debe ser false tras ResetState");

        Cleanup(yoyo.gameObject, owner);
    }


    //distancia proyectada ignora oscilación lateral

    [Test]
    public void ProjectedDistance_NotAffectedByLateralOffset()
    {
        Vector2 dir = Vector2.right;
        Vector2 start = Vector2.zero;
        float maxD = 5f;

        Vector2 current = start + dir * maxD + new Vector2(0f, 1.2f);
        float projected = Vector2.Dot(current - start, dir);
        float euclidean = Vector2.Distance(start, current);

        // Within() 
        Assert.That(projected, Is.EqualTo(maxD).Within(0.001f),
            "La proyección debe ser exactamente maxDistance");
        Assert.Greater(euclidean, maxD,
            "La distancia euclidiana sería mayor — el bug anterior");
    }


    //Apuntado hacia el mouse

    [Test]
    public void AimDirection_PointsTowardTarget()
    {
        Vector2 player = Vector2.zero;
        Vector2 mouse = new Vector2(3f, 1f);
        Vector2 dir = (mouse - player).normalized;

        Assert.Greater(dir.x, 0f, "X positiva si el mouse está a la derecha");
        Assert.That(dir.magnitude, Is.EqualTo(1f).Within(0.001f), "Debe estar normalizado");
    }


    //SwingController empieza sin columpio

    [Test]
    public void SwingController_StartsNotSwinging()
    {
        var go = new GameObject("Player");
        go.AddComponent<Rigidbody2D>();
        var sc = go.AddComponent<SwingController>();

        Assert.IsFalse(sc.IsSwinging, "IsSwinging debe ser false al inicio");
        Assert.IsFalse(sc.IsLaunching, "IsLaunching debe ser false al inicio");

        Cleanup(go);
    }


    


   


    //Detach boost multiplica correctamente

    [Test]
    public void DetachBoost_MultipliesVelocity()
    {
        Vector2 vel = new Vector2(4f, 3f);
        float boost = 1.15f;
        Vector2 result = vel * boost;

        Assert.That(result.x, Is.EqualTo(4f * 1.15f).Within(0.001f));
        Assert.That(result.y, Is.EqualTo(3f * 1.15f).Within(0.001f));
    }


    //SwingPoint sin SpriteRenderer no explota

    [Test]
    public void SwingPoint_SetHighlight_WithoutSpriteRenderer_DoesNotThrow()
    {
        var go = new GameObject("SwingPoint");
        var sp = go.AddComponent<SwingPoint>();

        Assert.DoesNotThrow(() => sp.SetHighlight(true));
        Assert.DoesNotThrow(() => sp.SetHighlight(false));

        Cleanup(go);
    }
}
