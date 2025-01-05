using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

[RequireComponent(typeof(Projector))]
public class FogProjector : MonoBehaviour
{
    [SerializeField] private Material _projectorMaterial;
    [SerializeField] private float _blendSpeed = 25.0f;
    [SerializeField] private int _textureScale = 2;
    [SerializeField] private RenderTexture _fogTexture;
    [SerializeField] private Camera _fogCamera;

    private RenderTexture _prevTexture;
    private RenderTexture _currTexture;
    private Projector _projector;
    private float _blendAmount;

    private void Awake()
    {
        //transform.parent.transform.position = Vector3.zero;
        //transform.parent.transform.rotation = Quaternion.identity;

        if (!TryGetComponent<Projector>(out _projector))
        {
            Debug.LogError("No projector found");
        }
        _projector.enabled = true;

        if (_fogCamera == null)
        {
            Debug.LogError("No fog camera found");
        }

        //_fogTexture = new RenderTexture(
        //     64,
        //     64,
        //     0,
        //     RenderTextureFormat.ARGB32);
        //_fogTexture.filterMode = FilterMode.Bilinear;

        _prevTexture = GenerateTexture();
        _currTexture = GenerateTexture();

        //_fogCamera.targetTexture = _fogTexture;

        // Projector materials aren't instanced, resulting in the material asset getting changed.
        // Instance it here to prevent us from having to check in or discard these changes manually.
        _projector.material = new Material(_projectorMaterial);

        _projector.material.SetTexture("_PrevTexture", _prevTexture);
        _projector.material.SetTexture("_CurrTexture", _currTexture);

        StartNewBlend();
    }

    RenderTexture GenerateTexture()
    {
        RenderTexture rt = new RenderTexture(
            _fogTexture.width * _textureScale,
            _fogTexture.height * _textureScale,
            0,
            _fogTexture.format);
        rt.filterMode = FilterMode.Bilinear;

        //RenderTexture rt = new RenderTexture(
        //    64 * _textureScale, // Adjust resolution based on scale
        //    64 * _textureScale,
        //    0,
        //    RenderTextureFormat.ARGB32);
        //rt.filterMode = FilterMode.Bilinear;

        return rt;
    }

    //private void Update()
    //{
    //    transform.parent.transform.position = Vector3.zero;
    //    transform.parent.transform.rotation = Quaternion.identity;
    //}

    public void StartNewBlend()
    {
        StopCoroutine(BlendFog());
        _blendAmount = 0;
        // Swap the textures
        Graphics.Blit(_currTexture, _prevTexture);
        Graphics.Blit(_fogTexture, _currTexture);

        StartCoroutine(BlendFog());
    }

    IEnumerator BlendFog()
    {
        while (_blendAmount < 1)
        {
            // increase the interpolation amount
            _blendAmount += Time.deltaTime * _blendSpeed;
            // Set the blend property so the shader knows how much to lerp
            // by when checking the alpha value
            _projector.material.SetFloat("_Blend", _blendAmount);
            yield return null;
        }
        // once finished blending, swap the textures and start a new blend
        StartNewBlend();
    }
}
