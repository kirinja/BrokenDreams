using UnityEngine;

public interface ICharacterState
{
	void Enter();
	CharacterStateData Update(Vector2 input, float deltaTime);
	void Exit();
}
