using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon
{
    public float DamageMultiplier = 1.2f;
    public string Title = "";

    public Weapon(string _title)
    {
        Title = _title;
    }
}
