using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;
using MonoGame.Extended.BitmapFonts;
using MonoGameJamProject.Towers;
using MonoGameJamProject.UI;
using System;

namespace MonoGameJamProject
{
    public class MainClass : Game
    {
        private const int startingLives = 10;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        RenderTarget2D renderTarget01;
        Tower latestHoveredOverTower;
        Input input;
        HUD hud;
        Sidebar sidebarUI;
        // list for all towers
        Tower selectedTower = null, previewTower = null;
        CoolDownTimer difficultyCooldown;

        public MainClass()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
        }
        protected override void Initialize()
        {
            Utility.currentGamestate = Utility.GameState.Playing;
            // TODO: Add your initialization logic here
            Window.ClientSizeChanged += Window_ClientSizeChanged;
            Utility.Window = Window;
            Utility.board = new Board(15, 10);
            Utility.TowerList = new List<Tower>();
            input = new Input();
            hud = new HUD(input);
            sidebarUI = new Sidebar(new Vector2(190, 10));
            difficultyCooldown = new CoolDownTimer(30f);
            latestHoveredOverTower = null;
            ResetPlayingState();
            base.Initialize();
        }
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            renderTarget01 = new RenderTarget2D(GraphicsDevice, Utility.board.GridSize * Utility.board.Width, Utility.board.GridSize * Utility.board.Height);
            Utility.assetManager = new AssetManager(Content);
            Utility.assetManager.PlayMusic("break_space", 0.3F);
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
            if(Utility.currentGamestate == Utility.GameState.Playing)
            {
                UpdatePlayingState(gameTime);
                if (Utility.numberOfLives < 0)
                {
                    MediaPlayer.Stop();
                    Utility.currentGamestate = Utility.GameState.GameOver;
                }
            }
           else if(Utility.currentGamestate == Utility.GameState.GameOver)
            {
                input.Update();
                if (input.KeyPressed(Keys.R))
                {
                    Utility.currentGamestate = Utility.GameState.Playing;
                    Utility.assetManager.PlayMusic("break_space", 0.3F);
                    ResetPlayingState();
                }
            }

