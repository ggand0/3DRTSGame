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
		public bool Stational { get; set; }
		public bool RimLighting { get; set; }

		private static Effect lightingEffect;
		private Vector3 rotationSpeed = new Vector3(0.1f,0.04f, 0);

		//public override bool IsHitWith(Drawable d)
		private static readonly float DEF_REVOLUTION_SPEED = 0.05f;//0.0005f;
		private float revolutionAngle, revolutionSpeed;

		private Matrix BuildRotationMatrix()
		{
			float yaw = Utility.NextDouble(random, 0, 360);
			float pitch = Utility.NextDouble(random, 0, 360);
			float roll = Utility.NextDouble(random, 0, 360);

			return Matrix.CreateFromYawPitchRoll(yaw, pitch, roll);
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
		protected override void UpdateWorldMatrix()
		{
			_world = Matrix.CreateScale(Scale) * RotationMatrix *//Matrix.CreateFromYawPitchRoll(Rotation.X, Rotation.Y, Rotation.Z) *
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
		

		/// <summary>
		/// Cloneメソッドを基底クラスで実装した場合、派生クラスで実装し直す必要あり
		/// </summary>
		/// <returns></returns>
        public override object Clone()
        {
            Asteroid cloned = (Asteroid)MemberwiseClone();

            if (lightingEffect != null) {
                //cloned.lightingEffect = lightingEffect.Clone();
            }

            return cloned;
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
			if (!Stational) Move(1);

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
			//SetEffectParameter(lightingEffect, "AccentColor", (Color.Red * dColor).ToVector4());
			SetEffectParameter(lightingEffect, "AccentColor", (Color.Transparent).ToVector4());
			SetEffectParameter(lightingEffect, "DoShadowMapping", false);
			SetEffectParameter(lightingEffect, "DoRimLighting", true);
			if (RimLighting) {
				//Vector3 centerToCamera = Vector3.Normalize(CameraPosition - Position);
				//SetEffectParameter(lightingEffect, "CenterToCamera", centerToCamera);

				// 必ずSetModelEffectするときにCloneしないでfalseにしておく。
				// otherwise参照されないのでここでパラメータ変えても全く反映されない
				SetEffectParameter(lightingEffect, "CenterToCamera", Vector3.Negate(level.camera.Direction));
			}

			base.Draw(View, Projection, CameraPosition);
		}


		#region Constructors
		public void Initialize()
		{
			revolutionAngle = CalcInitialAngle();
		}
		public Asteroid(Vector3 position, float scale, string fileName)
			:this(position, Vector3.Zero, scale, 1, fileName)
		{
		}
		public Asteroid(Vector3 position, Vector3 destination, float scale, float speed, string fileName)
			: this(position, destination, scale, speed, fileName, false)
		{
		}
		public Asteroid(Vector3 position, Vector3 destination, float scale, float speed, string fileName, bool rimLighting)
			: base(position, scale, fileName)
		{
			this.Destination = destination;
			this.RimLighting = rimLighting;
			Speed = speed;


			//Rotation = new Vector3((float)random.NextDouble() / 200f, (float)random.NextDouble() / 200f, (float)random.NextDouble() / 200f);

			//lightingEffect = content.Load<Effect>("Lights\\AsteroidLightingEffect");	// load Prelighting Effect
			SetEffectParameter(lightingEffect, "AccentColor", Color.Red.ToVector4());
			SetEffectParameter(lightingEffect, "DoShadowMapping", false);
			SetEffectParameter(lightingEffect, "TextureEnabled", true);
			SetEffectParameter(lightingEffect, "DoRimLighting", rimLighting);
			if (rimLighting) {
				SetEffectParameter(lightingEffect, "RimColor", Color.Red.ToVector4());
				SetEffectParameter(lightingEffect, "CenterToCamera", level.camera.Direction);
			}


			// Accent Colorを後で変えたいので、変更をそのまま反映させるためfalseにする！←というわけでもなかった...
			SetModelEffect(lightingEffect, true);
			//SetModelEffect(lightingEffect, false);

			revolutionAngle = CalcInitialAngle();
			RotationMatrix = BuildRotationMatrix();
			MaxHitPoint = 15;
			HitPoint = MaxHitPoint;
		}
		static Asteroid()
		{
			lightingEffect = content.Load<Effect>("Lights\\AsteroidLightingEffect");
		}
		#endregion
	}
}
