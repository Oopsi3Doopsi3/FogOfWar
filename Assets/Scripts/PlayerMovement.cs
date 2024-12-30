using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private InputActionAsset _inputAsset;
    [SerializeField] private float _movementSpeed = 5.0f;
    [SerializeField] private float _rotationSpeed = 5.0f;
    private InputAction _movementAction;
    private Vector2 _movementVector;
    private Rigidbody _rigidbody;

    private void Awake()
    {
        if (!_inputAsset)
        {
            Debug.LogError("No input action asset found");
        }

        if (!TryGetComponent<Rigidbody>(out _rigidbody))
        {
            Debug.LogError("No rigidbody found");
        }

        _movementAction = _inputAsset.FindActionMap("Gameplay").FindAction("Movement");
    }

    private void OnEnable()
    {
        _movementAction?.Enable();
    }

    private void OnDisable()
    {
        _movementAction?.Disable();
    }

    private void Update()
    {
        _movementVector = _movementAction.ReadValue<Vector2>();
        Vector3 move = new Vector3(_movementVector.x, 0, _movementVector.y);
        _rigidbody.MovePosition(_rigidbody.position + move * _movementSpeed * Time.deltaTime);

        if (move != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move, Vector3.up);
            _rigidbody.MoveRotation(Quaternion.Slerp(_rigidbody.rotation, targetRotation, _rotationSpeed * Time.deltaTime));
        }
    }
}
