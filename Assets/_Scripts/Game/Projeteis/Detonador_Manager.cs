using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detonador_Manager : MonoBehaviour
{
    [Header("STATUS")]
    public Unidades_Manager UM;

    [Header("VISUAL")]
    public ParticleSystem explosion_PS;
    public GameObject visual_GO;
    public SphereCollider colisor;

    [Header("AUDIO")]
    private AudioSource saida_de_som;
    private SoundPool SP; // É o script onde todos os sons ficam guardado

    private void OnDisable()
    {
        colisor.enabled = true;
        visual_GO.SetActive(true);
    }
    void Start()
    {
        SP = FindObjectOfType<SoundPool>();
        saida_de_som = SP.Saida_De_Som;
    }

    void Explode()
    {
        explosion_PS.Play();
        saida_de_som.PlayOneShot(SP.explosao, SP.volume);
        colisor.enabled = false;
        visual_GO.SetActive(false);
        StartCoroutine("desativar_temp");
        UM.disponivel = true;
    }
    IEnumerator desativar_temp()
    {
        yield return new WaitForSeconds(10f);
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<IA_Zumbis>())
        {
            other.GetComponent<IA_Zumbis>().Morte();
            Explode();
        }
    }
}
