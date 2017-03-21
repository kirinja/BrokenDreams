using System.Collections.Generic;
using UnityEngine;

public class PlayerAttributes : MonoBehaviour
{
    public float MaxSpeed = 3f;
    public float GroundAccelerationTime = 0.2f;
    public float AirAccelerationTime = 1.0f;
    public float MaxJumpLength = 6.1f;
    public float MaxJumpHeight = 4.2f;
	public float MinJumpHeight = 1.0f;
    public float GroundDeaccelerationTime = 0.05f;
    public float MaxRotationTime = 0.2f;
	public int MaxHP = 5;
    public List<Ability> Abilities = new List<Ability>();
}
