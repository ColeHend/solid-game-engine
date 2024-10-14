using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace solid_game_engine.Shared.Enums
{
	public enum Controls
	{
		NONE,
		UP,
		DOWN,
		LEFT,
		RIGHT,
		START,
		A,
		B
	}
	public class InputWrap
	{
		public PlayerIndex PlayerIndex { get; set; }
		public PlayerIndex? GamePadIndex { get; set; }
		public bool HasKeyboard { get; set; }
		public bool InputAvailable { get; set; }
		private KeyboardState PreviousKstate { get; set; }
		private KeyboardState CurrentKstate { get; set; }
		private GamePadState PreviousPad { get; set; }
		private GamePadState CurrentPad { get; set; }
		private PlayerIndex MaxPlayers { get; set; }

		public InputWrap(PlayerIndex playerIndex, PlayerIndex maxPlayers)
		{
			MaxPlayers = maxPlayers;
			AssignInputDevice(playerIndex);
		}

		public void AssignInputDevice(PlayerIndex playerIndex)
		{
			PlayerIndex = playerIndex;
			int totalPlayers = ((int)MaxPlayers) + 1;

			// Initialize input states
			CurrentKstate = Keyboard.GetState();
			PreviousKstate = CurrentKstate;
			CurrentPad = GamePad.GetState(playerIndex);
			PreviousPad = CurrentPad;

			// Get list of connected gamepads
			List<PlayerIndex> connectedGamepads = new List<PlayerIndex>();
			for (int i = 0; i < 4; i++)
			{
				PlayerIndex index = (PlayerIndex)i;
				var state = GamePad.GetState(index);
				if (state.IsConnected)
				{
					connectedGamepads.Add(index);
				}
			}

			if (playerIndex == PlayerIndex.One)
			{
				HasKeyboard = true;
				InputAvailable = true;

				if (connectedGamepads.Count >= totalPlayers)
				{
					GamePadIndex = PlayerIndex.One;
				}
				else
				{
					GamePadIndex = null;
				}
			}
			else
			{
				HasKeyboard = false;
				int playerNumber = (int)playerIndex;

				if (connectedGamepads.Count >= totalPlayers)
				{
					InputAvailable = true;
					GamePadIndex = playerIndex;
				}
				else
				{
					if (connectedGamepads.Count >= playerNumber)
					{
						InputAvailable = true;
						GamePadIndex = connectedGamepads[playerNumber - 1];
					}
					else
					{
						InputAvailable = false;
						GamePadIndex = null;
					}
				}
			}
		}

		// Update method should be called once per frame
		public void Update(GameTime gameTime)
		{
			PreviousKstate = CurrentKstate;
			CurrentKstate = Keyboard.GetState();
			if (GamePadIndex != null)
			{
				PreviousPad = CurrentPad;
				CurrentPad = GamePad.GetState((PlayerIndex)GamePadIndex);
			}
		}

		// Check if control is currently pressed
		public bool IsPressed(Controls controls)
		{
			if (!InputAvailable)
				return false;

			bool keyboardPressed = false;
			bool gamepadPressed = false;

			if (HasKeyboard)
			{
				keyboardPressed = IsKeyboardPressed(controls, CurrentKstate);
			}

			if (CurrentPad.IsConnected)
			{
				gamepadPressed = IsGamepadPressed(controls, CurrentPad);
			}

			return keyboardPressed || gamepadPressed;
		}

		// Check if control was previously released
		public bool WasReleased(Controls controls)
		{
			if (!InputAvailable)
				return false;

			bool keyboardReleased = false;
			bool gamepadReleased = false;

			if (HasKeyboard)
			{
				keyboardReleased = IsKeyboardReleased(controls, PreviousKstate);
			}

			if (PreviousPad.IsConnected)
			{
				gamepadReleased = IsGamepadReleased(controls, PreviousPad);
			}

			return (HasKeyboard ? keyboardReleased : true) && (PreviousPad.IsConnected ? gamepadReleased : true);
		}

		// Check if control was just pressed
		public bool IsSinglePressed(Controls controls)
		{
			return WasReleased(controls) && IsPressed(controls);
		}

		// Helper methods for keyboard and gamepad input checks
		private bool IsKeyboardPressed(Controls controls, KeyboardState kState)
		{
			switch (controls)
			{
				case Controls.UP:
					return kState.IsKeyDown(Keys.Up);
				case Controls.DOWN:
					return kState.IsKeyDown(Keys.Down);
				case Controls.LEFT:
					return kState.IsKeyDown(Keys.Left);
				case Controls.RIGHT:
					return kState.IsKeyDown(Keys.Right);
				case Controls.START:
					return kState.IsKeyDown(Keys.M) || kState.IsKeyDown(Keys.Enter);
				case Controls.A:
					return kState.IsKeyDown(Keys.Space);
				default:
					return false;
			}
		}

		private bool IsGamepadPressed(Controls controls, GamePadState gState)
		{
			switch (controls)
			{
				case Controls.UP:
					return gState.DPad.Up == ButtonState.Pressed || gState.ThumbSticks.Left.Y > 0;
				case Controls.DOWN:
					return gState.DPad.Down == ButtonState.Pressed || gState.ThumbSticks.Left.Y < 0;
				case Controls.LEFT:
					return gState.DPad.Left == ButtonState.Pressed || gState.ThumbSticks.Left.X < 0;
				case Controls.RIGHT:
					return gState.DPad.Right == ButtonState.Pressed || gState.ThumbSticks.Left.X > 0;
				case Controls.START:
					return gState.Buttons.Start == ButtonState.Pressed;
				case Controls.A:
					return gState.Buttons.A == ButtonState.Pressed;
				default:
					return false;
			}
		}

		private bool IsKeyboardReleased(Controls controls, KeyboardState kState)
		{
			switch (controls)
			{
				case Controls.UP:
					return kState.IsKeyUp(Keys.Up);
				case Controls.DOWN:
					return kState.IsKeyUp(Keys.Down);
				case Controls.LEFT:
					return kState.IsKeyUp(Keys.Left);
				case Controls.RIGHT:
					return kState.IsKeyUp(Keys.Right);
				case Controls.START:
					return kState.IsKeyUp(Keys.M) && kState.IsKeyUp(Keys.Enter);
				case Controls.A:
					return kState.IsKeyUp(Keys.Space);
				default:
					return false;
			}
		}

		private bool IsGamepadReleased(Controls controls, GamePadState gState)
		{
			switch (controls)
			{
				case Controls.UP:
					return gState.DPad.Up == ButtonState.Released && gState.ThumbSticks.Left.Y == 0;
				case Controls.DOWN:
					return gState.DPad.Down == ButtonState.Released && gState.ThumbSticks.Left.Y == 0;
				case Controls.LEFT:
					return gState.DPad.Left == ButtonState.Released && gState.ThumbSticks.Left.X == 0;
				case Controls.RIGHT:
					return gState.DPad.Right == ButtonState.Released && gState.ThumbSticks.Left.X == 0;
				case Controls.START:
					return gState.Buttons.Start == ButtonState.Released;
				case Controls.A:
					return gState.Buttons.A == ButtonState.Released;
				default:
					return false;
			}
		}
	}

}