using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace _3DRTSGame
{
	public class UnitPanel : UIObject
	{
		private static readonly int Thickness = 150;
		private static readonly Vector2 ICON_REGION_POS = new Vector2(0, Game1.Height / 2f);
		private static readonly Vector2 CARD_REGION_POS = new Vector2(Game1.Width / 2f, Game1.Height - Thickness);
		/// <summary>
		/// 予め測った値
		/// </summary>
		private static readonly float MENU_FONT_SIZE = 28;
		//public int Thickness { get; private set; }

		private int height;

		private Texture2D[] cardTextures, iconTextures;// 位置計算のために用意
		private UnitCard[] unitCards;
		private UIButton[] icons;

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
			return pos.Y >= new Vector2(0, Game1.Height - Thickness).Y;
		}

		private void DrawStrings()
		{
			Vector2 pos = new Vector2(0, Game1.Height - Thickness);
			spriteBatch.DrawString(game.menuFont, "ORDER", pos, Color.White);
			pos = new Vector2(Game1.Width/2f, Game1.Height - Thickness);
			spriteBatch.DrawString(game.menuFont, "UNIT", pos, Color.White);
			//float d = game.menuFont.MeasureString("A").Y;
		}
		private void DrawIcons(GameTime gameTime)
		{
			foreach (UIButton u in icons) {
				u.Draw(gameTime);
			}
		}
		private void DrawUnitCards(GameTime gameTime)
		{
			Vector2 cardPos = CARD_REGION_POS;
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
		public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
		{
			base.Draw(gameTime);

			Vector2 pos = new Vector2(0, Game1.Height - Thickness);
			spriteBatch.Draw(texture, new Rectangle((int)pos.X, (int)pos.Y, (int)Game1.Width, (int)Game1.Height), new Color(0, 0, 0, 100));

			DrawStrings();
			DrawUnitCards(gameTime);
			DrawIcons(gameTime);
		}

		public UnitPanel()
		{
			texture = content.Load<Texture2D>("Textures\\whiteBoard");
			iconTextures = new Texture2D[] {
				content.Load<Texture2D>("Textures\\UI\\Icons\\halt"),
				content.Load<Texture2D>("Textures\\UI\\Icons\\move"),
				content.Load<Texture2D>("Textures\\UI\\Icons\\ccw"),
				content.Load<Texture2D>("Textures\\UI\\Icons\\cw"),
				content.Load<Texture2D>("Textures\\UI\\Icons\\laser"),
				content.Load<Texture2D>("Textures\\UI\\Icons\\rocket1")
			};
			cardTextures = new Texture2D[] {
				//content.Load<Texture2D>("Textures\\UI\\ui_deepsapce"),
				//content.Load<Texture2D>("Textures\\UI\\ui_tdrs")
				content.Load<Texture2D>("Textures\\UI\\Icons\\sattelite_l"),
				content.Load<Texture2D>("Textures\\UI\\Icons\\sattelite_m"),
				content.Load<Texture2D>("Textures\\UI\\Icons\\iss"),
				content.Load<Texture2D>("Textures\\UI\\Icons\\world"),
			};

			//Thickness = 150;
			/*
			UIPosition = pos;
			unitCards = new UnitCard[] {
				new UnitCard(typeof(ArmedSatellite), cardTextures[0], pos, 0.3f),
				new UnitCard(typeof(ArmedSatellite), cardTextures[1], pos + new Vector2(cardTextures[0].Width * 0.3f + 50, 0), 0.3f),
			};*/
			//Vector2 pos = new Vector2(Game1.Width / 2f, Game1.Height - Thickness + game.menuFont.MeasureString("A").Y);
			Vector2 pos = new Vector2(Game1.Width / 2f, Game1.Height - Thickness + MENU_FONT_SIZE);
			unitCards = new UnitCard[cardTextures.Length];
			Type[] types = new Type[] { typeof(ArmedSatellite), typeof(ArmedSatellite), typeof(ArmedSatellite), typeof(ArmedSatellite) };
			float totalLength = 0;
			float scale = 0.8f;
			float dis = 25;
			
			/*unitCards = new UnitCard[] {
				new UnitCard(typeof(ArmedSatellite), cardTextures[0], pos, scale),
				new UnitCard(typeof(ArmedSatellite), cardTextures[1], pos + new Vector2(cardTextures[0].Width * scale + dis, 0), scale),
			};*/
			for (int i = 0; i < cardTextures.Length; i++) {
				unitCards[i] = new UnitCard(types[i], cardTextures[i], pos + new Vector2(totalLength * scale + dis * i, 0), scale);
				totalLength += cardTextures[i].Width;
			}

			pos = new Vector2(0, Game1.Height - Thickness + MENU_FONT_SIZE);
			icons = new UIButton[iconTextures.Length];
			totalLength = 0;
			scale = 0.5f;
			for (int i = 0; i < iconTextures.Length; i++) {
				icons[i] = new UIButton(iconTextures[i], pos + new Vector2(totalLength, 0), scale);
				totalLength += iconTextures[i].Width * scale;
			}

			// もう使わないのでDispose
			//for (int i = 0; i < iconTextures.Length; i++) iconTextures[i].Dispose();
			//for (int i = 0; i < cardTextures.Length; i++) cardTextures[i].Dispose();
		}
	}
}
