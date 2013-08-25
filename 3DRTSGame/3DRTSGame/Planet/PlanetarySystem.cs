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
		public Planet[] TargetPlanets { get; private set; }
        public Planet[] BackGroundPlanets { get; private set; }
        public Drawable[] BackGroundObjects { get; private set; }


		public void Load(Vector3[] sunPositions, Vector3[] planetPositions)
		{
            throw new NotImplementedException();
		}


		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			foreach (Sun s in Stars) {
				s.Update(gameTime);
			}
			foreach (Planet p in TargetPlanets) {
				p.Update(gameTime);
			}
            foreach (Planet p in BackGroundPlanets) {
                p.Update(gameTime);
            }
            foreach (Drawable p in BackGroundObjects) {
                p.Update(gameTime);
            }
		}
		public override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);

			foreach (Sun s in Stars) {
				s.Draw(gameTime);
			}
			foreach (Planet p in TargetPlanets) {
				p.Draw(gameTime);
			}
            foreach (Planet p in BackGroundPlanets) {
                p.Draw(gameTime);
            }
            foreach (Drawable p in BackGroundObjects) {
                p.Draw(gameTime);
            }
		}

        private void Initialize()
        {
            foreach (DamageablePlanet p in TargetPlanets) {
                level.Planets.Add(p);
                level.TargetPlanets.Add(p);
                level.Models.Add(p);
            }
            foreach (Planet p in BackGroundPlanets) {
                level.Planets.Add(p);
                level.Models.Add(p);
            }
            foreach (Drawable p in BackGroundObjects) {
                // AsteroidBeltなどは、UpdateとDrawさえすればよいので、Levelのリストに追加する必要はない
                //level.Models.Add(p);
            }
        }
		public PlanetarySystem()
			:this(new Vector3[] { Vector3.Zero }, new Vector3[] { new Vector3(0, 100, 100) })
		{
		}
		public PlanetarySystem(Vector3[] starPositions, Vector3[] planetPositions)
		{
			Load(starPositions, planetPositions);
		}
        public PlanetarySystem(Sun[] stars, DamageablePlanet[] targetPlnaets, Planet[] backGroundPlnaets, Drawable[] backGroundObjects)
        {
            this.Stars = stars;
            this.TargetPlanets = targetPlnaets;
            this.BackGroundPlanets = backGroundPlnaets;
            this.BackGroundObjects = backGroundObjects;

            // Levelのリストに追加する処理もここで行う
            Initialize();
        }
	}
}
