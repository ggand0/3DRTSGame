using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _3DRTSGame
{
	public class CircleRenderer
	{
		static readonly int DEGREE_INCREMENT = 5;

		//List of vertices
		static List<VertexPositionColor> _Vertices;

		//Convert to array to avoid ToArray for every draw
		static VertexPositionColor[] _VertexArray;

		//Color of the vertices
		static Color _VertexColor;

		static BasicEffect _Effect;

		//Transforms for the circle
		static Matrix _Transforms;
		static Vector3 _Position, _Scale, _Rotation;

		//Radius of the circle
		static float _Radius;

		public static Vector3 Position
		{
			get { return _Position; }
			set { _Position = value; Transform(); }
		}
		public static Vector3 Rotation
		{
			get { return _Rotation; }
			set { _Rotation = value; Transform(); }
		}
		public static Vector3 Scale
		{
			get { return _Scale; }
			set { _Scale = value; Transform(); }
		}
		public static Matrix Transforms
		{
			get { return _Transforms; }
			set { _Transforms = value; }
		}
		public CircleRenderer(GraphicsDevice graphicsDevice, Color color, float radius)
		{
			_Effect = new BasicEffect(graphicsDevice);
			_Effect.VertexColorEnabled = true;
			_VertexColor = color;
			_Transforms = Matrix.Identity;
			_Radius = radius;
			_Scale = Vector3.One;

			_Vertices = new List<VertexPositionColor>();
			Setup();

			_VertexArray = _Vertices.ToArray();
		}/**/

		private static void Setup()
		{
			//For each number of points : 360/ DEGREE_INCREMENT
			for (int i = 0; i <= 360; i += DEGREE_INCREMENT) {
				VertexPositionColor tmp = new VertexPositionColor();

				// X => R * Cos(Angle), Y = 0, Z => R * Sin(Angle)
				tmp.Position = new Vector3(_Radius * (float)Math.Cos(MathHelper.ToRadians(i)), 0, _Radius * (float)Math.Sin(MathHelper.ToRadians(i)));
				tmp.Color = _VertexColor;
				_Vertices.Add(tmp);
			}
		}
		/// <summary>
		/// Update transform when pos, rot, scale is changed
		/// </summary>
		private static void Transform()
		{
			_Transforms = Matrix.CreateScale(_Scale) *
			Matrix.CreateFromYawPitchRoll(_Rotation.Y, _Rotation.X, _Rotation.Z) *
			Matrix.CreateTranslation(_Position);
		}

		public void Draw(GraphicsDevice graphicsDevice, Matrix view, Matrix projection, Matrix transforms, Color color, float radius)
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
			_VertexColor = color;
			_Radius = radius;*/


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
