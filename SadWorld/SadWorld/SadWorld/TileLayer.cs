using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SadWorld
{
    public class TileLayer
    {
        static int tileWidth = 40;
        static int tileHeight = 30;

        public static int TileWidth
        {
            get { return tileWidth; }
            set
            {
                tileWidth = (int)MathHelper.Clamp(value, 20f, 100f);
            }

        }
        public static int TileHeight
        {
            get { return tileHeight; }
            set
            {
                tileWidth = (int)MathHelper.Clamp(value, 20f, 100f);
            }

        }

        int[,] map;

        public TileLayer(int width, int height)
        {
            map = new int[height, width];
        }

        public TileLayer(int[,] existingMap)
        {
            map = (int[,])existingMap.Clone();
        }

    }
}
