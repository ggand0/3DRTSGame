using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace _3DRTSGame
{
	public class Asteroid : Object, IDamageable
	{
		//protected static readonly int DEF_HIT_POINT = 30;

		public Vector3 Destination { get; set; }
		public float Speed { get; private set; }
		/// <summary>
		/// (Yaw, Pitch, Roll)
		/// </summary>
		public Vector3 Rotation { get; protected set; }

		private Effect lightingEffect;
		private Vector3 rotationSpeed = new Vector3(0.1f,0.04f, 0);

		//public override bool IsHitWith(Drawable d)
		private static readonly float DEF_REVOLUTION_SPEED = 0.05f;//0.0005f;
		private float revolutionAngle, revolutionSpeed;

		/// <summary>
		/// Cloneメソッドを基底クラスで実装した場合、派生クラスで実装し直す必要あり
		/// </summary>
		/// <returns></returns>
        public override object Clone()
        {
            Asteroid cloned = (Asteroid)MemberwiseClone();

            if (lightingEffect != null) {
                cloned.lightingEffect = lightingEffect.Clone();
            }

            return cloned;
        }
		protected override void UpdateWorldMatrix()
		{
			_world = Matrix.CreateScale(Scale) * Matrix.CreateFromYawPitchRoll(Rotation.X, Rotation.Y, Rotation.Z) *
				Matrix.CreateTranslation(Position);
		}
		protected virtual void Move(int pattern)
		{
			switch (pattern) {
				default :
					Rotation += rotationSpeed;
					Velocity = (Vector3.Normalize(Destination - Position)) * Speed;
					Position += Velocity;
					break;
				case 1:
					float a = 0.15f, b = 0.5f;

					revolutionSpeed = 1f;//DEF_REVOLUTION_SPEED;
					revolutionAngle -= MathHelper.ToRadians(revolutionSpeed);
					Velocity = new Vector3(a * (float)Math.Exp(b * revolutionAngle) * (float)Math.Cos(revolutionAngle), 0,
						a * (float)Math.Exp(b * revolutionAngle) * (float)Math.Sin(revolutionAngle));
					float radius = (Position - Destination).Length();
					Position = Destination + Velocity;
					//Position += Velocity * 100;
					//Position += Velocity;
					break;
			}
		}
		private float CalcInitialAngle()
		{
			float radius = (Position - Destination).Length();
			Vector3 velocity = new Vector3((float)Math.Cos(revolutionAngle), 0,
					(float)Math.Sin(revolutionAngle));
			Vector3 def = Destination + velocity * radius;
			Vector3 v1 = Vector3.Normalize(Destination - def);
			Vector3 v2 = Vector3.Normalize(Destination - Position);

			//return (float)Math.Acos(Vector3.Dot(v1, v2));
			return MathHelper.ToRadians(1440);
		}


		public void Damage()
		{
			HitPoint--;

			if (HitPoint <= 0) {
				Die();
			}
		}
		public override void Update(GameTime gameTime)
		{
			/*revolutionSpeed = DEF_REVOLUTION_SPEED;
				revolutionAngle += MathHelper.ToRadians(revolutionSpeed);
				Vector3 velocity = new Vector3((float)Math.Cos(revolutionAngle), 0,
					(float)Math.Sin(revolutionAngle));
				float radius = (Position - Center).Length();
				Vector3 tmp = Center + velocity * radius;
				Position = tmp;
			*/
			Move(1);

			UpdateWorldMatrix();
			transformedBoundingSphere = new BoundingSphere(
						Position
						, Model.Meshes[0].BoundingSphere.Radius * Scale);
		}
		int blinkCount;
		float e;
		public override void Draw(Matrix View, Matrix Projection, Vector3 CameraPosition)
		{
			blinkCount++;
			if (blinkCount % 5 == 0) e += 60f;//30f;//.02f;
			float dColor = (float)Math.Sin(e * 8) / 2.0f + 0.5f;
			SetEffectParameter(lightingEffect, "AccentColor", Color.Red);

			base.Draw(View, Projection, CameraPosition);
		}

		public void Initialize()
		{
			revolutionAngle = CalcInitialAngle();
		}
		public Asteroid(Vector3 position, float scale, string fileName)
			:this(position, Vector3.Zero, scale, 1, fileName)
		{
		}
		public Asteroid(Vector3 position, Vector3 destination, float scale, float speed, string fileName)
			: base(position, scale, fileName)
		{
			this.Destination = destination;
			Speed = speed;
			//Rotation = new Vector3((float)random.NextDouble() / 200f, (float)random.NextDouble() / 200f, (float)random.NextDouble() / 200f);

			lightingEffect = content.Load<Effect>("Lights\\AsteroidLightingEffect");	// load Prelighting Effect
			SetEffectParameter(lightingEffect, "AccentColor", Color.Red);
			// Accent Colorを後で変えたいので、変更をそのまま反映させるためfalseにする
			SetModelEffect(lightingEffect, true);					// set effect to each modelmeshpart

			revolutionAngle = CalcInitialAngle();
			MaxHitPoint = 15;
			HitPoint = MaxHitPoint;
		}
	}
}
