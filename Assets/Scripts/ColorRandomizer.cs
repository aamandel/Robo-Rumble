using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorRandomizer : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        float R = Random.value;
        float G = Random.value;
        float B = Random.value;

        spriteRenderer.material.color = new Color(R, G, B);
    }

}
