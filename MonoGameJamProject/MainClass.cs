using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;

namespace MonoGameJamProject
{
    public class MainClass : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Input input;
        Board board;
        // list for all towers
        List<Tower> towerList;
        Minion minion;
        Tower selectedTower = null;

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
            towerList = new List<Tower>();
            input = new Input();
            board = new Board(10, 5);
            AddTower(3, 2, board.GridSize);
            minion = new Minion(1.7f, 4.2f);

            base.Initialize();
        }

        private void AddTower(int x, int y, int size)
        {
            Tower tower = new Tower(x, y, size);
            towerList.Add(tower);
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
            input.Update();
            foreach(Tower t in towerList)
            {
                t.size = board.GridSize;
            }
            TowerMovementChecker();
            base.Update(gameTime);
        }

        private void TowerMovementChecker()
        {
            if (input.MouseGridPosition(board.GridSize).X > board.Width || input.MouseGridPosition(board.GridSize).X < 0)
                return;
            if (input.MouseGridPosition(board.GridSize).Y > board.Height || input.MouseGridPosition(board.GridSize).Y < 0)
                return;
            if (input.MouseRightButtonPressed)
            {
                if (selectedTower == null)
                {
                    foreach (Tower t in towerList)
                    {
                        if (t.X == input.MouseGridPosition(board.GridSize).X && t.Y == input.MouseGridPosition(board.GridSize).Y)
                            selectedTower = t;
                    }
                }
                //TODO: check if there is no path on this position, and if there isn't place the tower
                else
                {
                    // for now it just places the tower wherever you click
                    selectedTower.X = input.MouseGridPosition(board.GridSize).X;
                    selectedTower.Y = input.MouseGridPosition(board.GridSize).Y;
                    selectedTower = null;
                }
            }
            
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            board.Draw(spriteBatch);
            foreach(Tower t in towerList)
            {
                t.Draw(spriteBatch);
            }
            minion.Draw(spriteBatch, 0.3f, board.GridSize);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
