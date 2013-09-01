using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace _3DRTSGame
{
	public enum IFF
	{
		Friend,
		Foe
	}

	/// <summary>
	/// やはりこの基底クラスを作らざるをえない
	/// </summary>
	public class Bullet : Drawable
	{
		protected static float MAX_DISTANCE = 10000;

		/// <summary>
		/// 敵弾か味方弾かの情報
		/// </summary>
		public IFF Identification { get; private set; }
		public float Speed { get; private set; }
		public Vector3 Direction { get; protected set; }
		public Vector3 StartPosition { get; protected set; }
		//public static Dictionary<Type, int> DamageValue { get; protected set; }
        public static Dictionary<Type, int> DamageValueFriend { get; protected set; }
        public static Dictionary<Type, int> DamageValueFoe { get; protected set; }
		protected float distanceTravelled;


		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
		}
		public virtual bool IsActiveNow()
		{
			return false;
		}

		public Bullet(IFF identification, Vector3 position, Vector3 direction, float speed)
		{
			this.Identification = identification;
			this.StartPosition = position;
			this.Direction = direction;
			this.Speed = speed;
		}
		static Bullet()
		{
			// ダメージ表の初期化
			DamageValueFriend = new Dictionary<Type, int>();
			DamageValueFriend.Add(typeof(LaserBillboardBullet), 5);
            DamageValueFriend.Add(typeof(Missile), 10);/**/

            DamageValueFoe = new Dictionary<Type, int>();
            DamageValueFoe.Add(typeof(LaserBillboardBullet), 1);
            DamageValueFoe.Add(typeof(Missile), 5);

            /*DamageValue = new Dictionary<string, int>();
            DamageValue.Add("FriendlyLaserBolt", 5);
            DamageValue.Add("FoeLaserBolt", 1);
            DamageValue.Add("Missile", 10);*/
		}
	}
}
