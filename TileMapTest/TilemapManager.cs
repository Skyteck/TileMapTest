using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TileMapTest
{

    public class TilemapManager
    {
        List<TileMap> mapList = new List<TileMap>();
        public TileMap ActiveMap;

        public void AddMap(TileMap newMap)
        {
            mapList.Add(newMap);
        }

        public TileMap findMap(Vector2 pos)
        {
            foreach (TileMap map in mapList)
            {
                if (map.mapTilePos.X == pos.X && map.mapTilePos.Y == pos.Y)
                {
                    return map;
                }
            }
            return null;
        }

        public TileMap findMapByName(String name)
        {
            foreach (TileMap map in mapList)
            {
                if (map.name.Equals(name))
                {
                    return map;
                }
            }
            return null;
        }

        public Tile findTile(Vector2 pos)
        {
            Vector2 posToTileMapPos = PosToWorldTilePos(pos);
            TileMap mapClicked = findMap(PosToWorldTilePos(pos));
            if(mapClicked == null)
            {
                return null;
            }
            Vector2 localTileMapPos = new Vector2(pos.X - (posToTileMapPos.X * mapClicked.mapWidth), pos.Y - (posToTileMapPos.Y * mapClicked.mapHeight));
            if (mapClicked != null)
            {
                Tile clickedTile = mapClicked.findClickedTile(PosToMapPos(localTileMapPos));
                return clickedTile;
            }
            return null;
        }

        public List<Tile> FindAdjacentTiles(Vector2 pos, bool allowDiagonal = true)
        {
            Tile targetTile = findTile(pos);
            List<Tile> adjacentTiles = new List<Tile>();
            int tileWidth = targetTile.destRect.Width;
            //get top center tile
            Tile topCenter = findTile(new Vector2(targetTile.tileCenter.X, targetTile.tileCenter.Y - tileWidth));
            if (topCenter != null && topCenter.walkable)
            {
                adjacentTiles.Add(topCenter);
            }
            Tile LeftTile = findTile(new Vector2(targetTile.tileCenter.X - tileWidth, targetTile.tileCenter.Y));
            if (LeftTile != null && LeftTile.walkable)
            {
                adjacentTiles.Add(LeftTile);
            }
            Tile rightTIle = findTile(new Vector2(targetTile.tileCenter.X + tileWidth, targetTile.tileCenter.Y));
            if (rightTIle != null && rightTIle.walkable)
            {
                adjacentTiles.Add(rightTIle);
            }
            Tile bottomCenter = findTile(new Vector2(targetTile.tileCenter.X, targetTile.tileCenter.Y + tileWidth));
            if (bottomCenter != null && bottomCenter.walkable)
            {
                adjacentTiles.Add(bottomCenter);
            }

            //adjacentTiles.Add(targetTile);

            if (allowDiagonal)
            {
                Tile topleft = findTile(new Vector2(targetTile.tileCenter.X - tileWidth, targetTile.tileCenter.Y - tileWidth));
                if (topleft.walkable)
                {
                    adjacentTiles.Add(topleft);
                }
                Tile topRight = findTile(new Vector2(targetTile.tileCenter.X + tileWidth, targetTile.tileCenter.Y - tileWidth));
                if (topRight.walkable)
                {
                    adjacentTiles.Add(topRight);
                }
                Tile bottomLeft = findTile(new Vector2(targetTile.tileCenter.X - tileWidth, targetTile.tileCenter.Y + tileWidth));
                if (bottomLeft.walkable)
                {
                    adjacentTiles.Add(bottomLeft);
                }
                Tile bottomRight = findTile(new Vector2(targetTile.tileCenter.X + tileWidth, targetTile.tileCenter.Y + tileWidth));
                if (bottomRight.walkable)
                {
                    adjacentTiles.Add(bottomRight);
                }
            }

            return adjacentTiles;
        }

        public List<Tile> AStarTwo(Tile tileOne, Tile tileTwo)
        {
            if (tileOne == tileTwo) return null;

            Dictionary<Tile, float> ClosedNodes = new Dictionary<Tile, float>();
            Dictionary<Tile, float> OpenNodes = new Dictionary<Tile, float>();
            Dictionary<Tile, Tile> Path = new Dictionary<Tile, Tile>();
            Dictionary<Tile, float> gScore = new Dictionary<Tile, float>();
            gScore.Add(tileOne, 0);
            Dictionary<Tile, float> fScore = new Dictionary<Tile, float>();
            float startCost = Math.Abs(tileOne.tileCenter.X - tileTwo.tileCenter.X) + Math.Abs(tileOne.tileCenter.Y - tileTwo.tileCenter.Y);
            fScore.Add(tileOne, startCost);

            OpenNodes.Add(tileOne, startCost);

            while(OpenNodes.Count > 0)
            {
                Tile current = OpenNodes.OrderBy(x => x.Value).First().Key;

                if(current == tileTwo)
                {
                    //done
                    return BuildPath(Path, current);
                }

                OpenNodes.Remove(current);
                float closestFScore = Math.Abs(current.tileCenter.X - tileOne.tileCenter.X) + Math.Abs(current.tileCenter.Y - tileOne.tileCenter.Y);
                ClosedNodes.Add(current, closestFScore);

                List<Tile> adjacents = this.FindAdjacentTiles(current.tileCenter, false);

                foreach(Tile t in adjacents)
                {
                    if(ClosedNodes.ContainsKey(t))
                    {
                        continue;
                    }


                    float idk = gScore[current] + Math.Abs(current.tileCenter.X - t.tileCenter.X) + Math.Abs(current.tileCenter.Y - t.tileCenter.Y);

                    if(!OpenNodes.ContainsKey(t))
                    {
                        OpenNodes.Add(t, Math.Abs(t.tileCenter.X - tileOne.tileCenter.X) + Math.Abs(t.tileCenter.Y - tileOne.tileCenter.Y));
                    }
                    else if(idk >= gScore[t])
                    {
                        continue;
                    }

                    if(Path.ContainsKey(t))
                    {
                        Path[t] = current;
                    }
                    else
                    {
                        Path.Add(t, current);

                    }

                    if(gScore.ContainsKey(t))
                    {
                        gScore[t] = idk;
                    }
                    else
                    {
                        gScore.Add(t, idk);
                    }

                    if (fScore.ContainsKey(t))
                    {
                        fScore[t] = gScore[t] + Math.Abs(t.tileCenter.X - tileTwo.tileCenter.X) + Math.Abs(t.tileCenter.Y - tileTwo.tileCenter.Y);
                    }
                    else
                    {
                        fScore.Add(t, gScore[t] + Math.Abs(t.tileCenter.X - tileTwo.tileCenter.X) + Math.Abs(t.tileCenter.Y - tileTwo.tileCenter.Y));
                    }

                    
                }

            }
            return null;
        }

        public List<Tile> AStar(Tile tileOne,Tile tileTwo)
        {
            if (tileOne == tileTwo) return null;
            bool pathFound = false;
            Dictionary<Tile, float> OpenNodes = new Dictionary<Tile, float>();
            OpenNodes.Add(tileOne, Math.Abs(tileOne.tileCenter.X - tileTwo.tileCenter.X) + Math.Abs(tileOne.tileCenter.Y - tileTwo.tileCenter.Y));
            Dictionary<Tile, float> PastNodes = new Dictionary<Tile, float>();
            Dictionary<Tile, Tile> Path = new Dictionary<Tile, Tile>();
            Dictionary<Tile, float> currentDistance = new Dictionary<Tile, float>();

            //currentDistance.Add(tileOne, Math.Abs(tileOne.tileCenter.X - tileTwo.tileCenter.X) + Math.Abs(tileOne.tileCenter.Y - tileTwo.tileCenter.Y));

            while(OpenNodes.Count > 0)
            {
                Tile closest = OpenNodes.OrderBy(x => x.Value).First().Key;

                if(closest == tileTwo)
                {
                    // make path
                    Console.WriteLine("Path Found");
                    return BuildPath(Path, closest);
                }

                OpenNodes.Remove(closest);
                //if(!PastNodes.ContainsKey(closest))
                //{
                    PastNodes.Add(closest, Math.Abs(closest.tileCenter.X - tileTwo.tileCenter.X) + Math.Abs(closest.tileCenter.Y - tileTwo.tileCenter.Y));
                //}

                currentDistance.Add(closest, Math.Abs(closest.tileCenter.X - tileTwo.tileCenter.X) + Math.Abs(closest.tileCenter.Y - tileTwo.tileCenter.Y));

                List<Tile> adjacents = this.FindAdjacentTiles(closest.tileCenter, false);

                foreach(Tile t in adjacents)
                {
                    if (PastNodes.ContainsKey(t))
                    {
                        //if(currentWeight >= currentDistance[closest])
                        //{
                            continue;
                        //}
                    }

                    float currentWeight = Math.Abs(t.tileCenter.X - tileTwo.tileCenter.X) + Math.Abs(t.tileCenter.Y - tileTwo.tileCenter.Y);
                    if (!PastNodes.ContainsKey(t) || currentWeight < currentDistance[closest])
                    {
                        if(Path.ContainsKey(t))
                        {
                            Path[t] = closest;
                        }
                        else
                        {
                            Path.Add(t, closest);
                        }



                        if (!OpenNodes.ContainsKey(t))
                        {
                            OpenNodes.Add(t, Math.Abs(t.tileCenter.X - tileTwo.tileCenter.X) + Math.Abs(t.tileCenter.Y - tileTwo.tileCenter.Y));
                        }
                    }
                }
            }
            return null;
        }

        public List<Tile> BuildPath(Dictionary<Tile, Tile> Path, Tile current)
        {
            if (!Path.ContainsKey(current))
            {
                return new List<Tile> { current };
            }

            var path = BuildPath(Path, Path[current]);
            path.Add(current);
            return path;
            //List<Tile> thePath = new List<Tile>();
            //while(Path.ContainsKey(current))
            //{
            //    thePath.Add(current);
            //    current = Path[current];
            //}
            //return thePath;
        }

        public List<Tile> FindPath(Tile tileOne, Tile TileTwo)
        {
            List<Tile> pathTiles = new List<Tile>();
            while(tileOne != TileTwo)
            {
                pathTiles.Add(tileOne);
                List<Tile> adjacentTiles = this.FindAdjacentTiles(tileOne.tileCenter, false);
                Tile closest = FindClosestTile(adjacentTiles, TileTwo);
                if(tileOne == closest)
                {
                    return pathTiles;
                }
                tileOne = closest;
                if(tileOne == TileTwo)
                {
                    pathTiles.Add(TileTwo);
                }
            }

            //get tiles adjacent to tileOne
            // loop through adjacent tiles to find closest and return it
            // add it to pathTIles
            // repeat using new tile
            

            return pathTiles;
        }
        public Tile FindClosestTile(List<Tile> list, Tile target)
        {
            float distance = 99999999;
            Tile closest = list.First();
            float newDistance = Vector2.Distance(closest.tileCenter, target.tileCenter);

            foreach (Tile tile in list)
            {
                newDistance = Vector2.Distance(tile.tileCenter, target.tileCenter);
                if (newDistance < distance)
                {
                    distance = newDistance;
                    closest = tile;
                }
            }

            return closest;
        }


        public Vector2 PosToWorldTilePos(Vector2 pos)
        {

            int clickMapX = (int)pos.X / 1600;
            int clickMapY = (int)pos.Y / 1600;
            return new Vector2(clickMapX, clickMapY);
        }

        private Vector2 PosToMapPos(Vector2 pos)
        {
            //need to change this to the coordinates within the tilemap itself...
            int clickMapX = (int)pos.X / 32;
            int clickMapY = (int)pos.Y / 32;
            return new Vector2(clickMapX, clickMapY);
        }

        internal Tile findWalkableTile(Vector2 newPos)
        {
            Tile newTile = findTile(newPos);
            if (newTile.walkable)
            {
                return newTile;
            }
            return null;
        }

        public void Draw(SpriteBatch spriteBatch, Rectangle vp)
        {
            foreach (TileMap map in mapList)
            {
                map.Draw(spriteBatch, vp);
            }
        }

        public void LoadMap(String mapname, ContentManager Content)
        {
            TileMap testMap = new TileMap(mapname, Content);
            mapList.Add(testMap);
            testMap.active = true;

        }
    }

    public class Node
    {
        Tile myTile;
        float EstCost;

        public Node(Tile one, Tile two)
        {
            myTile = one;
            EstCost = Vector2.Distance(myTile.tileCenter, two.tileCenter);
        }
    }
}
