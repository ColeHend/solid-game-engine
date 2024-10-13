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
				public bool HasKeyboard { get; set; }
				private KeyboardState PreviousKstate { get; set; }
				private KeyboardState CurrentKstate { get; set; }
				private GamePadState PreviousPad { get; set; }
				private GamePadState CurrentPad { get; set; }
				public InputWrap(bool hasKeyboard = true)
				{
					PlayerIndex = PlayerIndex.One;
					HasKeyboard = hasKeyboard;
					CurrentKstate = Keyboard.GetState();
					PreviousKstate = CurrentKstate;
					CurrentPad = GamePad.GetState(PlayerIndex);
					PreviousPad = CurrentPad;
				}
				public InputWrap(PlayerIndex playerIndex, bool hasKeyboard = false)
				{
					PlayerIndex = playerIndex;
					HasKeyboard = hasKeyboard;
					CurrentKstate = Keyboard.GetState();
					PreviousKstate = CurrentKstate;
					CurrentPad = GamePad.GetState(PlayerIndex);
					PreviousPad = CurrentPad;
				}

				public bool isPressed(Controls controls, bool useCurrent = true)
				{
					var kState = useCurrent ? CurrentKstate : PreviousKstate;
					var gState = useCurrent ? CurrentPad : PreviousPad;
					var hasKeys = (bool has)=>HasKeyboard?has:false;
					switch (controls)
					{
						case Controls.UP:
							return hasKeys(kState.IsKeyDown(Keys.Up)) || gState.DPad.Up == ButtonState.Pressed;
						case Controls.DOWN:
							return hasKeys(kState.IsKeyDown(Keys.Down)) || gState.DPad.Down == ButtonState.Pressed;
						case Controls.LEFT:
							return hasKeys(kState.IsKeyDown(Keys.Left)) || gState.DPad.Left == ButtonState.Pressed;
						case Controls.RIGHT:
							return hasKeys(kState.IsKeyDown(Keys.Right)) || gState.DPad.Right == ButtonState.Pressed;
						case Controls.START:
							return hasKeys(kState.IsKeyDown(Keys.M)) || hasKeys(kState.IsKeyDown(Keys.Enter)) || gState.Buttons.Start == ButtonState.Pressed;
						case Controls.A:
							return hasKeys(kState.IsKeyDown(Keys.Space)) || gState.Buttons.A == ButtonState.Pressed;
						default:
							return false;
					}
				}

				public bool isReleased(Controls controls, bool useCurrent = true)
				{
					var kState = useCurrent ? CurrentKstate : PreviousKstate;
					var gState = useCurrent ? CurrentPad : PreviousPad;
					var hasKeys = (bool has)=>HasKeyboard?has:true;
					switch (controls)
					{
						case Controls.UP:
							return hasKeys(kState.IsKeyUp(Keys.Up)) && gState.DPad.Up == ButtonState.Released;
						case Controls.DOWN:
							return hasKeys(kState.IsKeyUp(Keys.Down)) && gState.DPad.Down == ButtonState.Released;
						case Controls.LEFT:
							return hasKeys(kState.IsKeyUp(Keys.Left)) && gState.DPad.Left == ButtonState.Released;
						case Controls.RIGHT:
							return hasKeys(kState.IsKeyUp(Keys.Right)) && gState.DPad.Right == ButtonState.Released;
						case Controls.START:
							return hasKeys(kState.IsKeyUp(Keys.M)) && hasKeys(kState.IsKeyUp(Keys.Enter)) && gState.Buttons.Start == ButtonState.Pressed;
						case Controls.A:
							return hasKeys(kState.IsKeyUp(Keys.Space)) && gState.Buttons.A == ButtonState.Released;
						default:
							return false;
					}
				}

				public void Update()
				{
					PreviousKstate = CurrentKstate;
					CurrentKstate = Keyboard.GetState();
					PreviousPad = CurrentPad;
					CurrentPad = GamePad.GetState(PlayerIndex);
				}

				public bool isSinglePressed(Controls controls)
				{
					return isPressed(controls, false) && isReleased(controls);
				}
    }
}