﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SadWorld
{
    public class MapCell
    {
        public List<int> BaseTiles = new List<int>();
        public List<int> HeightTiles = new List<int>();

        public bool Walkable { get; set; }

        public int TileID
        {
            get { return BaseTiles.Count > 0 ? BaseTiles[0] : 0; }
            set
            {
                if (BaseTiles.Count > 0)
                    BaseTiles[0] = value;
                else
                    AddBaseTile(value);
            }
        }

        public void AddHeightTile(int tileID)
        {
            HeightTiles.Add(tileID);
        }

        public void AddBaseTile(int tileID)
        {
            BaseTiles.Add(tileID);
        }

        

        public MapCell(int tileID)
        {
            TileID = tileID;
            Walkable = true;
        }
    }
}
