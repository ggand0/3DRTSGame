using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _3DRTSGame
{
	/// <summary>
	/// Characterにダメージを与えるクラス。DamageManagerで管理される
	/// </summary>
	public class Damage
	{
		private Level level;
		private int count;

		/// <summary>
		/// 何フレーム目でダメージを入れるか(HPを減らすか)
		/// </summary>
		public int DamageFrame { get; private set; }
		/// <summary>
		/// 攻撃したObject
		/// </summary>
		public IAttackable Attacker { get; private set; }
		/// <summary>
		/// 攻撃されたObject
		/// </summary>
		public IDamageable Defender { get; private set; }
		/// <summary>
		/// ダメージ処理を完了したか。DamageManagerでこのフラグが立ったDamageを削除する。
		/// </summary>
		public bool hasDamaged { get; private set; }

		public Damage(Level level, IAttackable attacker, IDamageable defender)
		{
			this.level = level;
			this.Attacker = attacker;
			this.Defender = defender;

			DamageFrame = 0;
		}

		public void Update()
		{
			if (Defender.DamageType[0]) {
				// この方針で書くか、もしくはadObjectじゃなくてadIDamageableにするか。後者のほうが多分綺麗
				//if (adObject.Second is IDamageable && (adObject as IDamageable).damageFromAttacking) {
				if (Attacker.IsAttacking) {
					if (count == DamageFrame) {
						Defender.IsDamaged = true;
						Defender.Damage();
					}

					Defender.IsDamaged = false;                                             // 決まったフレームでのみtrueにする。それ以外ではfalse
					count++;
				} else {
					hasDamaged = true;// Defenderにダメージを与え終えたので終了フラグを立てる。この時点でDefender.IsDamaged==falseなはず

					if (level.DamageManager.Damages.Any((x) => !x.Equals(this) && x.Defender.Equals(this.Defender))) {
					} else {
						Defender.DamageType[0] = false;
					}
					count = 0;
				}
			}
		}
		
	}
}
