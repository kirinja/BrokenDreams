using UnityEngine;

public struct CharacterStateData
{
	public Vector2 Movement;

	public ICharacterState NewState;

	public bool RunNewStateSameUpdate;

	public CharacterStateData(Vector2 movement, ICharacterState newState, bool runNewStateSameUpdate)
	{
		Movement = movement; 
		NewState = newState;
		RunNewStateSameUpdate = runNewStateSameUpdate;
	}
}