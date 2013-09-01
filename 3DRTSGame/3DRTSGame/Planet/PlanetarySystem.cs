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
		private CircleRenderer orbitRenderer;

		public Sun[] Stars { get; private set; }
		public List<DamageablePlanet> TargetPlanets { get; private set; }
        public List<Planet> BackGroundPlanets { get; private set; }
        public List<Drawable> BackGroundObjects { get; private set; }


		public void RemoveObjects()
		{

		}
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
			/*foreach (Planet p in TargetPlanets) {
				p.Update(gameTime);
			}
            foreach (Planet p in BackGroundPlanets) {
                p.Update(gameTime);
            }*/
            foreach (Drawable p in BackGroundObjects) {
                p.Update(gameTime);
            }
		}


        public void Draw(GameTime gameTime, Camera camera)
        {
            base.Draw(camera);

            foreach (Sun s in Stars) {
                s.Draw(false, camera.View, camera.Projection);
            }
            /*foreach (Planet p in TargetPlanets) {
                p.Draw(camera.View, camera.Projection, camera.Position);
            }
            foreach (Planet p in BackGroundPlanets) {
                p.Draw(camera.View, camera.Projection, camera.Position);
            }*/
            foreach (Drawable p in BackGroundObjects) {
                //p.Draw(camera.View, camera.Projection, camera.Position);
                p.Draw(gameTime);
            }
        }


		#region Constructors
        /// <summary>
        /// Levelの該当リストへそれぞれ追加する。ライティング処理を反映させるために必要。
        /// </summary>
		private void AddObjectsToLevelList()
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
            }/**/
        }
		private void Initialize()
		{
			/*TargetPlanets = new List<DamageablePlanet>();
			BackGroundPlanets = new List<Planet>();
			BackGroundObjects = new List<Drawable>();*/
            AddObjectsToLevelList();
            // PLanet.Drawで描画させるべき
			//orbitRenderer = new CircleRenderer(Level.graphicsDevice, Color.White, 50);
		}
		public PlanetarySystem()
			:this(new Vector3[] { Vector3.Zero }, new Vector3[] { new Vector3(0, 100, 100) })
		{
		}
		public PlanetarySystem(Vector3[] starPositions, Vector3[] planetPositions)
		{
			Initialize();
			Load(starPositions, planetPositions);
		}
        public PlanetarySystem(Sun[] stars, DamageablePlanet[] targetPlnaets, Planet[] backGroundPlnaets, Drawable[] backGroundObjects)
        {

            this.Stars = stars;
            this.TargetPlanets = targetPlnaets.ToList<DamageablePlanet>();
            this.BackGroundPlanets = backGroundPlnaets.ToList<Planet>();
            this.BackGroundObjects = backGroundObjects.ToList<Drawable>();

            // Levelのリストに追加する処理もここで行う
            //AddObjectsToLevelList();
			Initialize();
		}
		#endregion
	}
}
