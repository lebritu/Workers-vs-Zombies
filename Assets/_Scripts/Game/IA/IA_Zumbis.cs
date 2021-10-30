using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IA_Zumbis : MonoBehaviour
{
    public enum Tipo_Select { Zumbi, Explosivo, Cuspidor, Hulk}
    public Tipo_Select Tipo;

    [Header("STATUS")]
    public int hp;
    public int max_hp;
    public int Dano; // o numero de dano que este inimigo pode causar
    public float speed;
    public float cadencia;
    public SphereCollider colisor;
    public Transform alvo;
    private ObjectPooler OP;
    private Game_Manager GM;
    private IA_Unidades ia_unidade; // armazena a unida que esta sendo atacada
    public bool ativo, indo_para_o_alvo_B, morto_B;
    public bool FinalWave; // condição para saber se são os ultimos zumbis

    [Header("VISUAL")]
    public Animator anim;
    public ParticleSystem hit_PS, morte_sangue_PS;
    public GameObject corpo_visual; 

    [Header("ZUMBI EXPLOSIVO")]
    public ParticleSystem explosao_PS;

    [Header("ZUMBI HULK")]
    public ParticleSystem impacto_PS;

    [Header("ZUMBI CUSPIDOR")]
    public Transform cuspe_point;
    private bool perto_da_unidade; // condição expecial para o cuspidor quando chega perto da unidade

    [Header("AUDIO")]
    private AudioSource saida_de_som;
    private SoundPool SP; // É o script onde todos os sons ficam guardado

    private void OnDisable()
    {
        ativo = false;
        colisor.enabled = true;
        indo_para_o_alvo_B = false;
        perto_da_unidade = false;
        CancelInvoke("Atacando");
        corpo_visual.SetActive(true);
    }
    void Start()
    {
        GM = FindObjectOfType<Game_Manager>();
        SP = FindObjectOfType<SoundPool>();
        saida_de_som = SP.Saida_De_Som;
        OP = FindObjectOfType<ObjectPooler>();
    }


    void Update()
    {
        IrAtéOAlvo();
    }

    void IrAtéOAlvo()
    {
        if (indo_para_o_alvo_B)
        {
            transform.position =  Vector3.MoveTowards(transform.position, alvo.position, speed * Time.deltaTime);
        }
    }

    public void RecebeDano(int dmg)
    {
        hp -= dmg;
        hit_PS.Play();
        if (Random.value <= 0.3) // probabilidade do zumbi fazer um som de dor       
        {
            saida_de_som.PlayOneShot(SP.hit_zumbi, SP.volume);
        }
        saida_de_som.PlayOneShot(SP.hit, SP.volume/2);
        if (hp <= 0)
        {
            Morte();
        }
    }
    public void Morte()
    {
        if (!morto_B)
        {
            morto_B = true;
            colisor.enabled = false;
            indo_para_o_alvo_B = false;
            morte_sangue_PS.Play();
            if (FinalWave)
            {
                GM.CallBackFinalWave();
            }
            if(ia_unidade != null)
            {
                ia_unidade.ia_zumbi_list.Remove(this);
            }
            anim.SetTrigger("Morte");
            StartCoroutine("tempo_para_sumir");
            CancelInvoke("Atacando");
            CancelInvoke("Cuspindo");
            StopCoroutine("atack_temp");
            StopCoroutine("cuspe_temp");
            if(Random.value <= 0.6f) // probabilidade do zumbi fazer um som de morte
            {
                saida_de_som.PlayOneShot(SP.morte_zumbi, SP.volume);
            }

        }
    }

    public void Comando(Transform t) // marca o local indicado
    {
        alvo = t;
        indo_para_o_alvo_B = true;
        anim.SetBool("Andar", true);

        switch (Tipo)
        {
            case Tipo_Select.Cuspidor:
                InvokeRepeating("Cuspindo", 10f, cadencia);
                break;
        }
    }
    void IniciarAtack(IA_Unidades ia)
    {
        ia_unidade = ia;
        ia_unidade.ia_zumbi_list.Add(this);
        indo_para_o_alvo_B = false;
        anim.SetBool("Andar", false);

        switch (Tipo)
        {
            case Tipo_Select.Zumbi:
                InvokeRepeating("Atacando", 0.1f, cadencia);
                break;
            case Tipo_Select.Explosivo:
                Explode();
                break;
            case Tipo_Select.Cuspidor:
                perto_da_unidade = true;
                break;
            case Tipo_Select.Hulk:
                InvokeRepeating("Atacando", 0.1f, cadencia);
                break;
        }
    }

    void Atacando()
    {
        anim.SetTrigger("Zumbi_Atack");
        StartCoroutine("atack_temp");
    }
    IEnumerator atack_temp()
    {
        yield return new WaitForSeconds(1.5f);
        ia_unidade.RecebeDano(Dano);
        switch (Tipo)
        {
            case Tipo_Select.Hulk:
                impacto_PS.Play();
                saida_de_som.PlayOneShot(SP.impacto, SP.volume);
                break;
        }
    }

    void Cuspindo()
    {
        anim.SetTrigger("Zumbi_Cuspe");
        StartCoroutine("cuspe_temp");
        indo_para_o_alvo_B = false;
    }
    IEnumerator cuspe_temp()
    {
        yield return new WaitForSeconds(0.5f);
        OP.spawnFromPool("Cuspe", cuspe_point.position, cuspe_point.rotation);
        yield return new WaitForSeconds(0.6f);
        if (!perto_da_unidade)
        {
            indo_para_o_alvo_B = true;
        }

    }

    void Explode() // quando o zumbi explosivo encostar na unidade inimiga ele explode e mata a unidade instantanêamente
    {
        explosao_PS.Play();
        saida_de_som.PlayOneShot(SP.explosao, SP.volume);
        ia_unidade.Morte();
        corpo_visual.SetActive(false);
        StartCoroutine("tempo_para_sumir");
    }
    IEnumerator tempo_para_sumir()
    {
        yield return new WaitForSeconds(8f);
        gameObject.SetActive(false);
    }

    public void IA_unidade_morta_Callback() // quando a unidade que esta sendo atacada for derrotada ela aciona está função
    {
        colisor.enabled = false;
        CancelInvoke("Atacando");
        StopCoroutine("atack_temp");
        indo_para_o_alvo_B = true;
        anim.SetBool("Andar", true);
        colisor.enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<IA_Unidades>())
        {
            IniciarAtack(other.GetComponent<IA_Unidades>());
        }
    }
}
