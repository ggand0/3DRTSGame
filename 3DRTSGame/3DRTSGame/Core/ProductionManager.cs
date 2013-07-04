using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace _3DRTSGame
{
	/// <summary>
	/// イメージとしてはPlayerがProductionManagerを利用してオブジェクトを買うような構造
	/// </summary>
	public class ProductionManager
	{
		private static float DEF_SPAWN_HEIGHT = 0;

		private Level4 level;
		private Player player;

		private Type currentSelection;
		private bool isDragging;
		public UIManager UIManager { get; private set; }
		public enum SelectionState
		{
			Selected,
			GivingPosition,
			None
		}
		private SelectionState state;
		private Dictionary<Type, Object> uiModels;
		private Object currentUIModel;
		private BillboardSystem uiRing;
		//private Satellite tmpSatellite;

		private Dictionary<string, int> unitCosts;

		/*private Type GetUnitType()
		{
			return null;
		}*/
		private Vector3 GetRayPlaneIntersectionPoint(Ray ray, Plane plane)//Nullable<Vector3>
		{
			float? distance = ray.Intersects(plane);
			//return distance.HasValue ? ray.Position + ray.Direction * distance.Value : null;
			return distance.HasValue ? ray.Position + ray.Direction * distance.Value : Vector3.Zero;
		}
		private bool CanDeploy()
		{
			//Vector2 pos = MouseInput.GetMousePosition();
			//return pos.Y <= Position
			return !UnitPanel.IsMouseInside();
		}
		private Vector3 Get3DMousePoint()
		{
			Vector2 mousePos = MouseInput.GetMousePosition();

			// rayとplaneのintersection pointを計算する
			Vector3 nearsource = new Vector3((float)mousePos.X, (float)mousePos.Y, 0f);
			Vector3 farsource = new Vector3((float)mousePos.X, (float)mousePos.Y, 1f);

			Matrix world = Matrix.CreateTranslation(0, 0, 0);
			Vector3 nearPoint = Level.graphicsDevice.Viewport.Unproject(nearsource, level.camera.Projection, level.camera.View, world);
			Vector3 farPoint = Level.graphicsDevice.Viewport.Unproject(farsource, level.camera.Projection, level.camera.View, world);
			// Create a ray from the near clip plane to the far clip plane.
			Vector3 direction = farPoint - nearPoint;
			direction.Normalize();
			Ray pickRay = new Ray(nearPoint, direction);
			//Plane planeXZ = new Plane(Vector3.Up, 0);
			Plane planeGround = new Plane(Vector3.Up, -DEF_SPAWN_HEIGHT);

			return GetRayPlaneIntersectionPoint(pickRay, planeGround);
		}
		private void HandleInput()
		{
			// unit card上で左クリックしたら
			if (currentSelection != null && MouseInput.IsOnButtonDownL()) {
				//currentSelection = UIManager.GetUnitType();
				isDragging = true;
				
				//object[] args = new object[] { SatelliteWeapon.Missile, level.waterPlanet.Position + new Vector3(400, 50, 0),
				//waterPlanet.Position, 10f, "Models\\Dawn", "SoundEffects\\missile0" };
				// キャストしたらダメな気がするが...モデル表示するだけだからいいか
				// メチャクチャ重いのでこれもpreloadさせることに
				//object[] args = new object[] { Vector3.Zero, 10, "Models\\DeepSpace" };
				//currentUIModel = (Object)Activator.CreateInstance(currentSelection, args);
				currentUIModel = uiModels[currentSelection];

			}
			if (isDragging) {// && !UnitPanel.IsMouseInside()) {
				currentUIModel.Position = Get3DMousePoint();
				currentUIModel.World = Matrix.CreateScale(currentUIModel.Scale) * Matrix.CreateTranslation(currentUIModel.Position);
				//uiModel.Update(gameTime);
			}
			if (isDragging && MouseInput.IsOnButtonUpL() && CanDeploy()) {
				isDragging = false;

				// 3D空間上の位置をGet
				Vector3 intersectionPoint = currentUIModel.Position;//Get3DMousePoint();

				//level.Models.Add(new ArmedSatellite(intersectionPoint, 10, "Models\\DeepSpace"));
				//level.Models[level.Models.Count - 1].RenderBoudingSphere = false;
				Satellite tmpSatellite = ObjectPool.SatellitePool.Dequeue();
				//tmpSatellite.Position = intersectionPoint;
				tmpSatellite.Initialize(intersectionPoint, level.TargetPlanets[0].Position);// いずれ任意の惑星に。
				level.Models.Add(tmpSatellite);
				level.Satellites.Add(tmpSatellite);

				player.UseMoney(tmpSatellite);
			}
		}
		public void Update(GameTime gameTime)
		{
			currentSelection = UIManager.GetUnitType();
			if (currentSelection != null) {
				string d = "ok";
			}

			HandleInput();
		}
		public void Draw(GameTime gameTime)
		{
			if (isDragging && !UnitPanel.IsMouseInside()) {
				Camera c = level.camera;
				currentUIModel.Draw(c.View, c.Projection, c.Position);

				//uiRing.Position = uiModel.Position;
				uiRing.SetPosition(currentUIModel.Position);
				uiRing.Draw(c.View, c.Projection, Vector3.UnitX, Vector3.UnitZ);
			}
		}

		public ProductionManager(Level4 level)
		{
			this.level = level;
			this.player = level.player;

			uiRing = new BillboardSystem(Level.graphicsDevice, Level.content, Level.content.Load<Texture2D>("Textures\\UI\\GlowRing2"),
						new Vector2(512), new Vector3[] {Vector3.Zero });//currentUIModel.Position
			uiModels = new Dictionary<Type, Object>();
			uiModels.Add(typeof(ArmedSatellite), new ArmedSatellite(Vector3.Zero, 10, "Models\\DeepSpace"));

		}
		// 使ってない
		public ProductionManager(Level4 level, UIManager uiManager)
		{
			this.level = level;
			this.player = level.player;
			this.UIManager = uiManager;
			uiRing = new BillboardSystem(Level.graphicsDevice, Level.content, Level.content.Load<Texture2D>("Textures\\UI\\GlowRing2"),
				Vector2.One, new Vector3[] { currentUIModel.Position });

			unitCosts = new Dictionary<string, int>();
			unitCosts.Add("ArmedSatellite", 100);
			unitCosts.Add("SpaceStation", 500);
		}
	}
}
