using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ClearScript;
using Microsoft.ClearScript.V8;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace solid_game_engine.Shared.Systems
{

	public enum CoreScripts
	{
		Game
	}
	public class ScriptSystem
	{
		public V8ScriptEngine Engine { get; }
		private string _scripts { get; set; }
		public ScriptSystem()
		{
			Engine = new V8ScriptEngine(V8ScriptEngineFlags.EnableDebugging);

			Engine.AccessContext = typeof(Program);
			// Expose a C# object to JavaScript
			// --------- Add objects to the engine --------------
			AddSystemObjects(Engine);
			AddCustomObjects(Engine);

			//----- Define a require function in JavaScript -----
			Engine.Execute(@"
				function require(path) {
						var fullPath = path;
						if (!Path.IsPathRooted(path)) {
								fullPath = Path.Combine(Environment.CurrentDirectory, path);
						}
						var code = File.ReadAllText(fullPath);
						var module = { exports: {} };
						var exports = module.exports;
						var func = new Function('require', 'module', 'exports', code);
						func(require, module, exports);
						return module.exports;
				}
			");
		}

		public void AddHostObject(string name, object obj)
		{
			Engine.AddHostObject(name, obj);
		}

		public void AddHostType(string name, Type type)
		{
			Engine.AddHostType(name, type);
		}

		public void LoadAndExecute()
		{
			_scripts = LoadScript("./scripts/main.js");
			Engine.Execute(_scripts);
		}
		public void RunInit(CoreScripts script)
		{
			switch (script)
			{
				case CoreScripts.Game:
					if (Engine.Script.CoreInit is Action)
					{
						Engine.Script.CoreInit();
					}
					break;
				default:
					break;
			}
		}

		public void RunLoadContent(CoreScripts script, SpriteBatch spriteBatch)
		{
			switch (script)
			{
				case CoreScripts.Game:
					if (Engine.Script?.CoreLoadContent is Action<SpriteBatch>)
					{
						Engine.Script.CoreLoadContent(spriteBatch);
					}
					break;
				default:
					break;
			}
		}

		public void RunUpdate(CoreScripts script, GameTime gameTime)
		{
			switch (script)
			{
				case CoreScripts.Game:
					if (Engine.Script.CoreUpdate != null)
					{
						Engine.Script.CoreUpdate(gameTime);
					}
					break;
				default:
					break;
			}
		}

		public void RunDraw(CoreScripts script, SpriteBatch spriteBatch)
		{
			switch (script)
			{
				case CoreScripts.Game:
					if (Engine.Script.CoreDraw is Action<SpriteBatch>)
					{
						Engine.Script.CoreDraw(spriteBatch);
					}
					break;
				default:
					break;
			}
		}

		private void AddCustomObjects(V8ScriptEngine engine)
		{
			engine.AddHostType("Texture2D", typeof(Texture2D));
			engine.AddHostType("Vector2", typeof(Vector2));
			engine.AddHostType("Color", typeof(Color));
			engine.AddHostType("Keyboard", typeof(Keyboard));
			engine.AddHostType("Keys", typeof(Keys));
			engine.AddHostType("GamePad", typeof(GamePad));
			// var sceneManager = new SceneManager();
			// engine.AddHostObject("sceneManager", sceneManager);
		}

		private void AddSystemObjects(V8ScriptEngine engine)
		{
			var assemblies = new[] {
					typeof(object).Assembly,                             
					typeof(System.Linq.Enumerable).Assembly,              
					typeof(System.IO.File).Assembly,                     
					typeof(System.Net.WebClient).Assembly,                
					typeof(System.Text.StringBuilder).Assembly,          
					typeof(System.Threading.Tasks.Task).Assembly,         
					typeof(System.Collections.Generic.List<>).Assembly,  
					typeof(System.Collections.ArrayList).Assembly,       
					typeof(System.Math).Assembly,                       
					typeof(System.Guid).Assembly,                  
					typeof(System.Random).Assembly,   
			};

			var hostTypeCollection = new HostTypeCollection(assemblies);
			engine.AddHostObject("System", hostTypeCollection);
			engine.AddHostObject("host", new HostFunctions());
			engine.AddHostObject("fs", new Func<string, string>(File.ReadAllText));

			engine.AddHostType(typeof(Console));
			engine.AddHostObject("host", new HostFunctions());
			engine.AddHostType("File", typeof(File));
			engine.AddHostType("Path", typeof(Path));
			engine.AddHostType("Environment", typeof(Environment));
		}

		private string LoadScript(string path)
		{
			using (var reader = new StreamReader(path))
			{
				return reader.ReadToEnd();
			}
		}
	}
}