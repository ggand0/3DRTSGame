using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace _3DRTSGame
{
	public static class Utility
	{
		private static Random rand = new Random();

		public static float CalcInitialAngle(Vector3 Position, Vector3 Center, float defAngle)
		{
			float radius = (Position - Center).Length();
			//float revolutionAngle = MathHelper.ToRadians(1);
			Vector3 velocity = new Vector3((float)Math.Cos(defAngle), 0,
					(float)Math.Sin(defAngle));
			Vector3 def = Center + velocity * radius;

			Vector3 v1 = Vector3.Normalize(Center - def);
			Vector3 v2 = Vector3.Normalize(Center - Position);

			float angle = (float)Math.Acos(Vector3.Dot(v1, v2));
			return def.Z > Position.Z ? -angle : angle;// 角度の大きさしか分からないのでこれで調整
		}
		public static Vector2 CalcLogarithmicSpiral(float a, float b, float theta)
		{
			/*//float aa = 0.15f, b = 0.5f, angle = MathHelper.ToRadians(1440);
			Vector2 pos = new Vector2(a * (float)Math.Exp(b * theta) * (float)Math.Cos(theta), 
					a * (float)Math.Exp(b * theta) * (float)Math.Sin(theta));
			return pos;*/
			return Utility.CalcLogarithmicSpiral(a, b, theta, Matrix.Identity);
		}
		public static Vector2 CalcLogarithmicSpiral(float a, float b, float theta, Matrix transform)
		{
			//float aa = 0.15f, b = 0.5f, angle = MathHelper.ToRadians(1440);
			Vector2 pos = new Vector2(a * (float)Math.Exp(b * theta) * (float)Math.Cos(theta),
					a * (float)Math.Exp(b * theta) * (float)Math.Sin(theta));

			return Vector2.Transform(pos, transform);
			//return transform * pos;
		}
		public static Matrix RotationMatrix(float theta)
		{
			throw new NotImplementedException();
			//return Matrix.Identity;
		}

		public static float NextDouble(Random r, double min, double max)
		{
			return (float)(min + r.NextDouble() * (max - min));
		}
		public static Vector3 RandomUnitVectorInPlane(Matrix xform, Vector3 axis)
		{
			xform *= Matrix.CreateFromAxisAngle(axis, (float)NextDouble(rand, 0.0, 360.0));
			Vector3 ruv = xform.Right;
			ruv.Normalize();
			return ruv;
		}

		static Utility()
		{
			
		}
	}
}
