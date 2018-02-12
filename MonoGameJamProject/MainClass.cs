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
        HUD hud;
        Board board;
        // list for all towers
        List<Tower> towerList;
        List<Minion> minionList;
        Tower selectedTower = null, previewTower = null;
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
            minionList = new List<Minion>();
            input = new Input();
            board = new Board(10, 5);
            hud = new HUD(input, board.GridSize);
            AddTower(3, 1, Utility.TowerType.Sniper);
            AddTower(3, 3, Utility.TowerType.Shotgun);
            AddTower(3, 5, Utility.TowerType.FlameThrower);
            Minion m = new Minion(3, 3);
            minionList.Add(m);
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
            Utility.mainFont = Content.Load<SpriteFont>("jura");
            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
           
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            // TODO: Add your update logic here
            hud.GridSize = board.GridSize;
            hud.Update(gameTime);
            TowerMovementChecker();
            TowerSwitchInput();
            input.Update();
            base.Update(gameTime);
        }

        public bool IsWithinDimensions()
        {
            if (input.MouseToGameGrid(board.GridSize).X >= board.FullWidth - 1 || input.MouseToGameGrid(board.GridSize).X <= 0)
                return false;
            else if (input.MouseToGameGrid(board.GridSize).Y >= board.FullHeight - 1 || input.MouseToGameGrid(board.GridSize).Y <= 0)
                return false;
            else
                return true;
        }

        private void TowerSwitchInput()
        {
            if(previewTower != null)
            {
                if (input.KeyPressed(Keys.D1))
                {
                    previewTower = new FlameThrower(selectedTower.X, selectedTower.Y);
                }
                else if (input.KeyPressed(Keys.D2))
                {
                    previewTower = new Shotgun(selectedTower.X, selectedTower.Y);
                }
                else if (input.KeyPressed(Keys.D3))
                {
                    previewTower = new Sniper(selectedTower.X, selectedTower.Y);
                }
            }
        }
        private void TowerMovementChecker()
        {
            if (!IsWithinDimensions())
                return;

            if (input.MouseRightButtonPressed)
            {
                if (previewTower == null)
                {
                    foreach (Tower t in towerList)
                    {
                        if (t.X == input.MouseToGameGrid(board.GridSize).X && t.Y == input.MouseToGameGrid(board.GridSize).Y)
                        {
                            selectedTower = t;
                            previewTower = t;
                        }
                    }
                }
                //TODO: check if there is no path on this position, and if there isn't place the tower
                else
                {
                    // for now it just places the tower wherever you click
                    foreach(Tower t in towerList)
                    {
                        if (t == selectedTower)
                            continue;
                        if(t.X == input.MouseToGameGrid(board.GridSize).X && t.Y == input.MouseToGameGrid(board.GridSize).Y)
                            return;
                    }

                    AddTower(input.MouseToGameGrid(board.GridSize).X, input.MouseToGameGrid(board.GridSize).Y,previewTower.type);
                    towerList.Remove(selectedTower);
                    previewTower = null;
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
            if (previewTower != null)
            {
                if (IsWithinDimensions())
                    hud.DrawPlacementIndicator(spriteBatch, previewTower.MinimumRange);
                selectedTower.DrawSelectionHightlight(spriteBatch, board.GridSize);
            }

            foreach (Tower t in towerList)
                t.Draw(spriteBatch, board.GridSize);
            foreach(Minion m in minionList)
                m.Draw(spriteBatch, 0.3f, board.GridSize);

            hud.DrawPlayTime(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void HoveringOverTowerChecker(SpriteBatch s, int gridSize)
        {
            if (!IsWithinDimensions())
                return;
            foreach (Tower t in towerList)
            {
                if (t.X == input.MouseToGameGrid(board.GridSize).X && t.Y == input.MouseToGameGrid(board.GridSize).Y)
                    hud.DrawRangeIndicators(s, new Point(t.X, t.Y) , t.MinimumRange);
            }
        }
    }
}
