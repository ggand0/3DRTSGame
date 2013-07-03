using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace _3DRTSGame
{
	/// <summary>
	/// 惑星系。恒星、惑星の数・配置などを格納。位置は恒星の中心位置とする。
	/// </summary>
	public class PlanetarySystem : Drawable
	{
		public Sun[] Stars { get; private set; }
		public Planet[] Planets { get; private set; }

		public void Load(Vector3[] sunPositions, Vector3[] planetPositions)
		{

		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			foreach (Sun s in Stars) {
				s.Update(gameTime);
			}
			foreach (Planet p in Planets) {
				p.Update(gameTime);
			}
		}
		public override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);

			foreach (Sun s in Stars) {
				s.Draw(gameTime);
			}
			foreach (Planet p in Planets) {
				p.Draw(gameTime);
			}
		}

		
		public PlanetarySystem()
			:this(new Vector3[] { Vector3.Zero }, new Vector3[] { new Vector3(0, 100, 100) })
		{
		}
		public PlanetarySystem(Vector3[] sunPositions, Vector3[] planetPositions)
		{
			Load(sunPositions, planetPositions);
		}
	}
}
