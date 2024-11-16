declare class GraphicsDevice {
	viewport: Viewport;
	graphicsProfile: GraphicsProfile;
	presentationParameters: PresentationParameters;

	clear(color: Color): void;
	dispose(): void;
	// Add other methods and properties as needed
}

declare class Viewport {
	x: number;
	y: number;
	width: number;
	height: number;
	minDepth: number;
	maxDepth: number;

	constructor(x?: number, y?: number, width?: number, height?: number);
	getAspectRatio(): number;
}

declare enum GraphicsProfile {
	Reach,
	HiDef,
}

declare class PresentationParameters {
	backBufferWidth: number;
	backBufferHeight: number;
	// Add other properties as needed
}
