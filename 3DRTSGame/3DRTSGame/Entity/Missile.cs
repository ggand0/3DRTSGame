﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _3DRTSGame
{
	/// <summary>
	/// 軌跡エフェクトを付けて動かしまくりたい！
	/// </summary>
	public class Missile : Bullet, ICloneable
	{
		public Object Target { get; private set; }

		// RendererとしてObjectを持たせよう
		//public Model Model { get; private set; }
		public Object Renderer { get; private set; }
		public Vector3 Velocity { get; private set; }

		private static BasicEffect basicEffect;
		private Matrix _world;
		private Vector3 upPosition;
		//private float Scale;
		private BillboardStrip billboardStrip;
		private List<Vector3> positions;
		private BoundingSphere boundingSphere;
		private void UpdateLocus()
		{
			positions.Add(Position);
			billboardStrip.Positions = positions;
			if (positions.Count >= BillboardStrip.MAX_SIZE) {//120
				positions.RemoveAt(0);
			} else if (positions.Count > 0) {//positions.Count >= 2
				//billboardStrip.AddBillboard(Level.graphicsDevice, content, content.Load<Texture2D>("Textures\\Mercury\\Laser"), new Vector2(10, 40), positions[positions.Count - 2], positions[positions.Count - 1]);
				//billboardStrip.AddBillboard(Level.graphicsDevice, content, content.Load<Texture2D>("Textures\\Laser2"), new Vector2(10, 40), positions[positions.Count - 2], positions[positions.Count - 1]);
				billboardStrip.AddVertices();
			}
		}

		public override void Update(GameTime gameTime)
		{
			// とりあえず敵が追跡中に死んだら自分も殺すようにしておく
			/*if (!Target.IsAlive || !Target.IsActive) {
				Die();
				return;
			}*/

			Direction = Vector3.Normalize(Target.Position - Position);
			Velocity = Direction * Speed;

			Position += Velocity;
			upPosition += Velocity;

			UpdateWorldMatrix();
			boundingSphere = new BoundingSphere(Position, Renderer.Model.Meshes[0].BoundingSphere.Radius * Renderer.Scale);

			Renderer.Position = Position;
			//Renderer.World = _world;
			UpdateLocus();
			billboardStrip.Update(gameTime);

			this.IsActive = IsActiveNow();
		}

		public override bool IsActiveNow()
		{
			distanceTravelled = Vector3.Distance(StartPosition, Position);
			if (distanceTravelled > MAX_DISTANCE || !Target.IsAlive || !Target.IsActive) {
				return false;
			} else {
				return true;
			}
		}
		protected void UpdateWorldMatrix()
		{
			/*_world = Matrix.Identity;
			_world *= Matrix.CreateScale(Renderer.Scale);
			_world *= Matrix.CreateTranslation(Position);
			Vector3 workVector = _world.Translation;
			workVector.Normalize();*/

			Vector3 up = Vector3.Normalize(upPosition - Position);
			up.Normalize();
			Vector3 right = Vector3.Cross(Direction, up);
			right.Normalize();

			_world = Matrix.Identity;
			_world.Forward = Direction;
			_world.Up = up; ;
			_world.Right = right;/**/
			_world *= Matrix.CreateScale(Renderer.Scale);
			_world *= Matrix.CreateTranslation(Position);
			Renderer.World = _world;
		}
		public override bool IsHitWith(Object o)
		{
			//return Renderer.IsHitWith(o.transformedBoundingSphere);
			return boundingSphere.Intersects(o.transformedBoundingSphere);
		}
        public void Initialize(Object target, Vector3 position, Vector3 direction)
        {
            this.Target = target;
            this.Position = position;
            this.Direction = direction;
        }
        public object Clone()
        {
            Missile cloned = (Missile)MemberwiseClone();

            // 参照を持っているのは、Target, Renderer, positions:
            if (this.Target != null) {
                // あ、Targetは参照し続けないとダメだな
                //cloned.Target = (Object)this.Target.Clone();
                //cloned.Target = (Object)this.Target.Clone();
            }
            if (this.Renderer != null) {
                cloned.Renderer = (Object)this.Renderer.Clone();
            }
            if (this.positions != null) {
                // Listを直接Clone出来ないのでこのような操作を行っている
                cloned.positions = new List<Vector3>();
                for (int i = 0; i < positions.Count; i++) {
                    cloned.positions.Add(this.positions[i]);
                }
            }
            if (this.billboardStrip != null) {
                cloned.billboardStrip = (BillboardStrip)this.billboardStrip.Clone();
                // 面倒なのでnewする
                /*cloned.billboardStrip = new BillboardStrip(Level.graphicsDevice, content,
                content.Load<Texture2D>("Textures\\Lines\\smoke"), new Vector2(10, 30), positions, true);*/
            }

            return cloned;
        }

		public override void Draw(Camera camera)
		{
			Object.SetEffectParameter(lightingEffect, "CenterToCamera", level.camera.Direction);
			Renderer.Draw(camera.View, camera.Projection, camera.Position);
			billboardStrip.Draw(camera.View, camera.Projection, camera.Up, camera.Right, camera.Position);
		}

		static Effect lightingEffect;
		// Constructors
		public Missile(Object user, float speed)
			: this(IFF.Friend, user, null, speed, user.Direction, user.Position, 1, "Models\\pea_proj")
		{
		}
		public Missile(IFF identification, Object user, Object target, float speed, Vector3 direction, Vector3 position, float scale, string filePath)
			:base(identification, position, direction, speed)
		{
			/*this.Position = position;
			startPosition = position;
			this.Speed = speed;
			this.Direction = direction;
			Direction.Normalize();
			this.Right = user.Right;
			this.Up = user.Up;
			this.Velocity = Direction * speed;
			Load(filePath);*/

			MAX_DISTANCE = 2000;
			this.Target = target;
			Position = position;
			upPosition = position + Vector3.Up;

			
			positions = new List<Vector3>();

			if (lightingEffect == null) {
				lightingEffect = content.Load<Effect>("Lights\\MissileLightingEffect");
				Object.SetEffectParameter(lightingEffect, "DoRimLighting", true);
				Object.SetEffectParameter(lightingEffect, "RimColor", Color.Blue.ToVector4());
				Object.SetEffectParameter(lightingEffect, "CenterToCamera", level.camera.Direction);
			}
			Renderer = new Object(position, scale, filePath, false);// effectを設定すると「textureCoordinate0がありません」エラー
			Renderer.SetModelEffect(lightingEffect, true);
			billboardStrip = new BillboardStrip(Level.graphicsDevice, content,
				content.Load<Texture2D>("Textures\\Lines\\smoke"), new Vector2(10, 30), positions, true);
		}
		static Missile()
		{
			basicEffect = new BasicEffect(Level.graphicsDevice);
		}
	}
}
