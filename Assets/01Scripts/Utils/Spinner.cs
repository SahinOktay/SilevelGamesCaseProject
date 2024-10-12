using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spinner : MonoBehaviour
{
    [SerializeField] private Vector3 angularVelocity;

    void Update()
    {
        transform.eulerAngles += angularVelocity * Time.deltaTime;
    }
}
