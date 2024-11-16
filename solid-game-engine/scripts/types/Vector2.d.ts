declare class Vector2 {
	x: number;
	y: number;

	constructor(x?: number, y?: number);

	// Instance Methods
	length(): number;
	lengthSquared(): number;
	normalize(): void;
	toString(): string;
	copyFrom(source: Vector2): void;
	set(x: number, y: number): void;

	// Static Methods
	static add(value1: Vector2, value2: Vector2): Vector2;
	static subtract(value1: Vector2, value2: Vector2): Vector2;
	static multiply(value1: Vector2, value2: Vector2): Vector2;
	static divide(value1: Vector2, value2: Vector2): Vector2;
	static distance(value1: Vector2, value2: Vector2): number;
	static distanceSquared(value1: Vector2, value2: Vector2): number;
	static dot(value1: Vector2, value2: Vector2): number;
	static lerp(value1: Vector2, value2: Vector2, amount: number): Vector2;
	static zero: Vector2;
	static one: Vector2;
	static unitX: Vector2;
	static unitY: Vector2;
}
