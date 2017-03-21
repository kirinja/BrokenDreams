using UnityEngine;

public interface ICharacterState3D
{
    Velocity3D Velocity { get; }
    void Enter();
    void Update(float horizontalInput, float verticalInput, float deltaTime);
    CharacterStateSwitch3D HandleCollisions(CollisionFlags collisionFlags);
    void Exit();
    void AttemptStateSwitch(CharacterStateSwitch3D state);
}