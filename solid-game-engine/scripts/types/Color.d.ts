declare class Color {
	r: number;
	g: number;
	b: number;
	a: number;

	constructor();
	constructor(r: number, g: number, b: number, a?: number);
	constructor(vector4: Vector4);

	// Methods
	toVector4(): Vector4;
	toVector3(): Vector3;
	toString(): string;
	multiply(value: number): Color;
	equals(color: Color): boolean;

	// Static Properties (Predefined Colors)
	static Transparent: Color;
	static Black: Color;
	static White: Color;
	static Red: Color;
	static Green: Color;
	static Blue: Color;
	static Yellow: Color;
	static Purple: Color;
	static Orange: Color;
	static Gray: Color;
	// Add other predefined colors as needed
}

declare class Vector3 {
	x: number;
	y: number;
	z: number;

	constructor(x?: number, y?: number, z?: number);
}

declare class Vector4 {
	x: number;
	y: number;
	z: number;
	w: number;

	constructor(x?: number, y?: number, z?: number, w?: number);
}
