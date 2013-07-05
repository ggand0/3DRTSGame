using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using System.Threading;

namespace _3DRTSGame
{
	public class Level4 : Level
	{
		private enum LevelState
		{
			Loading,
			Ready
		}
		private LevelState currentState;
		private bool loaded;

		public Object Target { get; private set; }
		public Object Ground { get; private set; }
		public Object Teapot { get; private set; }
		public ArmedSatellite Satellite { get; private set; }

		private PrelightingRenderer renderer;
		private GridRenderer grid;
		private EnergyRingEffect discoidEffect;
		private ExplosionEffect explosionTest, smallExplosion, bigExplosion, midExplosion;
		private Planet planet;
		private Star star;
		private Sun sunCircle;
		private Effect shadowEffect;
		public Sun sun { get; private set; }
		public List<Asteroid> Asteroids { get; private set; }
		public List<Planet> Planets { get; private set; }
		public List<DamageablePlanet> TargetPlanets { get; private set; }
		public List<Fighter> Fighters { get; private set; }

        private Random random;
        private List<ExplosionEffect> ex = new List<ExplosionEffect>();
		private int count;
        private Matrix RotationMatrix = Matrix.Identity;
		private EnemyManager enemyManager;
		private UIManager uiManager;
		private ProductionManager productionManager;

		/*private static readonly int ASTEROID_POOL_NUM = 30;
		public Queue<Asteroid> AsteroidPool { get; private set; }
		private static readonly int SMALL_EXPLOSION_EFFECT_NUM = 50;
		private static readonly int MID_EXPLOSION_EFFECT_NUM = 50;
		private static readonly int BIG_EXPLOSION_EFFECT_NUM = 10;
		private static readonly int MAX_SATELLITE_NUM = 10;*/
		private ExplosionEffect effectTmp;
		private AsteroidBelt asteroidBelt;


        /// <summary>
        /// Utility専用クラスにまとめるべき
        /// </summary>
		public static float NextDouble(Random r, double min, double max)
		{
			return (float)(min + r.NextDouble() * (max - min));
		}

