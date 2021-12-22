using UnityEngine;

[System.Serializable]
public class Audio
{

    public string name; // Name of the audio

    public AudioClip clip; //The Audio Clip Reference

    [Range(0f, 1f)]
    public float volume; //Adjust Volume

    [Range(.1f, 3f)]
    public float pitch; //Adject pitch

    public bool enableLoop; //If the audio can repeat

    [HideInInspector] public AudioSource source;

    public void Play(float _volume = 100, bool _oneShot = false)
    {
        switch (_oneShot)
        {
            case true:
                source.PlayOneShot(clip, _volume / 100);
                break;
            default:
                source.Play();
                source.volume = _volume / 100;
                break;
        }
    }
}
