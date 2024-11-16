/***
 * @class CoreSystem
 */
class CoreSystem {
	constructor() {}
	init() {}
	/***
	 * @method loadContent
	 * @description Loads the content of the game.
	 * @param {SpriteBatch} spriteBatch The sprite batch to draw the content.
	 */
	loadContent(spriteBatch) {}
	/***
	 * @method update
	 * @description Updates the game.
	 * @param {GameTime} gameTime The game time.
	 */
	update(gameTime) {
		Console.WriteLine(`Game update: ${gameTime.totalGameTime}`);
	}

	/***
	 * @method draw
	 * @description Draws the game.
	 * @param {SpriteBatch} spriteBatch The sprite batch to draw the game.
	 */
	draw(spriteBatch) {}
}
module.exports = CoreSystem;