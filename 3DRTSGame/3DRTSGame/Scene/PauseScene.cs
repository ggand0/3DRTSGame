using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _3DRTSGame
{
	public class PauseScene : MaskMenuScene
	{
		protected Camera camera;

		public PauseScene(Level privousScene, string title, params Menu[] menu)
			: base(privousScene, title, menu)
		{
			this.camera = privousScene.camera;
			Load();
		}
		public override void Load()
		{
			base.Load();
			mask = content.Load<Texture2D>("Textures\\whiteBoard");
		}

		protected void HandleInput()
		{
            if (JoyStick.IsOnKeyDown(3)) {
                isEndScene = true;
                //if (!game.isMuted) cancel.Play(SoundControl.volumeAll, 0f, 0f);
            }
		}
		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
			HandleInput();
			camera.Update(gameTime);
		}
	}
}
