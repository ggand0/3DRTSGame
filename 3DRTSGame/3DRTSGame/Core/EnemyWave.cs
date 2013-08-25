using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _3DRTSGame
{
	public struct EnemyInfo
	{
		public Type EnemyType { get; set; }
		/// <summary>
		/// 何らかの属性を格納するのに使用。たとえばAsteroidの飛来軌道パターンなど。
		/// </summary>
		public string EnemySubType { get; set; }
		public float SpawnProbability { get; set; }
		public int MaxSpawnNumPerRate { get; set; }

		public EnemyInfo(Type type, float prob)
			: this(type, "", prob)
		{
		}
		public EnemyInfo(Type type, string subType, float prob)
			: this(type, subType, prob, 1)
		{
		}
		public EnemyInfo(Type type, string subType, float spawnProbability, int maxSpawnNumPerRate)
			: this()
		{
			EnemySubType = subType;
			EnemyType = type;
			SpawnProbability = spawnProbability;
			MaxSpawnNumPerRate = maxSpawnNumPerRate;
		}
	}
	public struct EnemyWave
	{
		public EnemyInfo[] EnemyData { get; set; }
		public int AILevel { get; set; }
		public int SpawnRate { get; set; }
		public int SpawnNum { get; set; }
		public float MaxDistance { get; set; }
		/// <summary>
		/// Waveが開始されてから、最後の敵ユニットが投入されるまでの時間
		/// </summary>
		public float MaxTimeSec { get; set; }

		public EnemyWave(EnemyInfo[] enemyData, int aiLevel, int spawnRate, int spawnNum, float maxDistance, float maxTimeSec)
			: this()
		{
			EnemyData = enemyData;
			AILevel = aiLevel;
			SpawnRate = spawnRate;
			SpawnNum = spawnNum;
			MaxDistance = maxDistance;
			MaxTimeSec = maxTimeSec;
		}
	}
}
