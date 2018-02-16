using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using MonoGameJamProject.Towers;
using MonoGameJamProject.UI;
using System;

namespace MonoGameJamProject
{
    public class MainClass : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        RenderTarget2D renderTarget01;
        Input input;
        HUD hud;
        
        // list for all towers
        List<Tower> towerList;
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
            Utility.board = new Board(15, 10);
            towerList = new List<Tower>();
            input = new Input();
            hud = new HUD(input);
            Utility.tdGameTimer = TimeSpan.Zero;
            AddTower(3, 1, Utility.TowerType.Sniper);
            AddTower(3, 3, Utility.TowerType.Shotgun);
            AddTower(3, 5, Utility.TowerType.FlameThrower);
            Utility.board.GeneratePath();
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
            renderTarget01 = new RenderTarget2D(GraphicsDevice, Utility.board.GridSize * Utility.board.Width, Utility.board.GridSize * Utility.board.Height);
            Utility.assetManager = new AssetManager(Content);
            // TODO: use this.Content to load your game content here
        }
        void Window_ClientSizeChanged(object sender, EventArgs e) {
            Utility.board.CacheGridSize();
            int w = Utility.board.GridSize * Utility.board.Width;
            int h = Utility.board.GridSize * Utility.board.Height;
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
            Utility.tdGameTimer += gameTime.ElapsedGameTime;
            Utility.board.Update(gameTime);
            TowerMovementChecker();
            TowerSwitchInput();
            input.Update();
            for(int i = towerList.Count - 1; i >= 0; i--)
            {
                Tile currenttile = Utility.board.GetTile(new Point(towerList[i].X, towerList[i].Y));
                if (Utility.board.IsTileOnPath(currenttile))
                    towerList[i].IsDisabled = true;
                else
                    towerList[i].IsDisabled = false;
                towerList[i].Update(gameTime);
            }


            if (input.MouseMiddleButtonPressed)
            {
                if (Utility.board.Paths.Count > 1) {
                    Utility.board.ClearPaths();
                }
                Utility.board.GeneratePath();
            }
            if (input.MouseLeftButtonPressed)
            {
                if (Utility.board.Paths.Count > 0) {
                    Minion m = new Minion(0, 0, Utility.MinionType.fast);
                    Utility.board.Paths[0].AddMinion(m);
                }
                //ITSMYMINION.WalkTo(new Vector2(Utility.ScreenToGame(input.MousePosition.X, board.GridSize), Utility.ScreenToGame(input.MousePosition.Y, board.GridSize)));
            }
            base.Update(gameTime);
        }
        public bool IsWithinDimensions()
        {
            if (input.MouseToGameGrid().X >= Utility.board.FullWidth - 1 || input.MouseToGameGrid().X <= 0)
                return false;
            else if (input.MouseToGameGrid().Y >= Utility.board.FullHeight - 1 || input.MouseToGameGrid().Y <= 0)
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
                        if (t.X == input.MouseToGameGrid().X && t.Y == input.MouseToGameGrid().Y)
                        {
                            selectedTower = t;
                            previewTower = t;
                        }
                    }
                }
                //TODO: check if there is no path on this position, and if there isn't place the tower
                else
                {
                    if (!IsValidTileForTower(input.MouseToGameGrid().X, input.MouseToGameGrid().Y))
                        return;
                    AddTower(input.MouseToGameGrid().X, input.MouseToGameGrid().Y,previewTower.type);
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
            Tile currenttile = Utility.board.GetTile(new Point(x, y));
            if (Utility.board.IsTileOnPath(currenttile))
                return false;
            return true;
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(renderTarget01);
            GraphicsDevice.Clear(Color.Black);
            // TODO: Add your drawing code here
            spriteBatch.Begin();
            Utility.board.Draw(spriteBatch);
            HoveringOverTowerChecker(spriteBatch);
            // Highlight needs to be drawn before the actual towers
            if (previewTower != null)
            {
                if (IsWithinDimensions())
                    hud.DrawPlacementIndicator(spriteBatch, previewTower, IsValidTileForTower(input.MouseToGameGrid().X, input.MouseToGameGrid().Y));
                selectedTower.DrawSelectionHightlight(spriteBatch);
            }
            foreach (Tower t in towerList)
                t.Draw(spriteBatch);
            foreach(Path p in Utility.board.Paths)
            {
                p.DrawMinions(spriteBatch);
            }
            hud.DrawPlayTime(spriteBatch);
            spriteBatch.End();
            GraphicsDevice.SetRenderTarget(null);

            spriteBatch.Begin();
            spriteBatch.Draw(renderTarget01, new Vector2(0, 0), Color.White);
            spriteBatch.End();
            base.Draw(gameTime);
        }
        private void HoveringOverTowerChecker(SpriteBatch s)
        {
            if (!IsWithinDimensions())
                return;
            foreach (Tower t in towerList)
            {
                if (t.X == input.MouseToGameGrid().X && t.Y == input.MouseToGameGrid().Y && !t.IsDisabled)
                    hud.DrawRangeIndicators(s, new Point(t.X, t.Y) , t);
            }
        }
    }
}
