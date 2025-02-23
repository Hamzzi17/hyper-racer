using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public void Move(float direction)
    {
        transform.Translate(Vector3.right * (direction * Time.deltaTime));
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -2f, 2f), 0, transform.position.z);
    }
}
