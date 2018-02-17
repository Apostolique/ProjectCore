using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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
            Utility.numberOfLives = startingLives;
            Utility.Window = Window;
            Utility.board = new Board(15, 10);
            Utility.TowerList = new List<Tower>();
            input = new Input();
            hud = new HUD(input);
            sidebarUI = new Sidebar(new Vector2(190, 10));
            Utility.tdGameTimer = TimeSpan.Zero;
            difficultyCooldown = new CoolDownTimer(30f);
            difficultyCooldown.Reset();
            latestHoveredOverTower = null;
            Utility.board.GeneratePath();
            base.Initialize();
        }
        private void AddTower(int x, int y, Utility.TowerType type, int HotKeyNumber)
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
            tower.HotKeyNumber = HotKeyNumber;
            Utility.TowerList.Add(tower);
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
            if(Utility.currentGamestate == Utility.GameState.Playing)
            {
                UpdatePlayingState(gameTime);
                if (Utility.numberOfLives < 0)
                    Utility.currentGamestate = Utility.GameState.GameOver;
            }
           else if(Utility.currentGamestate == Utility.GameState.GameOver)
            {
                input.Update();
                if (input.KeyPressed(Keys.R))
                {
                    Utility.currentGamestate = Utility.GameState.Playing;
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
                string gameOverString = "GAME OVER!\nYour number of kills: " + Utility.totalNumberOfKills + "\nYour reached difficulty: " + Utility.GameDifficulty + "\nPress R to restart!";
                spriteBatch.DrawString(Utility.assetManager.GetFont("Jura"), gameOverString, new Vector2(200, 100), Color.Yellow, 0f, Vector2.Zero, 0.7F, SpriteEffects.None, 0);
                spriteBatch.End();
            }

            base.Draw(gameTime);
        }

        private void ResetPlayingState()
        {
            Utility.numberOfLives = startingLives;
            Utility.totalNumberOfKills = 0;
            Utility.GameDifficulty = 0;
            previewTower = null;
            selectedTower = null;
            Utility.board.ResetPaths();
            Utility.TowerList.Clear();
            // Reset the difficulty etc.

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
            TowerHotkeySelector();
            NewTowerPlacementChecker();
            TowerSwitchInput();
            sidebarUI.Update(gameTime);
            input.Update();
            for (int i = Utility.TowerList.Count - 1; i >= 0; i--)
            {
                Tile currenttile = Utility.board.GetTile(new Point(Utility.TowerList[i].X, Utility.TowerList[i].Y));
                if (Utility.board.IsTileOnPath(currenttile))
                    Utility.TowerList[i].IsDisabled = true;
                else
                    Utility.TowerList[i].IsDisabled = false;
                Utility.TowerList[i].Update(gameTime);
            }

            /*if (input.MouseMiddleButtonPressed)
            {
                if (Utility.board.Paths.Count > 1)
                {
                    Utility.board.ClearPaths();
                }
                Utility.board.GeneratePath();
            }
            if (input.MouseLeftButtonPressed)
            {
                if (Utility.board.Paths.Count > 0)
                {
                    Minion m = new Minion(0, 0, Utility.MinionType.boss);
                    Utility.board.Paths[0].AddMinion(m);
                }
            }*/
        }
        private void NewTowerPlacementChecker()
        {
            if (!IsWithinDimensions())
                return;

            if (input.MouseRightButtonPressed)
            {
                if (!IsValidTileForTower(input.MouseToGameGrid().X, input.MouseToGameGrid().Y))
                {
                    Utility.assetManager.PlaySFX("sfx_error");
                    return;
                }
                if(Utility.maxTowers - Utility.TowerList.Count > 0)
                {
                    AddTower(input.MouseToGameGrid().X, input.MouseToGameGrid().Y, Utility.TowerType.Shotgun, Utility.TowerList.Count + 1);
                }
                else
                    Utility.assetManager.PlaySFX("sfx_error");
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
            foreach (Tower t in Utility.TowerList)
                t.Draw(spriteBatch);
            foreach (Path p in Utility.board.Paths)
            {
                p.DrawMinions(spriteBatch);
            }

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

        private void TowerHotkeySelector()
        {
            int goToTowerNumber = 0;
            if (input.KeyPressed(Keys.Q))
                goToTowerNumber = 1;
            else if (input.KeyPressed(Keys.W))
                goToTowerNumber = 2;
            else if (input.KeyPressed(Keys.E))
                goToTowerNumber = 3;
            else if (input.KeyPressed(Keys.R))
                goToTowerNumber = 4;
            else if (input.KeyPressed(Keys.R))
                goToTowerNumber = 5;
            else if (input.KeyPressed(Keys.Y))
                goToTowerNumber = 6;

            if(Utility.TowerList.Count >= goToTowerNumber && goToTowerNumber != 0)
            {
                foreach(Tower t in Utility.TowerList)
                {
                    if(t.HotKeyNumber == goToTowerNumber)
                    {
                        selectedTower = t;
                        previewTower = t;
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
                    AddTower(input.MouseToGameGrid().X, input.MouseToGameGrid().Y, previewTower.type, previewTower.HotKeyNumber);
                    Utility.TowerList.Remove(selectedTower);
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
    }
}
