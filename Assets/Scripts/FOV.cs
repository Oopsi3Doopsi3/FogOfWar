using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FOV : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<StaticWorldObject>(out StaticWorldObject staticWorldObject))
        {
            staticWorldObject.ToggleObjectFOV(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent<StaticWorldObject>(out StaticWorldObject staticWorldObject))
        {
            staticWorldObject.ToggleObjectFOV(false);
        }
    }
}
