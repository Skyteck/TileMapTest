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
        Tile hoveredTIle;
        Texture2D rectTex;
        Rectangle tileRect;
        BuildingGhost selectedBuilding;
        List<Building> buildings = new List<Building>();
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
            selectedBuilding = new BuildingGhost();
            selectedBuilding._Opacity = 0.5f;
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

            Tile clickedTile = _MapManager.findTile(InputHelper.MouseScreenPos);
            if(clickedTile != null)
            {

                selectedBuilding._Position = clickedTile.tileCenter;
            }
            
            if(InputHelper.IsKeyPressed(Keys.D1))
            {
                selectedBuilding.LoadContent("Art/BuildingRect", Content);
            }

            if(selectedBuilding != null && InputHelper.LeftButtonClicked)
            {
                Building nb = new Building();
                nb.LoadContent("Art/BuildingRect", Content);
                nb._Position = selectedBuilding._Position;
                buildings.Add(nb);
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
            if(selectedBuilding != null)
            {
                if(selectedBuilding._Texture != null)
                {
                    selectedBuilding.Draw(spriteBatch);
                }
            }

            foreach(Building b in buildings)
            {
                b.Draw(spriteBatch);
            }
            base.Draw(gameTime);
            spriteBatch.End();
        }


    }
}
