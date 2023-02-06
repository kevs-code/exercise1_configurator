using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateGO : MonoBehaviour {

    float speed = 10.0f;

    void Update() {

        transform.Rotate(Vector3.down * speed * Time.deltaTime);

        
    }
}
