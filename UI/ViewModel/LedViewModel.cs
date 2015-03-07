using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRobotGUI.ViewModel
{
	public class LedViewModel : InsViewModelBase
	{
		public LedViewModel()
		{
			Ins = Instruction.CreatFromOpcode(Instruction.LED);
		}		

		public LedViewModel(Instruction ins)
		{
			Ins = ins;
		}

		public int PowerLedColor
		{
			get
			{
				return Ins.parameters[1];
			}
			set
			{
				Ins.parameters[1] = value;
			}
		}

		public int PowerLedIntensity
		{
			get
			{
				return Ins.parameters[2];
			}
			set
			{
				Ins.parameters[2] = value;
			}
		}

		public bool PlayLed
		{
			get
			{
				return (Ins.parameters[0] & 2) != 0;
			}
			set
			{
				Ins.parameters[0] = SetBit(Ins.parameters[0], 1, value);				
			}
		}

		public bool AdvanceLed
		{
			get
			{
				return (Ins.parameters[0] & 8) != 0;
			}
			set
			{
				Ins.parameters[0] = SetBit(Ins.parameters[0], 3, value);

			}
		}

		/// <summary>
		/// Set the specific bit of an integer, begging from the small end.
		/// </summary>
		/// <param name="n">The integer being set. </param>
		/// <param name="index">The index from the small end. </param>
		/// <param name="value">0 or 1 </param>
		/// <returns>The integer after modification. </returns>
		private int SetBit(int n, int index, bool value)
		{
			// clear index-th bit.
			n = n & ~(1 << index);
			n = n | ((value ? 1 : 0) << index);
			return n;
		}


	
	}
}
