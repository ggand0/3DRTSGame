using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;

namespace _3DRTSGame
{
	public enum SatelliteWeapon
	{
		None,
		LaserBolt,
		Missile,
		LaserBeam
	}
	public class ArmedSatellite : Satellite
	{
		#region Fields&Properties
		private static Effect satelliteShadowEffect;
		private static readonly float MISSILE_SPEED = 30.0f;
		private static readonly float LASER_BOLT_SPEED = 60.0f;

		private SoundEffect shootSound;
		private List<SoundEffectInstance> currentSounds = new List<SoundEffectInstance>();
		private EnergyShieldEffect shieldEffect;

        private List<Object> visibleEnemies;
		public BoundingSphere SensorSphere { get; private set; }
		private Random r = new Random();
		private int count;
		private readonly int shootRate = 60;
		private int chargeTime;
		private bool canShoot;
		private BillboardStrip billboardStrip;
		private List<Vector3> positions;
		private int stripCount;

		public SatelliteWeapon Weapon { get; protected set; }
		public bool ShieldEnabled { get; protected set; }
		Object tmp1;
		Missile missileOrg;
		#endregion

		private Vector3 SearchTarget(int tactics)
		{
			float minDis = 9999;
			Vector3 min = new Vector3(9999);

			/*foreach (Asteroid a in (level as Level4).Asteroids) {
				if ((Position - a.Position).Length() < minDis) {
					minDis = (Position - a.Position).Length();
					min = a.Position;
				}
			}*/
			switch (tactics) {
				default:
					return visibleEnemies[r.Next(0, visibleEnemies.Count)].Position;
				case 1:
					foreach (Object o in visibleEnemies) {
						if (o.IsActive && (Position - o.Position).Length() < minDis) {
							minDis = (Position - o.Position).Length();
							min = o.Position;
						}
					}
					return min;
			}
		}
		private Object SearchTargetObj(int tactics)
		{
			float minDis = 9999;
			Vector3 min = new Vector3(9999);
			Object minObj = null;

			/*foreach (Asteroid a in (level as Level4).Asteroids) {
				if ((Position - a.Position).Length() < minDis) {
					minDis = (Position - a.Position).Length();
					min = a.Position;
				}
			}*/
			switch (tactics) {
				default:
					return visibleEnemies[r.Next(0, visibleEnemies.Count)];
				case 1:
					foreach (Object o in visibleEnemies) {
						if (o.IsActive && (Position - o.Position).Length() < minDis) {
							minDis = (Position - o.Position).Length();
							min = o.Position;
							minObj = o;
						}
					}
					return minObj;
			}
		}
		private void Shoot(int bulletType)
		{
			switch (bulletType) {
				case 0:
					//level.Bullets.Add(new EntityBullet(this, 1, new Vector3(1, 0, 0), this.Position, 20, "Models\\cube"));
					break;
				case 1:
					//level.Bullets.Add(new BillboardBullet(Level.graphicsDevice, content, Position, new Vector3(1, 0, 0), 1, content.Load<Texture2D>("Textures\\Mercury\\Star"), new Vector2(10) ));
					break;
				case 2:
					level.Bullets.Add(new LaserBillboardBullet(IFF.Friend, Level.graphicsDevice, content, Position,
						new Vector3(1, 0.5f, 0.3f), 1, content.Load<Texture2D>("Textures\\Lines\\laser0"), new Vector2(30, 20)));//Textures\\Mercury\\Laser
					break;
				case 3:
					Drawable tmp0 = SearchTargetObj(1);
					if (tmp0 != null) {
						//Vector3 dir = visibleEnemies[r.Next(0, visibleEnemies.Count)].Position;
						Vector3 dir = Vector3.Normalize(tmp0.Position - Position);
						/*level.Bullets.Add(new LaserBillboardBullet(IFF.Foe, Level.graphicsDevice, content, Position
						, Position + Direction * 200, Direction, 50, content.Load<Texture2D>("Textures\\Mercury\\Laser"), Color.Red, BlendState.AlphaBlend, new Vector2(200f, 100), 0));*/
						level.Bullets.Add(new LaserBillboardBullet(IFF.Friend, Level.graphicsDevice, content, Position, Position + dir * 80, dir, LASER_BOLT_SPEED,
							content.Load<Texture2D>("Textures\\Lines\\laser0"), Color.White, BlendState.Additive, new Vector2(10, 20), 0));//"Textures\\Mercury\\Laser" "Textures\\Lines\\laser0"
					}

					break;
				case 4:
					//if (visibleEnemies.Count > 0) {
						//Vector3 tmp = SearchTarget(0);
					//Vector3 tmp = SearchTarget(1);
					Drawable tmp = SearchTargetObj(1);
					//Vector3 dir1 = Vector3.Normalize(tmp - Position);
					if (tmp != null) {// 敵がいたなら
						Vector3 dir1 = Vector3.Normalize(tmp.Position - Position);
						level.Bullets.Add(new LaserBillboardBullet(IFF.Friend, Level.graphicsDevice, content, this, tmp, dir1, 1,
							content.Load<Texture2D>("Textures\\Lines\\laser0"), Color.White, BlendState.AlphaBlend, new Vector2(50, 100), 1));
						/*level.Bullets.Add(new LaserBillboardBullet(IFF.Friend, Level.graphicsDevice, content, Position, tmp, dir1, 1,
								content.Load<Texture2D>("Textures\\Lines\\laser0"), Color.White, BlendState.AlphaBlend, new Vector2(50, 100), 1));*/
					}


					// EnemiesをUpdateしてからArmedSatelliteをUpdateすれば当たる？


					break;
				case 5:
					//if (visibleEnemies.Count > 0) {
						//Vector3 tmp = SearchTarget(0);
                    //Object tmp1 = SearchTargetObj(0);
                    tmp1 = SearchTargetObj(0);
						Vector3 dir2 = Vector3.Normalize(tmp1.Position - Position);
                        // このnewする処理がマジで重い！
						//level.Bullets.Add(new Missile(IFF.Friend, this, tmp1, 5.0f, dir2, Position, 4, "Models\\AGM65Missile"));/**/
                        missileOrg.Initialize(tmp1, Position, dir2);
                        level.Bullets.Add((Missile)missileOrg.Clone());
					
					break;
			}
		}
		private void UpdateLocus()
		{
			positions.Add(Position);
            billboardStrip.Positions = positions;
			if (positions.Count >= BillboardStrip.MAX_SIZE) {//120
				positions.RemoveAt(0);
				billboardStrip.Positions = positions;
            } else if (positions.Count > 0) {//positions.Count >= 2
				//billboardStrip.AddBillboard(Level.graphicsDevice, content, content.Load<Texture2D>("Textures\\Mercury\\Laser"), new Vector2(10, 40), positions[positions.Count - 2], positions[positions.Count - 1]);
				//billboardStrip.AddBillboard(Level.graphicsDevice, content, content.Load<Texture2D>("Textures\\Laser2"), new Vector2(10, 40), positions[positions.Count - 2], positions[positions.Count - 1]);
				billboardStrip.AddVertices();
			}
		}
		private void CheckEnemies()
		{
			SensorSphere = new BoundingSphere(Position, SensorSphere.Radius);

			visibleEnemies.Clear();
			foreach (Object o in level.Enemies) {
				if (o.IsHitWith(SensorSphere)) {
					visibleEnemies.Add(o);
				}
			}
		}
		private bool IsInRange()
		{
			return visibleEnemies.Count > 0;
		}
		private void SetEffectParameters()
		{
			SetEffectParameter(satelliteShadowEffect, "DoRimLighting", true);
			//SetEffectParameter(satelliteShadowEffect, "RimColor", (Color.DodgerBlue * 0.8f).ToVector4());
            SetEffectParameter(satelliteShadowEffect, "RimColor", (Color.Green * 0.8f).ToVector4());
            SetEffectParameter(satelliteShadowEffect, "RimIntensity", 0.3f);// 0.01f
			//SetEffectParameter(satelliteShadowEffect, "CameraDirection", level.camera.Direction);/**/
			SetEffectParameter(satelliteShadowEffect, "CameraPosition", level.camera.Position);
		}

