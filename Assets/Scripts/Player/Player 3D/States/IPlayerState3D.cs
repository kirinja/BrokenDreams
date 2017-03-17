using UnityEngine;

public interface IPlayerState3D : IPlayerState
{
    void HandleMovement(PlayerController3D player, Vector2 input);
    void AttemptStateSwitch(PlayerController3D player, IPlayerState3D newState);
}
