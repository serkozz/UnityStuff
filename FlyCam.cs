using UnityEngine;

public class FlyCam : MonoBehaviour
{
    public float speed = 10f;
    public float sensitivity = 0.2f;
    
    private InputSystem_Actions _actions;
    private float _rotationX = 0f;
    private float _rotationY = 0f;
    private float _verticalMove = 0f;

    private void Awake()
    {
        _actions = new InputSystem_Actions();
        
        // Up and Down must be added first to input system
        _actions.Player.Up.performed += _ => _verticalMove = 1f;
        _actions.Player.Up.canceled += _ => _verticalMove = 0f;
        _actions.Player.Down.performed += _ => _verticalMove = -1f;
        _actions.Player.Down.canceled += _ => _verticalMove = 0f;
    }

    private void OnEnable() => _actions.Enable();
    private void OnDisable() => _actions.Disable();

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        // Mouse Look
        Vector2 lookInput = _actions.Player.Look.ReadValue<Vector2>();
        _rotationY += lookInput.x * sensitivity;
        _rotationX -= lookInput.y * sensitivity;
        _rotationX = Mathf.Clamp(_rotationX, -90f, 90f);
        transform.eulerAngles = new Vector3(_rotationX, _rotationY, 0);

        // WASD Movement
        Vector2 moveInput = _actions.Player.Move.ReadValue<Vector2>();
        Vector3 moveDirection = transform.right * moveInput.x + 
                                transform.forward * moveInput.y + 
                                Vector3.up * _verticalMove;

        // Apply Speed & Boost
        float boost = _actions.Player.Sprint.IsPressed() ? 2.5f : 1f;
        
        // MoveDirection without normalization for analog stick precision 
        // .normalized for consistent keyboard speed.
        transform.position += moveDirection * (speed * boost * Time.deltaTime);
    }
}
