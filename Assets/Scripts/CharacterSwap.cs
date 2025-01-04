using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;
using UnityEditor;

public class CharacterSwap : MonoBehaviour
{
    [SerializeField] private InputActionAsset _inputAsset;
    [SerializeField] private CinemachineVirtualCamera _camera;

    private InputAction _switchPlayerAction;
    private List<GameObject> _players = new List<GameObject>();
    private int _currentPlayerIndex = 0;

    private void Awake()
    {
        if (!_inputAsset)
        {
            Debug.LogError("No input action asset found");
        }

        if (!_camera)
        {
            Debug.LogError("No camera found");
        }

        _switchPlayerAction = _inputAsset.FindActionMap("Gameplay").FindAction("SwitchPlayer");

        FindAllPlayers();
    }

    private void FindAllPlayers()
    {
        GameObject[] allGameObjects = GameObject.FindObjectsOfType<GameObject>();

        foreach (GameObject obj in allGameObjects)
        {
            if (obj.CompareTag("Player"))
            {
                _players.Add(obj);
            }
        }

        if (_players.Count > 0)
        {
            SetActivePlayer();
        }
        else
        {
            Debug.LogError("No players found in the scene.");
        }
    }

    private void OnEnable()
    {
        _switchPlayerAction?.Enable();
        _switchPlayerAction.performed += OnSwitchPlayerPerformed;
    }

    private void OnDisable()
    {
        _switchPlayerAction?.Disable();
        _switchPlayerAction.performed -= OnSwitchPlayerPerformed;
    }

    private void OnSwitchPlayerPerformed(InputAction.CallbackContext context)
    {
        SwitchToNextPlayer();
    }

    private void SwitchToNextPlayer()
    {
        _players[_currentPlayerIndex].GetComponent<PlayerMovement>().enabled = false;

        _currentPlayerIndex = (_currentPlayerIndex + 1) % _players.Count;
        SetActivePlayer();
    }

    private void SetActivePlayer()
    {
        _players[_currentPlayerIndex].GetComponent<PlayerMovement>().enabled = true;

        //Update camera
        _camera.Follow = _players[_currentPlayerIndex].transform;
    }
}
