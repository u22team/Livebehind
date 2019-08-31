using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swing : MonoBehaviour
{
    [SerializeField] int swingDirection;
    Rigidbody rb;
    private void Start()
    {
        rb = this.gameObject.GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        rb.AddForce(Vector3.right * 0.05f * swingDirection, ForceMode.Impulse);
    }
}
