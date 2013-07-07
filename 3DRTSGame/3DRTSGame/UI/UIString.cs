using Microsoft.Xna.Framework;

namespace _3DRTSGame
{
	public class UIString : UIObject
	{
		private static readonly Vector2 TEXT_POSITION = new Vector2(0, 0);
		private int credit;

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			credit = level.player.Cregit;
		}
		public override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);

			spriteBatch.DrawString(game.menuFont, "Credit " + credit.ToString(), TEXT_POSITION, Color.White);
			spriteBatch.DrawString(game.menuFont, "BUTTONL " + MouseInput.BUTTONL().ToString(), TEXT_POSITION + new Vector2(0, 30), Color.White);
		}

		public UIString()
		{
			credit = level.player.Cregit;
		}
	}
}
