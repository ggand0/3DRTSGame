using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace _3DRTSGame
{
	public class MaskMenuScene : Scene
	{
		/// <summary>
		/// マスク中にBGMのボリュームをいくら下げるか。
		/// </summary>
		private static readonly float BGM_FADE_VOLUME = 0.7f;

		/// <summary>
		/// 選択可能なメニューの一覧。
		/// </summary>
		public enum Menu
		{
			BACK_TO_GAME,
			START_NEW_GAME,
			EXIT,
			COUNT   //これはメニューの数をカウントするためのモノなので最後においている。
			//メニューを追加する場合は、COUNTの上にメニューを追加した後、
			//表示するテキストを↓のMENU_TEXTに、動作をMENU_ACTIONSに定義する。
		}
		static string[] MENU_TEXTS = new string[(int)Menu.COUNT]{
            "Back to game",
            "Start new game",
            "Exit"
        };
		static Action<MaskMenuScene>[] MENU_ACTIONS
			= new Action<MaskMenuScene>[(int)Menu.COUNT]{
            (MaskMenuScene m)=>{},//BACK_TO_GAME
            (MaskMenuScene m)=>{//START_NEW_GAME
                m.ownerLevel.Reset(); 
            },
             (MaskMenuScene m)=>{//EXIT
                 Level.game.Exit();
             }
        };

		//データ
		/// <summary>
		/// 選択可能なメニュー。
		/// </summary>
		Menu[] menu;
		/// <summary>
		/// 選択しているメニューのインデックス。
		/// </summary>
		int menuIndex;
		/// <summary>
		/// 選択しているメニュー
		/// </summary>
		Menu selectedMenu { get { return menu[menuIndex]; } }
		/// <summary>
		/// このシーンを作成したレベル。
		/// </summary>
		Level ownerLevel;
		//リソース
		/// <summary>
		/// メニューのテキストを表示開始する場所。
		/// </summary>
		readonly Vector2 TEXT_POSITION;
		readonly Vector2 TITLE_POSITION;
		readonly Texture2D maskTexture;
		readonly SoundEffect selectSound;
		readonly SoundEffect enterSound;
		string title = "";


		//コンストラクタ
		/// <summary>
		/// 好きなMenuを選択可能なシーンを作成する。
		/// </summary>
		/// <param name="owner">このシーンを作成したLevel</param>
		/// <param name="menu">使用したいMenu</param>
		public MaskMenuScene(Level owner, params Menu[] menu)
			: this(owner, "", menu)
		{
		}
		public MaskMenuScene(Level owner, string title, params Menu[] menu)
			: base(owner)
		{
			this.ownerLevel = owner;
			this.menu = menu;
			this.title = title;

			
			TEXT_POSITION = new Vector2(Game1.Width / 2,
				Game1.Height / 2 - game.menuFont.MeasureString("A").Y * menu.Length);
			TITLE_POSITION = new Vector2(Game1.Width / 2 - game.menuFont.MeasureString(title).X / 2f,
				0 + game.menuFont.MeasureString("A").Y * menu.Length);
			//maskTexture = ownerLevel.LoadImage("whiteBoard");
			maskTexture = content.Load<Texture2D>("Textures\\whiteBoard");
			selectSound = content.Load<SoundEffect>("SoundEffects\\Menu\\selectSound");
			enterSound = content.Load<SoundEffect>("SoundEffects\\Menu\\enterSound");
			//ownerLevel.BGM.Volume = 1.0f - BGM_FADE_VOLUME;
		}

		//overrideメソッド
		public override void Load()
		{
			//throw new NotImplementedException();
		}
		public override void Update(GameTime gameTime)
		{
			//メニュー選択
			if (KeyInput.IsOnKeyDown(Keys.Up) && 0 < menuIndex) {
				menuIndex--;
				if (!Game1.mute) selectSound.CreateInstance().Play();
			} else if (KeyInput.IsOnKeyDown(Keys.Down) && menuIndex < menu.Length - 1) {
				menuIndex++;
				if (!Game1.mute) selectSound.CreateInstance().Play();
			}
			//メニュー決定・実行
			if (KeyInput.IsOnKeyUp(Keys.Enter)) {
				if (!Game1.mute) enterSound.CreateInstance().Play();
				//ownerLevel.BGM.Volume = 1.0f;
				this.isEndScene = true;
				MENU_ACTIONS[(int)selectedMenu](this);
			}
		}
		public override void Draw(GameTime gameTime)
		{
			ownerLevel.Draw(gameTime);

			spriteBatch.Begin();
			//マスクを表示
			spriteBatch.Draw(maskTexture, game.GraphicsDevice.Viewport.Bounds, Color.Black * 0.5f);
			if (title != "") {
				spriteBatch.DrawString(game.menuFont, title, TITLE_POSITION, Color.White);
			}

			//メニューのテキストを表示
			Vector2 v = TEXT_POSITION;
			Vector2 origin;
			for (int i = 0; i < menu.Length; i++) {
				origin = game.menuFont.MeasureString(MENU_TEXTS[(int)menu[i]]) / 2;
				spriteBatch.DrawString(game.menuFont, MENU_TEXTS[(int)menu[i]],
					v, (i == menuIndex ? Color.White : Color.Gray),
				   0, origin, 1, SpriteEffects.None, 0);
				//1列分空けて次のメニューを表示
				v.Y += origin.Y * 4;
			}

			spriteBatch.End();
		}
	}
}
