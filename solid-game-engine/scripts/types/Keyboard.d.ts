declare class Keyboard {
	static getState(): KeyboardState;
}

declare class KeyboardState {
	isKeyDown(key: Keys): boolean;
	isKeyUp(key: Keys): boolean;
	getPressedKeys(): Keys[];
	equals(other: KeyboardState): boolean;
}
