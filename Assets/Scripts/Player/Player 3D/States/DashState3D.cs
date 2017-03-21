using System;
using UnityEngine;

class DashState3D : ICharacterState3D
{
    public Velocity3D Velocity
    {
        get
        {
            throw new NotImplementedException();
        }
    }

    public void AttemptStateSwitch(CharacterStateSwitch3D state)
    {
        throw new NotImplementedException();
    }

    public void Enter()
    {
    }

    public void Exit()
    {
    }

    public CharacterStateSwitch3D HandleCollisions(CollisionFlags collisionFlags)
    {
        throw new NotImplementedException();
    }

    public void Update(float horizontalInput, float verticalInput, float deltaTime)
    {
        throw new NotImplementedException();
    }
}