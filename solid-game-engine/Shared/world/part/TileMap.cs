using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace solid_game_engine;
public class TileMap
{
		public string TilesetName { get; set; }
		public List<List<List<int>>> Tiles { get; set; }
}
