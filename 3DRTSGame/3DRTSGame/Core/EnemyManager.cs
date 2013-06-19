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
		public Type EnemyType { get; set; }
		public float SpawnProbability { get; set; }

		public EnemyInfo(Type type, float prob)
			: this()
		{
			EnemyType = type;
			SpawnProbability = prob;
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
			: this()
		{
			EnemyData = data;
			AILevel = aiLevel;
			SpawnRate = rate;
			SpawnNum = num;
			MaxDistance = dist;
			MaxTimeSec = time;
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
		//private float start;
		//private DateTime start;
		private int count, start;
		private Dictionary<String, object[]> arguments = new Dictionary<string, object[]>();

        private Dictionary<string, Object> enemiesOrg = new Dictionary<string, Object>();


        #region Methods
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
				Fighter f = new Fighter(new Vector3(NextDouble(random, -radius, radius), NextDouble(random, -radius, radius), NextDouble(random, -radius, radius))
					, level.Planets[0].Position, 20f, "Models\\fighter0");
				f.IsActive = true;
				f.RenderBoudingSphere = false;
				level.Enemies.Add(f);
				level.Models.Add(f);
			}
		}

		Asteroid a; Fighter f;
		private void SpawnEnemies(Type enemyType)
		{
			// 乱数が入らなかったらDictionaryで予めargumentsを計算してスマートにインスタンス生成が出来るのだが、
			// 乱数入りなので仕方なく関数内で条件分岐させることに。さらにswitch
			float radius = 3000;
			/*if (enemyType == typeof(Asteroid)) {
				a = new Asteroid(new Vector3(NextDouble(random, -radius, radius)
					, 0, NextDouble(random, -radius, radius)), level.sun.Position, 0.05f, "Models\\Asteroid");
				a.RenderBoudingSphere = false;
				level.Models.Add(a);
				level.Enemies.Add(a);
			} else if (enemyType == typeof(Fighter)) {
				//f = new Fighter(new Vector3(NextDouble(random, -radius, radius), NextDouble(random, -radius, radius), NextDouble(random, -radius, radius))
					//, level.Planets[0].Position, 20f, "Models\\fighter0");
				f = new Fighter(new Vector3(NextDouble(random, -radius, radius), NextDouble(random, -radius, radius), NextDouble(random, -radius, radius))
					, level.TargetPlanets[0].Position, 20f, "Models\\fighter0");
				f.RenderBoudingSphere = false;
				level.Models.Add(f);
				level.Enemies.Add(f);
			}*/
            if (enemyType == typeof(Asteroid)) {
                enemiesOrg["Asteroid"].Position = new Vector3(NextDouble(random, -radius, radius)
                    , 0, NextDouble(random, -radius, radius));
                enemiesOrg["Asteroid"].RenderBoudingSphere = false;

                a = (Asteroid)enemiesOrg["Asteroid"].Clone();
                a.Destination = level.TargetPlanets[0].Position;

                level.Models.Add(a);
                level.Enemies.Add(a);
            } else if (enemyType == typeof(Fighter)) {
                enemiesOrg["Fighter"].Position = new Vector3(NextDouble(random, -radius, radius),
                    NextDouble(random, -radius, radius), NextDouble(random, -radius, radius));
                enemiesOrg["Fighter"].RenderBoudingSphere = false;
                //enemiesOrg["Fighter"].Target = level.TargetPlanets[0];

                f = (Fighter)enemiesOrg["Fighter"].Clone();
                f.Target = level.TargetPlanets[0].Position;

                level.Models.Add(f);
                level.Enemies.Add(f);
            }
		}
		public void Update(GameTime gameTime)
		{
			// MAX_NUMよりspawnしてるオブジェクトの数が減ったら追加するだけのVersion
			/*SpawnAsteroids();
			SpawnFighters();*/
			count++;

			// Wave準拠Ver
			//float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
			if (state == WaveState.Start) {
				//start = DateTime.Now;
				start = count;// フレームで管理したいのでカウンタを作成した
				state = WaveState.Playing;
			} else if (state == WaveState.Playing) {
				//float time = (float)(DateTime.Now - start).TotalSeconds;
				//int time = (int)(DateTime.Now - start).TotalSeconds;
				int time = (count - start);

				if (time > waves[0].MaxTimeSec * 60) {
					state = WaveState.Interval;
					start = count;
				} else if (time % (waves[0].SpawnRate * 60) == 0) {// 60FPS
					for (int n = 0; n < waves[0].SpawnNum; n++) {
						double prob = random.NextDouble();
						double probs = prob;
						double sum = 0;

						foreach (EnemyInfo i in waves[0].EnemyData) {
							/*if (prob < waves[0].EnemyData[0].SpawnProbability) {// 要修正
								SpawnEnemies(i.EnemyType);
								break;
							}*/
							if (prob + sum < i.SpawnProbability) {
								SpawnEnemies(i.EnemyType);
								if (i.EnemyType.Name == "Fighter") {
									string d = "ok";
								}
								break;
							} else {
								//sum += i.SpawnProbability;
								//sum += prob;
								prob -= i.SpawnProbability;
							}
						}
					}
				}

			} else if (state == WaveState.Interval) {
				if (count - start > 10 * 60) {// 10秒で次のwaveへ
					// wavesのカウントに達していたらクリア

					// 残っているなら次のwaveへ
				}
			}
		}
        #endregion

        private void Initialize()
		{
			waves.Add(new EnemyWave(new EnemyInfo[] { new EnemyInfo(typeof(Asteroid), 0.5f), new EnemyInfo(typeof(Fighter), 0.5f) }, 0, 2, 3, 3000, 30));

			arguments.Add("_3DRTSGame.Asteroid", new object[] {
			});
			arguments.Add("_3DRTSGame.Fighter", new object[] {
			});

			state = WaveState.Start;

            // asteroid:4引数コンストラクタ以外を使うとeffectが初期化されずにnullのままになるので注意
            enemiesOrg.Add("Asteroid", new Asteroid(Vector3.Zero, Vector3.Zero, 0.05f, "Models\\Asteroid"));
            enemiesOrg.Add("Fighter", new Fighter(Vector3.Zero, Vector3.Zero, 20f, "Models\\fighter0"));
		}
		public EnemyManager(Level4 level)
		{
			this.level = level;
			Initialize();
		}
	}
}