		// ObjectPoolクラスへ移行
		/*private void LoadObjectPool()
		{
			AsteroidPool = new Queue<Asteroid>();
			for (int i = 0; i < ASTEROID_POOL_NUM; i++) {
				// どうせ使うときに値変えるので適当に設定
				AsteroidPool.Enqueue(new Asteroid(Vector3.Zero, 1f, "Models\\Asteroid"));
			}

			smallExplosion = new ExplosionEffect(content, graphicsDevice, "small", new Vector3(0, 50, 0), Vector2.One, false, "Xml\\Particle\\particleExplosion0.xml", false);
			for (int i = 0; i < SMALL_EXPLOSION_EFFECT_NUM; i++) {
				SmallExplosionPool.Enqueue((ExplosionEffect)smallExplosion.Clone());
			}
			midExplosion = new ExplosionEffect(content, graphicsDevice, "mid", new Vector3(0, 50, 0), Vector2.One, false, "Xml\\Particle\\particleExplosion2.xml", false);
			for (int i = 0; i < MID_EXPLOSION_EFFECT_NUM; i++) {
				MidExplosionPool.Enqueue((ExplosionEffect)midExplosion.Clone());
			}
			bigExplosion = new ExplosionEffect(content, graphicsDevice, "big", new Vector3(0, 50, 0), Vector2.One, false, "Xml\\Particle\\particleExplosion1.xml", false);
			for (int i = 0; i < BIG_EXPLOSION_EFFECT_NUM; i++) {
				BigExplosionPool.Enqueue((ExplosionEffect)bigExplosion.Clone());
			}
			//Satellite = new ArmedSatellite(new Vector3(300, 50, 300), star.Position, 5, "Models\\Dawn", "SoundEffects\\laser1");
			for (int i = 0; i < MAX_SATELLITE_NUM; i++) {//
				//SatellitePool.Enqueue((Satellite)Satellite.Clone());
				SatellitePool.Enqueue(new ArmedSatellite(new Vector3(300, 50, 300), star.Position, 5, "Models\\Dawn", "SoundEffects\\laser1"));
			}
		}*/
		private void AddAsteroids(int asteroidNum, float radius)
		{
			Asteroids = new List<Asteroid>();
			for (int i = 0; i < asteroidNum; i++) {
				//random = new Random();
				Asteroids.Add(new Asteroid(new Vector3(NextDouble(random, -radius, radius), 0, NextDouble(random, -radius, radius)),
					star.Position, 0.05f, 10, "Models\\Asteroid"));
				//Asteroids[i].Scale = 0.02f;//0.1f;
				//Asteroids[i].SetModelEffect(lightingEffect, true);					// set effect to each modelmeshpart
			}
		}
		protected override void Initialize()
		{
			insertLoadingScreen = true;

			base.Initialize();
			player = new Player(this);
			enemyManager = new EnemyManager(this);
			//productionManager = new ProductionManager(this); // Skyロード後に初期化する
			uiManager = new UIManager();
			

			// Entities
			Models = new List<Object>();
			/*Ground = new Object(new Vector3(0, -500, 0), 1f, "Models\\ground");//-200
			Ground.RenderBoudingSphere = false;
			Models.Add(Ground);*/
			Target = new Object(new Vector3(0, 20, 0), 20, "Models\\cube");

			

			// Initializes camera
			camera = new ArcBallCamera(Vector3.Zero);
			ParticleEmitter.camera = camera;

			// Set up the reference grid
			grid = new GridRenderer();
			grid.GridColor = Color.DarkSeaGreen * 0.3f;//Color.LimeGreen;
            grid.GridScale = 300f;//100f;
            grid.GridSize = 64;;//32;
			// Set the grid to draw on the x/z plane around the origin
			grid.WorldMatrix = Matrix.Identity;


			/**/Thread loadingThread0 = new Thread(this.Load);
			loadingThread0.Priority = ThreadPriority.Highest;
			loadingThread0.Start();
            Thread loadingThread1 = new Thread(ObjectPool.Load);
            loadingThread1.Priority = ThreadPriority.Highest;
            loadingThread1.Start();
		}
		public override void Load()
		{
			base.Load();

			// Set up the reference grid and sample camera
			grid.LoadGraphicsContent(graphicsDevice);

			Sky = new SkySphere(content, graphicsDevice, content.Load<TextureCube>("Textures\\SkyBox\\space4"), 100);// set 11 for debug
			productionManager = new ProductionManager(this);

			// Initialize Satellites
			// object poolにインスタンスは作成してあるが、levelのリストへエフェクトを追加するなどの処理が終ってないのでここで済ます。
			/*foreach (Satellite s in ObjectPool.SatellitePool) {
				if (s is ArmedSatellite) {
					(s as ArmedSatellite).Initialize();
				}
			}*/

			// Load stars
			star = new Star(new Vector3(-500, 100, 500), graphicsDevice, content, StarType.G);
			//LightPosition = star.Position;

			sun = new Sun(new Vector3(-2000, 500, 2000), graphicsDevice, content, spriteBatch);
			sunCircle = new Sun(LightPosition, graphicsDevice, content, spriteBatch);

			asteroidBelt = new AsteroidBelt(this, Vector3.Zero);

			// Set up object pool
			//LoadObjectPool();
			//ObjectPool.Load();

			// Load planets
			Planets = new List<Planet>();
			TargetPlanets = new List<DamageablePlanet>();
			//WaterPlanet waterPlanet = new WaterPlanet(new Vector3(-1000, 0, -1000), -LightPosition, device, content);
			WaterPlanet waterPlanet = new WaterPlanet(new Vector3(-100, 100, -100), LightPosition, graphicsDevice, content);
			IcePlanet icePlanet = new IcePlanet(new Vector3(-100, 100, -800), LightPosition, graphicsDevice, content);
			GasGiant gasGiant = new GasGiant(new Vector3(-100, 100, -2500), LightPosition, graphicsDevice, content);
			RockPlanet rockPlanet = new RockPlanet(graphicsDevice, content);
			MoltenPlanet moltenPlanet = new MoltenPlanet(graphicsDevice, content);

			//planet = moltenPlanet;
			//planet = gasGiant;
			//planet = icePlanet;
			planet = waterPlanet;
			//Planets.Add(waterPlanet);
			TargetPlanets.Add(waterPlanet);
			Planets.Add(icePlanet);
			//TargetPlanets.Add(icePlanet);
			Planets.Add(gasGiant);

			// Load asteroids
			random = new Random();
			Asteroids = new List<Asteroid>();
			//AddAsteroids(5, 2000);
			/*foreach (Asteroid o in Asteroids) {
				Models.Add(o);
			}*/
			foreach (Object o in Asteroids) {
				Enemies.Add(o);
			}


			// Load satellites
			/*//Satellite = new ArmedSatellite(new Vector3(300, 50, 300), star.Position, 5, "Models\\ISS", "SoundEffects\\laser1");
			Satellite = new ArmedSatellite(new Vector3(300, 50, 300), sun.Position, 5, "Models\\ISS", "SoundEffects\\laser0");
			Models.Add(Satellite);
			//Models.Add(new Satellite(false, waterPlanet.Position + new Vector3(400, 100, 600), waterPlanet.Position, 100f, "Models\\spacestation4"));
			Models.Add(new SpaceStation(false, waterPlanet.Position + new Vector3(400, 100, 600), waterPlanet.Position, 100f, "Models\\spacestation4"));
			//Models.Add(new ArmedSatellite(waterPlanet.Position + new Vector3(400, 50, 0), waterPlanet.Position, 0.01f, "Models\\TDRS", "SoundEffects\\License\\LAAT0"));
			Models.Add(new ArmedSatellite(SatelliteWeapon.Missile, waterPlanet.Position + new Vector3(400, 50, 0),
				waterPlanet.Position, 10f, "Models\\Dawn", "SoundEffects\\missile0"));// deepspace,10 / rosetta,10
			*/

			
			//Models.Add(new ArmedSatellite(waterPlanet.Position + new Vector3(400, 50, 0), waterPlanet.Position, 0.01f, "Models\\TDRS", "SoundEffects\\laser0"));

			// Load fighters
			//Models.Add(new Fighter(new Vector3(2000, 50, 1000), waterPlanet.Position, 20f, "Models\\fighter0"));
			//Enemies.Add(Models[Models.Count - 1]);
			Fighters = new List<Fighter>();
			Fighters.Add(new Fighter(new Vector3(2000, 50, 1000), waterPlanet.Position, 20f, "Models\\fighter0"));
			//Fighters.Add(new Fighter(new Vector3(100, 1000, 100), waterPlanet.Position, 20f, "Models\\fighter0"));
			//Fighters.Add(new Fighter(new Vector3(-100, -1000, -100), waterPlanet.Position, 20f, "Models\\fighter0"));// 同軸上だと正しく移動しないので注意
			//Fighters.Add(new Fighter(new Vector3(-2000, 50, -1000), waterPlanet.Position, 20f, "Models\\fighter0"));
			foreach (Fighter f in Fighters) {
				Enemies.Add(f);
			}
			foreach (Object o in Enemies) {
				Models.Add(o);
			}


			// Set up light effects !!
			// Objectの中で行うことに。
			/*shadowEffect = content.Load<Effect>("ProjectShadowDepthEffectV4");
			//Effect lightingEffect = content.Load<Effect>("PPModel");	// load Prelighting Effect
			foreach (Object o in Models) {
				//o.RenderBoudingSphere = false;
				if (!(o is Asteroid)) {
					o.SetModelEffect(shadowEffect, true);
					//o.SetModelEffect(Object.shadowEffect, true);
					//o.SetModelEffect(Object.shadowEffect, false);
				}
			}*/
			/*shadowEffect = content.Load<Effect>("ProjectShadowDepthEffectV4");
			foreach (Object o in Models) {
				//o.RenderBoudingSphere = false;
				if ((o is Planet)) {
					o.SetModelEffect(shadowEffect, true);
					//o.SetModelEffect(Object.shadowEffect, false);
				}
			}*/
			foreach (Planet p in Planets) {
				p.RenderBoudingSphere = false;
				//p.SetModelEffect(shadowEffect, true);
				Models.Add(p);
			}
			foreach (DamageablePlanet p in TargetPlanets) {
				p.RenderBoudingSphere = false;
				//p.SetModelEffect(shadowEffect, true);
				Models.Add(p);
			}


			renderer = new PrelightingRenderer(graphicsDevice, content, false);
			renderer.Models = Models;
			renderer.Camera = camera;
			renderer.Lights = new List<PointLight>() {
				//new PointLightCircle(new Vector3(0, 1000, 0), 2000, Color.White, 2000),	// 影のデバッグ用
				//new PointLight(new Vector3(0, 500, 0), Color.White, 2000),				// 太陽の光源
                new PointLight(sun.Position, Color.White, 5000),							// 太陽の光源
				new PointLight(new Vector3(0, 10000, 0), Color.White * .85f, 100000),		// シーン全体を照らす巨大なライトにする
			};
			renderer.ShadowLightPosition = new Vector3(300, 500, 300);//LightPosition;
			renderer.ShadowLightTarget = new Vector3(0, 0, 0);
			renderer.ShadowMult = 0.3f;//0.01f;//0.3f;
			LightPosition = renderer.Lights[0].Position;


			// Special effects
			EnergyRingEffect.game = game;
			//discoidEffect = new EnergyRingEffect(content, graphicsDevice, new Vector3(0, 0, 0), new Vector2(300));
			EnergyShieldEffect.game = game;
			/*//shieldEffect = new EnergyShieldEffect(content, graphicsDevice, Satellite.Position, new Vector2(300), 250);
			//explosionTest = new ExplosionEffect(content, graphicsDevice, new Vector3(0, 50, 0), Vector2.One, true, "Xml\\Particle\\particleExplosion0.xml", true);
			smallExplosion = new ExplosionEffect(content, graphicsDevice, new Vector3(0, 50, 0), Vector2.One, false, "Xml\\Particle\\particleExplosion0.xml", false);
			//smallExplosion = new ExplosionEffect(content, device, new Vector3(0, 50, 0), Vector2.One, false, "Xml\\Particle\\particleExplosion0.xml", true);
			bigExplosion = new ExplosionEffect(content, graphicsDevice, new Vector3(0, 50, 0), Vector2.One, false, "Xml\\Particle\\particleExplosion1.xml", false);
			midExplosion = new ExplosionEffect(content, graphicsDevice, new Vector3(0, 50, 0), Vector2.One, false, "Xml\\Particle\\particleExplosion2.xml", false);
			//lb = new LaserBillboard(graphicsDevice, content, content.Load<Texture2D>("Textures\\Laser2"), new Vector2(300, 50), new Vector3(0, 50, 0), new Vector3(100, 60, -100));*/

            while (true) {
                if (ObjectPool.LOADED) break;
            }
			loaded = true;
		}

