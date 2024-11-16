declare class GamePad {
	static getState(playerIndex: PlayerIndex): GamePadState;
	static getCapabilities(playerIndex: PlayerIndex): GamePadCapabilities;
}

declare enum PlayerIndex {
	One = 0,
	Two = 1,
	Three = 2,
	Four = 3,
}

declare class GamePadState {
	isConnected: boolean;
	buttons: GamePadButtons;
	dPad: GamePadDPad;
	thumbSticks: GamePadThumbSticks;
	triggers: GamePadTriggers;

	isButtonDown(button: Buttons): boolean;
	isButtonUp(button: Buttons): boolean;
}

declare class GamePadCapabilities {
	isConnected: boolean;
	gamePadType: GamePadType;
	hasAButton: boolean;
	hasBButton: boolean;
	hasXButton: boolean;
	hasYButton: boolean;
	// Add other capabilities as needed
}

declare enum GamePadType {
	Unknown,
	GamePad,
	Wheel,
	ArcadeStick,
	FlightStick,
	DancePad,
	Guitar,
	AlternateGuitar,
	DrumKit,
	BigButtonPad,
}

declare class GamePadButtons {
	a: ButtonState;
	b: ButtonState;
	x: ButtonState;
	y: ButtonState;
	leftShoulder: ButtonState;
	rightShoulder: ButtonState;
	leftStick: ButtonState;
	rightStick: ButtonState;
	back: ButtonState;
	start: ButtonState;

	// Methods
	hasFlag(button: Buttons): boolean;
}

declare enum Buttons {
	A = 1,
	B = 2,
	X = 4,
	Y = 8,
	LeftShoulder = 16,
	RightShoulder = 32,
	LeftStick = 64,
	RightStick = 128,
	Back = 256,
	Start = 512,
	// Add other buttons as necessary
}

declare enum ButtonState {
	Pressed,
	Released,
}

declare class GamePadDPad {
	up: ButtonState;
	down: ButtonState;
	left: ButtonState;
	right: ButtonState;
}

declare class GamePadThumbSticks {
	left: Vector2;
	right: Vector2;
}

declare class GamePadTriggers {
	left: number;
	right: number;
}
