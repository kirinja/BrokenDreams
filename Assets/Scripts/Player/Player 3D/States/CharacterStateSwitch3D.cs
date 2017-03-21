using UnityEngine;

public struct CharacterStateSwitch3D
{
    private readonly ICharacterState3D newState;
    private readonly Vector3 movementInput;
    private readonly float deltaTime;
    private readonly bool runImmediately;

    public CharacterStateSwitch3D(ICharacterState3D newState) : this(newState, Vector3.zero, 0.0f, false)
    {
    }

    public CharacterStateSwitch3D(ICharacterState3D newState, Vector3 movementInput, float deltaTime, bool runImmediately)
    {
        this.newState = newState;
        this.movementInput = movementInput;
        this.deltaTime = deltaTime;
        this.runImmediately = runImmediately;
    }

    public ICharacterState3D NewState
    {
        get { return newState; }
    }

    public Vector3 MovementInput
    {
        get { return movementInput; }
    }

    public float DeltaTime
    {
        get { return deltaTime; }
    }

    public bool RunImmediately
    {
        get { return runImmediately; }
    }
}