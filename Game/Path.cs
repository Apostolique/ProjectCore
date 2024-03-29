using System.Collections.Generic;
using System.Linq;
using Apos.Shapes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameProject {
    /// <summary>
    /// Goal: Base class for the game's pathways.
    ///       Steps:
    ///         1) Pick a tile on the edges.
    ///         2) Make a list of all the surrounding tiles.
    ///         3) Eliminate tiles that have a path of an other color.
    ///         4) Pick one of the available tiles based on:
    ///             a) If path is short, bias towards the center.
    ///             b) If path is long, bias towers the edges.
    ///             c) Also bias for longer paths if the mode is easy.
    /// </summary>
    class Path {
        public Path() {
            pathway = [];
            MinionList = [];
            _spawnSequenceTimer = new CoolDownTimer(0.2f);
            _spawnSequenceTimer.Reset();
            _pathsShown = 0;
            Sequence = Animation.Spawn;
            _minionSpawner = new CoolDownTimer(10f);
            _minionSpawner.Reset();
            _spawner = new Spawner();
            _done = false;
        }

        public enum Animation { Spawn, Despawn, None }
        public List<Tile> pathway;
        public List<Minion> MinionList;
        public Animation Sequence { get; set; }
        public bool Done => _done;

        public void Add(Tile p) {
            pathway.Add(p);
        }
        public Tile First() {
            return pathway.First();
        }
        public Tile Last() {
            return pathway.Last();
        }
        public int Count() {
            return pathway.Count;
        }
        public bool Contains(Tile tile) {
            if (Sequence == Animation.Spawn) {
                for (int i = 0; i < _pathsShown; i++) {
                    if (pathway[i] == tile) {
                        return true;
                    }
                }
            } else if (Sequence == Animation.Despawn) {
                for (int i = _pathsShown; i < pathway.Count; i++) {
                    if (pathway[i] == tile) {
                        return true;
                    }
                }
            } else {
                return pathway.Contains(tile);
            }

            return false;
        }
        public void Despawn() {
            _spawnSequenceTimer.Reset();
            _pathsShown = 0;
            Sequence = Animation.Despawn;
        }
        public void AddMinion(Minion m) {
            m.FollowPath(this);
            MinionList.Add(m);
        }
        public void Update(GameTime gameTime) {
            _spawner.Update(gameTime, this);
            _minionSpawner.Update(gameTime);
            if (_minionSpawner.IsExpired && _spawner.IsActive) {
                _spawner.IsActive = false;
            }
            if (!_spawner.IsActive && MinionList.Count == 0 && Sequence != Animation.Despawn) {
                Despawn();
            }
            if (Sequence == Animation.Spawn) {
                _spawnSequenceTimer.Update(gameTime);
                if (_spawnSequenceTimer.IsExpired) {
                    _pathsShown++;
                    Utility.AssetManager.PlaySFX("Robot_Servo_006", 0.1f);
                    _spawnSequenceTimer.Reset();
                    if (_pathsShown >= pathway.Count) {
                        Sequence = Animation.None;
                    }
                }
            } else if (Sequence == Animation.Despawn) {
                _spawnSequenceTimer.Update(gameTime);
                if (_spawnSequenceTimer.IsExpired)
                {
                    _pathsShown++;
                    Utility.AssetManager.PlaySFX("Robot_Servo_006", 0.1f);
                    _spawnSequenceTimer.Reset();
                    if (_pathsShown >= pathway.Count)
                    {
                        Sequence = Animation.None;
                        _done = true;
                    }
                }
            }
            for(int i = MinionList.Count - 1; i >= 0; i--) {
                MinionList[i].Update(gameTime);
                if (MinionList[i].Dead) {
                    MinionList.Remove(MinionList[i]);
                    Utility.TotalNumberOfKills++;
                    Utility.AssetManager.PlaySFX("Death");
                } else if (!MinionList[i].IsMoving) {
                    MinionList.Remove(MinionList[i]);
                    Utility.NumberOfLives--;
                    Utility.AssetManager.PlaySFX("BREAK_HIT_Celery_2");
                }
            }
        }

        public void Draw(ShapeBatch sb) {
            if (Sequence == Animation.Spawn) {
                for (int i = 0; i < _pathsShown; i++) {
                    DrawPathTile(sb, i);
                }
            } else if (Sequence == Animation.Despawn) {
                for (int i = _pathsShown; i < pathway.Count; i++) {
                    DrawPathTile(sb, i);
                }
            } else {
                for (int i = 0; i < pathway.Count; i++) {
                    DrawPathTile(sb, i);
                }
            }
        }
        public void DrawLine(ShapeBatch sb) {
            if (Sequence == Animation.Spawn) {
                for (int i = 1; i < _pathsShown; i++) {
                    DrawPathLine(sb, i);
                }
            } else if (Sequence == Animation.Despawn) {
                for (int i = _pathsShown + 1; i < pathway.Count; i++) {
                    DrawPathLine(sb, i);
                }
            } else {
                for (int i = 1; i < pathway.Count; i++) {
                    DrawPathLine(sb, i);
                }
            }
        }
        public void DrawMinions(SpriteBatch s) {
            foreach (Minion m in MinionList)
                m.Draw(s);
        }
        private void DrawPathTile(ShapeBatch sb, int i) {
            sb.FillRectangle(new Vector2(Utility.GameToScreen(pathway[i].X), Utility.GameToScreen(pathway[i].Y)), new Vector2(Utility.Board.GridSize, Utility.Board.GridSize), new Color(19, 59, 131));
        }
        private void DrawPathLine(ShapeBatch sb, int i) {
            float x1 = Utility.GameToScreen(pathway[i].X + 0.5f);
            float y1 = Utility.GameToScreen(pathway[i].Y + 0.5f);
            float x2 = Utility.GameToScreen(pathway[i - 1].X + 0.5f);
            float y2 = Utility.GameToScreen(pathway[i - 1].Y + 0.5f);

            sb.DrawLine(new Vector2(x1, y1), new Vector2(x2, y2), Utility.Board.GridSize * 0.1f, Color.White, Color.Black, Utility.Board.GridSize * 0.001f);
        }

        private readonly Spawner _spawner;
        private readonly CoolDownTimer _spawnSequenceTimer;
        private readonly CoolDownTimer _minionSpawner;
        private int _pathsShown;
        private bool _done;
    }
}
