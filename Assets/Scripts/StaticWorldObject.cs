using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class StaticWorldObject : MonoBehaviour
{
    [SerializeField] private GameObject _objectFOV;
    private Renderer _renderer;

    private void Awake()
    {
        if(_objectFOV == null)
        {
            Debug.LogError("No object FOV found");
        }

        _renderer = GetComponentInChildren<Renderer>();
        if(!_renderer)
        {
            Debug.LogError("No renderer found");
        }
    }

    public void ToggleObjectFOV(bool toggle)
    {
        _objectFOV.SetActive(toggle);

        if(toggle)
        {
            _renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        }
        else
        {
            _renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        } 
    }
}
