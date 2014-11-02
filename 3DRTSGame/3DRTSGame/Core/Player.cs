using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace _3DRTSGame
{
    /// <summary>
    /// 主にCreditとユニット操作・操作に関連するUIの描画を担当するクラス
    /// </summary>
	public class Player
	{
        private static readonly int DEF_MONEY = 1000;
        public static readonly Color UI_CIRCLE_COLOR = Color.LightGreen;
        public static readonly float UI_CIRCLE_RADIUS = 50;

		private enum MoveOrderState
		{
			NotOrdering,
			SettingPosition,
			SettingHeight,
		}
		private MoveOrderState currentMoveOrderState = MoveOrderState.NotOrdering;
		private Vector3 currentDestination;
		private Vector3 currentVector;
        private float initialHeight, currentHeight;

		

		public int Cregit { get; private set; }
		public List<Object> Units { get; private set; }

		private Level level;
		private UnitPanel unitPanel;
		//private Object currentSelection;
		private Satellite currentSelection;
		//private bool adjustingUnitHeight;
		private CircleRenderer circleRenderer = new CircleRenderer(Level.graphicsDevice, Color.White, 0);

		/// <summary>
		/// Production Managerより。使い回し。MouseInputとかにおくべきだろうが(ry
		/// </summary>
		/// <param name="ray"></param>
		/// <param name="plane"></param>
		/// <returns></returns>
		private Vector3 GetRayPlaneIntersectionPoint(Ray ray, Plane plane)
		{
			float? distance = ray.Intersects(plane);
			return distance.HasValue ? ray.Position + ray.Direction * distance.Value : Vector3.Zero;
		}
		private Ray GetPickRay()
		{
			Vector2 mousePos = MouseInput.GetCurrentMousePosition();

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
        /// ユニットのいる点が乗っている、カメラの平面と平行な平面とpickRayとの交点を求めて返す。
        /// </summary>
        /// <returns>pickRayと求める平面との交点</returns>
        private Vector3 GetSelectPositionIntersectionPoint()
        {
            //Vector3 planePos = new Vector3(currentSelection.Position.X, level.camera.Position.Y, currentSelection.Position.Z);
            Vector3 planePos = new Vector3((currentSelection.Position + currentVector).X, level.camera.Position.Y, (currentSelection.Position + currentVector).Z);
            Vector3 planePos0 = new Vector3(currentSelection.Position.X, 0, currentSelection.Position.Z);

            Vector3 planeNormal = planePos - level.camera.Position;                                                             // 同じ高さでのカメラから求める平面方向を向く法線になるはず
            Vector3 planeUnitNormal = Vector3.Normalize(planeNormal);
            float distance = Math.Abs(Vector3.Dot((Vector3.Zero - planePos0), planeNormal)) / (float)planeNormal.Length();      // 原点から平面までのdistanceが必要
            //float distance = Vector3.Dot((level.camera.Position - planePos), planeNormal);

            //Plane planeVertical = new Plane(planeNormal, planePos.Length());
            //Plane planeVertical = new Plane(planeNormal, -planePos.X);
            Plane planeVertical = new Plane(planeNormal, -distance);

            return GetRayPlaneIntersectionPoint(GetPickRay(), planeVertical);
        }
        /// <summary>
        /// マウスのY座標の差分から、高さを調整する
        /// </summary>
        /// <returns></returns>
        private float GetMovedHeight()
        {
            //return -(MouseInput.GetCurrentMousePosition().Y - MouseInput.GetPrevMousePosition().Y);
            return (MouseInput.GetCurrentMousePosition().Y - MouseInput.GetPrevMousePosition().Y) > 0 ? -10 : 10;
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
					currentSelection.Revolution = false;
					currentSelection.SetState(MovingState.Revolution);
					break;
				case "ccw":
					currentSelection.Revolution = true;
					currentSelection.RevolutionClockwise = false;
					currentSelection.SetState(MovingState.Revolution);
					break;
				case "cw":
					currentSelection.Revolution = true;
					currentSelection.RevolutionClockwise = true;
					currentSelection.SetState(MovingState.Revolution);
					break;
				case "move":
					currentMoveOrderState = MoveOrderState.SettingPosition;
					break;
				case "laser":
					if (currentSelection is ArmedSatellite) {
						(currentSelection as ArmedSatellite).Initialize(SatelliteWeapon.LaserBolt);
					}
					break;
				case "missile":
					if (currentSelection is ArmedSatellite) {
						(currentSelection as ArmedSatellite).Initialize(SatelliteWeapon.Missile);
					}
					break;
			}
		}
		private void HandleInput()
		{
            // 高度設定モードの切り替え処理
			if (KeyInput.IsOnKeyDown(Keys.LeftShift)) {
				/*if (currentMoveOrderState == MoveOrderState.SettingPosition) {
					currentMoveOrderState = MoveOrderState.SettingHeight;
				}
				if (currentSelection != null) {
					adjustingUnitHeight = true;
				}*/
				if (currentSelection != null) {
					if (currentMoveOrderState == MoveOrderState.SettingPosition) {
						currentVector = currentDestination - currentSelection.Position;
					}

					currentMoveOrderState = MoveOrderState.SettingHeight;
                    // [重要]Shift押下フレームでのpickRayとユニットのいる（垂直な）平面との交点の高さを求めておく
                    initialHeight = GetSelectPositionIntersectionPoint().Y;
				}
			} else if (KeyInput.IsOnKeyUp(Keys.LeftShift)) {
				if (currentMoveOrderState == MoveOrderState.SettingHeight) {
					currentMoveOrderState = MoveOrderState.SettingPosition;
				}
			}

            // 何かユニットが選択されていた場合、移動指示モードへ切り替える
			if (currentSelection != null && MouseInput.IsOnButtonDownR()) {
				currentMoveOrderState = MoveOrderState.SettingPosition;
			}

            // 高度設定モードならそのモードの終了、それ以外はユニット操作モード終了
			if (KeyInput.IsOnKeyDown(Keys.Escape)) {
				if (currentMoveOrderState == MoveOrderState.SettingHeight) {
					currentMoveOrderState = MoveOrderState.SettingPosition;
				} else {
					currentMoveOrderState = MoveOrderState.NotOrdering;
				}
			}

            // UI描画位置の計算:SettingHeight時も計算するようにした
            if (currentMoveOrderState == MoveOrderState.SettingPosition || currentMoveOrderState == MoveOrderState.SettingHeight) {
                // ユニットのいる高さのXZ平面との交点を求める

                Plane planeXZ = new Plane(Vector3.Up, -currentSelection.Position.Y);
                currentDestination = GetRayPlaneIntersectionPoint(GetPickRay(), planeXZ);
            }
            if (currentMoveOrderState == MoveOrderState.SettingHeight) {
                // ユニットのいる位置の、カメラのクリップ面と平行な面との交点を求める
				float height = GetSelectPositionIntersectionPoint().Y;// 交点の高さが求める高さ
                //currentDestination = currentSelection.Position + currentVector + new Vector3(0, height, 0);
                // Shift押した時点でいきなり高さがあるのは嫌なので、押した時点での高度との差分に設定することによって改善
                currentDestination = currentSelection.Position + currentVector + new Vector3(0, (height - initialHeight), 0);

                // マウスのY座標の差分によって、選択しているユニットの高度を調整する
                //currentHeight += GetMovedHeight();
                //currentDestination = currentSelection.Position + currentVector + new Vector3(0, currentHeight, 0);
            } else {
                currentHeight = 0;
            }


            // 左クリック時の入力処理
			if (MouseInput.IsOnButtonDownL()) {
                // 移動指示及び選択したユニットの高度設定
                if (currentSelection != null && currentMoveOrderState == MoveOrderState.SettingPosition) {
					currentMoveOrderState = MoveOrderState.NotOrdering;
					currentSelection.StartMove(currentDestination);
				} else if (currentSelection != null && currentMoveOrderState == MoveOrderState.SettingHeight) {
					currentSelection.StartMove(currentDestination);
					//adjustingUnitHeight = false;
					currentMoveOrderState = MoveOrderState.NotOrdering;
				}



                if (UnitPanel.IsMouseInside()) {// ボタン選択
                    // 何か操作アイコンが選択されていたらそのボタンに該当する処理
					if (unitPanel.CurrentSelectedIcon != "" && currentSelection != null) {
						ControlUnit(unitPanel.CurrentSelectedIcon);
					}
				} else {// ユニットが選択されているかどうかチェック
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

                    // 何もない場所でクリックしたときには全てリセットするようにする
					if (!selectSomething || KeyInput.IsOnKeyDown(Keys.Escape)) {
						foreach (Satellite s in level.Satellites) {
							currentSelection = null;
							s.IsSelected = false;
						}
					}
				}

			}
		}
        /// <summary>
        /// 敵を撃破した場合にCreditを追加する
        /// </summary>
        /// <param name="target"></param>
		public void AddMoney(Object target)
		{
			if (target is Asteroid) {
				Cregit += 10;
			} else if (target is Fighter) {
				Cregit += 20;
			}
		}
        /// <summary>
        /// ユニットを購入した場合にCreditを消費する
        /// </summary>
        /// <param name="unit"></param>
		public void UseMoney(Object unit)
		{
			if (unit is ArmedSatellite) {
				Cregit -= 200;
			} else if (unit is SpaceStation) {
                Cregit -= 500;
			}
		}

		public void Update()
		{
			HandleInput();
		}
        /// <summary>
        /// 主に、ユニットに移動指示を出すときのUIを描画する
        /// </summary>
		public void Draw()
		{
			if (currentSelection != null) {
				switch (currentMoveOrderState) {
					case MoveOrderState.SettingPosition:
						circleRenderer.Draw(Level.graphicsDevice, level.camera.View, level.camera.Projection,
							Matrix.CreateTranslation(currentSelection.Position), UI_CIRCLE_COLOR, (currentDestination - currentSelection.Position).Length());
						circleRenderer.Draw(Level.graphicsDevice, level.camera.View, level.camera.Projection,
							Matrix.CreateTranslation(currentDestination), UI_CIRCLE_COLOR, UI_CIRCLE_RADIUS);
						break;
					case MoveOrderState.SettingHeight:
						circleRenderer.Draw(Level.graphicsDevice, level.camera.View, level.camera.Projection,
							Matrix.CreateTranslation(currentSelection.Position), UI_CIRCLE_COLOR, currentVector.Length());

						circleRenderer.Draw(Level.graphicsDevice, level.camera.View, level.camera.Projection,
							Matrix.CreateTranslation(currentDestination), UI_CIRCLE_COLOR, UI_CIRCLE_RADIUS);
						circleRenderer.Draw(Level.graphicsDevice, level.camera.View, level.camera.Projection,// 地面のposを表す円
							Matrix.CreateTranslation(new Vector3(currentDestination.X, currentSelection.Position.Y, currentDestination.Z)), UI_CIRCLE_COLOR, UI_CIRCLE_RADIUS);

						DebugOverlay.Line(currentSelection.Position, currentSelection.Position + currentVector, UI_CIRCLE_COLOR);
						DebugOverlay.Line(currentSelection.Position + currentVector, currentDestination, UI_CIRCLE_COLOR);
                        /*circleRenderer.Draw(Level.graphicsDevice, level.camera.View, level.camera.Projection,
							Matrix.CreateTranslation(currentSelection.Position), UI_CIRCLE_COLOR, (currentDestination - currentSelection.Position).Length());
						circleRenderer.Draw(Level.graphicsDevice, level.camera.View, level.camera.Projection,
							Matrix.CreateTranslation(currentDestination), UI_CIRCLE_COLOR, UI_CIRCLE_RADIUS);
                        DebugOverlay.Line(new Vector3(currentDestination.X, 0, currentDestination.Z), currentDestination, UI_CIRCLE_COLOR);*/
						break;
				}
			}

			/*if (adjustingUnitHeight) {
				DebugOverlay.Line(currentSelection.Position, currentDestination, Color.White);
			}*/
		}

        // Constructor
		public Player(Level level)
		{
			this.level = level;
			this.unitPanel = UIManager.mainPanel;
			Cregit = DEF_MONEY;
		}
	}
}
