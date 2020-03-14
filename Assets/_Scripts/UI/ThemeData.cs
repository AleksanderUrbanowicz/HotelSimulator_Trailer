using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "UI Skin")]
public class ThemeData : ScriptableObject
{
   public ColorsSet colorsSet;

    public Color GetColor(int number)
    {

        if (colorsSet.colors != null && colorsSet.colors.Count > number)
        {

            return colorsSet.colors[number];
        }
        else return Color.magenta;
    }
}