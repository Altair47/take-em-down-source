using UnityEngine;
using System.Collections;

// ReSharper disable once CheckNamespace
namespace AkilliMum.SRP.PostProcessing
{
    public class SimpleRotaterPP : MonoBehaviour
    {
        public float XSpeed = 0;
        public float YSpeed = 0;
        public float ZSpeed = 0;

        void FixedUpdate()
        {
            gameObject.transform.Rotate(new Vector3(XSpeed, YSpeed, ZSpeed),
                Space.Self);
        }
    }
}
