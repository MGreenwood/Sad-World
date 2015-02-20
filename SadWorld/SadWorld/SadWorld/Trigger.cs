using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SadWorld
{
    public class Trigger
    {
        public Rectangle rect;
        public int triggerNumber; //separates trigger from one another to differentiate what the trigger does

        public Trigger(Vector2 position, int TriggerNum)
        {
            rect = new Rectangle((int)position.X,(int)position.Y,40,30);
            triggerNumber = TriggerNum;
        }
    }
}
