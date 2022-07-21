using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageParams
{
    float damage;
    string origin;
    public DamageParams(float _damage, string _origin)
    {
        this.damage = _damage;
        this.origin = _origin;
    }

    public float GetDamage()
    {
        return this.damage;
    }

    public string GetOrigin()
    {
        return this.origin;
    }
}