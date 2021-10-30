using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unidades_Manager : MonoBehaviour
{
    [Header("STATUS")]
    public Game_Manager GM; // É o controlador geral da partida
    public bool disponivel = true;
    

    [Header("VISUAL")]    
    public Color padrão_cor;
    public Color selecionado_cor;
    private MeshRenderer mesh_R;

    [Header("AUDIO")]
    public AudioSource saida_de_som;
    public SoundPool SP; // É o script onde todos os sons ficam guardado

    private void OnDisable()
    {
        DesactiveSlot();
    }
    private void Start()
    {
        mesh_R = GetComponent<MeshRenderer>();
        gameObject.SetActive(false);
    }

    void ActiveSlot()
    {
        mesh_R.material.SetColor("_BaseColor", selecionado_cor);
    } // Responsavel por trocar a cor do slot
    void DesactiveSlot()
    {
        mesh_R.material.SetColor("_BaseColor", padrão_cor);
    } // Responsavel por trocar a cor do slot

    void Place()
    {
        if(GM.IA.UM != null)
        {
            GM.IA.UM.disponivel = true;
        }
        GM.IA.Comando(transform, this);
        GM.DesactiveSlots();
        disponivel = false;
    } // responsavel por colocar as unidades nos lugares certo
    void PlaceDetonador()
    {
        var v = GM.OP.spawnFromPool("Explosivo", transform.position, Quaternion.Euler(Vector3.zero));
        saida_de_som.PlayOneShot(SP.spawn_Detonador, SP.volume);
        v.GetComponent<Detonador_Manager>().UM = this;
        GM.DesactiveSlots();
        GM.detonador_B = false;
        disponivel = false;
    }

    private void OnMouseDown()
    {
        if (GM.unidade_place_B && !GM.detonador_B)
        {
            Place();
        }
        else if (GM.unidade_place_B && GM.detonador_B)
        {
            PlaceDetonador();
        }
    }
    private void OnMouseEnter()
    {
        ActiveSlot();
    }
    private void OnMouseExit()
    {
        DesactiveSlot();
    }
}
