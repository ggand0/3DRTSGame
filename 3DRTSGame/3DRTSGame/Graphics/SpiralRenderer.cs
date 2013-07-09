using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _3DRTSGame
{
	public class SpiralRenderer
	{
		static readonly int DEGREE_INCREMENT = 1;

		//List of vertices
		List<VertexPositionColor> _Vertices;

		//Convert to array to avoid ToArray for every draw
		VertexPositionColor[] _VertexArray;

		//Color of the vertices
		Color _VertexColor;

		static BasicEffect _Effect;

		//Transforms for the circle
		Matrix _Transforms;
		Vector3 _Position, _Scale, _Rotation;

		public Vector3 Position
		{
			get { return _Position; }
			set { _Position = value; Transform(); }
		}
		public Vector3 Rotation
		{
			get { return _Rotation; }
			set { _Rotation = value; Transform(); }
		}
		public Vector3 Scale
		{
			get { return _Scale; }
			set { _Scale = value; Transform(); }
		}
		public Matrix Transforms
		{
			get { return _Transforms; }
			set { _Transforms = value; }
		}
		public SpiralRenderer(GraphicsDevice graphicsDevice, Color color, float startTheta, float endTheta, Vector3 origin)
			:this(graphicsDevice, color, startTheta, endTheta, origin, Matrix.Identity)
		{
		}
		public SpiralRenderer(GraphicsDevice graphicsDevice, Color color, float startTheta, float endTheta, Vector3 origin, Matrix affineTransform)
		{
			_Effect = new BasicEffect(graphicsDevice);
			_Effect.VertexColorEnabled = true;
			_VertexColor = color;
			_Transforms = Matrix.Identity;
			_Scale = Vector3.One;

			_Vertices = new List<VertexPositionColor>();
			Setup(origin, startTheta, endTheta, affineTransform);

			_VertexArray = _Vertices.ToArray();
		}

		private void Setup(Vector3 origin, float start, float end, Matrix affineTransform)
		{
			float a = 0.15f, b = 0.5f;

			//For each number of points : 360/ DEGREE_INCREMENT
			for (float i = start; i <= end; i += DEGREE_INCREMENT) {
				float rad = MathHelper.ToRadians(i);
				VertexPositionColor tmp = new VertexPositionColor();

				// X => R * Cos(Angle), Y = 0, Z => R * Sin(Angle)
				//tmp.Position = new Vector3(_Radius * (float)Math.Cos(MathHelper.ToRadians(i)), 0, _Radius * (float)Math.Sin(MathHelper.ToRadians(i)));
				Vector2 pos = Utility.CalcLogarithmicSpiral(a, b, MathHelper.ToRadians(i), affineTransform);
				tmp.Position = origin + new Vector3(pos.X, 0, pos.Y);
				

				tmp.Color = _VertexColor;
				_Vertices.Add(tmp);
			}
		}
		/// <summary>
		/// Update transform when pos, rot, scale is changed
		/// </summary>
		private void Transform()
		{
			_Transforms = Matrix.CreateScale(_Scale) *
			Matrix.CreateFromYawPitchRoll(_Rotation.Y, _Rotation.X, _Rotation.Z) *
			Matrix.CreateTranslation(_Position);
		}

		/// <summary>
		/// ここのtransformは位置のtransform
		/// </summary>
		/// <param name="graphicsDevice"></param>
		/// <param name="view"></param>
		/// <param name="projection"></param>
		/// <param name="transforms"></param>
		/// <param name="color"></param>
		/// <param name="radius"></param>
		public void Draw(GraphicsDevice graphicsDevice, Matrix view, Matrix projection, Matrix transforms, Color color)
		{
			/*if (_Effect == null) {
				_Effect = new BasicEffect(graphicsDevice);
				_Effect.VertexColorEnabled = true;
			}

			//_Transforms = Matrix.Identity;
			_Transforms = transforms;
			_Scale = Vector3.One;
			_Vertices = new List<VertexPositionColor>();
			Setup();
			_VertexArray = _Vertices.ToArray();
			_VertexColor = color;*/
			_Transforms = transforms;

			// 動かさない予定なので。
			//_Vertices = new List<VertexPositionColor>();// 初期化しないと塗りつぶすことに
			//Setup(); 

			_VertexArray = _Vertices.ToArray();


			// Tell the device we are sending Vertexpositioncolor elements
			// _Game.GraphicsDevice.VertexDeclaration = new VertexDeclaration(_Game.GraphicsDevice, VertexPositionColor.VertexElements);

			//Set the world, view, projection matrices
			_Effect.World = _Transforms;
			_Effect.View = view;
			_Effect.Projection = projection;
			foreach (EffectPass pass in _Effect.CurrentTechnique.Passes) {
				pass.Apply();

				//Draw primitive: n points => n-1 lines
				graphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, _VertexArray, 0, _Vertices.Count - 1);
			}
		}


	}
}
