using System;
using FontStashSharp;
using GameProject.Towers;
using GameProject.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace GameProject {
    public class GameRoot : Game {
        public GameRoot() {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
        }

        protected override void Initialize() {
            Utility.CurrentGameState = Utility.GameState.Playing;
            // TODO: Add your initialization logic here
            Window.ClientSizeChanged += Window_ClientSizeChanged;
            Utility.Window = Window;
            Utility.Board = new Board(15, 10);
            Utility.TowerList = [];
            _input = new Input();
            _hud = new HUD(_input);
            _sidebarUI = new Sidebar(new Vector2(190, 10));
            _difficultyCooldown = new CoolDownTimer(30f);
            _latestHoveredOverTower = null;
            ResetPlayingState();
            base.Initialize();
        }
        protected override void LoadContent() {
            _s = new SpriteBatch(GraphicsDevice);
            _renderTarget01 = new RenderTarget2D(GraphicsDevice, Utility.Board.GridSize * Utility.Board.Width, Utility.Board.GridSize * Utility.Board.Height);
            Utility.AssetManager = new AssetManager(Content);
            Utility.AssetManager.PlayMusic("break_space", 0.3F);
            // TODO: use this.Content to load your game content here
        }
        void Window_ClientSizeChanged(object? sender, EventArgs e) {
            Utility.Board.CacheGridSize();
            int w = Utility.Board.GridSize * Utility.Board.Width;
            int h = Utility.Board.GridSize * Utility.Board.Height;
            if (w < 1) {
                w = 1;
            }
            if (h < 1) {
                h = 1;
            }
            _renderTarget01.Dispose();
            _renderTarget01 = new RenderTarget2D(GraphicsDevice, w, h);
        }
        protected override void Update(GameTime gameTime) {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            if(Utility.CurrentGameState == Utility.GameState.Playing) {
                UpdatePlayingState(gameTime);
                if (Utility.NumberOfLives < 0)
                {
                    MediaPlayer.Stop();
                    Utility.AssetManager.PlayMusic("AbstractAmbiences-Mix_ST_37", 0.3F);
                    Utility.CurrentGameState = Utility.GameState.GameOver;
                }
            }
            else if(Utility.CurrentGameState == Utility.GameState.GameOver) {
                _input.Update();
                if (_input.KeyPressed(Keys.R)) {
                    Utility.CurrentGameState = Utility.GameState.Playing;
                    MediaPlayer.Stop();
                    Utility.AssetManager.PlayMusic("break_space", 0.3F);
                    ResetPlayingState();
                }
            }

            base.Update(gameTime);
        }
        protected override void Draw(GameTime gameTime) {
            if (Utility.CurrentGameState == Utility.GameState.Playing) {
                DrawPlayingState(_s);
            }
            else if (Utility.CurrentGameState == Utility.GameState.GameOver) {
                GraphicsDevice.Clear(Color.Black);
                GraphicsDevice.SetRenderTarget(null);
                _s.Begin();
                string gameOverString = "GAME OVER!\nYou killed: " + Utility.TotalNumberOfKills + " Minions!" + "\nYour Score: " + Utility.Score.ToString() + "\nAnd you survived for: " + Utility.TDGameTimer.Minutes.ToString("D2") + ":" + Utility.TDGameTimer.Seconds.ToString("D2") + "\nPress R to restart!";
                _s.DrawString(Utility.AssetManager.GetFont(34), gameOverString, new Vector2(200, 100), Color.Yellow);
                _s.End();
            }

            base.Draw(gameTime);
        }
        private void ResetPlayingState() {
            Utility.TDGameTimer = TimeSpan.Zero;
            Utility.NumberOfLives = _startingLives;
            Utility.TotalNumberOfKills = 0;
            Utility.GameDifficulty = 0;
            _difficultyCooldown.Reset();
            _previewTower = null;
            _selectedTower = null;
            Utility.Board.ResetPaths();
            Utility.TowerList.Clear();
        }
        private void UpdatePlayingState(GameTime gameTime) {
            Utility.TDGameTimer += gameTime.ElapsedGameTime;

            _difficultyCooldown.Update(gameTime);
            if (_difficultyCooldown.IsExpired) {
                _difficultyCooldown.Reset();
                Utility.GameDifficulty++;
            }

            Utility.Board.Update(gameTime);
            HoveringOverTowerChecker();
            TowerMovementChecker();
            TowerHotkeyHandler();
            TowerSwitchInput();
            _sidebarUI.Update(gameTime);
            _input.Update();
            if(_input.MouseRightButtonPressed && IsWithinDimensions()) {
                if (Utility.MaxTowers > Utility.TowerList.Count)
                    GameRoot.AddTower(_input.MouseToGameGrid().X, _input.MouseToGameGrid().Y, Utility.TowerType.FlameThrower, Utility.TowerList.Count + 1);
            }
            for (int i = Utility.TowerList.Count - 1; i >= 0; i--) {
                Tile currenttile = Utility.Board.GetTile(new Point(Utility.TowerList[i].X, Utility.TowerList[i].Y));
                if (Utility.Board.IsTileOnPath(currenttile))
                    Utility.TowerList[i].IsDisabled = true;
                else
                    Utility.TowerList[i].IsDisabled = false;
                Utility.TowerList[i].Update(gameTime);
            }
            /*
            if (input.MouseLeftButtonPressed)
            {
                if (Utility.board.Paths.Count > 0)
                {
                    Minion m = new Minion(0, 0, Utility.MinionType.fast);
                    Utility.board.Paths[0].AddMinion(m);
                }
            }
            */
        }
        private void DrawPlayingState(SpriteBatch spriteBatch) {
            GraphicsDevice.SetRenderTarget(_renderTarget01);
            GraphicsDevice.Clear(Color.Black);
            // TODO: Add your drawing code here
            spriteBatch.Begin();
            Utility.Board.Draw(spriteBatch);
            if (_latestHoveredOverTower != null)
                HUD.DrawRangeIndicators(spriteBatch, new Point(_latestHoveredOverTower.X, _latestHoveredOverTower.Y), _latestHoveredOverTower);

            // Highlight needs to be drawn before the actual towers
            if (_previewTower != null) {
                if (IsWithinDimensions())
                    _hud.DrawPlacementIndicator(spriteBatch, _previewTower, GameRoot.IsValidTileForTower(_input.MouseToGameGrid().X, _input.MouseToGameGrid().Y));
                _selectedTower!.DrawSelectionHighlight(spriteBatch);
            }
            foreach (Tower t in Utility.TowerList) t.Draw(spriteBatch);
            foreach (Path p in Utility.Board.Paths) {
                p.DrawMinions(spriteBatch);
            }
            // Draw projectiles and fire effect
            foreach(Tower t in Utility.TowerList) {
                if (t is Shotgun) (t as Shotgun)!.DrawProjectiles(spriteBatch);
                else if (t is FlameThrower) (t as FlameThrower)!.DrawFireEffect(spriteBatch);
            }
            spriteBatch.End();
            GraphicsDevice.SetRenderTarget(null);

            spriteBatch.Begin();
            spriteBatch.Draw(_renderTarget01, new Vector2(0, 0), Color.White);
            spriteBatch.End();
            // Draw sideBar
            spriteBatch.Begin();
            if (_latestHoveredOverTower != null) _sidebarUI.DrawTowerInfo(spriteBatch, _latestHoveredOverTower);
            _sidebarUI.Draw(spriteBatch);
            spriteBatch.End();
            _latestHoveredOverTower = null;
        }

        public bool IsWithinDimensions() {
            if (_input.MouseToGameGrid().X >= Utility.Board.FullWidth - 1 || _input.MouseToGameGrid().X <= 0)
                return false;
            else if (_input.MouseToGameGrid().Y >= Utility.Board.FullHeight - 1 || _input.MouseToGameGrid().Y <= 0)
                return false;
            else
                return true;
        }
        private void TowerHotkeyHandler() {
            int goToTowerNumber = 0;
            Utility.TowerType type = Utility.TowerType.FlameThrower;
            // Check what number
            if (_input.KeyPressed(Keys.Q) || _input.KeyPressed(Keys.A) || _input.KeyPressed(Keys.Z))
                goToTowerNumber = 1;
            else if (_input.KeyPressed(Keys.W) || _input.KeyPressed(Keys.S) || _input.KeyPressed(Keys.X))
                goToTowerNumber = 2;
            else if (_input.KeyPressed(Keys.E) || _input.KeyPressed(Keys.D) || _input.KeyPressed(Keys.C))
                goToTowerNumber = 3;
            else if (_input.KeyPressed(Keys.R) || _input.KeyPressed(Keys.F) || _input.KeyPressed(Keys.V))
                goToTowerNumber = 4;
            else if (_input.KeyPressed(Keys.T) || _input.KeyPressed(Keys.G) || _input.KeyPressed(Keys.B))
                goToTowerNumber = 5;
            else if (_input.KeyPressed(Keys.Y) || _input.KeyPressed(Keys.H) || _input.KeyPressed(Keys.N))
                goToTowerNumber = 6;

            // Check type
            if (_input.KeyPressed(Keys.Q) || _input.KeyPressed(Keys.W) || _input.KeyPressed(Keys.E) || _input.KeyPressed(Keys.R) || _input.KeyPressed(Keys.T) || _input.KeyPressed(Keys.Y))
                type = Utility.TowerType.FlameThrower;
            else if (_input.KeyPressed(Keys.A) || _input.KeyPressed(Keys.S) || _input.KeyPressed(Keys.D) || _input.KeyPressed(Keys.F) || _input.KeyPressed(Keys.G) || _input.KeyPressed(Keys.H))
                type = Utility.TowerType.Shotgun;
            else if (_input.KeyPressed(Keys.Z) || _input.KeyPressed(Keys.X) || _input.KeyPressed(Keys.C) || _input.KeyPressed(Keys.V) || _input.KeyPressed(Keys.B) || _input.KeyPressed(Keys.N))
                type = Utility.TowerType.Sniper;

            if (Utility.TowerList.Count >= goToTowerNumber && goToTowerNumber != 0 && IsWithinDimensions()) {
                foreach (Tower t in Utility.TowerList) {
                    if (t.HotkeyNumber == goToTowerNumber && GameRoot.IsValidTileForTower(_input.MouseToGameGrid().X, _input.MouseToGameGrid().Y)) {
                        _selectedTower = null;
                        _previewTower = null;
                        GameRoot.AddTower(_input.MouseToGameGrid().X, _input.MouseToGameGrid().Y, type, t.HotkeyNumber);
                        Utility.TowerList.Remove(t);
                        break;
                    }
                }
            }
        }

        private void TowerSwitchInput() {
            if (_previewTower != null) {
                if (_input.KeyPressed(Keys.D1)) {
                    _previewTower = new FlameThrower(_selectedTower!.X, _selectedTower.Y, _previewTower.HotkeyNumber);
                }
                else if (_input.KeyPressed(Keys.D2)) {
                    _previewTower = new Shotgun(_selectedTower!.X, _selectedTower.Y, _previewTower.HotkeyNumber);
                }
                else if (_input.KeyPressed(Keys.D3)) {
                    _previewTower = new Sniper(_selectedTower!.X, _selectedTower.Y, _previewTower.HotkeyNumber);
                }
            }
        }
        private void TowerMovementChecker() {
            if (!IsWithinDimensions())
                return;

            if (_input.MouseLeftButtonPressed) {
                if (_previewTower == null) {
                    foreach (Tower t in Utility.TowerList) {
                        if (t.X == _input.MouseToGameGrid().X && t.Y == _input.MouseToGameGrid().Y) {
                            _selectedTower = t;
                            _previewTower = t;
                        }
                    }
                }
                //TODO: check if there is no path on this position, and if there isn't place the tower
                else {
                    if (!GameRoot.IsValidTileForTower(_input.MouseToGameGrid().X, _input.MouseToGameGrid().Y))
                        return;
                    Utility.TowerList.Remove(_selectedTower!);
                    GameRoot.AddTower(_input.MouseToGameGrid().X, _input.MouseToGameGrid().Y, _previewTower.Type, _previewTower.HotkeyNumber);
                    _previewTower = null;
                }
            }
        }
        /// <summary>
        /// Checks if the given position is valid for a tower on the game grid
        /// </summary>
        /// <param name="x">x coordinate to check</param>
        /// <param name="y">y coordinate to check</param>
        /// <returns>whether the given position is valid for the tower</returns>
        private static bool IsValidTileForTower(int x, int y) {
            foreach (Tower t in Utility.TowerList)
            {
                if (t.X == x && t.Y == y)
                    return false;
            }
            Tile currenttile = Utility.Board.GetTile(new Point(x, y));
            if (Utility.Board.IsTileOnPath(currenttile))
                return false;
            return true;
        }
        private void HoveringOverTowerChecker() {
            if (!IsWithinDimensions())
                return;
            foreach (Tower t in Utility.TowerList) {
                if (t.X == _input.MouseToGameGrid().X && t.Y == _input.MouseToGameGrid().Y && !t.IsDisabled)
                    _latestHoveredOverTower = t;
            }
        }
        private static void AddTower(int x, int y, Utility.TowerType type, int HotKeyNumber) {
            Tower tower = type switch {
                Utility.TowerType.FlameThrower => new FlameThrower(x, y, HotKeyNumber),
                Utility.TowerType.Sniper => new Sniper(x, y, HotKeyNumber),
                Utility.TowerType.Shotgun => new Shotgun(x, y, HotKeyNumber),
                _ => throw new ArgumentException("invalid tower type: " + type),
            };
            Utility.TowerList.Add(tower);
        }

        private const int _startingLives = 10;
        readonly GraphicsDeviceManager _graphics;
        SpriteBatch _s = null!;
        RenderTarget2D _renderTarget01 = null!;
        Tower? _latestHoveredOverTower;
        Input _input = null!;
        HUD _hud = null!;
        Sidebar _sidebarUI = null!;
        // list for all towers
        Tower? _selectedTower = null, _previewTower = null;
        CoolDownTimer _difficultyCooldown = null!;
    }
}
