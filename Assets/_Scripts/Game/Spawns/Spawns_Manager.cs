using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawns_Manager : MonoBehaviour
{
    [Header("STATUS")]
    public string id_zumbi;
    public Transform alvo;
    public ObjectPooler OP;

    string sorteioNivel1() // escolhe aleatoriamente qual zumbi que será desovado
    {
        string s = "";
        if(Random.value <= 0.1f)
        {
            s = "Hulk";
        }
        else if (Random.value <= 0.3f)
        {
            s = "Zumbi_Explosivo";
        }
        else if (Random.value <= 0.4f)
        {
            s = "Cuspidor";
        }
        else
        {
            s = "Zumbi";
        }
        id_zumbi = s;
        return s;
    }
    string sorteioNivel2() // escolhe aleatoriamente qual zumbi que será desovado
    {
        string s = "";
        if (Random.value <= 0.1f)
        {
            s = "Hulk";
        }
        else if (Random.value <= 0.4f)
        {
            s = "Zumbi_Explosivo";
        }
        else if (Random.value <= 0.5f)
        {
            s = "Cuspidor";
        }
        else
        {
            s = "Zumbi";
        }
        id_zumbi = s;
        return s;
    }
    string sorteioNivel3() // escolhe aleatoriamente qual zumbi que será desovado
    {
        string s = "";
        if (Random.value <= 0.2f)
        {
            s = "Hulk";
        }
        else if (Random.value <= 0.5f)
        {
            s = "Zumbi_Explosivo";
        }
        else if (Random.value <= 0.6f)
        {
            s = "Cuspidor";
        }
        else
        {
            s = "Zumbi";
        }
        id_zumbi = s;
        return s;
    }

    public void SpawnNivel1()
    {
        var v = OP.spawnFromPool(sorteioNivel1(), transform.position, transform.rotation);
        v.GetComponent<IA_Zumbis>().Comando(alvo);
    }
    public void SpawnNivel2()
    {
        var v = OP.spawnFromPool(sorteioNivel2(), transform.position, transform.rotation);
        v.GetComponent<IA_Zumbis>().Comando(alvo);
    }
    public void SpawnNivel3()
    {
        var v = OP.spawnFromPool(sorteioNivel3(), transform.position, transform.rotation);
        v.GetComponent<IA_Zumbis>().Comando(alvo);
    }

}
