using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainerData
{
    public Attributes Attribute { get; private set; }
    public float StartTimer { get; private set; }
    public float Timer { get; set; }
    public int ID { get; private set; }

    public TrainerData(Attributes _attribute, float _timer, int _id)
    {
        Attribute = _attribute;
        Timer = StartTimer = _timer;
        ID = _id;
    }
}
