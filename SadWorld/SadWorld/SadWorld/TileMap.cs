using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna;
using System.IO;
using System.Xml;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;



namespace SadWorld
{
    public class MapRow
    {
        public List<MapCell> Columns = new List<MapCell>();
    }

    public class TileMap
    {
        public List<MapRow> Rows = new List<MapRow>();
        public int MapWidth = 192; //7680 length in pixels
        public int MapHeight = 108; //3240 height in pixels
        public List<Rectangle> portals = new List<Rectangle>();
        public List<Vector2> treeVectors = new List<Vector2>();


        public int mapNumber;
        public int submapNumber;

        string[,] map = new string[192, 108];
        public string[,] overlayMap = new string[192, 108];

        public void LoadMap(int mapNum, ref Texture2D[,] colorBackground, ref Texture2D[,] grayBackground, ref Texture2D[,] parallax,
            ContentManager content, ref List<MovingPlatform> MovingPlatforms) //0 is regular, 1 is overlay
        {
            for (int i = 1; i < 5; i++)
            {
                for (int j = 1; j < 4; j++)
                {
                    colorBackground[i-1,j-1] = 
                        content.Load<Texture2D>(@"Textures\Backgrounds\Level" + mapNum + @"\Level" + mapNum + @"Color\Level" + mapNum + "_" + j + "_" + i);
                    grayBackground[i-1, j-1] =
                        content.Load<Texture2D>(@"Textures\Backgrounds\Level" + mapNum + @"\Level" + mapNum + @"Gray\Level" + mapNum + "_" + j + "_" + i);
                    parallax[i - 1, j - 1] =
                        content.Load<Texture2D>(@"Textures\Backgrounds\Level" + mapNum + @"\Parallax\Level" + mapNum + "_" + j + "_" + i);
                }
            }

            List<MovingPlatform> movingPlatforms = new List<MovingPlatform>();


            
            Character.hiddenTiles.Clear();
            mapNumber = mapNum;

            XmlDocument xmlDoc = new XmlDocument(); // load map from xml
            xmlDoc.Load(@"Content\Maps\Level" + mapNum + @"\Level" + mapNum + ".tmx");
            XmlNodeList Map = xmlDoc.GetElementsByTagName("data");
            XmlNodeList Map_Overlay = xmlDoc.GetElementsByTagName("data");

            string powerUps = Map[0].InnerText; //portal map
            string portalMap = Map[1].InnerText; //portal map
            string tempMapOverlay = Map[2].InnerText; // overlay map
            string tempMap = Map[3].InnerText; // collision map
            string triggerMap = Map[4].InnerText; // trigger map
            

            #region load map
            tempMap.Trim();
            string[] mapNums = tempMap.Split('\n'); //string of all rows


            int why = 0; // Y
            foreach (string str in mapNums)
            {
                if (str != "")
                {
                    string[] num = str.Split(','); //num = each tile

                    for (int x = 0; x < 192; x++)
                    {
                        int tileSetWidth = Tile.TileSetTexture.Width / 40;
                        map[x, why] = num[x];
                        if (num[x] == (tileSetWidth * 6 + 0).ToString() || num[x] == (tileSetWidth * 6 + 1).ToString() || num[x] == (tileSetWidth * 6 + 2).ToString())
                        {
                            Character.hiddenTiles.Add(new Vector2(x * Tile.TileWidth, why * Tile.TileHeight));
                        }
                        else if(int.Parse(num[x]) == (tileSetWidth * 7 + 1))
                        {
                            MovingPlatform plat = new MovingPlatform(true, true, new Vector2(x * Tile.TileWidth, why * Tile.TileHeight));
                            movingPlatforms.Add(plat);
                        }
                    }

                    why++;
                }
            }
            #endregion

            MovingPlatforms = movingPlatforms;

            #region load overlay map
            tempMapOverlay.Trim();
            string[] mapNums2 = tempMapOverlay.Split('\n');


            int whynot = 0; // Y
            foreach (string str in mapNums2)
            {
                if (str != "")
                {
                    string[] num = str.Split(',');

                    for (int x = 0; x < 192; x++)
                    {
                        overlayMap[x, whynot] = num[x];
                    }

                    whynot++;
                }
            }
            #endregion

            #region load portal map 
            portalMap.Trim();
            string[] mapNums3 = portalMap.Split('\n');

            int numTiles = (Tile.TileSetTexture.Width / 40) * (Tile.TileSetTexture.Height / 30); //calculate portal number from file based on number of tiles in map tilesheet

            int whynot2 = 0; // Y
            foreach (string str in mapNums3)
            {
                if (str != "")
                {
                    string[] num = str.Split(',');

                    for (int x = 0; x < 192; x++)
                    {
                        if(int.Parse(num[x]) != 0)
                        {
                            portals.Add(new Rectangle(x, whynot2, int.Parse(num[x]) - numTiles, 30)); //store level number in width
                        }
                    }

                    whynot2++;
                }
            }
            #endregion

            string[] mapNums4 = powerUps.Split('\n');
            Character.powerUpList = new List<Powerup>();
            
            int whynot3 = 0; // Y
            foreach (string str in mapNums4)
            {
                if (str != "")
                {
                    string[] num = str.Split(',');

                    for (int x = 0; x < 192; x++)
                    {
                        int number = int.Parse(num[x]);
                        if (number != 0)
                        {
                            Character.powerUp powerUp = Character.powerUp.None;

                            switch(number - 126)
                            {
                                case 1:
                                    powerUp = Character.powerUp.Bomb;
                                    break;
                                case 2:
                                    powerUp = Character.powerUp.Tiles;
                                    break;
                                case 3:
                                    powerUp = Character.powerUp.Deflect;
                                    break;
                                case 4:
                                    powerUp = Character.powerUp.Speed;
                                    break;
                            }
                            //Character.powerUpList.Add(new Powerup(
                            //    new Vector2(x * Tile.TileWidth, whynot3 * Tile.TileHeight), 
                            //    new Vector2(mapNum, submapNum),
                            //    number - 126,
                            //    powerUp)); //store level number in width
                        }
                    }

                    whynot3++;
                }
            }



            for (int y = 0; y < MapHeight; y++)
            {
                MapRow thisRow = new MapRow();
                for (int x = 0; x < MapWidth; x++)
                {
                    thisRow.Columns.Add(new MapCell(int.Parse(map[x, y])));
                }
                Rows.Add(thisRow);
            }

            xmlDoc.Load(@"Content\Maps\StartPosition.xml");
            XmlNodeList positions = xmlDoc.GetElementsByTagName("Level");
            string positionString = positions[mapNum - 1].InnerText;
            string[] startposition = positionString.Split(',');
            
            Character.position = new Vector2(int.Parse(startposition[0]) * Tile.TileWidth, int.Parse(startposition[1])* Tile.TileHeight - 30);


            #region load trigger map
            triggerMap.Trim();
            string[] triggerNums = triggerMap.Split('\n');


            int yLayer = 0; // Y
            foreach (string str in triggerNums)
            {
                if (str != "")
                {
                    string[] num = str.Split(',');

                    for (int x = 0; x < 192; x++)
                    {
                        if (int.Parse(num[x]) > 0)
                        {
                            Trigger trigger = new Trigger(new Vector2(x*40, yLayer * 30), int.Parse(num[x]) - 406);
                            Scripts.Level1.triggerList.Add(trigger);
                        }
                        
                    }

                    yLayer++;
                }
            }
            #endregion


            // read in map from file

            //string line = string.Empty;
            //using (StreamReader sr = new StreamReader(@"Content\Maps\" + levelName))
            //{
            //    int x = 0;
            //    int y = 0;

            //    while (!sr.EndOfStream)
            //    {
            //        line = sr.ReadLine().Trim();

            //        if (string.IsNullOrEmpty(line))
            //        {
            //            continue;
            //        }

            //        string[] numbers = line.Split(' ');

            //        foreach (string e in numbers)
            //        {
            //            map[x, y] = e;

            //            x++;
            //        }

            //        x = 0;
            //        y++;

            //    }
            //}
            for (int x = 0; x < 192; x++)
            {
                for (int y = 0; y < 108; y++)
                {
                    if (int.Parse(map[x, y]) >= 0)
                    {
                        Rows[y].Columns[x].TileID = int.Parse(map[x, y]); // get the correct tileID
                    }
                }
            }
            
        }
    }

    
}
