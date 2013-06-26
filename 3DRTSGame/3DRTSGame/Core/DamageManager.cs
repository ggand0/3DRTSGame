using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _3DRTSGame
{
	public class DamageManager
	{
		public static Game1 game;
		private Level level;

		public List<Object> Attackers { get; private set; }
		public List<Object> Defender { get; private set; }
		public List<Damage> Damages { get; private set; }
		
		public void Update()
		{
			// ダメージを受けた敵の参照を渡す。参照なのでその後敵が画面外に出ても大丈夫。
			foreach (Pair<IAttackable, IDamageable> p in level.Damages) {
				if (Damages.Count > 0 &&
					Damages.Any((x) => x.Attacker == p.First && x.Defender == p.Second)) {
				} else {
					Damages.Add(new Damage(level, p.First, p.Second));
				}
				
			}

			if (Damages.Count > 0) {
				for (int i = 0; i < Damages.Count; i++) {
					Damages[i].Update();
				}

				/// <summary>
				/// UpdateとRemoveは分ける。
				/// ループ中にコレクションの要素を削除するときは末尾から先頭に向かってループをかければおｋ
				/// </summary>
				/// <see cref="http://www.atmarkit.co.jp/fdotnet/dotnettips/815listremove/listremove.html"/>
				for (int i = Damages.Count - 1; i >= 0; i--) {
					if (Damages[i].hasDamaged) {// ダメージを与え終えた(終了フラグ)
						Damages.RemoveAt(i);
					}
				}
			}
		}


		// Constructor
		public DamageManager(Level level)
		{
			this.level = level;
			Attackers = new List<Object>();
			Damages = new List<Damage>();
		}
	}
}