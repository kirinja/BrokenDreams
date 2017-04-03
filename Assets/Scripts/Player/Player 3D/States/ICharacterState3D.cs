using UnityEngine;

public interface ICharacterState3D
{
    void Enter();
    void Update(Vector2 input, bool forceRotate);
    CharacterStateSwitch3D HandleCollisions(CollisionFlags collisionFlags);
    void Exit();
    void AttemptStateSwitch(CharacterStateSwitch3D state);
}