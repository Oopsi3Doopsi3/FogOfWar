using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class StaticWorldObject : MonoBehaviour
{
    [SerializeField] private GameObject _objectFOV;

    private void Awake()
    {
        if(_objectFOV == null)
        {
            Debug.LogError("No object FOV found");
        }
    }

    public void ToggleObjectFOV(bool toggle)
    {
        _objectFOV.SetActive(toggle);
    }
}
