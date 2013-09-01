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
	/// PlayerがProductionManagerを利用してユニットを買うような構造
	/// </summary>
	public class ProductionManager
	{
		#region Fields & Properties
		private static float DEF_SPAWN_HEIGHT = 0;

		public UIManager UIManager { get; private set; }
		public enum SelectionState
		{
			Selected,
			GivingPosition,
			None
		}

		private Level4 level;
		private Player player;
		private string currentSelection;
		private SatelliteWeapon currrentWeaponTypeSelection;
		private bool isDragging;
		private SelectionState state;
		private Dictionary<string, Object> uiModels;
		private Object currentUIModel;
		private BillboardSystem uiRing;
		private Dictionary<string, int> unitCosts;
		private SatelliteWeapon[] unitWeaponTypes;
#endregion

        /// <summary>
        /// レイと平面の交差点を計算して返すメソッド。
        /// </summary>
        /// <param name="ray"></param>
        /// <param name="plane"></param>
        /// <returns>rayとplaneの交点。無い場合はゼロベクトルを返す。</returns>
		private Vector3 GetRayPlaneIntersectionPoint(Ray ray, Plane plane)
		{
			float? distance = ray.Intersects(plane);
			return distance.HasValue ? ray.Position + ray.Direction * distance.Value : Vector3.Zero;
		}
        /// <summary>
        /// マウスとデフォルト平面との交点を取得する。
        /// </summary>
        /// <returns></returns>
		private Vector3 Get3DMousePoint()
		{
			Vector2 mousePos = MouseInput.GetCurrentMousePosition();

			// rayとplaneのintersection pointを計算する
			Vector3 nearsource = new Vector3((float)mousePos.X, (float)mousePos.Y, 0f);
			Vector3 farsource = new Vector3((float)mousePos.X, (float)mousePos.Y, 1f);
			Matrix world = Matrix.CreateTranslation(0, 0, 0);
			Vector3 nearPoint = Level.graphicsDevice.Viewport.Unproject(nearsource, level.camera.Projection, level.camera.View, world);
			Vector3 farPoint = Level.graphicsDevice.Viewport.Unproject(farsource, level.camera.Projection, level.camera.View, world);

			// Create a ray from the near clip plane to the far clip plane.
			Vector3 direction = Vector3.Normalize(farPoint - nearPoint);
			Ray pickRay = new Ray(nearPoint, direction);
			Plane planeGround = new Plane(Vector3.Up, -DEF_SPAWN_HEIGHT);

			return GetRayPlaneIntersectionPoint(pickRay, planeGround);
		}
        /// <summary>
        /// マウスの位置がUIパネルの外にあるならデプロイ可能と判断する
        /// </summary>
        /// <returns>ユニットを配置可能かどうかを返す</returns>
		private bool CanDeploy()
		{
			return !UnitPanel.IsMouseInside();
		}
		private void HandleInput()
		{
			// unit card上で左クリックしたら
			if (currentSelection != null && currrentWeaponTypeSelection != SatelliteWeapon.None
				&& MouseInput.IsOnButtonDownL()) {
				isDragging = true;


				// キャストしたらダメな気がするが...モデル表示するだけだからいいか
				// メチャクチャ重いのでこれもpre-loadさせることにした
				//object[] args = new object[] { Vector3.Zero, 10, "Models\\DeepSpace" };
				//currentUIModel = (Object)Activator.CreateInstance(currentSelection, args);

				//currentUIModel = uiModels[currentSelection];
				currentUIModel = uiModels[currentSelection];
			}
			if (isDragging) {// && !UnitPanel.IsMouseInside()) {
				currentUIModel.Position = Get3DMousePoint();
				currentUIModel.World = Matrix.CreateScale(currentUIModel.Scale) * Matrix.CreateTranslation(currentUIModel.Position);
				//uiModel.Update(gameTime);
			}

			//if (isDragging && !MouseInput.BUTTONL() && !CanDeploy()) {
			if (isDragging && !MouseInput.IsOnButtonDownL() && !MouseInput.BUTTONL() && !CanDeploy()) {
			//if (isDragging && MouseInput.IsOnButtonUpL() && !CanDeploy()) {
				isDragging = false;
			//} else if (isDragging && !MouseInput.BUTTONL() && CanDeploy()) {
			} else if (isDragging && MouseInput.IsOnButtonUpL() && CanDeploy()) {
				isDragging = false;

				// 3D空間上の位置をGet
				Vector3 intersectionPoint = currentUIModel.Position;//Get3DMousePoint();

				// ObjectPoolからインスタンスを持ってきて、選択されているユニットタイプに応じて初期化
				//level.Models.Add(new ArmedSatellite(intersectionPoint, 10, "Models\\DeepSpace"));
				//level.Models[level.Models.Count - 1].RenderBoudingSphere = false;
				Satellite tmpSatellite = null;// = ObjectPool.SatellitePool.Dequeue();

				// いずれ任意の惑星に。
				//tmpSatellite.Initialize(intersectionPoint, level.TargetPlanets[0].Position);
				/*if (tmpSatellite is ArmedSatellite) {
					(tmpSatellite as ArmedSatellite).Initialize(currentUIModel.Model, intersectionPoint, level.TargetPlanets[0].Position);
					(tmpSatellite as ArmedSatellite).Initialize(currrentWeaponTypeSelection);
				}*/
				switch (currentSelection) {
					case "NormalSatellite":
						tmpSatellite = ObjectPool.SatellitePool.Dequeue();
						tmpSatellite = tmpSatellite as ArmedSatellite;
                        // とりあえず最初の惑星の中心を回らせているだけで、将来的には選択している惑星を対象とするようにする
						(tmpSatellite as ArmedSatellite).Initialize(currentUIModel.Model, intersectionPoint, level.TargetPlanets[0].Position);
						(tmpSatellite as ArmedSatellite).Initialize(currrentWeaponTypeSelection);
						break;
					case "LargeSatellite":
						tmpSatellite = ObjectPool.SatellitePool.Dequeue();
						tmpSatellite = tmpSatellite as ArmedSatellite;
						(tmpSatellite as ArmedSatellite).Initialize(currentUIModel.Model, intersectionPoint, level.TargetPlanets[0].Position);
						(tmpSatellite as ArmedSatellite).Initialize(currrentWeaponTypeSelection);
						break;
					case "SpaceStation":
						//tmpSatellite = tmpSatellite as SpaceStation;
						// 後でcloneに......
						//tmpSatellite = new SpaceStation(intersectionPoint, 100, "Models\\spacestation4");
						tmpSatellite = ObjectPool.SpaceStationPool.Dequeue();
						break;
				}

				level.Models.Add(tmpSatellite);
				level.Satellites.Add(tmpSatellite);

				player.UseMoney(tmpSatellite);

				// 連続クリックで連続spawnしないように初期化しておく
				currentSelection = null;
				currrentWeaponTypeSelection = SatelliteWeapon.None;
			}
		}
		public void Update(GameTime gameTime)
		{
			if (!isDragging && UnitPanel.IsMouseInside()) {
				// この２つを構造体にまとめてもいいな
				currentSelection = UIManager.GetUnitType();
				// 何も選択されていないときは、type=null, weaponType=SatelliteWeapon.Noneが返される
				currrentWeaponTypeSelection = UIManager.GetUnitWeaponType();
			}

			HandleInput();
		}
		public void Draw(GameTime gameTime)
		{
			if (isDragging && !UnitPanel.IsMouseInside()) {
				Camera c = level.camera;
				currentUIModel.Draw(c.View, c.Projection, c.Position);

				uiRing.SetPosition(currentUIModel.Position);
				uiRing.Draw(c.View, c.Projection, Vector3.UnitX, Vector3.UnitZ);
			}
		}

		public ProductionManager(Level4 level)
		{
			this.level = level;
			this.player = level.player;

			uiRing = new BillboardSystem(Level.graphicsDevice, Level.content, Level.content.Load<Texture2D>("Textures\\UI\\GlowRing2"),
						new Vector2(512), new Vector3[] {Vector3.Zero });
			/*uiModels = new Dictionary<Type, Object>();
			uiModels.Add(typeof(ArmedSatellite), new ArmedSatellite(Vector3.Zero, 10, "Models\\DeepSpace"));
			uiModels.Add(typeof(ArmedSatellite), new ArmedSatellite(Vector3.Zero, 10, "Models\\ISS"));*/
			uiModels = new Dictionary<string, Object>();
            uiModels.Add("NormalSatellite", new ArmedSatellite(Vector3.Zero, 10, "Models\\DeepSpace"));
			uiModels.Add("LargeSatellite", new ArmedSatellite(Vector3.Zero, 10, "Models\\ISS"));
			uiModels.Add("SpaceStation", new SpaceStation(Vector3.Zero, 100, "Models\\spacestation4"));

			unitCosts = new Dictionary<string, int>();
			unitCosts.Add("ArmedSatellite", 100);
			unitCosts.Add("SpaceStation", 500);

			unitWeaponTypes = new SatelliteWeapon[] { SatelliteWeapon.LaserBolt, SatelliteWeapon.Missile, SatelliteWeapon.LaserBolt, SatelliteWeapon.LaserBolt };
		}
	}
}
