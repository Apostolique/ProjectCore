using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using System.Collections.Generic;
using MonoGameJamProject.Towers;
using System;
using System.Linq;

namespace MonoGameJamProject
{
    public class MainClass : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        RenderTarget2D renderTarget01;
        Input input;
        HUD hud;
        Board board;
        // list for all towers
        List<Tower> towerList;
        List<Minion> minionList;
        Tower selectedTower = null, previewTower = null;

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
            Window.ClientSizeChanged += Window_ClientSizeChanged;
            Utility.Window = Window;
            towerList = new List<Tower>();
            minionList = new List<Minion>();
            input = new Input();
            board = new Board(15, 10);
            hud = new HUD(input, board.GridSize);
            AddTower(3, 1, Utility.TowerType.Sniper);
            AddTower(3, 3, Utility.TowerType.Shotgun);
            AddTower(3, 5, Utility.TowerType.FlameThrower);
            board.GeneratePath();
            Minion m = new Minion(3, 3, 0.3f);
            minionList.Add(m);

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
            renderTarget01 = new RenderTarget2D(GraphicsDevice, board.GridSize * board.Width, board.GridSize * board.Height);
            Utility.mainFont = Content.Load<SpriteFont>("jura");
            // TODO: use this.Content to load your game content here
        }
        void Window_ClientSizeChanged(object sender, EventArgs e) {
            int w = board.GridSize * board.Width;
            int h = board.GridSize * board.Height;
            if (w < 1) {
                w = 1;
            }
            if (h < 1) {
                h = 1;
            }
            renderTarget01.Dispose();
            renderTarget01 = new RenderTarget2D(GraphicsDevice, w, h);
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
            for(int i = towerList.Count - 1; i >= 0; i--)
            {
                Tile currenttile = board.GetTile(new Point(towerList[i].X, towerList[i].Y));
                if (board.IsTileOnPath(currenttile))
                    towerList[i].IsDisabled = true;
                else
                    towerList[i].IsDisabled = false;
                towerList[i].Update(gameTime, minionList);
            }
            for(int i = minionList.Count - 1; i >= 0; i--)
            {
                minionList[i].Update(gameTime);
                if (minionList[i].dead || !minionList[i].IsMoving)
                    minionList.Remove(minionList[i]);
            }

            if (input.MouseMiddleButtonPressed)
            {
                if (board.paths.Count > 3) {
                    board.ClearPaths();
                }
                board.GeneratePath();
            }
            if (input.MouseLeftButtonPressed)
            {
                if (board.paths.Count > 0) {
                    Minion m = new Minion(0, 0, 0.3f);
                    minionList.Add(m);
                    m.FollowPath(board.paths[0]);
                }
                //ITSMYMINION.WalkTo(new Vector2(Utility.ScreenToGame(input.MousePosition.X, board.GridSize), Utility.ScreenToGame(input.MousePosition.Y, board.GridSize)));
            }
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
                    if (!IsValidTileForTower(input.MouseToGameGrid(board.GridSize).X, input.MouseToGameGrid(board.GridSize).Y))
                        return;
                    AddTower(input.MouseToGameGrid(board.GridSize).X, input.MouseToGameGrid(board.GridSize).Y,previewTower.type);
                    towerList.Remove(selectedTower);
                    previewTower = null;
                }
            }
        }
        /// <summary>
        /// Checks if the given position is valid for a tower on the game grid
        /// </summary>
        /// <param name="x">x coordinate to check</param>
        /// <param name="y">y coordinate to check</param>
        /// <returns>whether the given position is valid for the tower</returns>
        private bool IsValidTileForTower(int x, int y)
        {
            foreach (Tower t in towerList)
            {
                if (t == selectedTower)
                    continue;
                if (t.X == x && t.Y == y)
                    return false;
            }
            Tile currenttile = board.GetTile(new Point(x, y));
            if (board.IsTileOnPath(currenttile))
                return false;
            return true;
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(renderTarget01);
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            board.Draw(spriteBatch);
            board.DrawPaths(spriteBatch, board.GridSize);
            board.DrawPathLines(spriteBatch, board.GridSize);
            HoveringOverTowerChecker(spriteBatch, board.GridSize);
            // Highlight needs to be drawn before the actual towers
            if (previewTower != null)
            {
                if (IsWithinDimensions())
                    hud.DrawPlacementIndicator(spriteBatch, previewTower.MinimumRange, IsValidTileForTower(input.MouseToGameGrid(board.GridSize).X, input.MouseToGameGrid(board.GridSize).Y));
                selectedTower.DrawSelectionHightlight(spriteBatch, board.GridSize);
            }

            foreach (Tower t in towerList)
                t.Draw(spriteBatch, board.GridSize);
            foreach(Minion m in minionList)
                m.Draw(spriteBatch, board.GridSize);

            hud.DrawPlayTime(spriteBatch);
            spriteBatch.End();

            GraphicsDevice.SetRenderTarget(null);

            spriteBatch.Begin();
            spriteBatch.Draw(renderTarget01, new Vector2(0, 0), Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
        private void HoveringOverTowerChecker(SpriteBatch s, int gridSize)
        {
            if (!IsWithinDimensions())
                return;
            foreach (Tower t in towerList)
            {
                if (t.X == input.MouseToGameGrid(board.GridSize).X && t.Y == input.MouseToGameGrid(board.GridSize).Y && !t.IsDisabled)
                    hud.DrawRangeIndicators(s, new Point(t.X, t.Y) , t.MinimumRange);
            }
        }
    }
}
