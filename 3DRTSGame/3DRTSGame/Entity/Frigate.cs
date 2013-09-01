using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace _3DRTSGame
{
	public class Frigate : MilitaryShip
	{
        public Frigate(Vector3 position, Vector3 target, float scale, string filePath)
			:base(position, target, scale, filePath)
		{
		}
	}
}
