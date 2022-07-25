using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public float maxFill, minFill, yVal;
    public SpriteRenderer fill, background;

    public void SetBar(float health)
    {
        if(health >= 100)
        {
            StartCoroutine(SetVisibility(false, 1f));
        }else if(health <= 0)
        {
            StartCoroutine(SetVisibility(false, 0f));
        }
        else
        {
            StartCoroutine(SetVisibility(true, 0f));
        }
        float barAmount = map(health, 0, 100, minFill, maxFill);
        fill.size = new Vector2(barAmount, yVal);
    }

    private IEnumerator SetVisibility(bool visible, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (visible)
        {
            fill.enabled = true;
            background.enabled = true;
        }
        else
        {
            fill.enabled = false;
            background.enabled = false;
        }
    }

    float map(float s, float a1, float a2, float b1, float b2)
    {
        return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
    }
}
