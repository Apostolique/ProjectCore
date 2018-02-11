﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGameJamProject
{
    public class MainClass : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Board board;
        Tower tower;
        Minion minion;

        public MainClass()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            Utility.Window = Window;
            board = new Board(10, 5);
            tower = new Tower(3, 2);
            minion = new Minion(1.7f, 4.2f);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            board.Draw(spriteBatch);
            tower.Draw(spriteBatch, board.BoardRatio);
            minion.Draw(spriteBatch, 0.3f, board.BoardRatio);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