		// 戻り値をNullable<Vector3>にしようと思ったけどできない
		private Vector3 GetRayPlaneIntersectionPoint(Ray ray, Plane plane)
		{
			float? distance = ray.Intersects(plane);
			//return distance.HasValue ? ray.Position + ray.Direction * distance.Value : null;
			return distance.HasValue ? ray.Position + ray.Direction * distance.Value : Vector3.Zero;
		}
		public override void Reset()
		{
			/*player = new Player(this);
			playerBullets = new List<Bullet>();
			enemies = new List<Enemy>();
			enemyBullets = new List<Bullet>();
			score = 0;
			count = 0;
			time = 0.0;
			BGM.Volume = 1.0f;
			BGM.Play();*/
			Initialize();
			Load();

			base.Reset();

		}
		protected override bool IsClear()
		{
			return enemyManager.WaveEnd;
		}
		protected override bool IsGameOver()
		{
			return !TargetPlanets.Any((x) => x.IsAlive);
		}
		protected override void Collide()
		{
			base.Collide();

			// AsteroidとTargetPlanets
			foreach (DamageablePlanet p in TargetPlanets) {
				foreach (Asteroid a in Asteroids) {
				
					if (a.IsActive && p.IsActive && a.IsHitWith(p)) {
						//a.Damage();
						a.Die();
						p.Damage();

						if (ObjectPool.MidExplosionPool.Count > 0) {
							ExplosionEffect effectTmp = ObjectPool.MidExplosionPool.Dequeue();
							effectTmp.Reset(a.Position);
							effectTmp.Run();
							effectManager.Add(effectTmp);
						} else {// 足りなかったら小さい爆発エフェクトで誤魔化す
							ExplosionEffect e = (ExplosionEffect)smallExplosion.Clone();
							e.Position = a.Position;
							e.Run();
							effectManager.Add(e);
						}
					}
				}
				foreach (Fighter f in Fighters) {
					//if (f.IsActive && p.IsActive && f.IsHitWith(p)) {
					if (f.IsActive && p.IsActive && p.IsHitWith(f)) {
						//a.Damage();
						f.Die();
						p.Damage();

						if (ObjectPool.SmallExplosionPool.Count > 0) {
							ExplosionEffect effectTmp = ObjectPool.SmallExplosionPool.Dequeue();
							effectTmp.Reset(f.Position);
							effectTmp.Run();
							effectManager.Add(effectTmp);
						}
					}
				}
			}

			// BulletとObject
			foreach (Bullet b in Bullets) {
				foreach (Object o in Enemies) {
					if (o is Asteroid && b.IsActive && o.IsActive && b.IsHitWith(o)) {
						if (!(b is LaserBillboardBullet && (b as LaserBillboardBullet).Mode == 1)) b.Die();
						(o as Asteroid).Damage();
						
						if (!o.IsActive) {
							player.AddMoney(o);

							if (ObjectPool.MidExplosionPool.Count > 0) {
								ExplosionEffect effectTmp = ObjectPool.MidExplosionPool.Dequeue();
								effectTmp.Reset(o.Position);
								effectTmp.Run();
								effectManager.Add(effectTmp);
							} else {
								ExplosionEffect e = (ExplosionEffect)smallExplosion.Clone();
								e.Position = o.Position;
								e.Run();
								effectManager.Add(e);
							}
						}
					}

					if (o is Fighter && o.IsActive && b.IsActive && b.Identification == IFF.Friend && b.IsHitWith(o)) {
						if (!(b is LaserBillboardBullet && (b as LaserBillboardBullet).Mode == 1)) b.Die();
						(o as Fighter).Damage();

						if (!o.IsActive) {
							player.AddMoney(o);

							if (ObjectPool.SmallExplosionPool.Count > 0) {
								ExplosionEffect effectTmp = ObjectPool.SmallExplosionPool.Dequeue();
								effectTmp.Reset(o.Position);
								effectTmp.Run();
								effectManager.Add(effectTmp);
							} else {
								ExplosionEffect e = (ExplosionEffect)smallExplosion.Clone();
								e.Position = o.Position;
								e.Run();
								effectManager.Add(e);
							}
						}
					}
				}

				// Collide with waterPlanet
				foreach (DamageablePlanet p in TargetPlanets) {
					if (b.Identification == IFF.Foe && b.IsHitWith(p)) {
						//b.IsActive = false;
						b.Die();
						p.Damage();

						if (!p.IsAlive) {
							ExplosionEffect e = (ExplosionEffect)bigExplosion.Clone();
							e.Position = b.Position;
							e.Run();
							effectManager.Add(e);
						}

						// 重い！数が多いのでもっと軽いエフェクトを作ろう
						/*ExplosionEffect e = (ExplosionEffect)smallExplosion.Clone();
						e.Position = b.Position;
						e.Run();
						effectManager.Add(e);*/
					}
				}
			}


			#region Remove dead objects
			if (Asteroids.Count > 0) {
				for (int j = 0; j < Asteroids.Count; j++) {
					if (!Asteroids[j].IsAlive) {
						Asteroids.RemoveAt(j);
					}
				}
			}
			if (Bullets.Count > 0) {
                for (int j = Bullets.Count - 1; j >= 0; j--) {
					if (!Bullets[j].IsActive) {
						Bullets.RemoveAt(j);
					}
				}
			}
			for (int j = Enemies.Count-1; j >= 0; j--) {
				if (!Enemies[j].IsAlive) {
					Enemies.RemoveAt(j);
				}
			}
			for (int j = Fighters.Count - 1; j >= 0; j--) {
				if (!Fighters[j].IsAlive) {
					Fighters.RemoveAt(j);
				}
			}
			for (int j = Satellites.Count - 1; j >= 0; j--) {
				if (!Satellites[j].IsAlive) {
					Satellites.RemoveAt(j);
				}
			}
			for (int j = Models.Count-1; j >=0; j--) {
				if (!Models[j].IsAlive) {
					if (Models[j] is Asteroid) {
						// 逆でも良いと思うけど、先に個別リストで消してから最後にModelsでプールに戻した後Removeしている。
						// オブジェクトプールに戻してから削除(戻す際には蘇生させてから)
						Models[j].IsActive = true;
						Models[j].IsAlive = true;
						Models[j].HitPoint = Models[j].MaxHitPoint;
						ObjectPool.AsteroidPool.Enqueue(Models[j] as Asteroid);
					}

					Models.RemoveAt(j);
				}
			}
			#endregion
		}
		public override void Update(GameTime gameTime)
		{
			float elapsed = (float)gameTime.TotalGameTime.TotalSeconds;
			count++;

			if (currentState == LevelState.Loading) {
				if (loaded) currentState = LevelState.Ready;
			} else {
				player.Update();
				enemyManager.Update(gameTime);
				productionManager.Update(gameTime);

				base.Update(gameTime);

				HandleInput();
				camera.Update(gameTime);
				renderer.Update(gameTime);
				LightPosition = renderer.Lights[0].Position;

				Sky.Update(gameTime);
				sun.Update(gameTime);
				sunCircle.Position = renderer.Lights[0].Position;
				sunCircle.Update(gameTime);

				asteroidBelt.Update(gameTime);
				foreach (Object o in Models) {
					if (o.IsAlive) o.Update(gameTime);
				}
				if (Models.Count > 100) {
					//throw new Exception("そろそろModelsのDelete処理を書くこと");
				}
				foreach (Bullet b in Bullets) {
					if (b.IsAlive) b.Update(gameTime);
				}

				if (planet.IsAlive) planet.Update(gameTime);

				Collide();

				effectManager.Update(gameTime);
				uiManager.Update(gameTime);
			}
		}