		public override void Update(GameTime gameTime)
		{
			count++;
			base.Update(gameTime);//RotationMatrix = Matrix.CreateRotationX((float)Math.PI);

			CheckEnemies();

			if (!canShoot && count > chargeTime) {
				canShoot = true;
			}
			if (canShoot && IsInRange()) {
				if (Weapon == SatelliteWeapon.Missile) {
					Shoot(5);
				} else if (Weapon == SatelliteWeapon.LaserBolt) {
					Shoot(3);
				} else if (Weapon == SatelliteWeapon.LaserBeam) {
					Shoot(4);
				}

				if (!Game1.mute) {
				    SoundEffectInstance ls = shootSound.CreateInstance();
				    ls.Volume = 0.05f;
				    ls.Play();
				    currentSounds.Add(ls);
				}

				canShoot = false;
				count = 0;
			}

			for (int i = currentSounds.Count - 1; i >= 0; i--) {
				if (currentSounds[i].State != SoundState.Playing) {
					currentSounds[i].Dispose();
					currentSounds.RemoveAt(i);
				}
			}

			if (ShieldEnabled) {
				shieldEffect.Position = Position;
				shieldEffect.Update(gameTime);
			}

			stripCount++;
			//if (stripCount % 15 == 0)
			UpdateLocus();
			billboardStrip.Update(gameTime);
		}
		public void Draw(GameTime gameTime, Matrix View, Matrix Projection, Vector3 CameraPosition)
		{
			SetEffectParameters();
			base.Draw(View, Projection, CameraPosition);

			billboardStrip.Draw(View, Projection, level.camera.Up, level.camera.Right, CameraPosition);

			// levelのリストでまとめて描画させることにした
			//shieldEffect.Draw(gameTime, View, Projection, CameraPosition, level.camera.Direction, level.camera.Up, level.camera.Right);
			if (IsSelected) {
				//uiRing.SetPosition(new Vector3(Position.X, -200, Position.Z));
				uiRing.SetPosition(Position);
				uiRing.Draw(View, Projection, Vector3.UnitX, Vector3.UnitZ);
			}
		}

