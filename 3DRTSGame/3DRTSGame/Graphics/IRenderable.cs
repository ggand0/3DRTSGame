using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace _3DRTSGame
{
	public interface IRenderable
	{
		void Draw(Matrix View, Matrix Projection, Vector3 CameraPosition);
		void SetClipPlane(Vector4? Plane);
	}
}
