using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FenceGate : MonoBehaviour {
    [SerializeField] public bool open = false;
    private float speed = 0.33f;

    public void OpenFenceGate() {
        Quaternion target = Quaternion.Euler(-90, 0, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, target, speed * Time.deltaTime);
    }
    
    public float GetZRotation() {
        return transform.rotation.z;
    }
}
