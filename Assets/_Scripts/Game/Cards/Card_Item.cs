using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Card", menuName = "Card")]
public class Card_Item : ScriptableObject
{
    public enum TipoCard { Atirador, Barricada, Gerador, Detonador}
    public TipoCard Tipo;
    public Sprite icon;
    public string nome;
    public int preco;
    public int maxDelay;
}