		/// <summary>
		/// object poolingの関係上コンストラクタに書けない処理をここに。
		/// ...の予定だったが、spawnする時の武器チェンジ等に使うことにした。
		/// </summary>
		public void Initialize(SatelliteWeapon weaponType)
		{
			//shieldEffect = new EnergyShieldEffect(content, game.GraphicsDevice, Position, new Vector2(150), 100);//300,250
			//level.transparentEffects.Add(shieldEffect);
			Weapon = weaponType;
		}
		public void Initialize(Model model, Vector3 position, Vector3 center)
		{
			base.Initialize(position, center);
			Model = model;

            this.Scale = 8;// 500;
			//RotationMatrix = Matrix.CreateRotationX(MathHelper.ToRadians(60));
            SetEffectParameters();
			//SetModelEffect(satelliteShadowEffect, false);
            SetModelEffect(satelliteShadowEffect, true);
		}
		#region Constructors
		public ArmedSatellite(Vector3 position, float scale, string fileName)
			: this(position, Vector3.Zero, scale, fileName)
		{
		}
		public ArmedSatellite(Vector3 position, Vector3 center, float scale, string fileName)
			: this(position, center, scale, fileName, "SoundEffects\\laser0")
		{
		}
		public ArmedSatellite(Vector3 position, Vector3 center, float scale, string fileName, string SEPath)
			: this(SatelliteWeapon.LaserBolt, position, center, scale, fileName, SEPath)
		{
		}
		public ArmedSatellite(SatelliteWeapon weaponType, Vector3 position, Vector3 center, float scale, string fileName, string SEPath)
			: base(true, position, center, scale, fileName)
		{
			ShieldEnabled = true;
			//Rotate = true;
			this.Weapon = weaponType;

			if (weaponType == SatelliteWeapon.LaserBolt) {
				chargeTime = random.Next(8, 15);
			} else if (weaponType == SatelliteWeapon.LaserBeam) {
				chargeTime = random.Next(10, 70);
			} else if (weaponType == SatelliteWeapon.Missile) {
				chargeTime = random.Next(60, 120);;
			}
			//chargeTime = weaponType == SatelliteWeapon.LaserBolt ? random.Next(10, 70) : random.Next(60, 120);
			shootSound = content.Load<SoundEffect>(SEPath);
			shieldEffect = new EnergyShieldEffect(content, game.GraphicsDevice, Position, new Vector2(150), 100);//300,250
			level.transparentEffects.Add(shieldEffect);

			positions = new List<Vector3>();
			billboardStrip = new BillboardStrip(Level.graphicsDevice, content, content.Load<Texture2D>("Textures\\Lines\\Line1T1"), new Vector2(10, 200), positions);//Line1T1

			visibleEnemies = new List<Object>();
			SensorSphere = new BoundingSphere(Position,500);//1000 150
			RenderBoudingSphere = false;


            // 仮に作成しておく
			missileOrg = new Missile(IFF.Friend, this, null, MISSILE_SPEED, Vector3.Zero, Position, 4, "Models\\AGM65Missile");//AGM65Missile AIM9LMissile
			uiRing = new BillboardSystem(Level.graphicsDevice, Level.content, Level.content.Load<Texture2D>("Textures\\UI\\GlowRing2"),
						new Vector2(512), new Vector3[] { Vector3.Zero });//currentUIModel.Position
			//SetModelEffect(satelliteShadowEffect, false);// 実行中にパラメータ値を変更したいので、参照をセットする
            //SetModelEffect(satelliteShadowEffect, true);
		}

		static ArmedSatellite()
		{
			satelliteShadowEffect = content.Load<Effect>("ProjectShadowDepthEffectV4");
            SetEffectParameter(shadowEffect, "DoRimLighting", false);
		}
		#endregion
		
	}
}
