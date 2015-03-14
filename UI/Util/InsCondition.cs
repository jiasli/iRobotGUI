
namespace iRobotGUI.Util
{
	/// <summary>
	/// A class for IF and LOOP
	/// </summary>
	public class InsCondition
	{
		public int sensor = 0;

		/// <summary>
		/// = != ...
		/// </summary>
		public int op = 0;

		/// <summary>
		/// The number being compared.
		/// </summary> 
		public int num = 0;

		public InsCondition()
		{

		}

		/// <summary>
		/// Construct a InsCondition from <see cref="Instruction"/>.
		/// </summary>
		/// <param name="conditionIns"></param>
		public InsCondition(Instruction conditionIns)
		{
			string opcode = conditionIns.opcode;
			sensor = conditionIns.paramList[0];
			op = conditionIns.paramList[1];
			num = conditionIns.paramList[2];
		}
	

		

	}

	
}
