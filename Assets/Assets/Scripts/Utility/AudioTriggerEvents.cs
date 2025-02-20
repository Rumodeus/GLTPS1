using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTriggerEvents : MonoBehaviour
{
    public AudioSource _audioSource;
    [SerializeField] private AudioClip[] voicelines;
    [SerializeField] private AudioClip soundEffect;
    public bool flag1;
    public bool flag2;
    public bool flag3;


    void OnTriggerEnter(Collider other)
    {
        if (_audioSource != null && voicelines.Length != 0)
        {
            if (other.gameObject.CompareTag("FuelingStationVoicelineTrigger") && !flag1)
            {
                if (_audioSource != null && voicelines[0] != null)
                {
                    _audioSource.Stop();
                    _audioSource.PlayOneShot(voicelines[0]);

                    flag1 = true;
                }
            }

            if (other.gameObject.CompareTag("PillarVoicelineTrigger") && !flag2)
            {
                if (_audioSource != null && voicelines[1] != null)
                {
                    _audioSource.Stop();
                    _audioSource.PlayOneShot(voicelines[1]);
                    flag2 = true;
                }
            }

            if (other.gameObject.CompareTag("FuelingStation") && !flag3)
            {
                if (_audioSource != null && voicelines[2] != null)
                {
                    _audioSource.Stop();
                    _audioSource.PlayOneShot(voicelines[2]);
                    flag3 = true;
                }
            }
        }
        if (other.gameObject.CompareTag("Untagged") ||
            other.gameObject.CompareTag("Metal") ||
            other.gameObject.CompareTag("Pipe") ||
            other.gameObject.CompareTag("Vehicle"))
        {
            if (soundEffect != null)
            {
                _audioSource.PlayOneShot(soundEffect);
            }
        }
    }
}

