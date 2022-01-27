using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class AudioController : MonoBehaviour
{   
    public enum ClipMultipleType { RANDOM, SEQUENCE }

    [Serializable]
    public class AudioClipSlot
    {
        public enum Type { SFX , MUSIC , OTHER };

        public string name;
        public List<AudioClip> clips;
        public bool loop;
        public bool playOnAwake;
        public bool playOnEnable;
        public ClipMultipleType multipleClipType; //Handles how multiple clips are selected
        [Range(0f,1f)]public float volume;
        [Range(0f,5f)]public float startDelay;
        [Range(0f,5f)]public float fadeInTime;
        [Range(0f,5f)]public float fadeOutTime;
        public Type type;

        private int m_sequenceClipIndex;
        private bool m_isPlayingOneShot;
        private AudioSource m_lastAudioSource;
        private Transform m_sourceClonesHolder;

        public AudioSource lastSource {
            get { return m_lastAudioSource; }
        }
        
        public Transform sourceClonesHolder {
            set { m_sourceClonesHolder = value; }
        }
        
        public AudioClip GetRandomClip()
        {
            if (clips != null && clips.Count > 0) {
                return clips[Random.Range(0, clips.Count)];
            }
            return null;
        }
        
        public AudioClip GetSequencedClip()
        {
            int _clipIndex = m_sequenceClipIndex;
            m_sequenceClipIndex++;
            if (m_sequenceClipIndex >= clips.Count) {
                m_sequenceClipIndex = 0;
            }
            
            return clips[_clipIndex];
        }
        
        public AudioClip GetClip()
        {
            switch (multipleClipType) {
                case ClipMultipleType.RANDOM:
                    return GetRandomClip();
                case ClipMultipleType.SEQUENCE:
                    return GetSequencedClip();
            }

            return null;
        }

        public void Mute()
        {
            if (lastSource != null) {
                lastSource.volume = 0f;
            }
        }
        
        public void Unmute()
        {
            if (lastSource != null) {
                float _volume = volume;

                if (PlayerPrefs.HasKey(type.ToString())) {
                    _volume *= PlayerPrefs.GetFloat(type.ToString());
                }

                lastSource.volume = _volume;
            }
        }
        
        public AudioSource PlayOn(AudioSource _source, int _index = -1)
        {
            if (loop && m_lastAudioSource != null) {
                if (m_lastAudioSource.isPlaying) {
                    return m_lastAudioSource;
                }
            }

            _source = CreateAudioSourceHolder(_source);
            
            AudioClip _clip = _index >= 0 ? clips[_index] : GetClip();
            
            if (volume <= 0f) {
                volume = 1f;
            }

            float _volume = volume;
            
            if (PlayerPrefs.HasKey(type.ToString())) {
                _volume *= PlayerPrefs.GetFloat(type.ToString());
            }

            _source.volume = _volume;
            _source.loop = loop;
            
            if (!_source.isPlaying || _source.clip != _clip) {
                _source.clip = _clip;
                m_lastAudioSource = _source;
                m_lastAudioSource.Play();
            }

            return _source;
        }
        
        public void Play(float _volume = -1f, int _index = -1)
        {
            if (m_isPlayingOneShot && loop) {
                return;
            }
            
            AudioClip _clip = _index >= 0 ? clips[_index] : GetClip();
            AudioSource.PlayClipAtPoint(_clip, Vector3.zero, _volume);
            m_isPlayingOneShot = true;
        }
        
        private AudioSource CreateAudioSourceHolder(AudioSource _source)
        {
            System.Type type = _source.GetType();
            GameObject _popObject = new GameObject();
            _popObject.name = string.Format("SingleShotAudio<{0}>", name);
            _popObject.transform.SetParent(m_sourceClonesHolder);
            Component _copy = _popObject.AddComponent(type);
            System.Reflection.FieldInfo[] _fields = type.GetFields();
            foreach (System.Reflection.FieldInfo _field in _fields)
            {
                _field.SetValue(_copy, _field.GetValue(_source));
            }

            AudioSource _copySource = _copy as AudioSource;
            _copySource.volume = volume;
            _copySource.outputAudioMixerGroup = _source.outputAudioMixerGroup;

            return _copySource;
        }
    }

    public AudioSource mainAudioSource;
    public List<AudioClipSlot> slots;

    private Transform m_sourceClonesHolder;
    private List<AudioSource> m_audioSources;

    public static void SetMuteToAllSources(bool _active)
    {
        List<AudioController> _controllers = new List<AudioController>(FindObjectsOfType<AudioController>());
        foreach (AudioController _controller in _controllers) {
            foreach (AudioClipSlot _slot in _controller.slots) {
                if (_active) {
                    _slot.Mute();
                } else {
                    _slot.Unmute();
                }
            }
        }
    }
    
    private void Awake()
    {
        m_audioSources = new List<AudioSource>();
        m_sourceClonesHolder = new GameObject().transform;
        m_sourceClonesHolder.name = "AudioSourceClonesHolder";
        m_sourceClonesHolder.SetParent(transform);
            
        if (mainAudioSource == null) {
            mainAudioSource = gameObject.GetComponent<AudioSource>();
            if (mainAudioSource == null) {
                mainAudioSource = gameObject.AddComponent<AudioSource>();
                mainAudioSource.playOnAwake = false;
            }
        }  
        m_audioSources.Add(mainAudioSource);

        foreach (AudioClipSlot _slot in slots) {
            if (_slot.playOnAwake) {
                PlaySlot(_slot.name);
            }
        }
        
        /*m_mirrorVirtualSources.Add(AudioSource.Instantiate(mainAudioSource));
            
        for (int i = 0; i < allowedSourceInstances; i++) {
            m_audioSources.Add(AudioSource.Instantiate(mainAudioSource));
            m_mirrorVirtualSources.Add(AudioSource.Instantiate(mainAudioSource));
        }*/
    }

    private void OnEnable()
    {
        foreach (AudioClipSlot _slot in slots) {
            if (_slot.playOnEnable) {
                PlaySlot(_slot.name);
            }
        }
    }

    public void FadeOut(string _slotName)
    {
        StartCoroutine(FadeOutSlot(GetSlot(_slotName)));
    }
    
    public void PlaySlot(string _slotName)
    {
        PlaySlot(_slotName, mainAudioSource);
    }

    public void PlaySlot(string _slotName, float _volume = 1f)
    {
        PlaySlot(_slotName, mainAudioSource, _volume);
    }
    
    public void PlaySlot(string _slotName, AudioSource _source, float _volume = -1f)
    {
        if (!CanPlaySlot(_slotName)) {
            return;
        }
        
        AudioClipSlot _slot = GetSlot(_slotName);
        _slot.volume = _volume == -1f ? _slot.volume : _volume;
        _slot.sourceClonesHolder = m_sourceClonesHolder;
        UnityAction _playAction = delegate
        {
            if (_slot.loop) {
                _slot.PlayOn(_source);
            } else {
                StartCoroutine(PlaySingleShotSlot(_slot, _source));
            }
            StartCoroutine(FadeInSlot(_slot, _source));
        };
        
        if (_slot.startDelay > 0f) {
            StartCoroutine(PlayDelayed(_slot.startDelay, _playAction));
        } else {
            _playAction.Invoke();
        }
    }

    public bool CanPlaySlot(string _name)
    {
        AudioClipSlot _slot = GetSlot(_name);
        return _slot.clips != null && _slot.clips.Count > 0 && gameObject.activeSelf;
    }
    
    public AudioClipSlot GetSlot(string _name)
    {        
        if (slots != null) {
            foreach (AudioClipSlot _slot in slots) {
                if (_slot.name == _name) {
                    return _slot;
                }
            }
        }

        return new AudioClipSlot();
    }
    
    public void MuteAllSlots(bool _mute)
    {
        foreach (AudioClipSlot _slot in slots) {
            if (_mute) {
                _slot.Mute();    
            } else {
                _slot.Unmute();
            }  
        }
    }

    private IEnumerator PlaySingleShotSlot(AudioClipSlot _slot, AudioSource _source)
    {
        if (_slot.clips.Count > 0) {
            _source = _slot.PlayOn(_source);
            yield return new WaitForSeconds(_source.clip.length - _slot.fadeOutTime);
            StartCoroutine(FadeOutSlot(_slot, _source));
        }
    }
    
    private IEnumerator PlayDelayed(float _delay, UnityAction _playAction)
    {
        yield return new WaitForSeconds(_delay);
        _playAction.Invoke();
    }
    
    private IEnumerator FadeInSlot(AudioClipSlot _slot, AudioSource _source)
    {
        if (_slot.fadeInTime > 0) {
            float _timeLeft = 0;
            while (_timeLeft < _slot.fadeInTime) {
                _timeLeft += Time.fixedDeltaTime;
                _source.volume = _slot.volume * _timeLeft / _slot.fadeInTime;
                yield return new WaitForSeconds(Time.fixedDeltaTime);
            }
        }
    }
    
    private IEnumerator FadeOutSlot(AudioClipSlot _slot, AudioSource _source = null)
    {
        if (_source == null) {
            _source = _slot.lastSource;
        }
        
        if (_slot.fadeInTime > 0) {
            float _timeLeft = _slot.fadeOutTime;
            while (_timeLeft > 0) {
                _timeLeft -= Time.fixedDeltaTime;
                _source.volume = _slot.volume * _timeLeft / _slot.fadeInTime;
                yield return new WaitForSeconds(Time.fixedDeltaTime);
            }
        }

        if (_source != null) {
            Destroy(_source.gameObject);
        }
    }

    private void Update()
    {
        /*foreach (AudioClipSlot _slot in slots) {
            _slot.Update();
        }*/
    }
}
