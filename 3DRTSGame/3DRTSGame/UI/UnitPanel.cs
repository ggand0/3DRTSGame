using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace _3DRTSGame
{
	public class UnitPanel : UIObject
	{
		public int Thickness { get; private set; }
		private int height;

		private Texture2D[] textures;// 位置計算のために用意
		private UnitCard[] unitCards;

		public Type GetUnitType()
		{
			Type type = null;
			foreach (UnitCard u in unitCards) {
				if (u.IsSelected()) {
					type = u.Type;
				}
			}

			return type;
		}
		public static bool IsMouseInside()
		{
			Vector2 pos = MouseInput.GetMousePosition();
			return pos.Y >= new Vector2(0, Game1.Height - 150).Y;
		}

		public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
		{
			base.Draw(gameTime);

			Vector2 pos = new Vector2(0, Game1.Height - Thickness);
			spriteBatch.Draw(texture, new Rectangle((int)pos.X, (int)pos.Y, (int)Game1.Width, (int)Game1.Height), new Color(0, 0, 0, 100));

			Vector2 cardPos = pos;
			int width = 50;
			float scale = 0.3f;
			/*foreach (Texture2D t in unitCards) {
				spriteBatch.Draw(t, cardPos, null, Color.White, 0, Vector2.Zero, new Vector2(scale), SpriteEffects.None, 0);
				cardPos += new Vector2(t.Width * scale + width, 0);
			}*/
			foreach (UnitCard u in unitCards) {
				u.Draw(gameTime);
			}
		}

		public UnitPanel()
		{
			texture = content.Load<Texture2D>("Textures\\whiteBoard");
			textures = new Texture2D[] {
				content.Load<Texture2D>("Textures\\UI\\ui_deepsapce"),
				content.Load<Texture2D>("Textures\\UI\\ui_tdrs")
			};

			Thickness = 150;
			Vector2 pos = new Vector2(0, Game1.Height - Thickness);
			UIPosition = pos;
			unitCards = new UnitCard[] {
				new UnitCard(typeof(ArmedSatellite), textures[0], pos, 0.3f),
				new UnitCard(typeof(ArmedSatellite), textures[1], pos + new Vector2(textures[0].Width * 0.3f + 50, 0), 0.3f),
			};
			
		}
	}
}
