using Microsoft.Xna.Framework;

namespace solid_game_engine.Shared.Enums;

public enum MovementTypes
{
	None,
	Random,
	Follow,
	FollowPath
}

public class NpcMovement
{
	public MovementTypes MovementType { get; set; }
	public Vector2 Target { get; set; }
	public Vector2? Start { get; set; }
}