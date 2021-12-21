using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAnimation : MonoBehaviour
{
    [SerializeField]
    float rotationSpeed = 1;

    Vector3 eularAngles;

    private void Start()
    {
        StartCoroutine(Animation());
    }

    IEnumerator Animation()
    {
        while (true)
        {
            Animate();
            yield return null;
        }
    }

    void Animate()
    {
        eularAngles = new Vector3(0f, 0f, (-1 * rotationSpeed) * (Screen.currentResolution.refreshRate / 60));
        transform.Rotate(eularAngles);
        Vector3 rotation = transform.rotation.eulerAngles;
        rotation.z = Mathf.Clamp(rotation.z, 0f, Mathf.Abs(360f));
        transform.eulerAngles = rotation;
    }
}
