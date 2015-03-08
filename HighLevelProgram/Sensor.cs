
namespace iRobotGUI
{
	public static class Sensor
	{
		#region iRobot Open API defined
		public const int SenBumpDrop = 0;
		public const int SenWall     = 1;
		public const int SenCliffL   = 2;
		public const int SenCliffFL  = 3;
		public const int SenCliffFR  = 4;
		public const int SenCliffR   = 5;
		public const int SenVWall    = 6;
		public const int SenOverC    = 7;
		public const int SenDirtL    = 8;
		public const int SenDirtR    = 9;
		public const int SenRemote   = 10;
		public const int SenButton   = 11;
		public const int SenDist1    = 12;
		public const int SenDist0    = 13;
		public const int SenAng1     = 14;
		public const int SenAng0     = 15;
		public const int SenState    = 16;
		public const int SenVolt1    = 17;
		public const int SenVolt0    = 18;
		public const int SenCurr1    = 19;
		public const int SenCurr0    = 20;
		public const int SenTemp     = 21;
		public const int SenCharge1  = 22;
		public const int SenCharge0  = 23;
		public const int SenCap1     = 24;
		public const int SenCap0     = 25;

		#endregion

		#region Self-defined
		// These are calculate by C

		// distance += (int)((sensors[SenDist1] << 8) | sensors[SenDist0]);
		// angle += (int)((sensors[SenAng1] << 8) | sensors[SenAng0]);
		public const int distance = 101;
		public const int angle    = 102;
		#endregion
	}
}
