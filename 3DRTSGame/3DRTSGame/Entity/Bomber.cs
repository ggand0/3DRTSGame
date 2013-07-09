using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace _3DRTSGame
{
	public class Bomber : Object, IDamageable
	{
		public Vector3 Target { get; set; }
		public Vector3 StartPosition { get; private set; }

		private List<BoundingSphere> obstacles;
		private BoundingSphere viewSphere;
		private BillboardStrip engineTrailEffect;
		private List<Vector3> positions;
        protected Vector3 upPosition;
        private int count, stripCount;
        private static readonly int shootRate = 10;//120
        private bool hasBuild, shouldShoot, turned;
		private SoundEffect shootSound;
		private List<SoundEffectInstance> currentSounds = new List<SoundEffectInstance>();


		#region Methods
		private void Shoot(int bulletType)
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
					/*level.Bullets.Add(new LaserBillboardBullet(Level.graphicsDevice, content, Position
						, Position + Direction, Direction, 50, content.Load<Texture2D>("Textures\\Mercury\\Laser"), new Vector2(10f, 40), 0));*/
					/*level.Bullets.Add(new LaserBillboardBullet(Level.graphicsDevice, content, Position
						,Position + Direction * 200, Direction,50, content.Load<Texture2D>("Textures\\Mercury\\Laser"), Color.Red, BlendState.AlphaBlend, new Vector2(200f, 100), 0));*/
					level.Bullets.Add(new LaserBillboardBullet(IFF.Foe, Level.graphicsDevice, content, Position
						, Position + Direction * 200, Direction, 50, content.Load<Texture2D>("Textures\\Mercury\\Laser"), Color.Red, BlendState.AlphaBlend, new Vector2(200f, 100), 0));
					break;
			}
		}
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

		public void Damage()
		{
			HitPoint--;

			if (HitPoint <= 0) {
				Die();
			}
		}
		public override object Clone()
		{
			Bomber cloned = (Bomber)MemberwiseClone();

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
		public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
		{
			if (IsActive) {
				/*CheckObstacles();
				AvoidCollision();
				//WayPointMove();
				AttackMove();*/

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
						Position
						, Model.Meshes[0].BoundingSphere.Radius * Scale);

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
		#endregion


		// Constructor
		private void Initialize()
		{
			// Initialize default up position. this value is used for calculating Up vector later.
			upPosition = Position + Vector3.UnitY;

			viewSphere = new BoundingSphere(Position, 500);
			obstacles = new List<BoundingSphere>();
		}
		public Bomber(Vector3 position, Vector3 target, float scale, string filePath)
			: base(position, scale, filePath, true)
		{
			this.StartPosition = position;
			this.Target = target;
			//this.State = FighterState.MoveToAttack;

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
