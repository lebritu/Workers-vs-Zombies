using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu_Manager : MonoBehaviour
{
    [Header("STATUS")]
    public Game_Manager GM;

    [Header("VISUAL")]
    public GameObject menu_UI;
    public GameObject Game_UI;
    public GameObject Opcoes_UI;
    public Animator contador_anim; // animator do contador para iniciar a partida
    public Animator menu_anim; // animator do menu
    public int volume_musica_int;
    public GameObject[] volume_musica_GO;
    public int volume_efeito_sonoro_int;
    public GameObject[] volume_efeito_sonoro_GO;


    [Header("AUDIO")]
    public AudioSource saida_de_som;
    public SoundPool SP; // É o script onde todos os sons ficam guardado

    public void StartGame()
    {      
        menu_anim.SetTrigger("Play");
        StartCoroutine("start_temp");
    }
    IEnumerator start_temp()
    {
        yield return new WaitForSeconds(0.5f);
        contador_anim.SetTrigger("Play");
        menu_UI.SetActive(false);
        Game_UI.SetActive(true);
        yield return new WaitForSeconds(3f);
        GM.StartGame();
    }
    public void QuitGame()
    {
        Application.Quit();
    }

    public void Abre_Opcoes()
    {
        Opcoes_UI.SetActive(true);
        Game_UI.SetActive(false);
        Time.timeScale = 0;
    }
    public void Fecha_Opcoes()
    {
        Opcoes_UI.SetActive(false);
        Game_UI.SetActive(true);
        Time.timeScale = 1;
    }

    public void Volume_Aumenta_Musica()
    {
        if (volume_musica_int < 9)
        {
            volume_musica_int++;
            SP.volume_musica += 0.1f;
            saida_de_som.PlayOneShot(SP.muda_volume, SP.volume_musica);
            SP.music_play.volume = SP.volume_musica;
            volume_musica_GO[volume_musica_int - 1].SetActive(true);
        }
    }
    public void Volume_Abaixa_Musica()
    {
        if (volume_musica_int > 0)
        {
            volume_musica_GO[volume_musica_int - 1].SetActive(false);
            volume_musica_int--;
            if (volume_musica_int == 0)
            {
                SP.volume_musica = 0;
            }
            else
            {
                SP.volume_musica -= 0.1f;
            }
            SP.music_play.volume = SP.volume_musica;
            saida_de_som.PlayOneShot(SP.muda_volume, SP.volume_musica);
        }
    }
    public void Volume_Aumenta_Efeito_Sonoro()
    {
        if (volume_efeito_sonoro_int < 9)
        {
            volume_efeito_sonoro_int++;
            SP.volume += 0.1f;
            saida_de_som.PlayOneShot(SP.muda_volume, SP.volume);
            volume_efeito_sonoro_GO[volume_efeito_sonoro_int - 1].SetActive(true);
        }
    }
    public void Volume_Abaixa_Efeito_Sonoro()
    {
        if (volume_efeito_sonoro_int > 0)
        {
            volume_efeito_sonoro_GO[volume_efeito_sonoro_int - 1].SetActive(false);
            volume_efeito_sonoro_int--;
            if (volume_efeito_sonoro_int == 0)
            {
                SP.volume = 0;
            }
            else
            {
                SP.volume -= 0.1f;
            }
            saida_de_som.PlayOneShot(SP.muda_volume, SP.volume);
        }
    }
}
