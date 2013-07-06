using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace _3DRTSGame
{
	public class AsteroidBelt : Drawable
	{
		private static int ASTEROID_NUM = 30;//100だとCLoneにしても30秒以上かかる

		public List<Asteroid> Asteroids { get; private set; }

		private Vector3 RandomRingPosition(Vector3 center, float min, float max)
		{
			float r = Utility.NextDouble(random, min, max);
			float theta = Utility.NextDouble(random, 0, 360);
			float x = r * (float)Math.Cos(MathHelper.ToRadians(theta));
			float y = r * (float)Math.Sin(MathHelper.ToRadians(theta));

			return new Vector3(x, 0, y);
		}
		private void Load()
		{
			Asteroids = new List<Asteroid>();
            Asteroid a = new Asteroid(Vector3.Zero, Vector3.Zero, 0.05f, 0, "Models\\Asteroid", true);
			for (int i = 0; i < ASTEROID_NUM; i++) {
				Vector3 pos = RandomRingPosition(Position, 3000, 3500);
                a.Position = pos;
				//Asteroids.Add(new Asteroid(pos, 0.05f, "Models\\Asteroid"));

				//Asteroids.Add(new Asteroid(pos, Vector3.Zero, 0.05f, 0, "Models\\Asteroid", true));
                Asteroids.Add((Asteroid)a.Clone());
			}
			foreach (Asteroid aa in Asteroids) {
				aa.Stational = true;
			}
		}

		public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
		{
			base.Update(gameTime);

			foreach (Asteroid a in Asteroids) {
				a.Update(gameTime);
			}
		}
		public override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);

			foreach (Asteroid a in Asteroids) {
				a.Draw(level.camera.View, level.camera.Projection, level.camera.Position);
			}
		}

		public AsteroidBelt(Level level, Vector3 position)
		{
			AsteroidBelt.level = level;
			this.Position = position;
			Load();
		}
	}
}
