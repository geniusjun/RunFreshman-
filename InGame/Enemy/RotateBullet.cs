using UnityEngine;

public class RotateBullet : MonoBehaviour
{
    public float rotationSpeed = 200f; // 회전 속도 (각도/초)

    private void Update()
    {
        transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
    }
}