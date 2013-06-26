using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _3DRTSGame
{
	/// <summary>
	/// 敵のHPゲージ
	/// </summary>
	public class UIEnemyGauge : UIObject
	{
		private Texture2D lifeBar, lifeBarBack;

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
		}
		public override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);
		}

		protected void DrawEnemyLifeBar(Object o, SpriteBatch spriteBatch)
		{
			// 以下、具体的数値は全てcamera.FarPlaneDistance = 10000000;時に調整したものであることに注意
			Vector3 transformed = Vector3.Transform(Position, level.camera.View);
			float depthRatio = level.camera.FarPlaneDistance / -transformed.Z;
			float lengthRatio = depthRatio / 10000f;
			float defRatio = lifeBar.Height / (float)lifeBar.Width;

			Viewport viewport = graphicsDevice.Viewport;
			Vector3 v = viewport.Project(Position + Vector3.Up * (o.Scale), level.camera.Projection, level.camera.View, Matrix.Identity);
			Vector2 drawPos = new Vector2(v.X + lengthRatio * 200, v.Y - lengthRatio * 100);//new Vector2(v.X - lifeBar.Width / 2f, v.Y - 50);
			int endOfLifeBarGreen = (int)((o.HitPoint / (float)o.DEF_HIT_POINT) * lifeBar.Width);

			//graphicsDevice.SetRenderTarget(null);
			spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
			//graphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
			graphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
			graphicsDevice.SamplerStates[1] = SamplerState.PointWrap;
			spriteBatch.Draw(lifeBarBack, drawPos, new Rectangle(0, 0, lifeBarBack.Width, lifeBarBack.Height), Color.White, 0, new Vector2(lifeBar.Width / 2f, lifeBar.Height / 2f),
				new Vector2(lengthRatio, defRatio * lengthRatio), SpriteEffects.None, 0);
			spriteBatch.Draw(lifeBar, drawPos, new Rectangle(0, 0, endOfLifeBarGreen, lifeBar.Height), Color.White, 0, new Vector2(lifeBar.Width / 2f, lifeBar.Height / 2f),
				//new Vector2(1 * (-transformed.Z / level.camera.FarPlaneDistance) * 10000, 0.5f), SpriteEffects.None, 0);
				//new Vector2(1 * (depthRatio) / 10000f, 0.5f), SpriteEffects.None, 0);
				new Vector2(lengthRatio, defRatio * (depthRatio) / 10000f), SpriteEffects.None, 0);//0.8f


			float leng1 = 50;
			float leng2 = 50;
			/*Primitives2D.DrawLine(spriteBatch, new Vector2(drawPos.X - lifeBar.Width / 2f - 5, drawPos.Y)
			 * , new Vector2(drawPos.X - lifeBar.Width / 2f - 5 - leng1, drawPos.Y), Color.Green);
			Primitives2D.DrawLine(spriteBatch, new Vector2(drawPos.X - lifeBar.Width / 2f - 5 - leng1, drawPos.Y)
			 * , new Vector2(drawPos.X - lifeBar.Width / 2f - 5 - leng1 -leng2, drawPos.Y + leng2), Color.Green);*/
			spriteBatch.End();


			// 指示線の描画：DrawPrimitivesを使う
			Matrix projectionMatrix = Matrix.CreateOrthographicOffCenter(0, graphicsDevice.Viewport.Width,
				graphicsDevice.Viewport.Height, 0, 0, 1f);
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
			//int[] indices = new int[] { 0, 1, 1, 2 };

			foreach (EffectPass p in basicEffect.CurrentTechnique.Passes) {
				p.Apply();
				graphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, vertices, 0, 2);
			}
		}


		public UIEnemyGauge()
		{

		}
	}
}
