using System.Collections.Generic;

namespace ForgetsUltimateShowdownModule
{
	public class ComponentInfo
	{
		//FML
		public List<int> Rules { get; set; }
		
		//A>N<D
		public List<ANDLogicGate> LogicGates { get; set; }
		
		//FI
		public Pair<int, int> IgnoredNumbers { get; set; }
		
		//FMW
		public int StartingNumber { get; set; }
		
		//FE
		public List<ForgetEverythingColor> FEColors { get; set; }

		//Stages
		public SimonsStagesColor SimonsStagesColor { get; set; }
		
		//FUN
		public List<int> Positions { get; set; }
	}
}

