using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Clase para el control de las variaciones de las estadisticas de los personajes sobre la UI
/// </summary>
public class StatsUI : MonoBehaviour
{
    //Personaje sobre el que se ejecutan los cambios
    public BattleBehaviour character;
    //Variables que manejan todo el tema de la UI
    public Slider hpBar, ramBar;
    public TMPro.TextMeshProUGUI hp, ram, attack, defense;
    public Image sprite;
    
    /// <summary>
    /// Metodo para actualizar la UI con los datos del personaje
    /// </summary>
    public void update()
    {
        hpBar.maxValue = character.megas;
        ramBar.maxValue = character.ram;
        hpBar.value = character.megas;
        ramBar.value = character.ram;
        attack.text = character.attack + "";
        defense.text = character.defense + "";
        hp.text = character.getHpText();
        ram.text = character.getRamText();
        sprite.sprite = character.GetComponentInParent<SpriteRenderer>().sprite;
    }
}
