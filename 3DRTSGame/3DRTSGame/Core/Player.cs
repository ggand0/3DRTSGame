using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace _3DRTSGame
{
	public class Player
	{
		private enum MoveOrderState
		{
			NotOrdering,
			SettingPosition,
			SettingHeight,
			Done
		}
		private MoveOrderState currentMoveOrderState = MoveOrderState.NotOrdering;
		private Vector3 currentDestination;

		private static readonly int DEF_MONEY = 1000;
		public int Cregit { get; set; }
		public List<Object> Units { get; private set; }

		private Level level;
		private UnitPanel unitPanel;
		//private Object currentSelection;
		private Satellite currentSelection;
		private bool adjustingUnitHeight;


		/// <summary>
		/// Production Managerより。使い回し。MouseInputとかにおくべきだろうが(ry
		/// </summary>
		/// <param name="ray"></param>
		/// <param name="plane"></param>
		/// <returns></returns>
		private Vector3 GetRayPlaneIntersectionPoint(Ray ray, Plane plane)//Nullable<Vector3>
		{
			float? distance = ray.Intersects(plane);
			//return distance.HasValue ? ray.Position + ray.Direction * distance.Value : null;
			return distance.HasValue ? ray.Position + ray.Direction * distance.Value : Vector3.Zero;
		}
		private Ray GetPickRay()
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
			return pickRay;
		}

		/// <summary>
		/// いずれデリゲートに。
		/// </summary>
		/// <param name="name"></param>
		private void ControlUnit(string name)
		{
			switch (name) {
				default:
					break;
				case "halt":
					(currentSelection as Satellite).Revolution = false;
					break;
				case "ccw":
					(currentSelection as Satellite).Revolution = true;
					(currentSelection as Satellite).RevolutionClockwise = false;
					break;
				case "cw":
					(currentSelection as Satellite).Revolution = true;
					(currentSelection as Satellite).RevolutionClockwise = true;
					break;
				case "move":
					currentMoveOrderState = MoveOrderState.SettingPosition;
					break;
			}
		}
		private void HandleInput()
		{
			if (KeyInput.IsOnKeyDown(Keys.LeftShift)) {
				if (currentMoveOrderState == MoveOrderState.SettingPosition) {
					currentMoveOrderState = MoveOrderState.SettingHeight;
				}
				if (currentSelection != null) {
					adjustingUnitHeight = true;
				}
			} else if (KeyInput.IsOnKeyUp(Keys.LeftShift)) {
				if (currentMoveOrderState == MoveOrderState.SettingHeight) {
					currentMoveOrderState = MoveOrderState.SettingPosition;
				}
			}


			if (currentMoveOrderState == MoveOrderState.SettingPosition) {
				Plane planeXZ = new Plane(Vector3.Up, 0);
				currentDestination = GetRayPlaneIntersectionPoint(GetPickRay(), planeXZ);
			}
			if (adjustingUnitHeight) {
				// 高ささえ求められれば良いから向きはどうでもいいか
				Vector3 planeNormal = Vector3.UnitX;
				Vector3 planePos = new Vector3(currentSelection.Position.X, 0, currentSelection.Position.Z);
				//Plane planeVertical = new Plane(planeNormal, planePos.Length());
				Plane planeVertical = new Plane(planeNormal, -planePos.X);
				float height = GetRayPlaneIntersectionPoint(GetPickRay(), planeVertical).Y;// 交差点の高さが求める高さである
				currentDestination = currentSelection.Position + new Vector3(0, height, 0);
			}
			if (MouseInput.IsOnButtonDownL()) {
				// 移動指示
				if (currentMoveOrderState == MoveOrderState.SettingPosition) {
					currentMoveOrderState = MoveOrderState.NotOrdering;
					currentSelection.StartMove(currentDestination);
				}
				// 選択したユニットの高度設定
				if (adjustingUnitHeight) {
					currentSelection.StartMove(currentDestination);
					adjustingUnitHeight = false;
				}

				if (UnitPanel.IsMouseInside()) {// ボタン選択
					if (unitPanel.CurrentSelectedIcon != "") {// 何か操作アイコンが選択されていたら
						ControlUnit(unitPanel.CurrentSelectedIcon);
					}
				} else {// ユニット選択
					Ray pickRay = GetPickRay();
					bool selectSomething = false;

					foreach (Satellite s in level.Satellites) {
						if (pickRay.Intersects(s.transformedBoundingSphere).HasValue) {
							s.IsSelected = true;
							currentSelection = s;
							selectSomething = true;
							break;
						}
					}

					if (!selectSomething) {// 何もない場所でクリックしたときには全てリセットするようにする
						foreach (Satellite s in level.Satellites) {
							currentSelection = null;
							s.IsSelected = false;
						}
					}
				}
			}
		}
		public void AddMoney(Object target)
		{
			if (target is Asteroid) {
				Cregit += 10;
			} else if (target is Fighter) {
				Cregit += 20;
			}
		}
		public void UseMoney(Object unit)
		{
			if (unit is ArmedSatellite) {
				Cregit -= 500;
			} else {
			}
		}
		public void Update()
		{
			HandleInput();
		}
		public void Draw()
		{
			if (currentSelection != null) {
				switch (currentMoveOrderState) {
					case MoveOrderState.SettingPosition:
						//DebugOverlay.
						CircleRenderer.Draw(Level.graphicsDevice, level.camera.View, level.camera.Projection,
							Matrix.CreateTranslation(currentSelection.Position), Color.White, (currentDestination - currentSelection.Position).Length());
						break;
				}
			}

			if (adjustingUnitHeight) {
				DebugOverlay.Line(currentSelection.Position, currentDestination, Color.White);
			}
		}

		public Player(Level level)
		{
			this.level = level;
			this.unitPanel = UIManager.mainPanel;
			Cregit = DEF_MONEY;
		}
	}
}
