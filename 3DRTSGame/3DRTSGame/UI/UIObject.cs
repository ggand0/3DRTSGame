using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace _3DRTSGame
{
	public class UIObject : Drawable
	{
		protected Texture2D texture;
		protected Vector2 UIPosition;
		protected float scale;

		protected virtual void HandleInput()
		{
		}
		public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
		{
			HandleInput();
			base.Update(gameTime);
		}

		public UIObject()
		{
		}
	}
}
