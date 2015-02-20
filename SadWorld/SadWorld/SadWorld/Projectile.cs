using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;


namespace SadWorld
{
    public class Projectile
    {

        public Texture2D texture;
        public Vector2 movement;
        public Vector2 position;
        public bool timeToExplode = false;
        Texture2D particleTexture;
        List<ParticleEmitter> emitters = new List<ParticleEmitter>();
        public bool drawProjectile = true;
        public bool deleteMe = false;
        public bool hasIntersected = false;
        public ParticleEmitter explosionEmitter;
        public bool hasDeflected = false;
        float particleSizeOffset = 1f;
        int particleType;
        

        float speed = 25;

        public Projectile(Vector2 StartPosition, Vector2 playerPosition, Texture2D ParticleTexture, int projType)
        {
            particleTexture = ParticleTexture;
            movement = playerPosition - StartPosition;

            if (movement != Vector2.Zero)
                movement.Normalize();
            else
                movement = new Vector2(1, 0);
            position = StartPosition;

            particleType = projType;
            if(projType == 0)
                emitters.Add(new ParticleEmitter(movement, particleTexture, Color.Blue, Color.Green, new Vector2(0, 1), true));
            else
                emitters.Add(new ParticleEmitter(movement, particleTexture, Color.Purple, Color.Black, new Vector2(0, 1), true));
        }

        public void LoadContent(Texture2D Texture, ContentManager Content )
        {
            texture = Texture;
        }

        public bool Update(GameTime gameTime, float holeSize, Texture2D ParticleTextureGood)
        {
            if (particleType == 0)
            {
                position += movement * speed;

                foreach (ParticleEmitter emit in emitters)
                {
                    emit.Update(gameTime, position, hasIntersected, false, particleTexture, particleSizeOffset);
                    if (emit.numParticles == 0 && explosionEmitter != null && explosionEmitter.numParticles == 0)
                        deleteMe = true;
                }

                if (Vector2.Distance(Character.position, position) > 2000)
                    return true;
                else
                return false;
            }
            else if (particleType == 1)
            {
                if (!hasIntersected)
                {
                    position += movement * speed;
                    Rectangle auraRect = new Rectangle(
                            ((int)Character.position.X + 20) - (int)((300 * holeSize) / 2),
                            ((int)Character.position.Y + 30) - (int)((300 * holeSize) / 2),
                             (int)(300 * holeSize) - 20,
                             (int)(300 * holeSize) - 20);

                    Rectangle rect = new Rectangle((int)position.X, (int)position.Y, 20, 20);
                    Rectangle playerRect = new Rectangle((int)Character.position.X + 10, (int)Character.position.Y, 20, 60);
                    if (AuraIntersect(auraRect, rect) && !hasIntersected && !hasDeflected)
                    {
                        if (Character.currentPowerup.ToString() == "Deflect")
                        {
                            movement = position - (Character.position + new Vector2(20, 30));
                            particleTexture = ParticleTextureGood;
                            particleSizeOffset = .5f;
                            movement.Normalize();
                            hasDeflected = true;
                        }
                        else
                        {
                            Character.Health -= 5;
                            hasIntersected = true;
                        }
                        return false;
                    }
                }

                if (explosionEmitter != null)
                    explosionEmitter.Update(gameTime, position, hasIntersected, hasDeflected, particleTexture, particleSizeOffset);
                foreach (ParticleEmitter emit in emitters)
                {
                    emit.Update(gameTime, position, hasIntersected, hasDeflected, particleTexture, particleSizeOffset);
                    if (emit.numParticles == 0 && explosionEmitter != null && explosionEmitter.numParticles == 0)
                        deleteMe = true;
                }
                return true;
            }
            return false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            
            //if(!hasIntersected)
            //spriteBatch.Draw(texture, position, new Rectangle(0,0,50,50), Color.White, 0f, Vector2.Zero, .5f, SpriteEffects.None, 0f);
            foreach (ParticleEmitter emitter in emitters)
            {
                emitter.Draw(spriteBatch);
            }

            if(timeToExplode)
            {
                explosionEmitter.Draw(spriteBatch);
            }

        }

        bool AuraIntersect(Rectangle circle, Rectangle rect)
        {
            circle.X += 15;
            circle.Y += 15;
            circle.Width -= 15;
            circle.Height -= 15;
            Vector2 circleDistance;
            circleDistance.X = Math.Abs(circle.X + circle.Width / 2 - rect.X - rect.Width / 2);
            circleDistance.Y = Math.Abs(circle.Y + circle.Width / 2 - rect.Y - rect.Height / 2);

            if (circleDistance.X > rect.Width / 2 + circle.Width / 2) return false;
            if (circleDistance.Y > rect.Height / 2 + circle.Height / 2) return false;

            if (circleDistance.X <= (rect.Width / 2)) { return true; }
            if (circleDistance.Y <= (rect.Height / 2)) { return true; }

            double crnrDistance_sq = Math.Pow((circleDistance.X - rect.Width / 2), 2) + Math.Pow((circleDistance.Y - rect.Height / 2), 2);
            return crnrDistance_sq <= (Math.Pow(circle.Width / 2, 2));
        }
    }
}
