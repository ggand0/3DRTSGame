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
	public enum MovingState
	{
		Stational,
		Moving,
		Revolution
	}
	public class Satellite : Object
	{
		private static readonly float DEF_REVOLUTION_SPEED = 0.25f;
		private static readonly float DEF_MOVE_SPEED = 1f;
		
		protected MovingState currentMovingState = MovingState.Revolution;
		private Vector3 currentDestination;

		public bool Rotate { get; protected set; }
		//public bool Revolution { get; protected set; }
		/// <summary>
		/// Means planet position
		/// </summary>
		public Vector3 Center { get; protected set; }
		public float Radius { get; protected set; }
		public float Roll { get; protected set; }
		public float Pitch { get; protected set; }
		private float rotationSpeed, revolutionSpeed, revolutionAngle;
		protected BillboardSystem uiRing;

		// プレイヤーから操作できるプロパティ：
		public bool RevolutionClockwise { get; set; }
		public bool Revolution { get; set; }

		private Vector3 initialPoint;
		private float CalcInitialAngle()
		{
			float radius = (Position - Center).Length();
			Vector3 velocity = new Vector3((float)Math.Cos(revolutionAngle), 0,
					(float)Math.Sin(revolutionAngle));
			Vector3 def = Center + velocity * radius;


			Vector3 v1 = Vector3.Normalize(Center - def);
			Vector3 v2 = Vector3.Normalize(Center - Position);

			//return Vector3.Dot(v1, v2) - (float)Math.PI/2f;
			//return (float)Math.Acos(Vector3.Dot(v1, v2));// 正規化されているので距離で割る必要は無いはず
			//float angle = (float)Math.Acos(Vector3.Dot(v1, v2));

			//atan2(mouseY - cirleCenterY, circleCenterX - mouseX);
			float angle = (float)Math.Atan2(Position.Z - Center.Z, Position.X - Center.X);

			//return def.Z > Position.Z ? -angle : angle;// 角度の大きさしか分からないのでこれで調整
			return angle;
		}

		public void StartMove(Vector3 destination)
		{
			currentMovingState = MovingState.Moving;
			currentDestination = destination;
		}
		public void EndMove()
		{
			Center = new Vector3(Center.X, currentDestination.Y, Center.Z);
			revolutionAngle = CalcInitialAngle();
			currentMovingState = MovingState.Revolution;
		}
		public void SetState(MovingState state)
		{
			Center = new Vector3(Center.X, Position.Y, Center.Z);
			revolutionAngle = CalcInitialAngle();
			currentMovingState = state;
		}
		private void UpdateRevolution()
		{
			if (Rotate) {
				Roll += rotationSpeed;
			}
			if (Revolution) {
				revolutionSpeed = DEF_REVOLUTION_SPEED;//1

				// 時計回りか反時計回りに公転
				if (RevolutionClockwise) {
					revolutionAngle += MathHelper.ToRadians(revolutionSpeed);
				} else {
					revolutionAngle -= MathHelper.ToRadians(revolutionSpeed);
				}


				Vector3 velocity = new Vector3((float)Math.Cos(revolutionAngle), 0,
					(float)Math.Sin(revolutionAngle));
				//Vector3 tmp = StarPosition + velocity * 3000;

				float radius = (Position - Center).Length();
				//Vector3 tmp = Center + velocity * radius;
				Vector3 tmp = Center + velocity * radius;

				Position = tmp;// ここが位置が与えた値と食い違う原因
				//Position = tmp + initialPoint;
			}
		}
		public override void Update(GameTime gameTime)
		{
			switch (currentMovingState) {
				default:
					UpdateRevolution();
					break;
				/*case MovingState.Revolution:
					UpdateRevolution();
					break;*/
				case MovingState.Moving:
					Vector3 v = Vector3.Normalize(currentDestination - Position) * DEF_MOVE_SPEED;
					Position += v;

					if ((currentDestination - Position).Length() < DEF_MOVE_SPEED) {
						EndMove();
					}
					break;
			}

			base.Update(gameTime);
		}
		protected override void UpdateWorldMatrix()
		{
			//base.UpdateWorldMatrix();
			_world = Matrix.CreateScale(Radius) * Matrix.CreateRotationY(Roll) * Matrix.CreateRotationX(Pitch)
				* Matrix.CreateTranslation(Position);
		}

		private CircleRenderer circleRenderer;
		public override void Draw(Matrix View, Matrix Projection, Vector3 CameraPosition)
		{
			base.Draw(View, Projection, CameraPosition);
		
			if (currentMovingState == MovingState.Moving && !DrawingPrePass) {
				// static クラスだと面倒＆描画した円がカメラ移動でぶれるので、インスタンスクラスに変更
				circleRenderer.Draw(Level.graphicsDevice, View, Projection, Matrix.CreateTranslation(currentDestination), Player.UI_CIRCLE_COLOR, Player.UI_CIRCLE_RADIUS);//X:-788.4521 Y:0 Z:-573.3202
				DebugOverlay.Line(Position, currentDestination, Player.UI_CIRCLE_COLOR);
			}
		}
		

		/// <summary>
		/// object pooling用に、外部から初期化できるようなメソッド
		/// </summary>
		public void Initialize(Vector3 position, Vector3 center)
		{
			this.Position = position;
			//this.Center = center;
			this.Center = new Vector3(center.X, position.Y, center.Z);
			revolutionAngle = CalcInitialAngle();
		}
		
		#region Constructors
		public Satellite(Vector3 position, float scale, string fileName)
			//: base(position)
			:this(false, position, Vector3.Zero, scale, fileName)
		{
			/*Rotate = false;
			Revolution = false;
			this.initialPoint = position;
			RevolutionClockwise = true;

			circleRenderer = new CircleRenderer(Level.graphicsDevice, Color.White, 50);*/
		}
		public Satellite(bool revolution, Vector3 position, Vector3 center, float scale, string fileName)
			:base(position, scale, fileName, true)
		{
			this.Revolution = revolution;
			//this.Center = center;
			this.Center = new Vector3(center.X, position.Y, center.Z);
			this.initialPoint = position;
			revolutionAngle = CalcInitialAngle();
			RevolutionClockwise = true;

			circleRenderer = new CircleRenderer(Level.graphicsDevice, Color.White, 50);
		}
		#endregion
	}
}
