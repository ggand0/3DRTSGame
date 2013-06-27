using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _3DRTSGame
{
	/// <summary>
	/// 敵のHPゲージ。DamageablePlanetのUI描画コードと同様
	/// </summary>
	public class UIEnemyGauge : UIObject
	{
		private Texture2D lifeBar, lifeBarBack;
		private BasicEffect basicEffect;

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
		}
		public override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);

			foreach (Object o in level.Enemies) {
				DrawEnemyLifeBar(o, spriteBatch);
			}
		}

		protected void DrawEnemyLifeBar(Object o, SpriteBatch spriteBatch)
		{
			// 以下、具体的数値は全てcamera.FarPlaneDistance = 10000000;時に調整したものであることに注意
			Vector3 transformed = Vector3.Transform(o.Position, level.camera.View);
			float depthRatio = level.camera.FarPlaneDistance / -transformed.Z;
			float lengthRatio = depthRatio / 10000f;
			//float lengthRatio = depthRatio / level.camera.FarPlaneDistance / 1000f;
			float defRatio = lifeBar.Height / (float)lifeBar.Width;
			float scale = 0.5f;

			Viewport viewport = Level.graphicsDevice.Viewport;
			Vector3 v = viewport.Project(o.Position + Vector3.Up * (o.Scale), level.camera.Projection, level.camera.View, Matrix.Identity);
			//Vector2 drawPos = new Vector2(v.X + lengthRatio * 200, v.Y - lengthRatio * 100);
			Vector2 drawPos = new Vector2(v.X + lengthRatio, v.Y - lengthRatio * 50);
			int endOfLifeBarGreen = (int)((o.HitPoint / (float)o.MaxHitPoint) * lifeBar.Width);

			
			Level.graphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
			Level.graphicsDevice.SamplerStates[1] = SamplerState.PointWrap;

			spriteBatch.Draw(lifeBarBack, drawPos, new Rectangle(0, 0, (int)(lifeBarBack.Width * scale), (int)(lifeBarBack.Height * scale)), Color.White,
				0, new Vector2(lifeBar.Width / 2f, lifeBar.Height / 2f),
				new Vector2(lengthRatio, defRatio * lengthRatio), SpriteEffects.None, 0);
			spriteBatch.Draw(lifeBar, drawPos, new Rectangle(0, 0, (int)(endOfLifeBarGreen * scale), (int)(lifeBar.Height * scale)), Color.White,
				0, new Vector2(lifeBar.Width / 2f, lifeBar.Height / 2f),
				new Vector2(lengthRatio, defRatio * (depthRatio) / 10000f), SpriteEffects.None, 0);



			// 指示線は数が多いので描画しない
			/*float leng1 = 50;
			float leng2 = 50;
			
			Matrix projectionMatrix = Matrix.CreateOrthographicOffCenter(0, Level.graphicsDevice.Viewport.Width,
				Level.graphicsDevice.Viewport.Height, 0, 0, 1f);
			/// these wont change, so we can set them now
			/// and not have to set them in every Draw() call
			basicEffect.World = Matrix.Identity;
			basicEffect.View = Matrix.Identity;
			basicEffect.Projection = projectionMatrix;
			basicEffect.VertexColorEnabled = true;
			VertexPositionColor[] vertices = new VertexPositionColor[4];
			vertices[0] = new VertexPositionColor(new Vector3(drawPos.X + (-lifeBar.Width / 2f - 5) * lengthRatio, drawPos.Y, 0), Color.Green);
			vertices[1] = new VertexPositionColor(new Vector3(drawPos.X + (-lifeBar.Width / 2f - 5 - leng1) * lengthRatio, drawPos.Y, 0), Color.Green);
			vertices[2] = new VertexPositionColor(new Vector3(drawPos.X + (-lifeBar.Width / 2f - 5 - leng1) * lengthRatio, drawPos.Y, 0), Color.Green);
			vertices[3] = new VertexPositionColor(new Vector3(drawPos.X + (-lifeBar.Width / 2f - 5 - leng1 - leng2) * lengthRatio, drawPos.Y + leng2 * lengthRatio, 0), Color.Green);

			foreach (EffectPass p in basicEffect.CurrentTechnique.Passes) {
				p.Apply();
				Level.graphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, vertices, 0, 2);
			}*/
		}


		public UIEnemyGauge()
		{
			basicEffect = new BasicEffect(Level.graphicsDevice);
			lifeBar = content.Load<Texture2D>("Textures\\UI\\LifeBar2");
			lifeBarBack = content.Load<Texture2D>("Textures\\UI\\LifeBar2Back");
		}
	}
}
