﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace _3DRTSGame
{
	public interface IBullet
	{
		bool IsActiveNow();
		IFF Identify();
		void Draw(Matrix View, Matrix Projection, Matrix CameraPosition);
		void Update(GameTime gameTime);
	}
}
