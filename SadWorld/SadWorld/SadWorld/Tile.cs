using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SadWorld
{
    static class Tile
    {
        static public int TileWidth = 40;
        static public int TileHeight = 30;
        static public int TileStepX = 0;
        static public int TileStepY = 0;
        static public int OddRowXOffset = 0;
        static public int HeightTileOffset = 0;

        static public Texture2D TileSetTexture;
        

        static public Rectangle GetSourceRectangle(int tileIndex)
        {
            int tileY = tileIndex / (TileSetTexture.Width / TileWidth);
            int tileX = tileIndex % (TileSetTexture.Width / TileWidth);

            return new Rectangle(tileX * TileWidth , tileY * TileHeight, TileWidth, TileHeight);
        }
    }
}
