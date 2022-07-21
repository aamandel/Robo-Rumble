using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorAssigner : MonoBehaviour
{
    private Color myColor;
    public List<SpriteRenderer> sprites;

    public void SetColor(Color _color)
    {
        myColor = _color;
        foreach (SpriteRenderer SR in sprites)
        {
            SR.color = myColor;
        }
    }
}
