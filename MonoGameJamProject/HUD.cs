﻿using MonoGame.Extended;
using MonoGameJamProject.Towers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace MonoGameJamProject
{
    class HUD
    {
        Input input;
        int gridSize;
        public HUD(Input iInput, int iGridSize)
        {
            input = iInput;
            gridSize = iGridSize;
        }

        public void DrawPlacementIndicator(SpriteBatch s, int minimumRange)
        {
            s.FillRectangle(new RectangleF(input.MouseGridPosition(gridSize).X * gridSize, input.MouseGridPosition(gridSize).Y * gridSize, gridSize, gridSize), Color.White * 0.5F);
            for (int i = -minimumRange; i <= minimumRange; i++)
                DrawMinimumRangeIndicators(s, new Point(input.MouseGridPosition(gridSize).X, input.MouseGridPosition(gridSize).Y), minimumRange);
        }

        public void DrawMinimumRangeIndicators(SpriteBatch s, Point origin, int minimumRange)
        {
            for (int i = -minimumRange; i <= minimumRange; i++)
            {
                for (int j = -minimumRange; j <= minimumRange; j++)
                {
                    if (i == 0 && j == 0)
                        continue;
                    s.FillRectangle(new RectangleF((origin.X + i) * gridSize, (origin.Y + j) * gridSize, gridSize, gridSize), Color.Red * 0.1f);
                }
            }
        }

        public int GridSize
        {
            get { return gridSize; }
            set { gridSize = value; }
        }
    }
}