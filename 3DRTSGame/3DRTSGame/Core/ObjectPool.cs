using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace _3DRTSGame
{
	public class ObjectPool
	{
		public static bool LOADED = false;

		// １つのインスタンスしか使わないand必要ないので基本staticで
		private static readonly int ASTEROID_POOL_NUM = 30;
		private static readonly int SMALL_EXPLOSION_EFFECT_NUM = 50;
		private static readonly int MID_EXPLOSION_EFFECT_NUM = 50;
		private static readonly int BIG_EXPLOSION_EFFECT_NUM = 10;
		private static readonly int MAX_SATELLITE_NUM = 10;
		private static readonly int MAX_SPACESTATION_NUM = 3;

		public static Queue<Asteroid> AsteroidPool { get; private set; }
		public static Queue<ExplosionEffect> SmallExplosionPool { get; private set; }
		public static Queue<ExplosionEffect> MidExplosionPool { get; private set; }
		public static Queue<ExplosionEffect> BigExplosionPool { get; private set; }
		public static Queue<Satellite> SatellitePool { get; private set; }
		public static Queue<Satellite> SpaceStationPool { get; private set; }

		public static ContentManager content;
		public static GraphicsDevice graphicsDevice;
		private static ExplosionEffect smallExplosion, midExplosion, bigExplosion;

		//public static void Load(GraphicsProfile graphicsProfile)
        public static void Load(object graphicsProfile)
		{
            GraphicsProfile graphics = (GraphicsProfile)graphicsProfile;
			AsteroidPool = new Queue<Asteroid>();
			for (int i = 0; i < ASTEROID_POOL_NUM; i++) {
				// どうせ使うときに値変えるので適当に設定
				AsteroidPool.Enqueue(new Asteroid(Vector3.Zero, 1f, "Models\\Asteroid"));
			}
			// staticに出来ないorz

            if (graphics == GraphicsProfile.Light) {
                smallExplosion = new ExplosionEffect(content, graphicsDevice, "small", new Vector3(0, 50, 0), Vector2.One, false, "Xml\\Particle\\particleExplosionSmall.xml", false);
			    midExplosion = new ExplosionEffect(content, graphicsDevice, "mid", new Vector3(0, 50, 0), Vector2.One, false, "Xml\\Particle\\particleExplosion2.xml", false);
                bigExplosion = new ExplosionEffect(content, graphicsDevice, "big", new Vector3(0, 50, 0), Vector2.One, false, "Xml\\Particle\\particleExplosion1.xml", false);
            } else {
                smallExplosion = new ExplosionEffect(content, graphicsDevice, "small", new Vector3(0, 50, 0), Vector2.One, false, "Xml\\Particle\\particleExplosion0.xml", false);
                midExplosion = new ExplosionEffect(content, graphicsDevice, "mid", new Vector3(0, 50, 0), Vector2.One, false, "Xml\\Particle\\particleExplosion2.xml", false);
                bigExplosion = new ExplosionEffect(content, graphicsDevice, "big", new Vector3(0, 50, 0), Vector2.One, false, "Xml\\Particle\\particleExplosion1.xml", false);
            }
			

            for (int i = 0; i < SMALL_EXPLOSION_EFFECT_NUM; i++) {
				SmallExplosionPool.Enqueue((ExplosionEffect)smallExplosion.Clone());
			}
			for (int i = 0; i < MID_EXPLOSION_EFFECT_NUM; i++) {
				MidExplosionPool.Enqueue((ExplosionEffect)midExplosion.Clone());
			}
			
			for (int i = 0; i < BIG_EXPLOSION_EFFECT_NUM; i++) {
				BigExplosionPool.Enqueue((ExplosionEffect)bigExplosion.Clone());
			}
			//Satellite = new ArmedSatellite(new Vector3(300, 50, 300), star.Position, 5, "Models\\Dawn", "SoundEffects\\laser1");
			for (int i = 0; i < MAX_SATELLITE_NUM; i++) {//
				//SatellitePool.Enqueue((Satellite)Satellite.Clone());
				SatellitePool.Enqueue(new ArmedSatellite(new Vector3(300, 50, 300), Vector3.Zero, 5, "Models\\Dawn", "SoundEffects\\laser1"));
			}
			SpaceStationPool = new Queue<Satellite>();
			for (int i = 0; i < MAX_SPACESTATION_NUM; i++) {
				SpaceStationPool.Enqueue(new SpaceStation(false, new Vector3(300, 50, 300), Vector3.Zero, 100, "Models\\spacestation4"));
			}

			LOADED = true;
		}

		/*static ObjectPool(ContentManager content, GraphicsDevice graphicsDevice)
		{
			ObjectPool.content = content;
			ObjectPool.graphicsDevice = graphicsDevice;
		}*/
		static ObjectPool()
		{
			SmallExplosionPool = new Queue<ExplosionEffect>();
			MidExplosionPool = new Queue<ExplosionEffect>();
			BigExplosionPool = new Queue<ExplosionEffect>();
			SatellitePool = new Queue<Satellite>();
		}
	}
}
