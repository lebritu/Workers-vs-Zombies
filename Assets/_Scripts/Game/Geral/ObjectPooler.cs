using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    //Esse é um script para otimizar o jogo e evitar de fazer muitos instantiete 

    [System.Serializable]
    public class Pool
    {
        public string Tag;
        public GameObject Prefab;
        public int Size;
    }

    public List<Pool> Pools;

    public Dictionary<string, Queue<GameObject>> PoolDictionary;

    private void Start() // Função responsavel por instanciar os prefabs ao iniciar o jogo 
    {
        PoolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool Pool in Pools)
        {
            Queue<GameObject> objecPool = new Queue<GameObject>();

            for (int i = 0; i < Pool.Size; i++)
            {
                GameObject obj = Instantiate(Pool.Prefab);
                obj.SetActive(false);
                objecPool.Enqueue(obj);
            }

            PoolDictionary.Add(Pool.Tag, objecPool);
        }
    }

    public GameObject spawnFromPool(string tag, Vector3 Position, Quaternion rotation) // função responsavel por desovar os prefabs
    {
        if (!PoolDictionary.ContainsKey(tag))
        {
            Debug.Log("Prefab com a tag " + tag + " Não existe");
            return null;
        }

        GameObject objectToSpawn = PoolDictionary[tag].Dequeue();

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = Position;
        objectToSpawn.transform.rotation = rotation;

        IpoolObject PooledObject = objectToSpawn.GetComponent<IpoolObject>();

        if(PooledObject != null)
        {
            PooledObject.OnObjectSpawn();
        }

        PoolDictionary[tag].Enqueue(objectToSpawn);

        return objectToSpawn;
    }
    private void OnDestroy()
    {
        PoolDictionary.Clear();
        Pools.Clear();
    }

}
