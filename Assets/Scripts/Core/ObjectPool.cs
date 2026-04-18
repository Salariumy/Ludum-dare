using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 通用对象池：避免频繁 Instantiate/Destroy 带来的性能开销
/// 用法：ObjectPool.Instance.Get(prefab) / ObjectPool.Instance.Return(prefab, obj)
/// </summary>
public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance { get; private set; }

    private Dictionary<GameObject, Queue<GameObject>> pools = new Dictionary<GameObject, Queue<GameObject>>();

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    /// <summary>从池中取出一个对象，没有就新建</summary>
    public GameObject Get(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        if (!pools.ContainsKey(prefab))
            pools[prefab] = new Queue<GameObject>();

        GameObject obj;
        if (pools[prefab].Count > 0)
        {
            obj = pools[prefab].Dequeue();
            obj.transform.SetParent(parent);
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            obj.SetActive(true);
        }
        else
        {
            obj = Instantiate(prefab, position, rotation, parent);
            // 记录该对象对应的预制体，方便归还
            var tag = obj.AddComponent<PoolTag>();
            tag.Prefab = prefab;
        }
        return obj;
    }

    /// <summary>归还对象到池中</summary>
    public void Return(GameObject obj)
    {
        var tag = obj.GetComponent<PoolTag>();
        if (tag == null || tag.Prefab == null)
        {
            Destroy(obj);
            return;
        }

        obj.SetActive(false);
        obj.transform.SetParent(transform); // 归到 ObjectPool 节点下

        if (!pools.ContainsKey(tag.Prefab))
            pools[tag.Prefab] = new Queue<GameObject>();

        pools[tag.Prefab].Enqueue(obj);
    }
}

/// <summary>挂在池化对象上，标记其对应的预制体</summary>
public class PoolTag : MonoBehaviour
{
    [HideInInspector] public GameObject Prefab;
}
