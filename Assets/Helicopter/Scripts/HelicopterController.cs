using UnityEngine;
using UnityEngine.UI;

public class HelicopterController : MonoBehaviour
{
    public AudioSource HelicopterSound;
    public ControlPanel ControlPanel;
    public Rigidbody HelicopterModel;
    public HeliRotorController MainRotorController;
    public HeliRotorController SubRotorController;

    public float TurnForce = 1.5f;
    public float ForwardForce = 10f;
    public float ForwardTiltForce = 20f;
    public float TurnTiltForce = 30f;
    public float EffectiveHeight = 100f;

    public float turnTiltForcePercent = 1.5f;
    public float turnForcePercent = 0.8f;

    [Header("Sensitivity")]
    public float baseSensitivity = 1f;
    private float currentSensitivity = 1f;

    private float _engineForce;
    public float EngineForce
    {
        get { return _engineForce; }
        set
        {
            if (MainRotorController != null) MainRotorController.RotarSpeed = value * 80;
            if (SubRotorController != null) SubRotorController.RotarSpeed = value * 40;
            if (HelicopterSound != null) HelicopterSound.pitch = Mathf.Clamp(value / 40, 0, 1.2f);
            if (UIGameController.runtime != null && UIGameController.runtime.EngineForceView != null)
                UIGameController.runtime.EngineForceView.text = string.Format("Engine value [ {0} ] ", (int)value);

            _engineForce = value;
        }
    }

    private Vector2 hMove = Vector2.zero;
    private Vector2 hTilt = Vector2.zero;
    private float hTurn = 0f;
    public bool IsOnGround = true;

    void Start()
    {
        if (ControlPanel != null)
        {
            ControlPanel.KeyPressed += OnKeyPressed;
        }

        // Get sensitivity from settings
        if (SettingsManager.Instance != null)
        {
            currentSensitivity = SettingsManager.Instance.GetSensitivity();
            Debug.Log("Helicopter sensitivity set to: " + currentSensitivity);
        }
        else
        {
            currentSensitivity = baseSensitivity;
            Debug.Log("Using base sensitivity: " + baseSensitivity);
        }
    }

    void Update()
    {
        // Direct keyboard input if no ControlPanel
        if (ControlPanel == null)
        {
            HandleDirectInput();
        }
    }

    void FixedUpdate()
    {
        LiftProcess();
        MoveProcess();
        TiltProcess();
    }

    void HandleDirectInput()
    {
        // R key toggles engine on/off
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (EngineForce > 0)
            {
                EngineForce = 0; // Turn off
            }
            else
            {
                EngineForce = 15f; // Turn on to stable hover
            }
        }

