using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SadWorld
{
    public class Light
    {
        public Vector3 Position;
        public Vector4 Color;
        public float Power;

        internal void UpdateEffect(EffectParameter effectParameter)
        {
            effectParameter.StructureMembers["position"].SetValue(Position);
            effectParameter.StructureMembers["color"].SetValue(Color);
            effectParameter.StructureMembers["power"].SetValue(Power);
        }
    }
}
