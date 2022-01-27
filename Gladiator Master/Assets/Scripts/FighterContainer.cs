using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains FighterContainer and FighterData classes
/// </summary>

[System.Serializable]
public class FighterContainer
{
    [SerializeField]
    private List<FighterData> Fighters;

    public FighterContainer()
    {
        Fighters = new List<FighterData>();
    }

    public bool IsEmpty()
    {
        return Count() == 0;
    }

    public int Count()
    {
        return Fighters.Count;
    }

    public FighterData Get(int index)
    {
        return Fighters[index];
    }

    public void Add(FighterData _fighter)
    {
        Fighters.Add(_fighter);
    }

    public void Remove(FighterData _fighter)
    {
        Fighters.Remove(_fighter);
    }

    public FighterData Find(string _name)
    {
        if (_name == "")
        {
            return null;
        }
        foreach (FighterData fighter in Fighters)
        {
            if (_name == fighter.Name)
            {
                return fighter;
            }
        }
        return null;
    }

    public override string ToString()
    {
        string res = "|| ";
        foreach (var fighter in Fighters)
        {
            res += fighter.Name + " ";
        }
        return res;
    }
}

[System.Serializable]
public class FighterData
{
    private const int M_STAT_BOOST_MIN = 1;
    private const int M_STAT_BOOST_MAX = 15;
    public string Name;
    public int Strength;
    public int Speed;
    public int Agility;
    public int Stamina;
    public Weapon EquippedWeapon;
    public Shield EquippedShield;
    

    public int Price
    {
        get
        {
            return 3 * Strength + 2 * Speed + Agility + 3 * Stamina;
        }
    }


    public FighterData(string _name, int _strength, int _speed, int _agility, int _stamina, string _weapon = "", string _shield = "")
    {
        Name = _name;
        Strength = _strength;
        Speed = _speed;
        Agility = _agility;
        Stamina = _stamina;
        EquippedWeapon = new Weapon(_weapon);
        EquippedShield = new Shield(_shield);
    }

    public FighterData(FighterData _fighter)
    {
        Name = _fighter.Name;
        Strength = _fighter.Strength;
        Speed = _fighter.Speed;
        Agility = _fighter.Agility;
        Stamina = _fighter.Stamina;
        //ID = Count++;
    }

    public int NextDamageValue()
    {
        return (int)(Random.Range(Strength / 2, Strength + 1) * EquippedWeapon.DamageMultiplier);
    }

    public override string ToString()
    {
        return $"Name: {Name}, Strength: {Strength}, " +
            $"Speed: {Speed}, Agility: {Agility}, Stamina: {Stamina}";
    }

    public int GetAttribute(Attributes _attribute)
    {
        switch(_attribute)
        {
            case Attributes.Strength:
                {
                    return Strength;
                }
            case Attributes.Speed:
                {
                    return Speed;
                }
            case Attributes.Agility:
                {
                    return Agility;
                }
            case Attributes.Stamina:
                {
                    return Stamina;
                }
            default:
                {
                    return -1;
                }
        }
    }

    public void RandomBoostAllAttributes()
    {
        Strength += Random.Range(M_STAT_BOOST_MIN, M_STAT_BOOST_MAX);
        if (Strength > 100)
        {
            Strength = 100;
        }
        Speed += Random.Range(M_STAT_BOOST_MIN, M_STAT_BOOST_MAX);
        if (Speed > 100)
        {
            Speed = 100;
        }
        Agility += Random.Range(M_STAT_BOOST_MIN, M_STAT_BOOST_MAX);
        if (Agility > 100)
        {
            Agility = 100;
        }
        Stamina += Random.Range(M_STAT_BOOST_MIN, M_STAT_BOOST_MAX);
        if (Stamina > 100)
        {
            Stamina = 100;
        }
    }

    public void FighterStatBoost(Attributes _attribute, int _value)
    {
        switch (_attribute)
        {
            case Attributes.Strength:
                {
                    Strength += _value;
                    if (Strength > 100)
                    {
                        Strength = 100;
                    }
                    break;
                }
            case Attributes.Speed:
                {
                    Speed += _value;
                    if (Speed > 100)
                    {
                        Speed = 100;
                    }
                    break;
                }
            case Attributes.Agility:
                {
                    Agility += _value;
                    if (Agility > 100)
                    {
                        Agility = 100;
                    }
                    break;
                }
            case Attributes.Stamina:
                {
                    Stamina += _value;
                    if (Stamina > 100)
                    {
                        Stamina = 100;
                    }
                    break;
                }
        }
    }
}
