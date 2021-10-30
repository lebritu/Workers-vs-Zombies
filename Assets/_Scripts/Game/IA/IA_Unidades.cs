using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class IA_Unidades : MonoBehaviour
{ 
    public enum tipo_select { Atirador,Gerador,Barricada}
    public tipo_select Tipo;

    [Header("STATUS")]
    public NavMeshAgent agent;
    public Unidades_Manager UM; // slot de onde a unidade foi designada
    public SphereCollider colisor;
    public List<IA_Zumbis> ia_zumbi_list; // armazena o zumbi que está atacando
    public int hp, max_hp; // vida da unidade
    private Game_Manager GM;
    private ObjectPooler OP;  
    private bool ativo, checkpoint_B, gerando_energia_B, morto_B;
    private Transform slot_point; // posição de chegada 

    [Header("VISUAL")]
    public Animator anim;
    public ParticleSystem hit_PS;

    [Header("ATIRADOR SETUP")]
    public float cadencia;
    public Transform arremesso_point;

    [Header("GERADOR SETUP")]
    public float time_gerador; // tempo que leva para gerar energia;
    public float max_time_gerador; // tempo que leva para gerar energia;
    public GameObject gerador_desmontado;
    public GameObject gerador;

    [Header("BARRICADA SETUP")]
    public ParticleSystem quebrabarricada_PS; // particula que aparece quando a barricada é destruida
    public GameObject barricada_desmontada;
    public GameObject barricada;

    [Header("AUDIO")]
    private AudioSource saida_de_som;
    private SoundPool SP; // É o script onde todos os sons ficam guardado

    private void OnDisable()
    {
        ativo = false;
        checkpoint_B = false;
        morto_B = false;
        gerando_energia_B = false;
        colisor.enabled = false;
        CancelInvoke("Atirador_Arremesso");
        ia_zumbi_list.Clear();
    }

    void Start()
    {
        GM = FindObjectOfType<Game_Manager>();
        agent.SetDestination(GM.wait_point.position);
        SP = FindObjectOfType<SoundPool>();
        saida_de_som = SP.Saida_De_Som;
        OP = FindObjectOfType<ObjectPooler>();
    }

    void Update()
    {
        if (!ativo)
        {
            ativo = true;
            VoiceSpawn();
            hp = max_hp;
            VerificadorDeUnidade();
        }
        UpdateAnim();
        VerificadorDeChegada();
        GeradorDeEnergia();
    }
    void VerificadorDeUnidade() // verifica que tipo de unidade é ao desovar
    {
        switch (Tipo)
        {
            case tipo_select.Gerador:
                gerador_desmontado.SetActive(true);
                gerador.SetActive(false);
                break;
            case tipo_select.Barricada:
                barricada_desmontada.SetActive(true);
                barricada.SetActive(false);
                break;
        }
    }

    public void RecebeDano(int dmg)
    {
        hp -= dmg;
        hit_PS.Play();
        saida_de_som.PlayOneShot(SP.hit, SP.volume / 2);
        if (Random.value <= 0.3) // probabilidade de uma unidade fazer um som de dor       
        {
            saida_de_som.PlayOneShot(SP.hit_unidade, SP.volume);
        }     
        if(hp <= 0)
        {
            Morte();
        }
        switch (Tipo)
        {
            case tipo_select.Barricada:
                if (Random.value <= 0.7) // probabilidade de uma unidade fazer um som de dor       
                {
                    saida_de_som.PlayOneShot(SP.hit_barricada, SP.volume);
                }
                if (!morto_B)
                {
                    anim.SetTrigger("Hit");
                }
                break;
        }
    }
    public void Morte()
    {
        if (!morto_B)
        {
            morto_B = true;
            colisor.enabled = false;
            anim.SetTrigger("Morte");
            CancelInvoke("Atirador_Arremesso");
            StartCoroutine("tempo_para_sumir");
            saida_de_som.PlayOneShot(SP.morte_unidade, SP.volume);
            UM.disponivel = true;
            switch (Tipo)
            {
                case tipo_select.Barricada:
                    barricada.SetActive(false);
                    quebrabarricada_PS.Play();
                    saida_de_som.PlayOneShot(SP.quebra_barricada, SP.volume);
                    break;
            }
            if (GM.unidade_place_B)// verifica se o jogador está preparado para colocar outra unidade 
            {
                UM.gameObject.SetActive(true);
            }
            for (int i = 0; i < ia_zumbi_list.Count; i++)
            {
                ia_zumbi_list[i].IA_unidade_morta_Callback();
            }
            return;
        }
    }
    IEnumerator tempo_para_sumir()
    {
        yield return new WaitForSeconds(8f);
        gameObject.SetActive(false);
    }

    void UpdateAnim()// responsavel pela animação da unidade
    {
        anim.SetFloat("Z", agent.velocity.magnitude, 0.1f, Time.deltaTime);
    }
    void VoiceSpawn() // comando de voz de cada unidade quando desovada
    {
        switch (Tipo)
        {
            case tipo_select.Atirador:
                saida_de_som.PlayOneShot(SP.spawn_Atirador, SP.volume);
                break;
            case tipo_select.Gerador:
                saida_de_som.PlayOneShot(SP.spawn_Gerador, SP.volume);
                break;
            case tipo_select.Barricada:
                saida_de_som.PlayOneShot(SP.spawn_Barricada, SP.volume);
                break;
        }
    }

    public void Comando(Transform t, Unidades_Manager um) // ir até o local indicado
    {
        slot_point = t;
        UM = um;
        anim.SetTrigger("Idle");
        agent.SetDestination(t.position);
        checkpoint_B = false;
        colisor.enabled = false;
        switch (Tipo)
        {
            case tipo_select.Atirador:
                saida_de_som.PlayOneShot(SP.ok_Atirador, SP.volume);
                break;
            case tipo_select.Gerador:
                saida_de_som.PlayOneShot(SP.ok_Gerador, SP.volume);
                break;
            case tipo_select.Barricada:
                saida_de_som.PlayOneShot(SP.ok_Barricada, SP.volume);
                break;
        } // comando de voz de cada unidade quando designada
    }
    void VerificadorDeChegada() // verifica se a unidade chegou até o destino
    {
        if (!checkpoint_B && UM != null)
        {
            if(Vector3.Distance(transform.position, slot_point.position) <= 0.1f)
            {
                checkpoint_B = true;
                colisor.enabled = true;
                transform.rotation = Quaternion.Euler(0, 90, 0);
                switch (Tipo)
                {
                    case tipo_select.Atirador:
                        InvokeRepeating("Atirador_Arremesso", 0.1f, cadencia);
                        break;
                    case tipo_select.Gerador:
                        Gerador_Montar();
                        break;
                    case tipo_select.Barricada:
                        Barricada_Montar();
                        break;
                }
            }
        }
    }
    void ChangePosicao() // inicia a troca de posição da unidade
    {
        if(UM != null && !GM.unidade_place_B)
        {
            GM.IA = this;
            CancelInvoke("Atirador_Arremesso");
            UM.disponivel = true;
            saida_de_som.PlayOneShot(SP.unidade_para_onde, SP.volume);
            GM.unidade_place_B = true;
            GM.ActiveSlots();
        }
    }

    void Atirador_Arremesso()
    {
        anim.SetTrigger("Arremesso");
        StartCoroutine("arremesso_temp");
    }
    IEnumerator arremesso_temp()
    {
        yield return new WaitForSeconds(0.6f);
        OP.spawnFromPool("Chave", arremesso_point.position, arremesso_point.rotation);
    }

    void Gerador_Montar()
    {
        anim.SetTrigger("MontarGerador");
        transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
        gerador.SetActive(true);
        gerando_energia_B = true;
    }
    void GeradorDeEnergia()
    {
        if (gerando_energia_B)
        {
            time_gerador += 1 * Time.deltaTime;

            if(time_gerador >= max_time_gerador)
            {
                time_gerador = 0;
                GanhaEnergia();
            }
        }
    } // responsavel por gerar energia
    void GanhaEnergia() // desova as energias
    {
        OP.spawnFromPool("Energia", arremesso_point.position, arremesso_point.rotation);
        OP.spawnFromPool("Energia", arremesso_point.position, arremesso_point.rotation);
        saida_de_som.PlayOneShot(SP.ganha_energia, SP.volume);
    }

    void Barricada_Montar()
    {
        anim.SetTrigger("MontarBarricada");
        transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
        barricada.SetActive(true);
    }

    private void OnMouseDown()
    {
        ChangePosicao();
    }
}
