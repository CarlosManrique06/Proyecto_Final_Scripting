using System.Collections.Generic;
using UnityEngine;


public class YoyoPool : MonoBehaviour
{
    public static YoyoPool Instance { get; private set; }

    public GameObject yoyoPrefab;

    
    public int initialSize = 3;

    private Queue<GameObject> pool = new Queue<GameObject>();

    

    void Awake()
    {
        // Singleton simple
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        // Pre-llenar el pool
        for (int i = 0; i < initialSize; i++)
        {
            GameObject go = CreateNew();
            go.SetActive(false);
            pool.Enqueue(go);
        }
    }

    public GameObject Get(Vector2 position)
    {
        GameObject go;

        if (pool.Count > 0)
        {
            go = pool.Dequeue();
        }
        else
        {
            
            go = CreateNew();
        }

        go.transform.position = position;
        go.transform.rotation = Quaternion.identity;
        go.SetActive(true);

        // Resetear el componente Yoyo para que empiece limpio
        Yoyo yoyo = go.GetComponent<Yoyo>();
        if (yoyo != null) yoyo.ResetState();

        return go;
    }
    //Devuelve un yo-yo al pool en vez de destruirlo.
    public void Return(GameObject go)
    {
        if (go == null) return;
        go.SetActive(false);
        pool.Enqueue(go);
    }

   

    GameObject CreateNew()
    {
        GameObject go = Instantiate(yoyoPrefab, transform); // hijo del pool
        go.SetActive(false);
        return go;
    }
}
