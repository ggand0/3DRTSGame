using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace _3DRTSGame
{
	public class Player
	{
		private static readonly int DEF_MONEY = 1000;
		public int Cregit { get; set; }
		public List<Object> Units { get; private set; }

		private void HandleInput()
		{

		}
		public void AddMoney(Object target)
		{
			if (target is Asteroid) {
				Cregit += 10;
			} else if (target is Fighter) {
				Cregit += 20;
			}
		}
		public void Update()
		{

		}

		public Player()
		{
			Cregit = DEF_MONEY;
		}
	}
}
