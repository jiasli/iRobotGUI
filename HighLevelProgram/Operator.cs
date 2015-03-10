using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRobotGUI
{
	public static class Operator
	{
		public const int EQUAL                 = 0;
		public const int NOT_EQUAL             = 1;
		public const int GREATER_THAN          = 2;
		public const int GRAETER_THAN_OR_EQUAL = 3;
		public const int LESS_THAN             = 4;
		public const int LESS_THAN_OR_EQUAL    = 5;

		/// <summary>
		/// Get the text symbol of an operator that is used in C.
		/// </summary>
		/// <param name="opeartorNo"></param>
		/// <returns></returns>
		public static string GetOperatorTextSymbol(int opeartorNo)
		{
			switch (opeartorNo)
			{
				case Operator.EQUAL:
					return "==";
				case Operator.NOT_EQUAL:
					return "!=";
				case Operator.GREATER_THAN:
					return ">";
				case Operator.GRAETER_THAN_OR_EQUAL:
					return ">=";
				case Operator.LESS_THAN:
					return "<";
				case Operator.LESS_THAN_OR_EQUAL:
					return "<=";
				default:
					return "";
			}
		}
	}
}
