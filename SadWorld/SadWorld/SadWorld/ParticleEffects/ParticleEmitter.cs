using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna;

namespace SadWorld
{
    public class ParticleEmitter
    {

        
       // Vector2 startPosition;
        //Vector2 position;
        List<Particle> particles = new List<Particle>();
        public Texture2D particleTexture;
        Vector2 velocity, direction;
        Color color1, color2;
        bool dofadeColor;
        public int numParticles;
        float scaleOffset = 1f;

        public ParticleEmitter(Vector2 projectileVelocity, Texture2D ParticleTexture, Color Color1, Color Color2, Vector2 particleDirection, bool fadeColor)
        {
            dofadeColor = fadeColor;
            color1 = Color1;
            color2 = Color2;            
            particleTexture = ParticleTexture;
            velocity = projectileVelocity;
            direction = particleDirection;
        }

        

        public void Update(GameTime gameTime, Vector2 projectilePosition, bool hasIntersected, bool hasDeflected, Texture2D particleTextureNew, float SizeOffset)
        {
            if (SizeOffset != scaleOffset)
                scaleOffset = SizeOffset;

            particleTexture = particleTextureNew;
            if (hasDeflected)
            {
                color1 = Color.Blue;
                color2 = Color.Green;
            }
            Random rand = new Random();


            if (!hasIntersected)
            {
                Particle particle = new Particle(color1);
                particle.position = projectilePosition;
                particle.velocity = velocity + direction + new Vector2(rand.Next(-2, 2), rand.Next(-2, 2));
                particles.Add(particle);

                particle = new Particle(color2);
                particle.position = projectilePosition;
                particle.velocity = velocity + direction + new Vector2(rand.Next(-2, 2), rand.Next(-2, 2));
                particles.Add(particle);
            }
            

            if(particles.Count() > 0)
            {
                for (int x = particles.Count() - 1; x > -1; x--)
                {
                    particles[x].startTime += gameTime.ElapsedGameTime.Milliseconds;
                    
                    particles[x].position += particles[x].velocity;
                    particles[x].color.A -= 10;

                    if (dofadeColor)
                    {
                        if (particles[x].color.R <= 255)
                            particles[x].color.R += 2;
                        if (particles[x].color.G <= 255)
                            particles[x].color.G += 2;
                        if (particles[x].color.B <= 255)
                            particles[x].color.B += 2;
                    }

                    if (particles[x].startTime > 350)
                    {
                        particles.RemoveAt(x);
                    }
                }
            }

            numParticles = particles.Count();

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if(particles.Count() != 0)
            foreach (Particle particle in particles)
            {
                spriteBatch.Draw(particleTexture, Camera.WorldToScreen(new Vector2(particle.position.X - (particleTexture.Width * particle.scale) / 2, particle.position.Y - (particleTexture.Height * particle.scale) / 2)),
                    new Rectangle(0, 0, particleTexture.Width, particleTexture.Height), 
                    particle.color, 0f,
                    Vector2.Zero,
                    particle.scale * scaleOffset,
                    SpriteEffects.None, 1f);
            }
        }
    }
}
