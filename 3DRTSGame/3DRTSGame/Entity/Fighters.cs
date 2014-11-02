using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace _3DRTSGame
{
	public enum ShipFormation
	{
		Random,
		Straight,
	}

    /// <summary>
    /// Fighterの編隊を管理するクラス
    /// </summary>
    public class Fighters : Drawable, ICloneable
    {
        public List<MilitaryShip> Ships { get; private set; }
		public ShipFormation Formation { get; private set; }
		public Vector3 InitialPosition { get; private set; }
		public Vector3 Target { get; private set; }
		
		private Vector3 cohesion, alighnment;
		private List<Vector3> division;// object内に持たせるか迷ってる
		private readonly float radius;

		public override object Clone()
		{
			Fighters cloned = (Fighters)MemberwiseClone();
			if (this.Ships != null) {
				// Listを直接Clone出来ないのでこのような操作を行っている
				cloned.Ships = new List<MilitaryShip>();
				for (int i = 0; i < Ships.Count; i++) {
					cloned.Ships.Add((MilitaryShip)this.Ships[i].Clone());
					//cloned.Ships.Add((Fighter)(Ships[i] as Fighter).Clone());
				}
			}

			return cloned;
		}

		/// <summary>
		/// 編隊に関する変数のUpdateのみ行い、他の更新処理や描画は個々のFighterに委譲する
		/// </summary>
		/// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            /*base.Update(gameTime);

			Vector3 averagePos = Vector3.Zero;
			Vector3 averageVelocity = Vector3.Zero;
			float speed = 1.5f, sepSpeed = 1;

			// 結合：隣接ユニットだけ観察するべきだが、とりあえず編隊全体を見る仕様で。
			BoundingSphere bs = new BoundingSphere(Position, radius / 3f);
			foreach (Fighter f in Ships) {
				BoundingSphere b = new BoundingSphere(f.Position, radius / 3f);
				if (b.Intersects(bs)) {
					averagePos += f.Position;
					averageVelocity += f.Velocity;
				}
			}
			averagePos /= Ships.Count;
			averageVelocity /= Ships.Count;
			foreach (Fighter f in Ships) {
				f.Velocity += Vector3.Normalize(averagePos - f.Position) * speed;
			}

			// 整列
			foreach (Fighter f in Ships) {
				if (averageVelocity != Vector3.Zero) {
					f.Velocity += Vector3.Normalize(averageVelocity) * speed;
				}
			}
			
			// 分離：これは流石に隣接ユニットだけ見る必要がある
			foreach (Fighter f in Ships) {
				foreach (Fighter ff in Ships) {
					if (f.Equals(ff)) continue;
					BoundingSphere bs0 = new BoundingSphere(f.Position, radius / 3f);
					BoundingSphere bs1 = new BoundingSphere(ff.Position, radius / 3f);

					if (bs0.Intersects(bs1)) {
						Vector3 sepVector = f.Position - ff.Position;
						f.Velocity += Vector3.Normalize(sepVector) * sepSpeed;
					}
				}
			}

			foreach (Fighter f in Ships) {
				f.Velocity *= 0.9f;
			}*/
        }

		public void Initialize(Vector3 position, Vector3 target)
		{
			this.InitialPosition = position;
			this.Target = target;
			foreach (Fighter f in Ships) {
				//f.Position = position + new Vector3(Utility.NextDouble(random, -radius, radius));
				//f.Target = target;
				f.Initialize(position + new Vector3(Utility.NextDouble(random, -radius, radius)), target);
			}

			foreach (Fighter f in Ships) {
				level.Enemies.Add(f);
				level.Fighters.Add(f);
				level.Models.Add(f);
			}
			// 自クラスはリストに追加するべきか？
			// Modelsにはどの道addできないので追加する方針の場合は専用リストの用意が必要
			//level.Enemies.Add(this);
			//level.Updaters.Add(this);
		}
		public Fighters(Vector3 position, Vector3 target, int num, ShipFormation formation)
		{
			radius = 100;
			this.InitialPosition = position;
			this.Position = position;
			Ships = new List<MilitaryShip>();
			for (int i = 0; i < num; i++) {
				Vector3 randomFactor = new Vector3(Utility.NextDouble(random, -radius, radius));
				Ships.Add(new Fighter(position + randomFactor, target + randomFactor, 20f, "Models\\fighter0"));
			}
		}
    }
}
