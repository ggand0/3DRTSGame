using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace _3DRTSGame
{
	public class UIManager : Drawable
	{
		public static SpriteBatch spriteBatch;
		private List<UIObject> interfaces = new List<UIObject>();
		private static UnitPanel mainPanel = new UnitPanel();


		public static Type GetUnitType()
		{
			return mainPanel.GetUnitType();
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			foreach (UIObject ui in interfaces) {
				ui.Update(gameTime);
			}
		}
		public override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);

			spriteBatch.Begin();
			foreach (UIObject ui in interfaces) {
				ui.Draw(gameTime);
			}
			mainPanel.Draw(gameTime);
			spriteBatch.End();
		}
		private void Initialize()
		{
			interfaces.Add(new MouseCursor());
			//interfaces.Add(new UnitPanel());
		}

		public UIManager()
		{
			
			Initialize();
		}
	}
}
