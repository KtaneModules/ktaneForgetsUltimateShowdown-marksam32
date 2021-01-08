using UnityEngine;

namespace ForgetsUltimateShowdownModule
{
	public class FUSLogger	
	{
		private int ModuleId { get; set; }

		public FUSLogger(int moduleId)
		{
			ModuleId = moduleId;
		}

		public void LogMessage(string message, params object[] parameters)
		{
			Debug.LogFormat("[Forget's Ultimate Showdown #{0}] {1}", ModuleId, string.Format(message, parameters));
		}
	}

}