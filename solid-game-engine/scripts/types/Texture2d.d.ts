declare class Texture2D {
	// Properties
	width: number;
	height: number;
	bounds: Rectangle;
	format: SurfaceFormat;

	// Constructors
	constructor(graphicsDevice: GraphicsDevice, width: number, height: number);
	constructor(graphicsDevice: GraphicsDevice, width: number, height: number, mipmap: boolean, format: SurfaceFormat);

	// Methods
	getData<T>(data: T[]): void;
	getData<T>(level: number, rect: Rectangle, data: T[], startIndex: number, elementCount: number): void;
	setData<T>(data: T[]): void;
	setData<T>(level: number, rect: Rectangle, data: T[], startIndex: number, elementCount: number): void;
	dispose(): void;
}

declare class Rectangle {
	x: number;
	y: number;
	width: number;
	height: number;

	constructor(x: number, y: number, width: number, height: number);
}

declare enum SurfaceFormat {
	Color,
	Bgr565,
	Bgra5551,
	Bgra4444,
	// Add other formats as needed
}
