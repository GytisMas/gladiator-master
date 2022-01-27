using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AudioManager : MonoBehaviour
{
    public const string CARD_CLICK = "CardClick";
    public const string PUNCH = "Punch";

    [Serializable]
    public struct AudioSlot
    {
        public string name;
        public List<AudioClip> clips;
    }

    public AudioSource source;
    public List<AudioSlot> Slots;
    
    
    public void PlaySlot(string _name)
    {
        foreach (AudioSlot _slot in Slots)
        {
            if (_name == _slot.name)
            {
                int random = Random.Range(0, _slot.clips.Count);
                source.PlayOneShot(_slot.clips[random]);
                return;
            }
        }
        Debug.Log("Sound \"" + _name + "\" not found");
    }
}
