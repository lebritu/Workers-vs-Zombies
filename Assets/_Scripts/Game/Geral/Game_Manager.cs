using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Game_Manager : MonoBehaviour
{
    [Header("STATUS")]
    public int carteira;
    public float time_speed; // Velocidade que o tempo corre
    public float time_game; // É o tempo que decorre para vencer a partida
    public float max_time_game; // É o tempo limite para vencer a partida
    public float intervalo_spawn; // Intervalo entre os spawns de zumbi
    public float intervalo_spawn_nivel_2; // Intervalo entre os spawns de zumbi nivel 2
    public float intervalo_spawn_nivel_3; // Intervalo entre os spawns de zumbi nivel 3
    public float time_start_spawn; // Tempo para iniciar os spawns de zumbi
    public bool unidade_place_B; // condição que libera a unidade a ser colocada
    public Transform spawn_point; // Local onde as unidades nascem
    public Transform wait_point; // Local onde as unidades esperam
    public IA_Unidades IA; // Variavel onde armazena a unidade comprada
    public Menu_Manager menu;
    public ObjectPooler OP;
    public Spawns_Manager[] Spawns;
    public Unidades_Manager[] unidade_slots; // são os slots disponiveis no nivel
    public List<IA_Zumbis> zumbis_restantes; // são os ultimos zumbis do nivel
    private int zumbis_restantes_qnt; // quantidade de zumbis restantes
    public bool Play_B; // Condição do game iniciado;
    public bool detonador_B; // condição exclusiva para usar o detonador
    private bool Nivel2_B, Nivel3_B, Final_B;

    [Header("VISUAL")]
    public TMP_Text Carteira_txt;
    public Image time_game_img;
    public GameObject vitoria_painel;
    public GameObject derrota_painel;
   
    
    void Update()
    {
        DisplayTimeGame();
        if (Input.GetKeyDown(KeyCode.P))
        {
            FinalWave();
        }
    }
    void DisplayTimeGame() // Atualiza o cronometro do game
    {
        if (Play_B)
        {
            time_game += time_speed * Time.deltaTime;
            time_game_img.fillAmount = time_game / max_time_game;

            if(time_game >= max_time_game && !Final_B)
            {
                Final_B = true;
                FinalWave();
            }
            if(time_game >= max_time_game / 3f && !Nivel2_B) // aqui verifica e aumenta o nivel da horda
            {
                Nivel2_B = true;
                Nivel2();
            }
            if (time_game >= max_time_game / 2f && !Nivel3_B) // aqui verifica e aumenta o nivel da horda
            {
                Nivel3_B = true;
                Nivel3();
            }
        }
    }

    void FinalWave() // verifica e adiciona todos os zumbis em campo os tornando os ultimos
    {
        CancelInvoke("StartSpawnsNivel1");
        CancelInvoke("StartSpawnsNivel2");
        CancelInvoke("StartSpawnsNivel3");

        IA_Zumbis[] iaz = FindObjectsOfType<IA_Zumbis>();
        for (int i = 0; i < iaz.Length; i++)
        {
            zumbis_restantes.Add(iaz[i]);
            iaz[i].FinalWave = true;
        }
        zumbis_restantes_qnt = iaz.Length;
    }
    public void CallBackFinalWave() // recebe um sinal de cada zumbi morto depois da função Finalwave
    {
        zumbis_restantes_qnt++;

        if(zumbis_restantes_qnt >= zumbis_restantes.Count)
        {
            Play_B = false;
            Vitoria();
        }
    }
    void Vitoria()
    {
        menu.saida_de_som.PlayOneShot(menu.SP.vitoria, menu.SP.volume);
        vitoria_painel.SetActive(true);
       
    }
    public void Derrota()
    {
        menu.saida_de_som.PlayOneShot(menu.SP.derrota, menu.SP.volume);
        derrota_painel.SetActive(true);
        CancelInvoke("StartSpawnsNivel1");
        CancelInvoke("StartSpawnsNivel2");
        CancelInvoke("StartSpawnsNivel3");
        Time.timeScale = 0;
    }

    public void StartGame()
    {
        InvokeRepeating("StartSpawnsNivel1", time_start_spawn, intervalo_spawn);
        Play_B = true;
    }

    void StartSpawnsNivel1()
    {
        int ale = Random.Range(0, Spawns.Length);
        Spawns[ale].SpawnNivel1();
    }
    void StartSpawnsNivel2()
    {
        int ale = Random.Range(0, Spawns.Length);
        Spawns[ale].SpawnNivel2();
    }
    void StartSpawnsNivel3()
    {
        int ale = Random.Range(0, Spawns.Length);
        Spawns[ale].SpawnNivel3();
    }

    void Nivel2() // essa função deixa o jogo mais dificil
    {
        CancelInvoke("StartSpawnsNivel1");
        InvokeRepeating("StartSpawnsNivel2", 0.1f, intervalo_spawn_nivel_2);
    }
    void Nivel3() // essa função deixa o jogo mais dificil
    {
        CancelInvoke("StartSpawnsNivel2");
        InvokeRepeating("StartSpawnsNivel3", 0.1f, intervalo_spawn_nivel_3);
    }

    public void ChangeCarteiraValor(int i) // Função responsavel pela atualização da carteira
    {
        carteira += i;
        Carteira_txt.text = carteira.ToString();
    }
    public void ActiveSlots() // Função responsavel por ativar slots que ainda estão vazios
    {
        foreach (Unidades_Manager uni in unidade_slots)
        {
            if (uni.disponivel)
            {
                uni.gameObject.SetActive(true);
            }
        }
    }
    public void DesactiveSlots() // Função responsavel por desativar os slots
    {
        for (int i = 0; i < unidade_slots.Length; i++)
        {
            unidade_slots[i].gameObject.SetActive(false);
        }

        unidade_place_B = false;
    }
}
