﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace _3DRTSGame
{
	public class ProductionManager
	{
		private Level4 level;

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
		private Object uiModel;
		private BillboardSystem uiRing;

		/*private Type GetUnitType()
		{
			return null;
		}*/
		Vector3 GetRayPlaneIntersectionPoint(Ray ray, Plane plane)//Nullable<Vector3>
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
			Plane planeXZ = new Plane(Vector3.Up, 0);
			float spawnHeight = -200;
			Plane planeGround = new Plane(Vector3.Up, -spawnHeight);

			return GetRayPlaneIntersectionPoint(pickRay, planeGround);
		}
		private void HandleInput()
		{
			// unit card上で左クリックしたら
			if (currentSelection != null && MouseInput.IsOnButtonDownL()) {
				//currentSelection = UIManager.GetUnitType();
				isDragging = true;
				
				//object[] args = new object[] { SatelliteWeapon.Missile, level.waterPlanet.Position + new Vector3(400, 50, 0), waterPlanet.Position, 10f, "Models\\Dawn", "SoundEffects\\missile0" };
				object[] args = new object[] { Vector3.Zero, 10, "Models\\DeepSpace" };
				uiModel = (Object)Activator.CreateInstance(currentSelection, args);// キャストしたらダメな気がするが...モデル表示するだけだからいいか
				if (uiRing == null) {
					uiRing = new BillboardSystem(Level.graphicsDevice, Level.content, Level.content.Load<Texture2D>("Textures\\UI\\GlowRing2"),
						new Vector2(512), new Vector3[] { uiModel.Position });
				}
			}
			if (isDragging && MouseInput.IsOnButtonUpL() && CanDeploy()) {
				isDragging = false;

				// 3D空間上の位置をGet
				Vector3 intersectionPoint = uiModel.Position;//Get3DMousePoint();

				level.Models.Add(new ArmedSatellite(intersectionPoint, 10, "Models\\DeepSpace"));
				//level.Models.Add(new Object(intersectionPoint, 20, "Models\\cube"));
				level.Models[level.Models.Count - 1].RenderBoudingSphere = false;
				//level.Models.Add(new ArmedSatellite(MouseInput.GetMousePosition(), 5, ""));
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
			if (isDragging) {
				uiModel.Position = Get3DMousePoint();
				uiModel.World = Matrix.CreateScale(uiModel.Scale) * Matrix.CreateTranslation(uiModel.Position);
				//uiModel.Update(gameTime);

				Camera c = level.camera;
				uiModel.Draw(c.View, c.Projection, c.Position);

				//uiRing.Position = uiModel.Position;
				uiRing.SetPosition(uiModel.Position);
				uiRing.Draw(c.View, c.Projection, Vector3.UnitX, Vector3.UnitZ);
			}
		}

		public ProductionManager(Level4 level)
		{
			this.level = level;
			
		}
		// 使ってない
		public ProductionManager(Level4 level, UIManager uiManager)
		{
			this.level = level;
			this.UIManager = uiManager;
			uiRing = new BillboardSystem(Level.graphicsDevice, Level.content, Level.content.Load<Texture2D>("Textures\\UI\\GlowRing2"), Vector2.One, new Vector3[] { uiModel.Position });
		}
	}
}