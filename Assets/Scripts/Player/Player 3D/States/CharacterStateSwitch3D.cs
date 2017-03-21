using UnityEngine;

public struct CharacterStateSwitch3D
{
    private readonly ICharacterState3D newState;
    private readonly Vector2 movementInput;

    public CharacterStateSwitch3D(ICharacterState3D newState) : this(newState, Vector3.zero)
    {
    }

    public CharacterStateSwitch3D(ICharacterState3D newState, Vector2 movementInput)
    {
        this.newState = newState;
        this.movementInput = movementInput;
    }

    public ICharacterState3D NewState
    {
        get { return newState; }
    }

    public Vector2 MovementInput
    {
        get { return movementInput; }
    }
}