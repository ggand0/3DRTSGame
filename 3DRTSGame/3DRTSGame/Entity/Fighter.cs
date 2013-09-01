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

        private void BuildDirectionChangeWayPoints()
        {
            changeDirWayPoints.Clear();

            // とりあえず今乗っている平面内での円形ターンするルートで
            //Matrix rotation = Matrix.CreateFromAxisAngle(Up, MathHelper.ToRadians(90));
            Vector3 optionalUnitVector =
                //Vector3.Transform(Direction, rotation);
                Right;
            Vector3 left = Vector3.Normalize(_world.Left);
            /*changeDirWayPoints.Add(Position + Direction * 100);//Vector3.Normalize(Velocity)
            changeDirWayPoints.Add(changeDirWayPoints[0] + Direction * 100 + optionalUnitVector * 100);
            changeDirWayPoints.Add(changeDirWayPoints[0] + optionalUnitVector * 200);
            changeDirWayPoints.Add(Position + optionalUnitVector * 200);*/
            changeDirWayPoints.Add(Position + Direction * 150 + left * 150);//Vector3.Normalize(Velocity)
            changeDirWayPoints.Add(Position + Direction * 300);
            changeDirWayPoints.Add(Position + Direction * 150 + Right * 150);
            changeDirWayPoints.Add(Position);
        }
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
        protected void AttackMove()
        {
            float speed = 1;
            float targetRadius = 200;
            float runLength = 1500;

            switch (State) {
                case FighterState.MoveToAttack:
                    hasBuild = false;
                    shouldShoot = true;
                    currentWayPoint = !turned ? (Target + Vector3.Normalize(StartPosition - Target) * runLength)
                        : (Target + Vector3.Normalize(Target - StartPosition) * runLength);
                    Velocity += Vector3.Normalize(currentWayPoint - Position) * speed;
                    Velocity *= 0.9f;

                    if ((currentWayPoint - Position).Length() < Velocity.Length() + 1.0f) {
                        State = FighterState.AttackMove;
                    }
                    break;
                case FighterState.AttackMove:
                    count++;

                    // 対象を貫くルートになるが、Collision Avoidanceの速度を常に足しているので避けられるはず
                    currentWayPoint = !turned ? (Target + Vector3.Normalize(Target - StartPosition) * runLength)// positionにするとカオス！
                        : (Target + Vector3.Normalize(StartPosition - Target) * runLength);
                    Velocity += Vector3.Normalize(currentWayPoint - Position) * speed;
                    Velocity *= 0.9f;

                    if ((Position - Target).Length() < targetRadius + speed + 200f) {
                        shouldShoot = false;// 通り過ぎたので
                    }
                    if (shouldShoot && count % shootRate == 0) {
                        Shoot(2);
                        
                    }

                    if ((currentWayPoint - Position).Length() < Velocity.Length() + 1.0f) {
                        State = FighterState.ChangeDirection;
                        shouldShoot = false;// 一応
                    }
                    break;
                case FighterState.ChangeDirection:
                    if (!hasBuild) {
                        BuildDirectionChangeWayPoints();
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
            Fighter cloned = (Fighter)base.Clone();

            if (changeDirWayPoints != null) {
                cloned.changeDirWayPoints = new List<Vector3>();
                foreach (Vector3 v in changeDirWayPoints) {
                    cloned.changeDirWayPoints.Add(v);
                }
            }

            return cloned;
        }

		public Fighter(Vector3 position, Vector3 target, float scale, string filePath)
			: base(position, target, scale, filePath)
		{
            changeDirWayPoints = new List<Vector3>();
		}
	}
}
