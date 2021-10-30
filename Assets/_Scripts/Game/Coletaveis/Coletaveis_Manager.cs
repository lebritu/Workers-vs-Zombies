using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coletaveis_Manager : MonoBehaviour
{
    [Header("STATUS")]
    public Rigidbody rb;
    public int energia_qnt;
    private Game_Manager GM;

    private bool Ativo;

    private void OnDisable()
    {
        Ativo = false;
        StopCoroutine("desativar_temp");
    }
    private void Start()
    {
        GM = FindObjectOfType<Game_Manager>();
    }

    private void Update()
    {
        if (!Ativo)
        {
            Ativo = true;
            Impulso();
        }
    }

    void Impulso()// joga o item pra cima quando desovado
    {
        rb.velocity = Vector3.zero;
        rb.AddForce(transform.up * 2, ForceMode.Impulse);
        int i = Random.Range(0, 100);
        if (i > 50)
        {
            int r = Random.Range(1, 2);
            rb.AddForce(transform.forward * r, ForceMode.Impulse);
        }
        else if (i < 50)
        {
            int l = Random.Range(1, 2);
            rb.AddForce(-transform.forward * l, ForceMode.Impulse);
        }
        StartCoroutine("desativar_temp");
    }

    private void OnMouseDown() //quando clicar com o mouse recebe a energia
    {
        GM.ChangeCarteiraValor(energia_qnt);
        GM.menu.saida_de_som.PlayOneShot(GM.menu.SP.pega_energia, GM.menu.SP.volume);
        gameObject.SetActive(false);
    }
    IEnumerator desativar_temp()
    {
        yield return new WaitForSeconds(10f);
        gameObject.SetActive(false);
    }
}
