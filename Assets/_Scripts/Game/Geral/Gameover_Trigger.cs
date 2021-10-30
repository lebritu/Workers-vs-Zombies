using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gameover_Trigger : MonoBehaviour
{
    public Game_Manager GM;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<IA_Zumbis>())
        {
            GM.Derrota();
            gameObject.SetActive(false);
        }
    }
}
