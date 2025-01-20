using UnityEngine;

public interface IRobotInput
{
    public bool JumpDown { get; set; }
    public bool JumpHeld { get; set; }
    public Vector2 Move { get; set; }
}