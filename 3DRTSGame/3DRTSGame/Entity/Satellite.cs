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
	public class Satellite : Object
	{
		private static readonly float DEF_REVOLUTION_SPEED = 0.25f;

		public bool Rotate { get; protected set; }
		public bool Revolution { get; protected set; }
		/// <summary>
		/// Means planet position
		/// </summary>
		public Vector3 Center { get; protected set; }
		public float Radius { get; protected set; }
		public float Roll { get; protected set; }
		public float Pitch { get; protected set; }
		private float rotationSpeed, revolutionSpeed, revolutionAngle;

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
			return (float)Math.Acos(Vector3.Dot(v1, v2));
		}

		public override void Update(GameTime gameTime)
		{
			if (Rotate) {
				Roll += rotationSpeed;
			}
			if (Revolution) {
				revolutionSpeed = DEF_REVOLUTION_SPEED;//1
				revolutionAngle += MathHelper.ToRadians(revolutionSpeed);
				Vector3 velocity = new Vector3((float)Math.Cos(revolutionAngle), 0,
					(float)Math.Sin(revolutionAngle));
				//Vector3 tmp = StarPosition + velocity * 3000;

				float radius = (Position - Center).Length();
				//Vector3 tmp = Center + velocity * radius;
				Vector3 tmp = Center + velocity * radius;

				Position = tmp;// ここが位置が与えた値と食い違う原因
				//Position = tmp + initialPoint;
			}

			base.Update(gameTime);
		}
		protected override void UpdateWorldMatrix()
		{
			//base.UpdateWorldMatrix();
			_world = Matrix.CreateScale(Radius) * Matrix.CreateRotationY(Roll) * Matrix.CreateRotationX(Pitch)
				* Matrix.CreateTranslation(Position);
		}

		#region Constructors
		public Satellite(Vector3 position, float scale, string fileName)
			: base(position)
		{
			Rotate = false;
			Revolution = false;
			this.initialPoint = position;
		}
		public Satellite(bool revolution, Vector3 position, Vector3 center, float scale, string fileName)
			:base(position, scale, fileName)
		{
			this.Revolution = revolution;
			this.Center = center;
			this.initialPoint = position;
			revolutionAngle = CalcInitialAngle();
		}
		#endregion
	}
}
