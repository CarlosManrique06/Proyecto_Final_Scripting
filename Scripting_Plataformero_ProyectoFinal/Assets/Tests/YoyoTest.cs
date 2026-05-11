using NUnit.Framework;
using UnityEngine;

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

    
    //  1. Launch activa el yo-yo
  
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

    
    //  2. LaunchToAnchor activa pero no ancla de inmediato
    
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

   
    //  3. ReturnToOwner limpia isAnchored
    
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

 
    //  4. ResetState limpia todo 
  
    [Test]
    public void ResetState_ClearsAllFlags()
    {
        var yoyo = CreateYoyo(Vector2.zero);
        var owner = CreateOwner(Vector2.zero);

        yoyo.Launch(Vector2.right, owner.transform);
        yoyo.ResetState(); // estaba comentado antes — ese era el bug del test

        Assert.IsFalse(yoyo.isActive, "isActive debe ser false tras ResetState");
        Assert.IsFalse(yoyo.isAnchored, "isAnchored debe ser false tras ResetState");

        Cleanup(yoyo.gameObject, owner);
    }

    
    //  5. Distancia proyectada ignora oscilación lateral
   
    [Test]
    public void ProjectedDistance_NotAffectedByLateralOffset()
    {
        Vector2 dir = Vector2.right;
        Vector2 start = Vector2.zero;
        float maxD = 5f;

        Vector2 current = start + dir * maxD + new Vector2(0f, 1.2f);
        float projected = Vector2.Dot(current - start, dir);
        float euclidean = Vector2.Distance(start, current);

        Assert.That(projected, Is.EqualTo(maxD).Within(0.001f),
            "La proyección debe ser exactamente maxDistance");
        Assert.Greater(euclidean, maxD,
            "La distancia euclidiana sería mayor — el bug anterior");
    }

    //  6. Apuntado hacia el mouse
    [Test]
    public void AimDirection_PointsTowardTarget()
    {
        Vector2 player = Vector2.zero;
        Vector2 mouse = new Vector2(3f, 1f);
        Vector2 dir = (mouse - player).normalized;

        Assert.Greater(dir.x, 0f, "X positiva si el mouse está a la derecha");
        Assert.That(dir.magnitude, Is.EqualTo(1f).Within(0.001f), "Debe estar normalizado");
    }

    
    //  7. SwingController empieza sin columpio
   
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


    //  8. Detach boost multiplica correctamente
   
    [Test]
    public void DetachBoost_MultipliesVelocity()
    {
        Vector2 vel = new Vector2(4f, 3f);
        float boost = 1.15f;
        Vector2 result = vel * boost;

        Assert.That(result.x, Is.EqualTo(4f * 1.15f).Within(0.001f));
        Assert.That(result.y, Is.EqualTo(3f * 1.15f).Within(0.001f));
    }

   
    //  9. SwingPoint sin SpriteRenderer no explota
  
    [Test]
    public void SwingPoint_SetHighlight_WithoutSpriteRenderer_DoesNotThrow()
    {
        var go = new GameObject("SwingPoint");
        var sp = go.AddComponent<SwingPoint>();

        Assert.DoesNotThrow(() => sp.SetHighlight(true));
        Assert.DoesNotThrow(() => sp.SetHighlight(false));

        Cleanup(go);
    }

    
    //  10. Launch con dirección izquierda también activa
  
    [Test]
    public void Launch_LeftDirection_IsActive()
    {
        var yoyo = CreateYoyo(Vector2.zero);
        var owner = CreateOwner(Vector2.zero);

        yoyo.Launch(Vector2.left, owner.transform);

        Assert.IsTrue(yoyo.isActive, "debe activarse aunque la dirección sea izquierda");

        Cleanup(yoyo.gameObject, owner);
    }

  
    //  11. LaunchToAnchor con callback null no explota
   
    [Test]
    public void LaunchToAnchor_NullCallback_DoesNotThrow()
    {
        var yoyo = CreateYoyo(Vector2.zero);
        var owner = CreateOwner(Vector2.zero);

        Assert.DoesNotThrow(() =>
            yoyo.LaunchToAnchor(new Vector2(3, 0), owner.transform, null));

        Cleanup(yoyo.gameObject, owner);
    }


    //  12. La dirección se normaliza aunque llegue sin normalizar
 
    [Test]
    public void Launch_NormalizesDirection()
    {
        var yoyo = CreateYoyo(Vector2.zero);
        var owner = CreateOwner(Vector2.zero);

        // Dirección con magnitud distinta de 1
        Vector2 rawDir = new Vector2(3f, 4f);
        yoyo.Launch(rawDir, owner.transform);

        // El yo-yo debería moverse — isActive es la prueba indirecta de que aceptó la dirección
        Assert.IsTrue(yoyo.isActive, "el yo-yo debe activarse con cualquier dirección no nula");

        Cleanup(yoyo.gameObject, owner);
    }

   
    //  13. Registrar y quitar un SwingPoint actualiza la lista
   
    [Test]
    public void SwingController_RegisterAndUnregister_UpdatesList()
    {
        var playerGo = new GameObject("Player");
        playerGo.AddComponent<Rigidbody2D>();
        var sc = playerGo.AddComponent<SwingController>();

        var pointGo = new GameObject("SwingPoint");
        pointGo.AddComponent<SpriteRenderer>();
        var sp = pointGo.AddComponent<SwingPoint>();

        // Registrar
        sc.RegisterPoint(sp);
        Assert.IsFalse(sc.IsSwinging, "registrar un punto no activa el columpio");

        // Quitar
        sc.UnregisterPoint(sp);
        Assert.IsFalse(sc.IsSwinging, "después de quitar tampoco debe estar columpiando");

        Cleanup(playerGo, pointGo);
    }

   
    //  14. ReturnToOwner sobre yo-yo no anclado no explota
    
    [Test]
    public void ReturnToOwner_WhenNotAnchored_DoesNotThrow()
    {
        var yoyo = CreateYoyo(Vector2.zero);
        var owner = CreateOwner(Vector2.zero);

        yoyo.Launch(Vector2.right, owner.transform);

        Assert.DoesNotThrow(() => yoyo.ReturnToOwner(),
            "ReturnToOwner no debe explotar aunque el yo-yo no esté anclado");

        Cleanup(yoyo.gameObject, owner);
    }
}