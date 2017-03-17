using System;
using UnityEngine;

public class DashState3D : IPlayerState3D
{
    public void HandleMovement(PlayerController3D player, Vector2 input)
    {
        throw new System.NotImplementedException();
    }

    /*public override void HandleCollision(PlayerController3D player, Collision collision)
    {
        // TODO
    }*/

    public void AttemptStateSwitch(PlayerController3D player, IPlayerState3D newState)
    {
        //TODO
    }
}
