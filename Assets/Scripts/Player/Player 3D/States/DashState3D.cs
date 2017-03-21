using System;
using UnityEngine;

class DashState3D : ICharacterState3D
{
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

    public void Update(Vector2 input)
    {
        throw new NotImplementedException();
    }
}