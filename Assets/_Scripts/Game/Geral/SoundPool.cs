using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPool : MonoBehaviour
{
    //Esse script é uma piscina onde é armazenado todos os audios do game

    public float volume, volume_musica;
    public AudioSource music_play, Saida_De_Som;

    public AudioClip usaCard, carregaCard, sem_energia, pega_energia, ganha_energia, explosao, impacto, morte_unidade, morte_zumbi, hit_unidade, hit_zumbi, hit_barricada, quebra_barricada;
    public AudioClip muda_volume, hit, vitoria, derrota;
    public AudioClip spawn_Atirador, spawn_Gerador, spawn_Barricada, spawn_Detonador;
    public AudioClip ok_Atirador, ok_Gerador, ok_Barricada, unidade_para_onde;
}
