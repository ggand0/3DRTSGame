using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace _3DRTSGame.Core
{
	struct Matrix2x2
	{
		/// 
		/// The actual underlying matrix representation.
		/// 
		private Matrix actMatRep;

		/// 
		/// Constructs an instance of the 2x2 matrix
		/// 
		public Matrix2x2(float a11, float a12,
						 float a21, float a22)
		{
			actMatRep = new Matrix(a11, a12, 0f, 0f,
								   a21, a22, 0f, 0f,
								   0f, 0f, 1f, 0f,
								   0f, 0f, 0f, 1f);
		}

		private Matrix2x2(Matrix _actMatRep)
		{
			actMatRep = _actMatRep;
		}

		static public Matrix2x2 operator *(Matrix2x2 A, Matrix2x2 B)
		{
			return new Matrix2x2(A.actMatRep * B.actMatRep);
		}

		public float Determinant()
		{
			//return actMapRep.Determinant();
			return actMatRep.Determinant();
		}

		#region Among Other Things
		//...
		#endregion
	}
}
