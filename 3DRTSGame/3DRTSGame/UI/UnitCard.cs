using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace _3DRTSGame
{
	public class UnitCard : UIObject
	{
		//public Type Type { get; private set; }
		public string Type { get; private set; }
		public SatelliteWeapon WeaponType { get; private set; }

		public bool IsSelected()
		{
			Vector2 pos = MouseInput.GetMousePosition();
			return pos.X >= UIPosition.X && pos.X <= UIPosition.X + texture.Width * scale
				&& pos.Y >= UIPosition.Y && pos.Y <= UIPosition.Y + texture.Height * scale;
		}
		/*public Type GetUnitType()
		{
			return Type;
		}*/

		public override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);

			spriteBatch.Draw(texture, UIPosition, null, Color.White, 0, Vector2.Zero, new Vector2(scale), SpriteEffects.None, 0);
		}

		public UnitCard(string uniType, Texture2D texture, Vector2 position, float scale)
			:this(uniType, SatelliteWeapon.LaserBolt, texture, position, scale)
		{
		}
		public UnitCard(string uniType, SatelliteWeapon weaponType, Texture2D texture, Vector2 position, float scale)
		{
			this.Type = uniType;
			this.WeaponType = weaponType;
			this.texture = texture;
			this.UIPosition = position;
			this.scale = scale;
		}
	}
}
