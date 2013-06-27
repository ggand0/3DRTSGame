using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _3DRTSGame
{
	/// <summary>
	/// ほとんどUnitCardと被っているが、新しく作ったほうがわかりやすいだろう
	/// </summary>
	public class UIButton : UIObject
	{
		public bool IsSelected()
		{
			Vector2 pos = MouseInput.GetMousePosition();
			return pos.X >= UIPosition.X && pos.X <= UIPosition.X + texture.Width * scale
				&& pos.Y >= UIPosition.Y && pos.Y <= UIPosition.Y + texture.Height * scale;
		}

		public override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);

			spriteBatch.Draw(texture, UIPosition, null, Color.White, 0, Vector2.Zero, new Vector2(scale), SpriteEffects.None, 0);
		}

		public UIButton(Texture2D texture, Vector2 position, float scale)
		{
			this.texture = texture;
			this.UIPosition = position;
			this.scale = scale;
		}
	}
}
