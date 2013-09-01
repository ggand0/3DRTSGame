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
    public class MilitaryShip : Object
    {
        protected static readonly int shootRate = 10;//120

		public FighterState State { get; protected set; }
		public Vector3 Target { get; set; }
		public Vector3 StartPosition { get; private set; }

		protected Vector3 currentWayPoint;
		protected int currentWayPointIndex;
		private Vector3[] wayPoints0, yawDebug, pitchDebug, rollDebug;
		

		private List<BoundingSphere> obstacles;
		private BoundingSphere viewSphere;
		private BillboardStrip engineTrailEffect;
		private List<Vector3> positions;

		private SoundEffect shootSound;
		private List<SoundEffectInstance> currentSounds = new List<SoundEffectInstance>();
        protected int count, stripCount;
        

        protected Vector3 upPosition;


		#region private mehods
		
		private void UpdateLocus()
		{
			if (IsActive) {
				positions.Add(Position);
				engineTrailEffect.Positions = positions;
				if (positions.Count >= BillboardStrip.MAX_SIZE) {
					positions.RemoveAt(0);
				} else if (positions.Count > 0) {
					engineTrailEffect.AddVertices();
				}
			} else {// 死亡時なので減らす
				positions.RemoveAt(0);
				engineTrailEffect.Positions = positions;
				if (positions.Count > 0) {
					engineTrailEffect.RemoveVertices();
				}
			}
		}
		/// <summary>
		/// WayPoint loop movement
		/// </summary>
		/// <param name="wayPoints"></param>
		private void DetectCurrentWaypoint(Vector3[] wayPoints)
		{
			float margin = 1.0f;
			if ((currentWayPoint - Position).Length() < Velocity.Length() + margin) {
				currentWayPointIndex++;
				if (currentWayPointIndex >= wayPoints.Length) currentWayPointIndex = 0;

				if (currentWayPointIndex == 1 || currentWayPointIndex == 3) Shoot(2);

				currentWayPoint = wayPoints[currentWayPointIndex];
			}
		}
		protected void Shoot(int bulletType)
		{
			switch (bulletType) {
				case 0:
					/*level.Bullets.Add(new EntityBullet(this, 1, new Vector3(1, 0, 0)
						, this.Position, 20, "Models\\cube"));*/
					break;
				case 1:
					/*level.Bullets.Add(new BillboardBullet(Level.graphicsDevice, content, Position
						, new Vector3(1, 0, 0), 1, content.Load<Texture2D>("Textures\\Mercury\\Star"), new Vector2(10)));*/
					break;
				case 2:
					level.Bullets.Add(new LaserBillboardBullet(IFF.Foe, Level.graphicsDevice, content, Position
						, Position + Direction * 200, Direction, 50, content.Load<Texture2D>("Textures\\Mercury\\Laser"), Color.Red, BlendState.AlphaBlend, new Vector2(200f, 100), 0));
					break;
			}

            // 発射音の再生
            if (!Game1.mute) {
                SoundEffectInstance ls = shootSound.CreateInstance();
                ls.Volume = 0.025f;
                ls.Play();
                currentSounds.Add(ls);
            }
		}
		
        /// <summary>
        /// 直線状に往復しつつ攻撃する戦術
        /// </summary>
		protected void BasicAttackMove()
		{
            if ((Target - Position).Length() < 1000) {
                State = FighterState.Attack;
            }

			if (State == FighterState.Move) {
				float speed = 5;
				Velocity = Vector3.Normalize(Target - Position) * speed;
				Direction = Vector3.Normalize(Velocity);
			} else if (State == FighterState.Attack) {
				Velocity = Vector3.Zero;
				count++;
				if (count % shootRate == 0) {
					Shoot(2);
				}
			}
		}
        /// <summary>
        /// Waypoint(予め作成しておく)に沿った移動を行う
        /// </summary>
		protected void WayPointMove()
		{
			DetectCurrentWaypoint(wayPoints0);

			// 速度の決定
			float speed = 1;//5
			//Velocity = Vector3.Normalize(currentWayPoint - Position) * speed;
			Vector3 dest = Vector3.Normalize(currentWayPoint - Position);
			/*float maxAnglePerFrame = (float)Math.PI / 2.0f / 20f;
			float angle = Vector3.Dot(Velocity, dest);
			Velocity += Vector3.Lerp(Velocity, dest, 0.2f);*/
			Velocity += dest * speed;
			Velocity *= 0.9f;

			Direction = Vector3.Normalize(Velocity);
		}
		
		
		private void CheckObstacles()
		{
			obstacles.Clear();
			viewSphere.Center = Position;
			foreach (Object o in level.Models) {
				if (o.Position != Position// とりあえず位置だけで自分じゃないかを判断する
					&& viewSphere.Intersects(o.transformedBoundingSphere)
					&& o.transformedBoundingSphere.Radius < 1000) {// Goundがリストに入るとまずいのでこれで除外
					obstacles.Add(o.transformedBoundingSphere);
				}
			}
		}
		private bool IsGoingToCollide(BoundingSphere target)
		{
			Vector3 v = Direction * 300;
			Vector3 dir = target.Center - Position;
			Vector3 dirProjected = Vector3.Dot(v, dir) * v / (v.Length() * v.Length());
			Vector3 b = dirProjected - dir;

			float l1 = v.Length();
			float l2 = dirProjected.Length();
			return target.Radius > b.Length() && l1 > l2;
		}
		private bool IsLeft(Vector3 vector, Vector3 targetPoint)
		{
			return Math.Sin(vector.X * (targetPoint.Y - 0) - vector.Y * (targetPoint.X - 0)) > 0;
		}
		private float CalcAvoidSpeed(float distance, float radius)
		{
			//float A = 0, B = 13000, n = 1, m = 2.5f, d = distance / 25f;
			float A = 0, B = 10 * radius, n = 1, m = 3.0f, d = distance / 25f;

			return -A / (float)Math.Pow(d, n) + B / (float)Math.Pow(d, m);
		}
		private void AddAvoidanceVelocity(Vector3 projectedDirection, Vector3 projectedDirection2, BoundingSphere bs)
		{
			// 距離の近さに応じて速さを調整するべきだろう
			float distance = (bs.Center - Position).Length();
			//float avoidSpeed = 1f;
			//float avoidSpeed = 2.5f / (distance / 100f);
			//float avoidSpeed = 2.5f / (distance / 25f);
			//float avoidSpeed = 1000000 / (distance * distance);

			// 回避速度にはポテンシャル関数を使うのはどうか？
			float avoidSpeed = CalcAvoidSpeed(distance, bs.Radius);

			Vector3 avoidVelocity = Vector3.Zero;

			// DirectionとUpからなる平面上での、Directionにおける障害物中心点の左右判定（左右どちらに避けるか）
			Vector3 planeToCenter = bs.Center - Position;
			float distancePlaneToCenter = Vector3.Dot(planeToCenter, Up);
			Vector3 projectedCenter = bs.Center - distancePlaneToCenter * Up;
			bool isLeft = IsLeft(projectedDirection, projectedCenter);

			avoidVelocity += isLeft ? _world.Right : _world.Left;// RightもLeftも0のケースが...


			// DirectionとRightからなる平面上での左右判定（上下どちらに避けるか）
			//Vector3 planeToCenter2 = bs.Center - Position;
			float distancePlaneToCenter2 = Vector3.Dot(planeToCenter, Right);
			Vector3 projectedCenter2 = bs.Center - distancePlaneToCenter * Right;
			bool isLeft2 = IsLeft(projectedDirection2, projectedCenter2);

			avoidVelocity += isLeft2 ? _world.Up : _world.Down;
			

			Velocity += avoidVelocity * avoidSpeed;
			//Velocity += Vector3.UnitX * avoidSpeed;
		}
		private void AvoidCollision()
		{
			Vector3 projectedDirection = Direction - Vector3.Dot(Direction, Up) * Up;
			Vector3 projectedDirection2 = Direction - Vector3.Dot(Direction, Right) * Right;

			foreach (BoundingSphere bs in obstacles) {
				if (IsGoingToCollide(bs)) {
					AddAvoidanceVelocity(projectedDirection, projectedDirection2, bs);
				}
			}
		}
		#endregion

		protected override void UpdateWorldMatrix()
		{
			Direction.Normalize();
			//Up = Vector3.UnitY; // Upが誤っているので表示されない
			Up = Vector3.Normalize(upPosition - Position);
			Up.Normalize();
			Right = Vector3.Cross(Direction, Up);
			Right.Normalize();

			// UpとDirectionが同軸上に存在してしまった場合は例外的に個別に決める
			if (Right == Vector3.Zero) {
				if (Direction == new Vector3(0, -1, 0) || Direction == new Vector3(0, 1, 0)) {
					Up = new Vector3(1, 0, 0);
					Right = Vector3.Cross(Direction, Up);
					Right.Normalize();
				}
			}

			Vector3 projectedDirection = Direction - Vector3.Dot(Direction, Vector3.UnitY) * Vector3.UnitY;
			float angleX = (float)Math.Acos((double)(Vector3.Dot(Direction, projectedDirection)
				/ (Direction.Length() * projectedDirection.Length())));
			//float angleY = Math.Atan();
			//float angleZ = Math.Atan();

			// ContentProcessorで処理した結果、デフォルトでUnitXの方向を向いているので、Directionと内積を取って角度を求めて回転させる。
			// Worldの3軸を直接変更した方が簡単だと思ったがUpが定まらないので無理
			/*_world = Matrix.Identity;
			_world = Matrix.CreateScale(Scale)
				//* Matrix.CreateRotationY((-Vector3.Dot(Vector3.UnitX, Direction) + MathHelper.ToRadians(90)))
				* Matrix.CreateFromYawPitchRoll(angleX//Vector3.Dot(Vector3.UnitZ, Direction
					, 0//-Vector3.Dot(Vector3.UnitY, Direction) + MathHelper.ToRadians(90)
					, 0)
				* Matrix.CreateTranslation(Position);*/

			/*_world = Matrix.Identity;
			_world *= Matrix.CreateScale(Scale);
			_world.Forward *= Direction;
			_world.Up *= Up;
			_world.Right *= Right;
			_world *= Matrix.CreateTranslation(Position);*/
			_world = Matrix.Identity;
			_world.Forward = Direction;
			_world.Up = Up;
			_world.Right = Right;
			_world *= Matrix.CreateScale(Scale);
			_world *= Matrix.CreateTranslation(Position);

			/*_world = Matrix.CreateWorld(Position, Direction, Vector3.Up);
			_world *= Matrix.CreateScale(Scale);*/
		}
        /// <summary>
        /// 移動処理（Waypointベース、状態遷移ベースなど）を実行するメソッド。
        /// 派生クラスで具体的に実装する
        /// </summary>
        protected virtual void Move()
        {
        }
		public override object Clone()
        {
            MilitaryShip cloned = (MilitaryShip)MemberwiseClone();

            if (positions != null) {
                cloned.positions = new List<Vector3>();
                foreach (Vector3 v in positions) {
                    cloned.positions.Add(v);
                }
            }
            if (currentSounds != null) {
                cloned.currentSounds = new List<SoundEffectInstance>();
                foreach (SoundEffectInstance v in currentSounds) {
                    cloned.currentSounds.Add(v);
                }
            }
            if (obstacles != null) {
                cloned.obstacles = new List<BoundingSphere>();
                foreach (BoundingSphere v in obstacles) {
                    cloned.obstacles.Add(v);
                }
            }
            if (engineTrailEffect != null) {
                cloned.engineTrailEffect = (BillboardStrip)engineTrailEffect.Clone();
            }


            //return base.Clone();
            return cloned;
        }
		public override void Update(GameTime gameTime)
		{
			if (IsActive) {
				CheckObstacles();
				AvoidCollision();
				//WayPointMove();
                Move();

                // 再生終了したSEインスタンスを削除
				for (int i = currentSounds.Count - 1; i >= 0; i--) {
					if (currentSounds[i].State != SoundState.Playing) {
						currentSounds[i].Dispose();
						currentSounds.RemoveAt(i);
					}
				}


				Position += Velocity;
				upPosition += Velocity;
				UpdateWorldMatrix();
				transformedBoundingSphere = new BoundingSphere(
						Position,
                        Model.Meshes[0].BoundingSphere.Radius * Scale);

				stripCount++;
				//if (stripCount % 5 == 0)
				UpdateLocus();
				engineTrailEffect.Update(gameTime);
			} else {
				UpdateLocus();
				engineTrailEffect.Update(gameTime);
				if (positions.Count == 0) IsAlive = false;
			}
		}
		public override void Die()
		{
			//base.Die();
			IsActive = false;
		}
		public override void Draw(Matrix View, Matrix Projection, Vector3 CameraPosition)
		{
			base.Draw(View, Projection, CameraPosition);

			if (!DrawingPrePass) {
				engineTrailEffect.Draw(level.camera);
			}
		}
        public void Damage(int damage)
        {
            HitPoint -= damage;

            if (HitPoint <= 0) {
                Die();
            }
        }

        // Constructor
		private void Initialize()
		{
			// Initialize default up position. this value is used for calculating Up vector later.
			upPosition = Position + Vector3.UnitY;

			viewSphere = new BoundingSphere(Position, 500);// 2000,100
			obstacles = new List<BoundingSphere>();

			// Initialize waypoints
			wayPoints0 = new Vector3[] {
				Target + Vector3.Normalize(StartPosition - Target) * 500,
				Target + Vector3.UnitY * 400,
				Target - Vector3.Normalize(StartPosition - Target) * 500,
				Target - Vector3.UnitY * 400,
			};
			yawDebug = new Vector3[] {
				Target + Vector3.Normalize(StartPosition - Target) * 500,
				Target + Vector3.UnitX * 500,
				Target - Vector3.Normalize(StartPosition - Target) * 500,
				Target - Vector3.UnitX * 500,
			};
			currentWayPoint = wayPoints0[0];
		}
		public MilitaryShip(Vector3 position, Vector3 target, float scale, string filePath)
			: base(position, scale, filePath, true)
		{
			this.StartPosition = position;
			this.Target = target;
			//this.State = FighterState.Move;
			this.State = FighterState.MoveToAttack;

			positions = new List<Vector3>();
			engineTrailEffect = new BillboardStrip(Level.graphicsDevice, content, content.Load<Texture2D>("Textures\\Lines\\Line2T1"),//"Textures\\Lines\\Line2T1"
				new Vector2(10, 100), positions);
			//shootSound = content.Load<SoundEffect>("SoundEffects\\laser1");//License\\LAAT1
			shootSound = content.Load<SoundEffect>("SoundEffects\\License\\LAAT1");

			Initialize();
			MaxHitPoint = 30;
			HitPoint = MaxHitPoint;
		}
    }
}
