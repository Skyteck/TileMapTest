using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace TileMapTest
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        TilemapManager _MapManager;
        List<Tile> path;
        Tile TileOne;
        Tile TileTwo;
        Texture2D rectTex;
        Rectangle tileRect;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            this.IsMouseVisible = true;
            this.Window.AllowUserResizing = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            _MapManager = new TilemapManager();
            _MapManager.LoadMap("ProtoLevel", Content);
            rectTex = Content.Load<Texture2D>(@"Art/edgeTex");
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            InputHelper.Update();

            Tile hoveredTile = _MapManager.findTile(InputHelper.MouseScreenPos);

            if(InputHelper.IsKeyPressed(Keys.D1))
            {
                TileOne = hoveredTile;
                if (TileOne != null && TileTwo != null)
                {
                    path = _MapManager.AStarTwo(TileOne, TileTwo);
                }
            }

            if(InputHelper.IsKeyPressed(Keys.D2))
            {
                TileTwo = hoveredTile;
                if (TileOne != null && TileTwo != null)
                {
                    path = _MapManager.AStarTwo(TileOne, TileTwo);
                }
            }

            if(InputHelper.IsKeyPressed(Keys.O))
            {
                TileOne = null;
                TileTwo = null;
            }


            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            // TODO: Add your drawing code here
            _MapManager.Draw(spriteBatch, GraphicsDevice.Viewport.Bounds);

            //if(hoveredTile != null)
            //{
            //    spriteBatch.Draw(rectTex, hoveredTile.destRect, Color.White);
            //}

            if(TileOne != null)
            {
                spriteBatch.Draw(rectTex, TileOne.destRect, Color.White);
            }


            if(TileTwo != null)
            {
                spriteBatch.Draw(rectTex, TileTwo.destRect, Color.Blue);
            }

            if(TileOne != null && TileTwo != null)
            {
                Tile prevTile = TileOne;
                if(path != null)
                {

                    foreach (Tile t in path)
                    {
                        DrawLine(spriteBatch, prevTile.tileCenter, t.tileCenter);
                        prevTile = t;
                    }
                }
            }

            base.Draw(gameTime);
            spriteBatch.End();
        }

        void DrawLine(SpriteBatch sb, Vector2 start, Vector2 end)
        {
            Vector2 edge = end - start;
            // calculate angle to rotate line
            float angle =
                (float)Math.Atan2(edge.Y, edge.X);


            sb.Draw(rectTex,
                new Rectangle(// rectangle defines shape of line and position of start of line
                    (int)start.X,
                    (int)start.Y,
                    (int)edge.Length(), //sb will strech the texture to fill this rectangle
                    1), //width of line, change this to make thicker line
                null,
                Color.Red, //colour of line
                angle,     //angle of line (calulated above)
                new Vector2(0, 0), // point in line about which to rotate
                SpriteEffects.None,
                0);

        }
    }
}
