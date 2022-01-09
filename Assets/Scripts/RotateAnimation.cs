using UnityEngine;

public class RotateAnimation : MonoBehaviour
{
    [SerializeField]
    float rotationSpeed = 1;

    Vector3 eularAngles;
    Vector3 rotation;
    Vector3 rotationEularAngleSpeed;

    private void Awake()
    {
        rotationEularAngleSpeed = new Vector3(0f, 0f, (-1 * rotationSpeed));
    }

    private void Update()
    {
        Animate();
    }

    public void Animate()
    {
        eularAngles = rotationEularAngleSpeed; ;
        transform.Rotate(eularAngles);
        rotation = transform.rotation.eulerAngles;
        rotation.z = Mathf.Clamp(rotation.z, 0f, Mathf.Abs(360f));
        transform.eulerAngles = rotation;
    }
}