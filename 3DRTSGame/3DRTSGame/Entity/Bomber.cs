using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace _3DRTSGame
{
	public class Bomber : MilitaryShip, IDamageable
	{
        protected override void Move()
        {
            base.Move();
        }

		// Constructor
		public Bomber(Vector3 position, Vector3 target, float scale, string filePath)
			: base(position, target, scale, filePath)
		{
		}
	}
}