        // Space = Up, Shift = Down
        if (Input.GetKey(KeyCode.Space))
            EngineForce += 0.15f * currentSensitivity;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            EngineForce -= 0.15f * currentSensitivity;
            if (EngineForce < 0) EngineForce = 0;
        }

        // Clamp max engine force
        EngineForce = Mathf.Clamp(EngineForce, 0, 50f);

        if (IsOnGround && EngineForce < 20f) return;

        // Movement - WASD
        float tempY = 0;
        float tempX = 0;

        // Auto-stabilize (smooth return to center)
        if (hMove.y > 0) tempY = -Time.deltaTime * 2f;
        else if (hMove.y < 0) tempY = Time.deltaTime * 2f;

        if (hMove.x > 0) tempX = -Time.deltaTime * 2f;
        else if (hMove.x < 0) tempX = Time.deltaTime * 2f;

        // WASD Input - with sensitivity applied
        if (Input.GetKey(KeyCode.W)) tempY = Time.deltaTime * 0.8f * currentSensitivity;  // Forward
        if (Input.GetKey(KeyCode.S)) tempY = -Time.deltaTime * 0.8f * currentSensitivity; // Backward
        if (Input.GetKey(KeyCode.A)) tempX = -Time.deltaTime * 0.5f * currentSensitivity; // Turn left
        if (Input.GetKey(KeyCode.D)) tempX = Time.deltaTime * 0.5f * currentSensitivity;  // Turn right

        hMove.x += tempX;
        hMove.x = Mathf.Clamp(hMove.x, -1, 1);
        hMove.y += tempY;
        hMove.y = Mathf.Clamp(hMove.y, -1, 1);
    }

    private void MoveProcess()
    {
        var turn = TurnForce * Mathf.Lerp(hMove.x, hMove.x * (turnTiltForcePercent - Mathf.Abs(hMove.y)), Mathf.Max(0f, hMove.y));
        hTurn = Mathf.Lerp(hTurn, turn, Time.fixedDeltaTime * TurnForce);
        HelicopterModel.AddRelativeTorque(0f, hTurn * HelicopterModel.mass * currentSensitivity, 0f);
        HelicopterModel.AddRelativeForce(Vector3.forward * Mathf.Max(0f, hMove.y * ForwardForce * HelicopterModel.mass * currentSensitivity));
    }

    private void LiftProcess()
    {
        var upForce = 1 - Mathf.Clamp(HelicopterModel.transform.position.y / EffectiveHeight, 0, 1);
        upForce = Mathf.Lerp(0f, EngineForce, upForce) * HelicopterModel.mass;
        HelicopterModel.AddRelativeForce(Vector3.up * upForce);
    }

    private void TiltProcess()
    {
        hTilt.x = Mathf.Lerp(hTilt.x, hMove.x * TurnTiltForce, Time.deltaTime);
        hTilt.y = Mathf.Lerp(hTilt.y, hMove.y * ForwardTiltForce, Time.deltaTime);
        HelicopterModel.transform.localRotation = Quaternion.Euler(hTilt.y, HelicopterModel.transform.localEulerAngles.y, -hTilt.x);
    }

    private void OnKeyPressed(PressedKeyCode[] obj)
    {
        float tempY = 0;
        float tempX = 0;

        if (hMove.y > 0) tempY = -Time.fixedDeltaTime;
        else if (hMove.y < 0) tempY = Time.fixedDeltaTime;

        if (hMove.x > 0) tempX = -Time.fixedDeltaTime;
        else if (hMove.x < 0) tempX = Time.fixedDeltaTime;

        foreach (var pressedKeyCode in obj)
        {
            switch (pressedKeyCode)
            {
                case PressedKeyCode.SpeedUpPressed:
                    EngineForce += 0.1f * currentSensitivity;
                    break;
                case PressedKeyCode.SpeedDownPressed:
                    EngineForce -= 0.12f * currentSensitivity;
                    if (EngineForce < 0) EngineForce = 0;
                    break;
                case PressedKeyCode.ForwardPressed:
                    if (IsOnGround) break;
                    tempY = Time.fixedDeltaTime * currentSensitivity;
                    break;
                case PressedKeyCode.BackPressed:
                    if (IsOnGround) break;
                    tempY = -Time.fixedDeltaTime * currentSensitivity;
                    break;
                case PressedKeyCode.LeftPressed:
                    if (IsOnGround) break;
                    tempX = -Time.fixedDeltaTime * currentSensitivity;
                    break;
                case PressedKeyCode.RightPressed:
                    if (IsOnGround) break;
                    tempX = Time.fixedDeltaTime * currentSensitivity;
                    break;
                case PressedKeyCode.TurnRightPressed:
                    if (IsOnGround) break;
                    var force = (turnForcePercent - Mathf.Abs(hMove.y)) * HelicopterModel.mass * currentSensitivity;
                    HelicopterModel.AddRelativeTorque(0f, force, 0);
                    break;
                case PressedKeyCode.TurnLeftPressed:
                    if (IsOnGround) break;
                    var force2 = -(turnForcePercent - Mathf.Abs(hMove.y)) * HelicopterModel.mass * currentSensitivity;
                    HelicopterModel.AddRelativeTorque(0f, force2, 0);
                    break;
            }
        }

        hMove.x += tempX;
        hMove.x = Mathf.Clamp(hMove.x, -1, 1);
        hMove.y += tempY;
        hMove.y = Mathf.Clamp(hMove.y, -1, 1);
    }

    private void OnCollisionEnter()
    {
        IsOnGround = true;
    }

    private void OnCollisionExit()
    {
        IsOnGround = false;
    }
}