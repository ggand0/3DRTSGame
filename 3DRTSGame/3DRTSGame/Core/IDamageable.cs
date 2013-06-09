using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _3DRTSGame
{
	/// <summary>
	/// ダメージを与えられるオブジェクトのインターフェース
	/// </summary>
	public interface IDamageable
	{
		//bool[] DamageType { get; set; }
		//bool IsDamaged { get; set; }

		void Damage();
	}
}
