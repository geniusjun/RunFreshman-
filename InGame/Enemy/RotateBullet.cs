using UnityEngine;

public class RotateBullet : MonoBehaviour
{
    public float rotationSpeed = 200f; // ȸ�� �ӵ� (����/��)

    private void Update()
    {
        transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
    }
}