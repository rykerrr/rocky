using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public abstract class Poolable : MonoBehaviour
{
    [SerializeField] protected string nameOfKey;

    public string NameOfKey => nameOfKey;

    private static Dictionary<string, Queue<GameObject>> objPool
        = new Dictionary<string, Queue<GameObject>>();

    public static GameObject Get(Func<GameObject> alternativeCreate, string key)
    {
        if (objPool.TryGetValue(key, out var queue) && queue.Count > 0)
        {
            var ret = queue.Dequeue() as GameObject;
            ret.GetComponent<Poolable>().Reactivate();
            return ret;
        }
        return alternativeCreate();
    }

    /// <summary>
    /// Return the object to the pool
    /// </summary>
    public void ReturnToPool()
    {
        gameObject.transform.parent = GameMaster.Instance.WaveSpawnerReference.PooledContainer;
        if (this.Reset())
        {
            var type = this.GetType();
            Queue<GameObject> queue;
            if (objPool.TryGetValue(nameOfKey, out queue))
            {
                queue.Enqueue(gameObject);
            }
            else
            {
                queue = new Queue<GameObject>();
                queue.Enqueue(gameObject);
                objPool.Add(nameOfKey, queue);
            }
        }
    }

    /// <summary>
    /// Reset the object so it is ready to go into the object pool
    /// </summary>
    /// <returns>whether the reset is successful.</returns>
    protected virtual bool Reset()
    {
        this.gameObject.SetActive(false);
        return true;
    }

    /// <summary>
    /// Reactive the object as it goes out of the object pool
    /// </summary>
    protected virtual void Reactivate()
    {
        this.gameObject.SetActive(true);
    }

    public static GameObject CreateObj(GameObject pref)
    {
        var prefCl = Instantiate(pref);
        return prefCl.gameObject;
    }

    public static void Clear()
    {
        objPool.Clear();
    }
}
