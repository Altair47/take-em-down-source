using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flex : MonoBehaviour {
    public void Start() {}

    public void Update() {
        transform.Rotate(0, Time.deltaTime * 10, 0);
    }
}
