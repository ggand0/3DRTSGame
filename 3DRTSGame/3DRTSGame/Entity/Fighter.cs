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
	public enum FighterState
	{
		Move,
		MoveToAttack,
		AttackMove,
		ChangeDirection,
		Attack,
		Flee,
		Wait// 逃げた後HP回復の待機状態を想定
	}
	public class Fighter : MilitaryShip, IDamageable
	{
        private List<Vector3> changeDirWayPoints;
        private bool hasBuild, shouldShoot, turned;
		private static readonly float RUN_LENGTH = 1500;
		private static readonly float WAYPOINT_DISTANCE = 500;


		protected override void DetectCurrentWaypoint(Vector3[] wayPoints, bool shoot)
		{
			float margin = 1.0f;
			if ((currentWayPoint - Position).Length() < Velocity.Length() + margin) {
				currentWayPointIndex++;
				if (currentWayPointIndex >= wayPoints.Length) {
					State = FighterState.ChangeDirection;
					shouldShoot = false;
					currentWayPointIndex = 0;
				}

				if (shoot && currentWayPointIndex == 1 || currentWayPointIndex == 3) Shoot(2);

				currentWayPoint = wayPoints[currentWayPointIndex];
			}
		}
        /// <summary>
        /// 方向転換の際に、専用に辿るウェイポイントを設定する
        /// </summary>
        private void BuildDirectionChangeWayPoints(float radius)
        {
            changeDirWayPoints.Clear();

            // 今乗っている平面内で円形ターンするルートだったが、
			// flockに関連して問題が起きたので単に反転するルートに変更
            //Matrix rotation = Matrix.CreateFromAxisAngle(Up, MathHelper.ToRadians(90));
			Vector3 optionalUnitVector = Right;//Vector3.Transform(Direction, rotation);
            Vector3 left = Vector3.Normalize(_world.Left);

			/*changeDirWayPoints.Add(Position + Direction * radius + left * radius);
			changeDirWayPoints.Add(Position + Direction * radius * 2);
			changeDirWayPoints.Add(Position + Direction * radius + Right * radius);
            changeDirWayPoints.Add(Position);*/
			changeDirWayPoints.Add(Position + Direction * radius);
			//changeDirWayPoints.Add(Position);
        }
        /// <summary>
        /// 方向転換用のウェイポイントを使用して移動ルートを変更する
        /// </summary>
        /// <returns></returns>
        private bool ChangeDirection()
        {
            float margin = 5.0f;
            if ((currentWayPoint - Position).Length() < Velocity.Length() + margin) {
                currentWayPointIndex++;
                if (currentWayPointIndex >= changeDirWayPoints.Count) {
                    return true;//currentWayPointIndex = 0;
                }

                currentWayPoint = changeDirWayPoints[currentWayPointIndex];
            }
            return false;
        }
        /// <summary>
        /// ヒットアンドアウェイを繰り返す動作。単純な状態遷移を利用
        /// </summary>
        protected void AttackMove()
        {
            float speed = 1;
            float targetRadius = 200;
            //float runLength = 1500;

            switch (State) {
                case FighterState.MoveToAttack:
                    hasBuild = false;
                    shouldShoot = true;
                    currentWayPoint = !turned ? (Target + Vector3.Normalize(StartPosition - Target) * RUN_LENGTH)
                        : (Target + Vector3.Normalize(Target - StartPosition) * RUN_LENGTH);
					//DetectCurrentWaypoint(wayPoints0);

                    Velocity += Vector3.Normalize(currentWayPoint - Position) * speed;
                    Velocity *= 0.9f;

                    if ((currentWayPoint - Position).Length() < Velocity.Length() + 1.0f) {
                        State = FighterState.AttackMove;

						float dis = 150;
						Vector3 test = Vector3.Cross(Direction, Right);
						test.Normalize();
						test = Vector3.Negate(test);
						wayPoints0 = new Vector3[] {
							!turned ? (Target + Vector3.Normalize(StartPosition - Target) * WAYPOINT_DISTANCE)
								: (Target + Vector3.Normalize(Target - StartPosition) * WAYPOINT_DISTANCE),
							//Target + Up * WAYPOINT_DISTANCE,
							!turned ? (Target + Vector3.Normalize(StartPosition - Target) * (WAYPOINT_DISTANCE - dis) + test * 200)
								: (Target + Vector3.Normalize(Target - StartPosition) * (WAYPOINT_DISTANCE - dis) + test * 200),/**/
							!turned ? (Target + Vector3.Normalize(Target - StartPosition) * RUN_LENGTH)
								: (Target + Vector3.Normalize(StartPosition - Target) * RUN_LENGTH)
						};
						currentWayPoint = wayPoints0[0];
						currentWayPointIndex = 0;

						Level4.debugPoints.Clear();
						foreach (Vector3 v in wayPoints0) {
							Level4.debugPoints.Add(v);
						}
                    }
                    break;
                case FighterState.AttackMove:
                    count++;

                    // 対象を貫くルートになるが、Collision Avoidanceの速度を常に足しているので避けられるはず
                    /*currentWayPoint = !turned ? (Target + Vector3.Normalize(Target - StartPosition) * RUN_LENGTH)
                        : (Target + Vector3.Normalize(StartPosition - Target) * RUN_LENGTH);*/
					DetectCurrentWaypoint(wayPoints0, false);
                    Velocity += Vector3.Normalize(currentWayPoint - Position) * speed;
                    Velocity *= 0.9f;

                    // 対象を通り過ぎた後は撃たない
                    if ((Position - Target).Length() < targetRadius + speed + 200f) {
                        shouldShoot = false;
                    }
                    // 対象に向かっている時は一定間隔で射撃
                    if (shouldShoot && count % shootRate == 0) {
                        Shoot(2);
                    }
                    // overrideしたDetect-メソッド内で状態遷移させることにした 
                    //if ((currentWayPoint - Position).Length() < Velocity.Length() + 1.0f) {
					/*if (currentWayPointIndex == wayPoints0.Length-1 && (currentWayPoint - Position).Length() < Velocity.Length() + 5.0f) {
                        State = FighterState.ChangeDirection;
                        shouldShoot = false;
                    }*/
                    break;
                case FighterState.ChangeDirection:
                    if (!hasBuild) {
                        BuildDirectionChangeWayPoints(200);
                        currentWayPointIndex = 0;
                        currentWayPoint = changeDirWayPoints[0];
                        hasBuild = true;
                        turned = turned ? false : true;
                    }
                    bool next = ChangeDirection();
                    if (next) {
                        State = FighterState.MoveToAttack;
                    }

                    Velocity += Vector3.Normalize(currentWayPoint - Position) * speed;
                    Velocity *= 0.9f;
                    break;
            }


            Direction = Vector3.Normalize(Velocity);
        }
        protected override void Move()
        {
            base.Move();

            AttackMove();
        }

        public override object Clone()
        {
			//Fighter cloned = (Fighter)MemberwiseClone();
			Fighter cloned = (Fighter)base.Clone();

            if (changeDirWayPoints != null) {
                cloned.changeDirWayPoints = new List<Vector3>();
                foreach (Vector3 v in changeDirWayPoints) {
                    cloned.changeDirWayPoints.Add(v);
                }
            }

            return cloned;
        }

		public void Initialize(Vector3 position, Vector3 target)
		{
			this.Position = position;
			this.StartPosition = position;
			this.Target = target;
			Velocity = Vector3.Zero;
			upPosition = Position + Vector3.UnitY;
			//_world = Matrix.Zero;
			_world = Matrix.Identity;
			UpdateWorldMatrix();
		}
		public Fighter(Vector3 position, Vector3 target, float scale, string filePath)
			: base(position, target, scale, filePath)
		{
            changeDirWayPoints = new List<Vector3>();

			/*wayPoints0 = new Vector3[] {
				Target + Vector3.Normalize(Target - StartPosition) * WAYPOINT_DISTANCE,
				Target + Up * WAYPOINT_DISTANCE,
				Target + Vector3.Normalize(Target - StartPosition) * RUN_LENGTH,
			};*/
		}
	}
}
