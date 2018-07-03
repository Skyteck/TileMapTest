using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiledSharp;

namespace TileMapTest
{

    public class TileMap
    {
        public TmxMap map;
        public Texture2D tileset;
        public Vector2 _Postion;
        public int tileWidth;

        public int tileHeight;

        public int tilesetTilesWide;

        public int tilesetTilesHigh;

        public List<Tile> backgroundTiles;

        public bool active = false;

        List<String> nearbyMaps;
        public Rectangle tileMapRect;

        public Vector2 mapTilePos;

        public String name;

        public List<Rectangle> WallList;

        public int mapWidth;
        public int mapHeight;
        public TileMap(String mapName, Microsoft.Xna.Framework.Content.ContentManager content)
        {
            name = mapName;
            backgroundTiles = new List<Tile>();
            nearbyMaps = new List<string>();
            map = new TmxMap("Content/Tilemaps/" + mapName + ".tmx");
            //nearbyMaps = mapName.Split('-').ToList();

            List<int> UnwalkableTileGids = FindUnwalkableTiles(map);
            string tileSetPath = map.Tilesets[0].Name.ToString();
            tileSetPath = "Art/Tilesets/" + tileSetPath;
            tileset = content.Load<Texture2D>(tileSetPath);

            tileHeight = map.Tilesets[0].TileHeight;
            tileWidth = map.Tilesets[0].TileWidth;
            tilesetTilesWide = tileset.Width / tileWidth;
            tilesetTilesHigh = tileset.Height / tileHeight;
            mapWidth = map.Width * tileWidth;
            mapHeight = map.Height * tileHeight;
            //this._Postion = new Vector2(Convert.ToInt32(nearbyMaps[0]) * mapWidth, Convert.ToInt32(nearbyMaps[1]) * mapHeight);
            this._Postion = Vector2.Zero;
            //mapTilePos = new Vector2(Convert.ToInt32(nearbyMaps[0]), Convert.ToInt32(nearbyMaps[1]));
            mapTilePos = Vector2.Zero;
            tileMapRect = new Rectangle((int)this._Postion.X, (int)this._Postion.Y, mapWidth, mapHeight);

            bool test = true;

            for (var i = 0; i < map.Layers[0].Tiles.Count; i++)
            {
                int gid = map.Layers[0].Tiles[i].Gid;
                // Empty tile, do nothing
                if (gid != 0)
                {
                    int tileFrame = gid - 1;
                    int column = tileFrame % tilesetTilesWide;
                    int row;
                    if (tileFrame + 1 > tilesetTilesWide)
                    {
                        row = tileFrame - column * tilesetTilesWide;
                    }
                    else
                    {
                        row = 0;
                    }

                    float x = (i % map.Width) * map.TileWidth;
                    float y = (float)Math.Floor(i / (double)map.Width) * map.TileHeight;

                    int tileX = (int)(x / map.TileWidth);
                    int tileY = (int)(y / map.TileHeight);
                    Rectangle tilesetRec = new Rectangle(tileWidth * column, tileHeight * row, tileWidth, tileHeight);

                    x += this._Postion.X;
                    y += this._Postion.Y;
                    Vector2 tilePos = new Vector2(x, y);
                    if (test == true)
                    {
                        test = false;
                    }
                    else
                    {
                        test = true;
                    }

                    bool walkable = true;
                    if (UnwalkableTileGids.Contains(gid))
                    {
                        walkable = false;
                    }

                    Tile newTile = new Tile(tileset, tilePos, tileWidth, tileHeight, column, row, true, new Vector2(tileX, tileY), walkable);
                    backgroundTiles.Add(newTile);
                }
            }
        }

        private List<Rectangle> FindWalls()
        {
            List<Rectangle> walls = new List<Rectangle>();
            if (map.Layers.Count > 1)
            {
                TmxLayer collisionLayer = map.Layers.Single(x => x.Name == "Collision");

                List<TmxLayerTile> collisionTiles = collisionLayer.Tiles.Where(x => x.Gid != 0).ToList();

                foreach (TmxLayerTile tile in collisionTiles)
                {
                    Vector2 rectPos = new Vector2(tile.X * 64, tile.Y * 64);
                    walls.Add(new Rectangle((int)rectPos.X, (int)rectPos.Y, 64, 64));
                }



            }
            return walls;
        }

        public TmxList<TmxObject> FindObjects()
        {
            if (map.ObjectGroups.Count >= 1)
            {
                if (map.ObjectGroups.Contains("Object Layer 1"))
                {
                    return map.ObjectGroups["Object Layer 1"].Objects;
                }

            }
            return null;
        }

        public TmxList<TmxObject> FindCollisions()
        {
            if (map.ObjectGroups.Count >= 1)
            {
                if (map.ObjectGroups.Contains("Collision"))
                {
                    return map.ObjectGroups["Collision"].Objects;
                }

            }
            return null;
        }

        public TmxList<TmxObject> FindNPCs()
        {
            if (map.ObjectGroups.Count >= 1)
            {
                if (map.ObjectGroups.Contains("NPC Layer"))
                {
                    return map.ObjectGroups["NPC Layer"].Objects;
                }

            }
            return null;
        }



        private List<int> FindUnwalkableTiles(TmxMap map)
        {
            List<int> notWalkableTiles = new List<int>();
            for (int i = 0; i < map.Tilesets[0].Tiles.Count; i++)
            {
                if (map.Tilesets[0].Tiles[i].Properties.ContainsKey("Walkable"))
                {
                    bool walkable = Convert.ToBoolean(map.Tilesets[0].Tiles[i].Properties["Walkable"]);
                    if (walkable == false)
                    {
                        notWalkableTiles.Add(i + 1);
                    }
                }
            }
            return notWalkableTiles;
        }



        public void Update(GameTime gameTime)
        {
            if (active)
            {
                List<Tile> activeTiles = backgroundTiles.FindAll(x => x.active == true);
                foreach (Tile tile in activeTiles)
                {
                    tile.Update(gameTime);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, Rectangle vp)
        {
            if (active)
            {
                int tilesDrawn = 0;
                List<Tile> drawTiles = new List<Tile>();
                drawTiles = backgroundTiles.FindAll(x => x.visible == true);
                foreach (Tile tile in drawTiles)
                {
                    if(tile.destRect.Intersects(vp))
                    {
                        tile.Draw(spriteBatch);
                        tilesDrawn++;
                    }

                }
                //Console.WriteLine(tilesDrawn);
            }
        }

        public Tile findClickedTile(Vector2 pos)
        {
            foreach (Tile tile in backgroundTiles)
            {
                if (tile.localPos.X == pos.X && tile.localPos.Y == pos.Y)
                {
                    return tile;
                }
            }
            return null;
        }
    }
}
