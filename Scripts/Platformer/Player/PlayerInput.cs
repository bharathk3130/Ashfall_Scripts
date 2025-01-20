using UnityEngine;

public class PlayerInput : MonoBehaviour, IRobotInput
{
    public bool JumpDown { get; set; }
    public bool JumpHeld { get; set; }
    public Vector2 Move { get; set; }
    
    void Update()
    {
        JumpDown = Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.C);
        JumpHeld = Input.GetButton("Jump") || Input.GetKey(KeyCode.C);
        Move = new Vector2(Input.GetAxisRaw("Horizontal"), 0);
    }
}