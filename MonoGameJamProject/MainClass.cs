using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using System.Collections.Generic;
using MonoGameJamProject.Towers;
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
        Path path;

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
            AddTower(3, 1, Utility.TowerType.Sniper);
            AddTower(3, 3, Utility.TowerType.Shotgun);
            AddTower(3, 5, Utility.TowerType.FlameThrower);
            minion = new Minion(1.7f, 4.2f);
            path = new Path();
            path.Add(new Point(0, 1));
            path.Add(new Point(1, 1));
            path.Add(new Point(2, 1));
            path.Add(new Point(2, 2));

            base.Initialize();
        }

        private void AddTower(int x, int y, Utility.TowerType type)
        {
            Tower tower = null;
            switch(type)
            {
                case Utility.TowerType.FlameThrower:
                    tower = new FlameThrower(x, y);
                    break;
                case Utility.TowerType.Sniper:
                    tower = new Sniper(x, y);
                    break;
                case Utility.TowerType.Shotgun:
                    tower = new Shotgun(x, y);
                    break;
                default: throw new ArgumentException("invalid tower type: " + type);
            }
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
            TowerMovementChecker();
            input.Update();
            base.Update(gameTime);
        }

        private bool IsWithinDimensions()
        {
            if (input.MouseGridPosition(board.GridSize).X >= board.FullWidth - 1 || input.MouseGridPosition(board.GridSize).X <= 0)
                return false;
            else if (input.MouseGridPosition(board.GridSize).Y >= board.FullHeight - 1 || input.MouseGridPosition(board.GridSize).Y <= 0)
                return false;
            else
                return true;
        }

        private void TowerMovementChecker()
        {
            if (!IsWithinDimensions())
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
            path.Draw(spriteBatch, board.GridSize);
            HoveringOverTowerChecker(spriteBatch, board.GridSize);
            // Highlight needs to be drawn before the actual towers
            if (selectedTower != null)
            {
                DrawPlacementIndicator(spriteBatch, board.GridSize);
                selectedTower.DrawSelectionHightlight(spriteBatch, board.GridSize);
            }
                
            foreach (Tower t in towerList)
            {
                t.Draw(spriteBatch, board.GridSize);
            }
            minion.Draw(spriteBatch, 0.3f, board.GridSize);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void HoveringOverTowerChecker(SpriteBatch s, int gridSize)
        {
            if (!IsWithinDimensions())
                return;
            foreach (Tower t in towerList)
            {
                if (t.X == input.MouseGridPosition(board.GridSize).X && t.Y == input.MouseGridPosition(board.GridSize).Y)
                    t.DrawMinimumRangeIndicators(s, gridSize);
            }
        }

        private void DrawPlacementIndicator(SpriteBatch s, int gridSize)
        {
            if (!IsWithinDimensions())
                return;
            s.FillRectangle(new RectangleF(input.MouseGridPosition(board.GridSize).X * gridSize, input.MouseGridPosition(board.GridSize).Y * gridSize, gridSize, gridSize), Color.White * 0.5F);
        }
    }
}
