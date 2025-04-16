using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(BoxCollider))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private FixedJoystick _movementJoystick;
    [SerializeField] private FixedJoystick _rotationJoystick;
    [SerializeField] private Animator _animator;
    [SerializeField] private float _movespeed;
    [SerializeField] private float _rotationSpeed = 720f; // Degrees per second

    void FixedUpdate()
    {
        // Inverted movement input
        float moveX = -_movementJoystick.Horizontal * _movespeed;
        float moveZ = -_movementJoystick.Vertical * _movespeed;

        _rigidbody.linearVelocity = new Vector3(moveX, _rigidbody.linearVelocity.y, moveZ);

        // Rotation input from second joystick
        float rotX = -_rotationJoystick.Horizontal;
        float rotZ = -_rotationJoystick.Vertical;
        Vector3 inputDirection = new Vector3(rotX, 0f, rotZ);

        Vector3 rotatedDirection = Quaternion.Euler(0, 270, 0) * inputDirection;

        // Only rotate if there's input
        if (rotX != 0 || rotZ != 0)
        {
            Vector3 direction = new Vector3(rotX, 0f, rotZ);
            Quaternion targetRotation = Quaternion.LookRotation(rotatedDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _rotationSpeed * Time.fixedDeltaTime);
        }
    }
}
