using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Card_Manager : MonoBehaviour
{
    [Header("STATUS")]
    public Card_Item card; // � a base do card
    public Game_Manager GM; // � o controlador geral da partida
    private float delay;
    private float max_Delay; // � o tempo maximo de carregamento do card
    public float time_speed = 1; // � a velocidade que o card vai levar para carregar
    private bool carregado_B; // � a condi��o quando estiver carregado e poder usar o card
    private int preco_i; // � o valor do card
    public ObjectPooler OP; // � o script onde todos os prefabs ficam armazendados;

    [Header("VISUAL")]
    public Image conteudo_delay_img;
    public Image icone_img;
    public Image card_Fundo_img; // � o fundo que mostra visualmente se o card est� ativo ou n�o
    public TMP_Text preco_txt;
    public TMP_Text nome_txt;
    public Animator anim, carteira_anim;
    public Color carregado_cor, descarregado_cor; // S�o as cores do card quando ativo ou n�o

    [Header("AUDIO")]
    public AudioSource saida_de_som;
    public SoundPool SP; // � o script onde todos os sons ficam guardado


    void Start()
    {
        ChangeCard(card.icon, card.preco, card.nome, card.maxDelay);
    }

    void Update()
    {
        Carregando();
    }

    void Carregando() // Est� � a fun��o que ser� responsavel pelo carregamento dos cards  
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

    public void ChangeCard(Sprite icone, int preco, string nome, float maxDelay) // Est� fun��o � responsavel pela atualiza��o do visual do card durante a gameplay
    {       
        icone_img.sprite = icone;
        preco_txt.text = preco.ToString();
        nome_txt.text = nome.ToString();
        max_Delay = maxDelay;
        preco_i = preco;
    }

    public void UsaCard() // Essa fun��o � a responsavel pela utiliza��o dos cards
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

                switch (card.Tipo) // � a condi��o que ir� ativar o tipo de card
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
