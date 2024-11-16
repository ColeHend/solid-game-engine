declare class SpriteBatch {
	constructor(graphicsDevice: GraphicsDevice);

	begin(
			sortMode?: SpriteSortMode,
			blendState?: BlendState,
			samplerState?: SamplerState,
			depthStencilState?: DepthStencilState,
			rasterizerState?: RasterizerState,
			effect?: Effect,
			transformMatrix?: Matrix
	): void;

	draw(
			texture: Texture2D,
			position: Vector2,
			sourceRectangle?: Rectangle,
			color?: Color,
			rotation?: number,
			origin?: Vector2,
			scale?: Vector2 | number,
			effects?: SpriteEffects,
			layerDepth?: number
	): void;

	end(): void;
	dispose(): void;
}

declare enum SpriteSortMode {
	Deferred,
	Immediate,
	Texture,
	BackToFront,
	FrontToBack,
}

declare class BlendState {
	// Predefined blend states
	static Additive: BlendState;
	static AlphaBlend: BlendState;
	static NonPremultiplied: BlendState;
	static Opaque: BlendState;
	// Add properties and methods as needed
}

declare enum SpriteEffects {
	None = 0,
	FlipHorizontally = 1,
	FlipVertically = 2,
}

declare class SamplerState {
	// Predefined sampler states
	static LinearClamp: SamplerState;
	static PointClamp: SamplerState;
	// Add properties and methods as needed
}

declare class DepthStencilState {
	// Predefined depth stencil states
	static Default: DepthStencilState;
	static DepthRead: DepthStencilState;
	static None: DepthStencilState;
	// Add properties and methods as needed
}

declare class RasterizerState {
	// Predefined rasterizer states
	static CullClockwise: RasterizerState;
	static CullCounterClockwise: RasterizerState;
	static CullNone: RasterizerState;
	// Add properties and methods as needed
}

declare class Effect {
	// Effect properties and methods
}

declare class Matrix {
	// Matrix properties and methods
	// You may need to define methods like createTranslation, createRotationZ, etc.
}
