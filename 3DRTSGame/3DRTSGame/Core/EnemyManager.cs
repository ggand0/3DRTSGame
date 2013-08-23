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
		/// <summary>
		/// 何らかの属性を格納するのに使用。たとえばAsteroidの飛来軌道パターンなど。
		/// </summary>
		public string EnemySubType { get; set; }
		public float SpawnProbability { get; set; }
        public int SpawnNum { get; set; }

		public EnemyInfo(Type type, float prob)
			: this(type, "", prob)
		{
		}
		public EnemyInfo(Type type, string subType, float prob)
			: this(type, subType, prob, 1)
		{
		}
        public EnemyInfo(Type type, string subType, float prob, int num)
            : this()
        {
            EnemySubType = subType;
            EnemyType = type;
            SpawnProbability = prob;
            SpawnNum = num;
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
		private static readonly int INTERVAL_SEC = 10;
        /// <summary>
        /// 対数螺旋ルートにおける初期角度(大きい程指数的に遠い位置)
        /// </summary>
        public static readonly int INI_THETA = 1440;

		private Level4 level;
		private Random random = new Random();
		private List<EnemyWave> waves = new List<EnemyWave>();

		/// <summary>
		/// Spawnし終わったかどうか
		/// </summary>
		public bool WaveEnd { get; private set; }
		/// <summary>
		/// 現在のWave数（インデックス）
		/// </summary>
		public int WaveCount { get; private set; }

		private WaveState state;
		private int count, start;

        private Dictionary<string, Object> enemiesOrg = new Dictionary<string, Object>();


        #region Methods
		#region old methods
		/*public static float NextDouble(Random r, double min, double max)
		{
			return (float)(min + r.Utility.NextDouble() * (max - min));
		}*/
		private void AddAsteroids(int asteroidNum, float radius)
		{
			for (int i = 0; i < asteroidNum; i++) {
				//random = new Random();
				level.Asteroids.Add(new Asteroid(new Vector3(Utility.NextDouble(random, -radius, radius), 0, Utility.NextDouble(random, -radius, radius))
					, level.sun.Position, 0.05f, 10, "Models\\Asteroid"));
				//Asteroids[i].Scale = 0.02f;//0.1f;
				//Asteroids[i].SetModelEffect(lightingEffect, true);					// set effect to each modelmeshpart
			}
		}
		private void SpawnAsteroids()
		{
			if (level.Enemies.Count < ASTEROID_MAX_SPAWN_NUM) {// = 15 -5
				float radius = 3000;
				Asteroid a = new Asteroid(new Vector3(Utility.NextDouble(random, -radius, radius)
					, 0, Utility.NextDouble(random, -radius, radius)), level.sun.Position, 0.05f, 5, "Models\\Asteroid");
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
				Fighter f = new Fighter(new Vector3(Utility.NextDouble(random, -radius, radius), Utility.NextDouble(random, -radius, radius), Utility.NextDouble(random, -radius, radius))
					, level.Planets[0].Position, 20f, "Models\\fighter0");
				f.IsActive = true;
				f.RenderBoudingSphere = false;
				level.Enemies.Add(f);
				level.Models.Add(f);
			}
		}
		#endregion

		Asteroid a; Fighter f;
		/// <summary>
		/// AsteroidのSpawn時の対数螺旋軌道を決定する
		/// </summary>
		/// <param name="pattern"></param>
		/// <returns></returns>
		private Vector2 CalcLogarithmicSpiral(int pattern)
		{
			switch (pattern) {
				default:
					return Utility.CalcLogarithmicSpiral(0.15f, 0.5f, MathHelper.ToRadians(INI_THETA));
				case 1:
                    return Utility.CalcLogarithmicSpiral(0.15f, 0.5f, MathHelper.ToRadians(INI_THETA));
				case 2:
                    return Utility.CalcLogarithmicSpiral(0.15f, 0.5f, MathHelper.ToRadians(INI_THETA));
			}
		}
        /// <summary>
        /// EnemyInfoからspawnさせる敵の型を決定し、色々と初期化してLevelのリストに追加する。
        /// </summary>
        /// <param name="enemyData">敵の型などの情報</param>
        /// <param name="index">何体目の敵か</param>
		private void SpawnEnemies(EnemyInfo enemyData, int index)
		{
			// 乱数が入らなかったらDictionaryで予めargumentsを計算してスマートにインスタンス生成が出来るのだが、
			// 乱数入りなので仕方なく関数内で条件分岐させることに。さらにswitchも使用できない
			float radius = 3000;

            if (enemyData.EnemyType == typeof(Asteroid)) {
                /*enemiesOrg["Asteroid"].Position = new Vector3(Utility.NextDouble(random, -radius, radius)
                    , 0, Utility.NextDouble(random, -radius, radius));*/

				/*float aa = 0.15f, b = 0.5f, angle = MathHelper.ToRadians(1440);
				enemiesOrg["Asteroid"].Position = level.TargetPlanets[0].Position +
					new Vector3(aa * (float)Math.Exp(b * angle) * (float)Math.Cos(angle), 0,
						aa * (float)Math.Exp(b * angle) * (float)Math.Sin(angle));*/

				Vector2 spiralPos = Vector2.Zero;
                // 対数螺旋ルートで移動させる場合ここに乱数でルート選択するコードを挿入する必要がある
                // 現在はEnemySubTypeで場合分けしている
				/*switch (enemyData.EnemySubType) {
					default:
                        spiralPos = Utility.CalcLogarithmicSpiral(0.15f, 0.5f, MathHelper.ToRadians(INI_THETA));
						break;
					case "1":
                        spiralPos = Utility.CalcLogarithmicSpiral(0.15f, 0.5f, MathHelper.ToRadians(INI_THETA), Matrix.CreateRotationY(MathHelper.ToRadians(90)));
						break;
				}*/
                float unitRadian = MathHelper.ToRadians(360 / (float)enemyData.SpawnNum);
                spiralPos = Utility.CalcLogarithmicSpiral(0.15f, 0.5f, MathHelper.ToRadians(INI_THETA), Matrix.CreateRotationY(unitRadian * index));

				enemiesOrg["Asteroid"].Position = level.TargetPlanets[0].Position + new Vector3(spiralPos.X, 0, spiralPos.Y);
                //enemiesOrg["Asteroid"].RenderBoudingSphere = false;
                //a = (Asteroid)enemiesOrg["Asteroid"].Clone();
				if (ObjectPool.AsteroidPool.Count > 0) {
					// いくつかのプロパティに変更を加えたいので、一時的にメンバ変数に格納する
					a = ObjectPool.AsteroidPool.Dequeue();

					/*a.Position = level.TargetPlanets[0].Position +
						new Vector3(aa * (float)Math.Exp(b * angle) * (float)Math.Cos(angle), 0,
							aa * (float)Math.Exp(b * angle) * (float)Math.Sin(angle));*/
					a.Position = new Vector3(spiralPos.X, 0, spiralPos.Y);
					a.Destination = level.TargetPlanets[0].Position;
					a.Scale = 0.05f;

					// 何故かtrueになってくれてないのでここでも蘇生作業してみる
					a.IsActive = true;
					a.IsAlive = true;
					// angleのせいかも
					a.Initialize(1, enemyData.EnemySubType);
				} else {
					a = (Asteroid)enemiesOrg["Asteroid"].Clone();
				}

                level.Models.Add(a);
                level.Enemies.Add(a);
				level.Asteroids.Add(a);
			} else if (enemyData.EnemyType == typeof(Fighter)) {
                enemiesOrg["Fighter"].Position = new Vector3(Utility.NextDouble(random, -radius, radius),
                    Utility.NextDouble(random, -radius, radius), Utility.NextDouble(random, -radius, radius));
                //enemiesOrg["Fighter"].RenderBoudingSphere = false;

                Fighter f = (Fighter)enemiesOrg["Fighter"].Clone();
                f.Target = level.TargetPlanets[0].Position;

                level.Models.Add(f);
                level.Enemies.Add(f);
				level.Fighters.Add(f);
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

				if (time > waves[WaveCount].MaxTimeSec * 60) {
					state = WaveState.Interval;
					start = count;
				} else if (time % (waves[WaveCount].SpawnRate * 60) == 0) {// 60FPS
					for (int n = 0; n < waves[WaveCount].SpawnNum; n++) {
						double prob = random.NextDouble();
						double probs = prob;
						double sum = 0;

						foreach (EnemyInfo i in waves[WaveCount].EnemyData) {
							/*if (prob < waves[WaveCount].EnemyData[0].SpawnProbability) {// 要修正
								SpawnEnemies(i.EnemyType);
								break;
							}*/
							if (prob + sum < i.SpawnProbability) {
								//SpawnEnemies(i.EnemyType);
								SpawnEnemies(i);
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
				if (count - start > 10 * INTERVAL_SEC && level.Enemies.Count == 0) {// 敵全滅かつ10秒経過で次のwaveへ
					WaveCount++;

					if (WaveCount == waves.Count) {// wavesのカウントに達していたらクリア
						WaveEnd = true;
					} else {// 残っているなら次のwaveへ
						state = WaveState.Start;
					}
				}
			}
		}


		private SpiralRenderer spiralRenderer, spiralRenderer1, spiralRenderer2;
		/// <summary>
		/// Asteoidsの飛来軌道の描画を行う
		/// </summary>
		/// <param name="gameTime"></param>
		public void Draw(GameTime gameTime, Camera camera)
		{
			if (spiralRenderer == null) {
                spiralRenderer = new SpiralRenderer(Level.graphicsDevice, new Color(Color.Orange.ToVector4() + new Color(0.2f, 0, 0, 0).ToVector4()) * 0.5f, 0, INI_THETA, level.TargetPlanets[0].Position);
				//spiralRenderer1 = new SpiralRenderer(Level.graphicsDevice, Color.MediumPurple * 0.5f, 0, 1440, level.TargetPlanets[0].Position, Matrix.CreateRotationZ(MathHelper.ToRadians(90)));
                spiralRenderer1 = new SpiralRenderer(Level.graphicsDevice, Color.MediumPurple * 0.5f, 0, INI_THETA, level.TargetPlanets[0].Position, Matrix.CreateRotationZ(MathHelper.ToRadians(90)));
			}
			spiralRenderer.Draw(Level.graphicsDevice, camera.View, camera.Projection, Matrix.Identity, Color.Orange);
			spiralRenderer1.Draw(Level.graphicsDevice, camera.View, camera.Projection, Matrix.Identity, Color.Orange);
		}
        #endregion

        private void Initialize()
		{
			//waves.Add(new EnemyWave(new EnemyInfo[] { new EnemyInfo(typeof(Asteroid), 0.5f), new EnemyInfo(typeof(Fighter), 0.5f) }, 0, 2, 3, 3000, 30));
			//waves.Add(new EnemyWave(new EnemyInfo[] { new EnemyInfo(typeof(Asteroid), 1f) }, 0, 1, 4, 3000, 60));
			waves.Add(new EnemyWave(new EnemyInfo[] { new EnemyInfo(typeof(Asteroid), "0", 1/3f), new EnemyInfo(typeof(Asteroid), "1", 1/3f)
                , new EnemyInfo(typeof(Asteroid), "2", 1/3f) }, 0, 1, 4, 3000, 60));
			waves.Add(new EnemyWave(new EnemyInfo[] { new EnemyInfo(typeof(Fighter), 1f) }, 0, 10, 1, 3000, 30));
			waves.Add(new EnemyWave(new EnemyInfo[] { new EnemyInfo(typeof(Asteroid), 0.5f), new EnemyInfo(typeof(Fighter), 0.5f) }, 0, 5, 4, 3000, 30));
			state = WaveState.Start;

            // asteroid:4引数コンストラクタ以外を使うとeffectが初期化されずにnullのままになるので注意
			enemiesOrg.Add("Asteroid", new Asteroid(Vector3.Zero, Vector3.Zero, 0.05f, 10, "Models\\Asteroid"));
            enemiesOrg.Add("Fighter", new Fighter(Vector3.Zero, Vector3.Zero, 20f, "Models\\fighter0"));
		}
		public EnemyManager(Level4 level)
		{
			this.level = level;
			Initialize();

			//spiralRenderer = new SpiralRenderer(Level.graphicsDevice, Color.Orange, 0, 1440, level.TargetPlanets[0].Position);
		}
	}
}
