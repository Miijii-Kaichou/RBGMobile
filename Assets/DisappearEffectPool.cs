using UnityEngine;
using Extensions;

public class DisappearEffectPool : MonoBehaviour
{
    private static DisappearEffectPool Instance;

    [SerializeField]
    GameObject _disapperEffectPrefab;

    [SerializeField]
    int _poolSize = 60;

    [SerializeField]
    GameObject[] _disappearEffectObjs;

    static GameObject[] DisappearEffectObjs => Instance._disappearEffectObjs;

    static DisappearEffect[] Effects;

    private void Awake()
    {
        Instance = this;

        _disappearEffectObjs = new GameObject[_poolSize];

        for (int i = 0; i < _poolSize; i++)
        {
            _disappearEffectObjs[i] = Instantiate(_disapperEffectPrefab, transform.parent);
            _disappearEffectObjs[i].SetActive(false);
            _disappearEffectObjs[i].transform.SetParent(transform);
        }

        Effects = _disappearEffectObjs.GrabComponents<DisappearEffect>();
    }

    public static void SpawnEffect(RectTransform transform, ColorType color)
    {
        GameObject objectToSpawn;

        for(int i = 0; i < DisappearEffectObjs.Length; i++)
        {
            objectToSpawn = DisappearEffectObjs[i];
            if (objectToSpawn.activeInHierarchy == false)
            {
                objectToSpawn.SetActive(true);
                Effects[i].AssignTintColor(color);
                objectToSpawn.GetComponent<RectTransform>().anchoredPosition = transform.anchoredPosition;
                objectToSpawn.transform.rotation = Quaternion.identity;
                return;
            }

        }
    }
}
