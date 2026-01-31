using System;
using Apos.Input;
using Apos.Shapes;
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
            _graphics = new GraphicsDeviceManager(this) {
#if KNI
                GraphicsProfile = GraphicsProfile.FL10_0
#else
                GraphicsProfile = GraphicsProfile.HiDef
#endif
            };
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
            _hud = new HUD();
            _sidebarUI = new Sidebar(new Vector2(190, 10));
            _difficultyCooldown = new CoolDownTimer(30f);
            _latestHoveredOverTower = null;
            ResetPlayingState();
            base.Initialize();
        }
        protected override void LoadContent() {
            InputHelper.Setup(this);
            _s = new SpriteBatch(GraphicsDevice);
            _sb = new ShapeBatch(GraphicsDevice, Content);
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
            InputHelper.UpdateSetup();
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            if(Utility.CurrentGameState == Utility.GameState.Playing) {
                UpdatePlayingState(gameTime);
                if (Utility.NumberOfLives < 0)
                {
                    Utility.AssetManager.PlayMusic("AbstractAmbiences-Mix_ST_37", 0.3F);
                    Utility.CurrentGameState = Utility.GameState.GameOver;
                }
            }
            else if(Utility.CurrentGameState == Utility.GameState.GameOver) {
                if (KeyboardCondition.Pressed(Keys.R)) {
                    Utility.CurrentGameState = Utility.GameState.Playing;
                    Utility.AssetManager.PlayMusic("break_space", 0.3F);
                    ResetPlayingState();
                }
            }

            InputHelper.UpdateCleanup();
            base.Update(gameTime);
        }
        protected override void Draw(GameTime gameTime) {
            if (Utility.CurrentGameState == Utility.GameState.Playing) {
                DrawPlayingState();
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
            if(MouseCondition.Pressed(MouseButton.RightButton) && IsWithinDimensions()) {
                if (Utility.MaxTowers > Utility.TowerList.Count)
                    AddTower(Utility.MouseToGameGrid().X, Utility.MouseToGameGrid().Y, Utility.TowerType.FlameThrower, Utility.TowerList.Count + 1);
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
        private void DrawPlayingState() {
            GraphicsDevice.SetRenderTarget(_renderTarget01);
            GraphicsDevice.Clear(Color.Black);
            // TODO: Add your drawing code here
            _sb.Begin();
            Utility.Board.Draw(_sb);
            if (_latestHoveredOverTower != null)
                HUD.DrawRangeIndicators(_sb, new Point(_latestHoveredOverTower.X, _latestHoveredOverTower.Y), _latestHoveredOverTower);

            // Highlight needs to be drawn before the actual towers
            if (_previewTower != null) {
                if (IsWithinDimensions())
                    HUD.DrawPlacementIndicator(_sb, _previewTower, IsValidTileForTower(Utility.MouseToGameGrid().X, Utility.MouseToGameGrid().Y));
                _selectedTower!.DrawSelectionHighlight(_sb);
            }
            foreach (Tower t in Utility.TowerList) t.Draw(_sb);
            _sb.End();
            _s.Begin();
            foreach (Tower t in Utility.TowerList) t.DrawHotkey(_s);
            foreach (Path p in Utility.Board.Paths) {
                p.DrawMinions(_s);
            }
            _s.End();
            _sb.Begin();
            // Draw projectiles and fire effect
            foreach(Tower t in Utility.TowerList) {
                if (t is Shotgun) (t as Shotgun)!.DrawProjectiles(_sb);
                else if (t is FlameThrower) (t as FlameThrower)!.DrawFireEffect(_sb);
            }
            _sb.End();
            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.Black);

            _s.Begin();
            _s.Draw(_renderTarget01, new Vector2(0, 0), Color.White);
            _s.End();
            // Draw sideBar
            _s.Begin();
            if (_latestHoveredOverTower != null) _sidebarUI.DrawTowerInfo(_s, _latestHoveredOverTower);
            _sidebarUI.Draw(_s);
            _s.End();
            _latestHoveredOverTower = null;
        }

        public static bool IsWithinDimensions() {
            if (Utility.MouseToGameGrid().X >= Utility.Board.FullWidth - 1 || Utility.MouseToGameGrid().X <= 0)
                return false;
            else if (Utility.MouseToGameGrid().Y >= Utility.Board.FullHeight - 1 || Utility.MouseToGameGrid().Y <= 0)
                return false;
            else
                return true;
        }
        private void TowerHotkeyHandler() {
            int goToTowerNumber = 0;
            Utility.TowerType type = Utility.TowerType.FlameThrower;
            // Check what number
            if (KeyboardCondition.Pressed(Keys.Q) || KeyboardCondition.Pressed(Keys.A) || KeyboardCondition.Pressed(Keys.Z))
                goToTowerNumber = 1;
            else if (KeyboardCondition.Pressed(Keys.W) || KeyboardCondition.Pressed(Keys.S) || KeyboardCondition.Pressed(Keys.X))
                goToTowerNumber = 2;
            else if (KeyboardCondition.Pressed(Keys.E) || KeyboardCondition.Pressed(Keys.D) || KeyboardCondition.Pressed(Keys.C))
                goToTowerNumber = 3;
            else if (KeyboardCondition.Pressed(Keys.R) || KeyboardCondition.Pressed(Keys.F) || KeyboardCondition.Pressed(Keys.V))
                goToTowerNumber = 4;
            else if (KeyboardCondition.Pressed(Keys.T) || KeyboardCondition.Pressed(Keys.G) || KeyboardCondition.Pressed(Keys.B))
                goToTowerNumber = 5;
            else if (KeyboardCondition.Pressed(Keys.Y) || KeyboardCondition.Pressed(Keys.H) || KeyboardCondition.Pressed(Keys.N))
                goToTowerNumber = 6;

            // Check type
            if (KeyboardCondition.Pressed(Keys.Q) || KeyboardCondition.Pressed(Keys.W) || KeyboardCondition.Pressed(Keys.E) || KeyboardCondition.Pressed(Keys.R) || KeyboardCondition.Pressed(Keys.T) || KeyboardCondition.Pressed(Keys.Y))
                type = Utility.TowerType.FlameThrower;
            else if (KeyboardCondition.Pressed(Keys.A) || KeyboardCondition.Pressed(Keys.S) || KeyboardCondition.Pressed(Keys.D) || KeyboardCondition.Pressed(Keys.F) || KeyboardCondition.Pressed(Keys.G) || KeyboardCondition.Pressed(Keys.H))
                type = Utility.TowerType.Shotgun;
            else if (KeyboardCondition.Pressed(Keys.Z) || KeyboardCondition.Pressed(Keys.X) || KeyboardCondition.Pressed(Keys.C) || KeyboardCondition.Pressed(Keys.V) || KeyboardCondition.Pressed(Keys.B) || KeyboardCondition.Pressed(Keys.N))
                type = Utility.TowerType.Sniper;

            if (Utility.TowerList.Count >= goToTowerNumber && goToTowerNumber != 0 && IsWithinDimensions()) {
                foreach (Tower t in Utility.TowerList) {
                    if (t.HotkeyNumber == goToTowerNumber && IsValidTileForTower(Utility.MouseToGameGrid().X, Utility.MouseToGameGrid().Y)) {
                        _selectedTower = null;
                        _previewTower = null;
                        AddTower(Utility.MouseToGameGrid().X, Utility.MouseToGameGrid().Y, type, t.HotkeyNumber);
                        Utility.TowerList.Remove(t);
                        break;
                    }
                }
            }
        }

        private void TowerSwitchInput() {
            if (_previewTower != null) {
                if (KeyboardCondition.Pressed(Keys.D1)) {
                    _previewTower = new FlameThrower(_selectedTower!.X, _selectedTower.Y, _previewTower.HotkeyNumber);
                }
                else if (KeyboardCondition.Pressed(Keys.D2)) {
                    _previewTower = new Shotgun(_selectedTower!.X, _selectedTower.Y, _previewTower.HotkeyNumber);
                }
                else if (KeyboardCondition.Pressed(Keys.D3)) {
                    _previewTower = new Sniper(_selectedTower!.X, _selectedTower.Y, _previewTower.HotkeyNumber);
                }
            }
        }
        private void TowerMovementChecker() {
            if (!IsWithinDimensions())
                return;

            if (MouseCondition.Pressed(MouseButton.LeftButton)) {
                if (_previewTower == null) {
                    foreach (Tower t in Utility.TowerList) {
                        if (t.X == Utility.MouseToGameGrid().X && t.Y == Utility.MouseToGameGrid().Y) {
                            _selectedTower = t;
                            _previewTower = t;
                        }
                    }
                }
                //TODO: check if there is no path on this position, and if there isn't place the tower
                else {
                    if (!IsValidTileForTower(Utility.MouseToGameGrid().X, Utility.MouseToGameGrid().Y))
                        return;
                    Utility.TowerList.Remove(_selectedTower!);
                    AddTower(Utility.MouseToGameGrid().X, Utility.MouseToGameGrid().Y, _previewTower.Type, _previewTower.HotkeyNumber);
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
                if (t.X == Utility.MouseToGameGrid().X && t.Y == Utility.MouseToGameGrid().Y && !t.IsDisabled)
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
        ShapeBatch _sb = null!;
        RenderTarget2D _renderTarget01 = null!;
        Tower? _latestHoveredOverTower;
        HUD _hud = null!;
        Sidebar _sidebarUI = null!;
        // list for all towers
        Tower? _selectedTower = null, _previewTower = null;
        CoolDownTimer _difficultyCooldown = null!;
    }
}
