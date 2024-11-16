const CoreSystem = new require('./scripts/CoreSystem.js');
var CoreUpdate = (gameTime)=>CoreSystem.update(gameTime)
var CoreDraw = (spriteBatch)=>CoreSystem.draw(spriteBatch)
var CoreLoadContent = (spriteBatch)=>CoreSystem.loadContent(spriteBatch)
var CoreInit = ()=>CoreSystem.init()
