using UnityEngine;

public class DisappearEffect : MonoBehaviour
{
    public ParticleSystem[] particleEffects;
    public ColorType _colorType;
    public Color[] _color;
    private void Update()
    {
        if (AllStopped())
            gameObject.SetActive(false);
    }

    public bool AllStopped()
    {
        bool allStopped = true;
        for (int i = 0; i < particleEffects.Length; i++)
        {
            if (particleEffects[i].isPlaying)
                allStopped = false;
        }

        return allStopped;
    }

    public void AssignTintColor(ColorType color)
    {
        _colorType = color;
        for (int i = 0; i < particleEffects.Length; i++)
        {
            ParticleSystem.MainModule main = particleEffects[i].main;
            main.startColor = _color[(int)_colorType];
        }
    }
}
