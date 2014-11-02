using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace _3DRTSGame
{
	public class Drawable : ICloneable
	{
		public static Game1 game;
		public static Level level;
		public static ContentManager content;
		public static SpriteBatch spriteBatch;

		protected Random random = new Random();

		public bool IsActive { get; set; }
		public bool IsAlive { get; set; }
		public Vector3 Position { get; set; }
		

		public virtual void Update(GameTime gameTime)
		{
		}
		public virtual void Draw(GameTime gameTime)
		{
		}
		public virtual object Clone()
		{
			return (Drawable)MemberwiseClone();
		}
		/*public virtual void Draw(Matrix View, Matrix Projection, Vector3 CameraPosition)
		{
		}
		public virtual void Draw(Matrix View, Matrix Projection, Vector3 Up, Vector3 Right)
		{
		}*/
		public virtual void Draw(Camera camera)
		{
		}


		public virtual bool IsHitWith(Drawable d)
		{
			return false;
		}
		public virtual bool IsHitWith(Object o)
		{
			return false;
		}
		public virtual void Die()
		{
			IsActive = false;
			IsAlive = false;
		}

		public Drawable()
		{
			IsActive = true;
			IsAlive = true;
		}
	}
}