		private void DrawScene(GameTime gameTime)
		{

		}
		Texture2D loadingScreen;
		public override void Draw(GameTime gameTime)
		{
			if (currentState == LevelState.Loading) {
				Thread.Sleep(100);

				lock (graphicsDevice) {
					spriteBatch.Begin();
					//マスクを表示
					spriteBatch.Draw(loadingScreen, game.GraphicsDevice.Viewport.Bounds, Color.White);
					spriteBatch.DrawString(game.menuFont, count.ToString(), Vector2.Zero, Color.Green);
					spriteBatch.End();
				}
			} else {
				base.Draw(gameTime);

				// Initialize values (for debug)
				ResetGraphicDevice();
				float sunDepth = Vector3.Transform(sun.Position, camera.View).Z;
				//sunDepth =   Vector3.Transform(Vector3.Transform(Vector3.Transform(sun.Position, sun.World), camera.View), camera.Projection).Z;
				//float sunFrontDepth = Vector3.Transform(Vector3.Transform(sun.Position + (Vector3.Normalize(sun.Position - camera.CameraPosition) * 200), sun.world), camera.View).Z;
				camera.FarPlaneDistance = 10000000;// もっと短くてよい


				// Draw pre-passes
				/*graphicsDevice.SetRenderTarget(maskLayer);
				graphicsDevice.Clear(Color.White);
				foreach (Object o in Models) {
					//o.DrawMask(camera.View, camera.Projection, camera.Position, ref maskLayer, sunDepth);
					if (camera.BoundingVolumeIsInView(o.transformedBoundingSphere)) {
						o.DrawMask(camera.View, camera.Projection, camera.Position, ref maskLayer, sunDepth);
					}
				}
				graphicsDevice.SetRenderTarget(null);*/
				graphicsDevice.SetRenderTarget(maskLayer);
				graphicsDevice.Clear(Color.White);
				graphicsDevice.SetRenderTarget(null);
				renderer.PreDraw();
				sun.PreDraw(camera.View, camera.Projection);
				graphicsDevice.Clear(Color.White);


				// Draw environment
				ResetGraphicDevice();
				Sky.Draw(camera.View, camera.Projection, camera.Position);
				ResetGraphicDevice();
				//sun.Draw(true, camera.View, camera.Projection, maskLayer);
				ResetGraphicDevice();
				sunCircle.Draw(false, camera.View, camera.Projection);
				//planet.Draw(camera.View, Matrix.CreateScale(200) * Matrix.CreateTranslation(new Vector3(-300, 0, -200)), camera.Projection, camera.Position);
				//planet.Draw(new Vector3(-300, 0, -200), camera.View, camera.Projection, camera.Position);
				//if (planet.IsActive) planet.Draw(camera.View, camera.Projection, camera.Position);
				//star.Draw(camera.View, camera.Projection);


				// Draw objects
				asteroidBelt.Draw(gameTime);
				foreach (Object o in Models) {
					if (o.IsAlive && camera.BoundingVolumeIsInView(o.transformedBoundingSphere)) {
						if (o is ArmedSatellite) {
							(o as ArmedSatellite).Draw(gameTime, camera.View, camera.Projection, camera.Position);
						} else {
							o.Draw(camera.View, camera.Projection, camera.Position);
						}
					}
				}
				foreach (Bullet b in Bullets) {
					if (b.IsActive) b.Draw(camera);
				}


				// Draw debug overlays
				renderer.Draw(gameTime);
				if (displayGrid) {
					grid.ProjectionMatrix = camera.Projection;
					grid.ViewMatrix = camera.View;
					// draw the reference grid so it's easier to get our bearings
					grid.Draw();
				}
				DebugOverlay.Arrow(Vector3.Zero, Vector3.UnitX * 1000, 1, Color.Red);
				DebugOverlay.Arrow(Vector3.Zero, Vector3.UnitY * 1000, 1, Color.Green);
				DebugOverlay.Arrow(Vector3.Zero, Vector3.UnitZ * 1000, 1, Color.Blue);
				DebugOverlay.Singleton.Draw(camera.Projection, camera.View);

				// Draw effects
				effectManager.Draw(gameTime, camera);

				foreach (EnergyShieldEffect ese in transparentEffects) {
					ese.Draw(gameTime, camera.View, camera.Projection, camera.Position, camera.Direction, camera.Up, camera.Right);
				}

				// Draw UIs
				productionManager.Draw(gameTime);
				uiManager.Draw(gameTime);
				player.Draw();
			}
		}


		public Level4(Scene previousScene)
			: base(previousScene)
		{
			displayGrid = true;

			currentState = LevelState.Loading;
			loadingScreen = content.Load<Texture2D>("Textures\\UI\\loading");
		}
	}
}
