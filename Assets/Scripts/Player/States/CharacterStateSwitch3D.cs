public struct CharacterStateSwitch3D
{
    private readonly ICharacterState3D _newState;


    public CharacterStateSwitch3D(ICharacterState3D newState)
    {
        _newState = newState;
    }


    public ICharacterState3D NewState
    {
        get { return _newState; }
    }
}