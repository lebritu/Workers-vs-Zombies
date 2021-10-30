using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Card_Manager : MonoBehaviour
{
    [Header("STATUS")]
    public Card_Item card; // É a base do card
    public Game_Manager GM; // É o controlador geral da partida
    private float delay;
    private float max_Delay; // É o tempo maximo de carregamento do card
    public float time_speed = 1; // É a velocidade que o card vai levar para carregar
    private bool carregado_B; // É a condição quando estiver carregado e poder usar o card
    private int preco_i; // É o valor do card
    public ObjectPooler OP; // É o script onde todos os prefabs ficam armazendados;

    [Header("VISUAL")]
    public Image conteudo_delay_img;
    public Image icone_img;
    public Image card_Fundo_img; // É o fundo que mostra visualmente se o card está ativo ou não
    public TMP_Text preco_txt;
    public TMP_Text nome_txt;
    public Animator anim, carteira_anim;
    public Color carregado_cor, descarregado_cor; // São as cores do card quando ativo ou não

    [Header("AUDIO")]
    public AudioSource saida_de_som;
    public SoundPool SP; // É o script onde todos os sons ficam guardado


    void Start()
    {
        ChangeCard(card.icon, card.preco, card.nome, card.maxDelay);
    }

    void Update()
    {
        Carregando();
    }

    void Carregando() // Está é a função que será responsavel pelo carregamento dos cards  
    {
        if (!carregado_B && GM.Play_B)
        {
            delay += time_speed * Time.deltaTime; 
            conteudo_delay_img.fillAmount = delay / max_Delay;
            if(delay >= max_Delay)
            {
                carregado_B = true;
                delay = max_Delay;
                card_Fundo_img.color = carregado_cor;
                anim.SetTrigger("Carregado");
                saida_de_som.PlayOneShot(SP.carregaCard, SP.volume/2);
            }
        }
    }

    public void ChangeCard(Sprite icone, int preco, string nome, float maxDelay) // Está função é responsavel pela atualização do visual do card durante a gameplay
    {       
        icone_img.sprite = icone;
        preco_txt.text = preco.ToString();
        nome_txt.text = nome.ToString();
        max_Delay = maxDelay;
        preco_i = preco;
    }

    public void UsaCard() // Essa função é a responsavel pela utilização dos cards
    {
        if (carregado_B && !GM.unidade_place_B)
        {
            if(GM.carteira >= preco_i)
            {
                GM.ChangeCarteiraValor(-preco_i);
                GM.unidade_place_B = true;
                delay = 0;
                conteudo_delay_img.fillAmount = delay / max_Delay;
                card_Fundo_img.color = descarregado_cor;
                saida_de_som.PlayOneShot(SP.usaCard, SP.volume);

                switch (card.Tipo) // é a condição que irá ativar o tipo de card
                {
                    case Card_Item.TipoCard.Atirador:
                        var v = OP.spawnFromPool("Atirador", GM.spawn_point.position, GM.spawn_point.rotation);
                        GM.IA = v.GetComponent<IA_Unidades>();
                        GM.ActiveSlots();
                        break;
                    case Card_Item.TipoCard.Gerador:
                        var v2 = OP.spawnFromPool("Gerador", GM.spawn_point.position, GM.spawn_point.rotation);
                        GM.IA = v2.GetComponent<IA_Unidades>();
                        GM.ActiveSlots();
                        break;
                    case Card_Item.TipoCard.Barricada:
                        var v3 = OP.spawnFromPool("Barricada", GM.spawn_point.position, GM.spawn_point.rotation);
                        GM.IA = v3.GetComponent<IA_Unidades>();
                        GM.ActiveSlots();
                        break;
                    case Card_Item.TipoCard.Detonador:
                        GM.detonador_B = true;
                        GM.ActiveSlots();
                        break;
                }
                carregado_B = false;
            }
            else
            {
                carteira_anim.SetTrigger("SemEnergia");
                saida_de_som.PlayOneShot(SP.sem_energia, SP.volume);
            }
        }
    }
}
