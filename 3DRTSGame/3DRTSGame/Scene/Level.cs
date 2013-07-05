using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace _3DRTSGame
{
	public class Level : Scene
	{
		public static GraphicsDevice graphicsDevice;
		public static bool mute = true;
		public static readonly Vector3 PlayfieldSize = new Vector3(100000);
		private static readonly Vector3 initialCameraPosition = new Vector3(0.0f, 50.0f, 5000.0f);

		public Camera camera { get; protected set; }
		public Player player { get; protected set; }
		protected Debug debug;
		protected EffectManager effectManager;
		protected bool displayGrid;
		protected RenderTarget2D maskLayer;
		protected bool insertLoadingScreen;

		public List<Object> Models { get; protected set; }
		//public List<IRenderable>  { get; protected set; }
		public SkySphere Sky { get; protected set; }

		public Vector3 LightPosition
			= new Vector3(-2000, 0, 2000);
			//= new Vector3(-200, 500, 200);


		// Members for RTS game (used in Level3, 4)
		public List<Bullet> Bullets { get; protected set; }
		public List<Object> Enemies { get; private set; }
		public List<Satellite> Satellites { get; private set; }
		public List<EnergyShieldEffect> transparentEffects { get; set; }

		/*public Queue<ExplosionEffect> SmallExplosionPool { get; private set; }
		public Queue<ExplosionEffect> MidExplosionPool { get; private set; }
		public Queue<ExplosionEffect> BigExplosionPool { get; private set; }
		public Queue<Satellite> SatellitePool { get; private set; }*/

		/// <summary>
		/// GraphicsDeviceのStateをデフォルトの状態に戻す。
		/// spriteBatchがDeviceにした変更を戻すのに使う
		/// </summary>
		public void ResetGraphicDevice()
		{
			graphicsDevice.BlendState = BlendState.Opaque;
			graphicsDevice.DepthStencilState = DepthStencilState.Default;
			graphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
			//device.SamplerStates[0] = SamplerState.LinearWrap;
			graphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
			graphicsDevice.SamplerStates[1] = SamplerState.PointWrap;
			graphicsDevice.SamplerStates[2] = SamplerState.PointWrap;
		}

		protected virtual void Collide()
		{
		}
		protected virtual void Initialize()
		{
			Models = new List<Object>();
			effectManager = new EffectManager();
			//Bullets = new List<Drawable>();
			Bullets = new List<Bullet>();
			Enemies = new List<Object>();
			transparentEffects = new List<EnergyShieldEffect>();
			/*SmallExplosionPool = new Queue<ExplosionEffect>();
			MidExplosionPool = new Queue<ExplosionEffect>();
			BigExplosionPool = new Queue<ExplosionEffect>();
			SatellitePool = new Queue<Satellite>();*/
			Satellites = new List<Satellite>();
			

			EnergyShieldEffect.level = this;
			BoundingSphereRenderer.level = this;
			//Object.level = this;
			Drawable.level = this;
			PrelightingRenderer.level = this;
			Water.level = this;
			GlassEffect.level = this;
			EnergyRingEffect.level = this;
			Planet.level = this;
			Star.level = this;
			UIObject.level = this;
			EffectManager.level = this;
		}
		protected virtual void HandleInput()
		{
			if (JoyStick.IsOnKeyDown(6)) {
				displayGrid = displayGrid ? false : true;
			}
			// Startボタンが押された時にPause Sceneにする
			if (JoyStick.IsOnKeyDown(8)) {
				//PushScene(new PauseMenu(this));
				return;
			}
			if (JoyStick.IsOnKeyDown(9)) {
				//isEndScene = true;
				//game.MoveNextLevel = true;
				PushScene(new PauseScene(this));
			}
		}
		protected virtual bool IsClear()
		{
			return false;
		}
		protected virtual bool IsGameOver()
		{
			return false;
		}
		public virtual void Reset()
		{
		}
		public override void Load()
		{
			maskLayer = new RenderTarget2D(graphicsDevice, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height, false, SurfaceFormat.Color, DepthFormat.Depth24);
		}
		public override void Update(GameTime gameTime)
		{
			HandleInput();

			if (IsClear()) {
				PushScene(new MaskMenuScene(this,
					"Game Cleared !",
					MaskMenuScene.Menu.START_NEW_GAME,
					MaskMenuScene.Menu.EXIT));
			} else if (IsGameOver()) {
				PushScene(new MaskMenuScene(this,
					"Game Over",
					MaskMenuScene.Menu.START_NEW_GAME,
					MaskMenuScene.Menu.EXIT));
			}
		}
		public override void Draw(GameTime gameTime)
		{
			// TODO: ここに描画コードを追加します。
			// Object継承オブジェクトの描画は各Drawで行うことにした。それ以外はDrawModelで描画する。
			//device.Clear(Color.CadetBlue);
			
		}

		public Level(Scene privousScene)
			:base(privousScene)
		{
			Initialize();
			//Load();
			if (!insertLoadingScreen) {
				Load();
			}
		}
		static Level()
		{
			//Sky = new SkySphere(content, graphicsDevice, content.Load<TextureCube>("Textures\\SkyBox\\space4"), 100);// set 11 for debug
		}
	}
}
