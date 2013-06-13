using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace _3DRTSGame
{
	public struct EnemyInfo
	{
		Type EnemyType { get; set; }
		float SpawnProbability { get; set; }

		public EnemyInfo(Type type, float prob)
		{
			this.EnemyType = type;
			this.SpawnProbability = prob;
		}
	}
	public struct EnemyWave
	{
		public EnemyInfo[] EnemyData { get; set; }
		public int AILevel { get; set; }
		public int SpawnRate { get; set; }
		public int SpawnNum { get; set; }
		public float MaxDistance { get; set; }
		/// <summary>
		/// Waveが開始されてから、最後の敵ユニットが投入されるまでの時間
		/// </summary>
		public float MaxTimeSec { get; set; }

		public EnemyWave(EnemyInfo[] data, int aiLevel, int rate, int num, float dist, float time)
		{
			this.EnemyData = data;
			this.AILevel = aiLevel;
			this.SpawnRate = rate;
			this.SpawnNum = num;
			this.MaxDistance = dist;
			this.MaxTimeSec = time;
		}
	}
	public enum WaveState
	{
		Interval, 
		Start,
		Playing
	}

	public class EnemyManager
	{
		//public static Level4 level;

		private static readonly int ASTEROID_MAX_SPAWN_NUM = 15;
		private static readonly int FIGHTER_MAX_SPAWN_NUM = 8;

		private Level4 level;
		private Random random = new Random();
		private List<EnemyWave> waves = new List<EnemyWave>();

		/// <summary>
		/// Spawnし終わったかどうか
		/// </summary>
		public bool WaveEnd { get; private set; }

		private WaveState state;
		private float start;


		public static float NextDouble(Random r, double min, double max)
		{
			return (float)(min + r.NextDouble() * (max - min));
		}
		private void AddAsteroids(int asteroidNum, float radius)
		{
			for (int i = 0; i < asteroidNum; i++) {
				//random = new Random();
				level.Asteroids.Add(new Asteroid(new Vector3(NextDouble(random, -radius, radius), 0, NextDouble(random, -radius, radius))
					, level.sun.Position, 0.05f, "Models\\Asteroid"));
				//Asteroids[i].Scale = 0.02f;//0.1f;
				//Asteroids[i].SetModelEffect(lightingEffect, true);					// set effect to each modelmeshpart
			}
		}
		private void SpawnAsteroids()
		{
			if (level.Enemies.Count < ASTEROID_MAX_SPAWN_NUM) {// = 15 -5
				float radius = 3000;
				Asteroid a = new Asteroid(new Vector3(NextDouble(random, -radius, radius)
					, 0, NextDouble(random, -radius, radius)), level.sun.Position, 0.05f, "Models\\Asteroid");
				//Asteroids[i].Scale = 0.02f;//0.1f;
				//a.SetModelEffect(shadowEffect, true);					// set effect to each modelmeshpart
				a.IsActive = true;
				a.RenderBoudingSphere = false;
				level.Enemies.Add(a);
				level.Models.Add(a);
			}
		}
		private void SpawnFighters()
		{
			if (level.Enemies.Count < ASTEROID_MAX_SPAWN_NUM) {// = 15 -5
				float radius = 3000;
				Fighter f = new Fighter(new Vector3(NextDouble(random, -radius, radius)	, NextDouble(random, -radius, radius), NextDouble(random, -radius, radius))
					, level.Planets[0].Position, 20f, "Models\\fighter0");
				f.IsActive = true;
				f.RenderBoudingSphere = false;
				level.Enemies.Add(f);
				level.Models.Add(f);
			}
		}
		public void Update(GameTime gameTime)
		{
			// MAX_NUMよりspawnしてるオブジェクトの数が減ったら追加するだけのVersion
			/*SpawnAsteroids();
			SpawnFighters();*/

			// Wave準拠Ver
			float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
			if (state == WaveState.Start) {
				start = elapsedTime;
			} else if (state == WaveState.Playing) {
				float time = elapsedTime - start;

				if (time % waves[0].SpawnRate == 0) {
					double prob = random.NextDouble();


				}
			}
		}

		private void Initialize()
		{
			waves.Add(new EnemyWave(new EnemyInfo[] { new EnemyInfo(typeof(Asteroid), 0.5f), new EnemyInfo(typeof(Fighter), 0.5f) } ,0, 3, 3, 3000, 60));
		}
		public EnemyManager(Level4 level)
		{
			this.level = level;
			Initialize();
		}
	}
}
