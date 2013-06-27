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
			float offset = 1;

			return pos.X >= UIPosition.X + offset && pos.X <= UIPosition.X + texture.Width * scale - offset
				&& pos.Y >= UIPosition.Y + offset && pos.Y <= UIPosition.Y + texture.Height * scale - offset;
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
		}
		/// <summary>
		/// 重くなるようならUnitPanel側で呼び出す（大まかな判定で計算量を減らせる）
		/// </summary>
		public void DrawFrameLine()
		{
			if (IsSelected()) {
				int offset = 0;
				Primitives2D.DrawRectangle(spriteBatch, new Rectangle((int)UIPosition.X + offset, (int)UIPosition.Y + offset,
					(int)(texture.Width * scale - offset), (int)(texture.Height * scale) - offset), Color.White, 3);
			}
		}
		public override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);

			spriteBatch.Draw(texture, UIPosition, null, Color.White, 0, Vector2.Zero, new Vector2(scale), SpriteEffects.None, 0);
			DrawFrameLine();
		}

		public UIButton(Texture2D texture, Vector2 position, float scale)
		{
			this.texture = texture;
			this.UIPosition = position;
			this.scale = scale;
		}
	}
}
