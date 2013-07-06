using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace _3DRTSGame
{
	public class LoadingScene : MenuScene
	{
		/// <summary>
		/// メニューのテキストを表示開始する場所。
		/// </summary>
		//readonly Vector2 TEXT_POSITION;
		//readonly Vector2 TITLE_POSITION;
		readonly Texture2D maskTexture;
		//readonly SoundEffect selectSound;
		//readonly SoundEffect enterSound;
		string title = "";


		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			if (ObjectPool.LOADED) {
				PushScene(new Level4(this));
			}
		}
		int c = 0;
		public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
		{
			Thread.Sleep(10);

			spriteBatch.Begin();
			//マスクを表示
			spriteBatch.Draw(maskTexture, game.GraphicsDevice.Viewport.Bounds, Color.White);
			if (title != "") {
				spriteBatch.DrawString(game.menuFont, title, TITLE_POSITION, Color.White);
			}
			c++;
			spriteBatch.DrawString(game.menuFont, c.ToString(), Vector2.Zero, Color.Green);
			spriteBatch.End();
		}
		public LoadingScene(Scene previousScene)
			:base (previousScene)
		{
			Thread loadingThread = new Thread(ObjectPool.Load);
			loadingThread.Priority = ThreadPriority.Highest;
			loadingThread.Start();

			maskTexture = content.Load<Texture2D>("Textures\\UI\\loading");
		}

	}
}