            base.Update(gameTime);
        }
        protected override void Draw(GameTime gameTime)
        {
            if (Utility.currentGamestate == Utility.GameState.Playing)
            {
                DrawPlayingState(spriteBatch);
            }
            else if (Utility.currentGamestate == Utility.GameState.GameOver)
            {
                GraphicsDevice.Clear(Color.Black);
                GraphicsDevice.SetRenderTarget(null);
                spriteBatch.Begin();
                string gameOverString = "GAME OVER!\nYou killed: " + Utility.totalNumberOfKills + " Minions!" + "\nYou reached difficulty: " + Utility.GameDifficulty + "\nAnd you survived for: " + Utility.tdGameTimer.Minutes.ToString("D2") + ":" + Utility.tdGameTimer.Seconds.ToString("D2") + "\nPress R to restart!";
                spriteBatch.DrawString(Utility.assetManager.GetFont("Jura"), gameOverString, new Vector2(200, 100), Color.Yellow, 0f, Vector2.Zero, 0.7F, SpriteEffects.None, 0);
                spriteBatch.End();
            }

            base.Draw(gameTime);
        }
        private void ResetPlayingState()
        {
            Utility.tdGameTimer = TimeSpan.Zero;
            Utility.numberOfLives = startingLives;
            Utility.totalNumberOfKills = 0;
            Utility.GameDifficulty = 0;
            difficultyCooldown.Reset();
            previewTower = null;
            selectedTower = null;
            Utility.board.ResetPaths();
            Utility.TowerList.Clear();
        }
        private void UpdatePlayingState(GameTime gameTime)
        {
            Utility.tdGameTimer += gameTime.ElapsedGameTime;

            difficultyCooldown.Update(gameTime);
            if (difficultyCooldown.IsExpired)
            {
                difficultyCooldown.Reset();
                Utility.GameDifficulty++;
            }
            
            Utility.board.Update(gameTime);
            HoveringOverTowerChecker();
            TowerMovementChecker();
            TowerHotkeyHandler();
            TowerSwitchInput();
            sidebarUI.Update(gameTime);
            input.Update();
            if(input.MouseRightButtonPressed && IsWithinDimensions())
            {
                if (Utility.MaxTowers > Utility.TowerList.Count)
                    AddTower(input.MouseToGameGrid().X, input.MouseToGameGrid().Y, Utility.TowerType.FlameThrower, Utility.TowerList.Count + 1);
            }
            for (int i = Utility.TowerList.Count - 1; i >= 0; i--)
            {
                Tile currenttile = Utility.board.GetTile(new Point(Utility.TowerList[i].X, Utility.TowerList[i].Y));
                if (Utility.board.IsTileOnPath(currenttile))
                    Utility.TowerList[i].IsDisabled = true;
                else
                    Utility.TowerList[i].IsDisabled = false;
                Utility.TowerList[i].Update(gameTime);
            }
            if (input.MouseLeftButtonPressed)
            {
                if (Utility.board.Paths.Count > 0)
                {
                    Minion m = new Minion(0, 0, Utility.MinionType.fast);
                    Utility.board.Paths[0].AddMinion(m);
                }
            }
        }
        private void DrawPlayingState(SpriteBatch spriteBatch)
        {
            GraphicsDevice.SetRenderTarget(renderTarget01);
            GraphicsDevice.Clear(Color.Black);
            // TODO: Add your drawing code here
            spriteBatch.Begin();
            Utility.board.Draw(spriteBatch);
            if (latestHoveredOverTower != null)
                hud.DrawRangeIndicators(spriteBatch, new Point(latestHoveredOverTower.X, latestHoveredOverTower.Y), latestHoveredOverTower);

            // Highlight needs to be drawn before the actual towers
            if (previewTower != null)
            {
                if (IsWithinDimensions())
                    hud.DrawPlacementIndicator(spriteBatch, previewTower, IsValidTileForTower(input.MouseToGameGrid().X, input.MouseToGameGrid().Y));
                selectedTower.DrawSelectionHightlight(spriteBatch);
            }
            foreach (Path p in Utility.board.Paths)
            {
                p.DrawMinions(spriteBatch);
            }
            // Draw projectiles and fire effect
            foreach(Tower t in Utility.TowerList)
            {
                if (t is Shotgun)
                    (t as Shotgun).DrawProjectiles(spriteBatch);
                if (t is FlameThrower)
                    (t as FlameThrower).DrawFireEffect(spriteBatch);
            }
            foreach (Tower t in Utility.TowerList)
                t.Draw(spriteBatch);
            spriteBatch.End();
            GraphicsDevice.SetRenderTarget(null);

            spriteBatch.Begin();
            spriteBatch.Draw(renderTarget01, new Vector2(0, 0), Color.White);
            spriteBatch.End();
            // Draw sideBar
            spriteBatch.Begin();
            if (latestHoveredOverTower != null)
                sidebarUI.DrawTowerInfo(spriteBatch, latestHoveredOverTower);
            sidebarUI.Draw(spriteBatch);
            spriteBatch.End();
            latestHoveredOverTower = null;
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
        private void TowerHotkeyHandler()
        {
            int goToTowerNumber = 0;
            Utility.TowerType type = Utility.TowerType.FlameThrower;
            // Check what number
            if (input.KeyPressed(Keys.Q) || input.KeyPressed(Keys.A) || input.KeyPressed(Keys.Z))
                goToTowerNumber = 1;
            else if (input.KeyPressed(Keys.W) || input.KeyPressed(Keys.S) || input.KeyPressed(Keys.X))
                goToTowerNumber = 2;
            else if (input.KeyPressed(Keys.E) || input.KeyPressed(Keys.D) || input.KeyPressed(Keys.C))
                goToTowerNumber = 3;
            else if (input.KeyPressed(Keys.R) || input.KeyPressed(Keys.F) || input.KeyPressed(Keys.V))
                goToTowerNumber = 4;
            else if (input.KeyPressed(Keys.T) || input.KeyPressed(Keys.G) || input.KeyPressed(Keys.B))
                goToTowerNumber = 5;
            else if (input.KeyPressed(Keys.Y) || input.KeyPressed(Keys.H) || input.KeyPressed(Keys.N))
                goToTowerNumber = 6;

            // Check type
            if (input.KeyPressed(Keys.Q) || input.KeyPressed(Keys.W) || input.KeyPressed(Keys.E) || input.KeyPressed(Keys.R) || input.KeyPressed(Keys.T) || input.KeyPressed(Keys.Y))
                type = Utility.TowerType.FlameThrower;
            else if (input.KeyPressed(Keys.A) || input.KeyPressed(Keys.S) || input.KeyPressed(Keys.D) || input.KeyPressed(Keys.F) || input.KeyPressed(Keys.G) || input.KeyPressed(Keys.H))
                type = Utility.TowerType.Shotgun;
            else if (input.KeyPressed(Keys.Z) || input.KeyPressed(Keys.X) || input.KeyPressed(Keys.C) || input.KeyPressed(Keys.V) || input.KeyPressed(Keys.B) || input.KeyPressed(Keys.N))
                type = Utility.TowerType.Sniper;

            if (Utility.TowerList.Count >= goToTowerNumber && goToTowerNumber != 0 && IsWithinDimensions())
            {
                foreach (Tower t in Utility.TowerList)
                {
                    if (t.HotkeyNumber == goToTowerNumber && IsValidTileForTower(input.MouseToGameGrid().X, input.MouseToGameGrid().Y))
                    {
                        AddTower(input.MouseToGameGrid().X, input.MouseToGameGrid().Y, type, t.HotkeyNumber);
                        Utility.TowerList.Remove(t);
                        break;
                    }
                }
            }
        }

        private void TowerSwitchInput()
        {
            if (previewTower != null)
            {
                if (input.KeyPressed(Keys.D1))
                {
                    previewTower = new FlameThrower(selectedTower.X, selectedTower.Y, previewTower.HotkeyNumber);
                }
                else if (input.KeyPressed(Keys.D2))
                {
                    previewTower = new Shotgun(selectedTower.X, selectedTower.Y, previewTower.HotkeyNumber);
                }
                else if (input.KeyPressed(Keys.D3))
                {
                    previewTower = new Sniper(selectedTower.X, selectedTower.Y, previewTower.HotkeyNumber);
                }
            }
        }
        private void TowerMovementChecker()
        {
            if (!IsWithinDimensions())
                return;

            if (input.MouseLeftButtonPressed)
            {
                if (previewTower == null)
                {
                    foreach (Tower t in Utility.TowerList)
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
                    Utility.TowerList.Remove(selectedTower);
                    AddTower(input.MouseToGameGrid().X, input.MouseToGameGrid().Y, previewTower.type, previewTower.HotkeyNumber);
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
            foreach (Tower t in Utility.TowerList)
            {
                if (t.X == x && t.Y == y)
                    return false;
            }
            Tile currenttile = Utility.board.GetTile(new Point(x, y));
            if (Utility.board.IsTileOnPath(currenttile))
                return false;
            return true;
        }
        private void HoveringOverTowerChecker()
        {
            if (!IsWithinDimensions())
                return;
            foreach (Tower t in Utility.TowerList)
            {
                if (t.X == input.MouseToGameGrid().X && t.Y == input.MouseToGameGrid().Y && !t.IsDisabled)
                    latestHoveredOverTower = t;
            }
        }
        private void AddTower(int x, int y, Utility.TowerType type, int HotKeyNumber)
        {
            Tower tower = null;
            switch(type)
            {
                case Utility.TowerType.FlameThrower:
                    tower = new FlameThrower(x, y, HotKeyNumber);
                    break;
                case Utility.TowerType.Sniper:
                    tower = new Sniper(x, y, HotKeyNumber);
                    break;
                case Utility.TowerType.Shotgun:
                    tower = new Shotgun(x, y, HotKeyNumber);
                    break;
                default: throw new ArgumentException("invalid tower type: " + type);
            }
            Utility.TowerList.Add(tower);
        }
    }
}
