using UnityEngine;

public interface ICharacterState3D
{
    void Enter();
    void Update(Vector2 input);
    CharacterStateSwitch3D HandleCollisions(CollisionFlags collisionFlags);
    void Exit();
    void AttemptStateSwitch(CharacterStateSwitch3D state);
}