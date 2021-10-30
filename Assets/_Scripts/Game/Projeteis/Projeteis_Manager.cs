using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projeteis_Manager : MonoBehaviour
{
    public enum Tipo_Select { Chave, Cuspe}
    public Tipo_Select Tipo;
    [Header("STATUS")]
    public float speed;

    private bool ativo;

    private void OnDisable()
    {
        ativo = false;
    }

    private void Update()
    {
        if (!ativo)
        {
            ativo = true;
            StartCoroutine("desativar_temp");
        }
        transportPosition();
    }

    void transportPosition() // sitema de transporte simples
    {
        switch (Tipo)
        {
            case Tipo_Select.Chave:
                transform.position = new Vector3(transform.position.x + speed * Time.deltaTime, transform.position.y, transform.position.z);
                break;
            case Tipo_Select.Cuspe:
                transform.position = new Vector3(transform.position.x - speed * Time.deltaTime, transform.position.y, transform.position.z);
                break;
        }
    }

    IEnumerator desativar_temp()
    {
        yield return new WaitForSeconds(10f);
        gameObject.SetActive(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        switch (Tipo)
        {
            case Tipo_Select.Chave:
                if (other.GetComponent<IA_Zumbis>())
                {
                    other.GetComponent<IA_Zumbis>().RecebeDano(1);
                    gameObject.SetActive(false);
                }
                if (other.CompareTag("Limitador")) // essa condição é para limitar o projetil para que ele não cause spawnkill
                {
                    gameObject.SetActive(false);
                }
                break;
            case Tipo_Select.Cuspe:
                if (other.GetComponent<IA_Unidades>())
                {
                    other.GetComponent<IA_Unidades>().RecebeDano(1);
                    gameObject.SetActive(false);
                }
                break;
        }
    }
}
