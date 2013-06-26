using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace _3DRTSGame
{
	public class UIObject : Drawable
	{
		public static SpriteBatch spriteBatch;
		public static ContentManager content;
		public static Game1 game;
		public static Level level;

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
