using System;
using System.Collections.Generic;
using System.Linq;
using KModkit;

namespace ForgetsUltimateShowdownModule
{
	public class SimonsStagesComponent : IFUSComponentSolver
	{
		private FUSLogger _logger;
		private List<int>[] opposites = new List<int>[2];
		public string Encrypt(KMBombInfo bombInfo, ComponentInfo componentInfo, NumberInfo numberInfo)
		{
			var number = numberInfo.Number;
			_logger = numberInfo.Logger;
			var colorRule = componentInfo.SimonsStagesColor;
			var lastDigit = bombInfo.GetSerialNumberNumbers().Last();
			opposites = new[]
			{
				new List<int>{lastDigit, (lastDigit + 1) % 10, (lastDigit + 2) % 10, (lastDigit + 3) % 10, (lastDigit + 4) % 10},
				new List<int>{(lastDigit + 9) % 10, (lastDigit + 8) % 10, (lastDigit + 7) % 10, (lastDigit + 6) % 10, (lastDigit + 5) % 10}
			};
			_logger.LogMessage("------Start of Simon's Stages------");
			_logger.LogMessage("Opposites:");
			_logger.LogMessage(opposites[0].Join());
			_logger.LogMessage(opposites[1].Join());
			_logger.LogMessage("The {0} rule applies.", colorRule.ToString());
			switch (colorRule)
			{
				case SimonsStagesColor.Red:
					return number;
				case SimonsStagesColor.Blue:
					_logger.LogMessage("Reversing the string.");
					return number.Reverse().Join("");
				case SimonsStagesColor.Pink:
					_logger.LogMessage("Taking the opposites of the string.");
					return number.Select(x => Opposite(int.Parse(x.ToString()))).Join("");
				case SimonsStagesColor.Lime:
					_logger.LogMessage("Reversing the string and taking the opposites.");
					return number.Select(x => Opposite(int.Parse(x.ToString()))).Reverse().Join("");
				case SimonsStagesColor.Cyan:
					_logger.LogMessage("Taking the opposites of the first and last.");
					var cyanAnswer = number.ToArray().Select(x => x.ToString()).ToArray();
					cyanAnswer[0] = Opposite(int.Parse(cyanAnswer[0])).ToString();
					cyanAnswer[11] = Opposite(int.Parse(cyanAnswer[11])).ToString();
					return cyanAnswer.Join("");
				case SimonsStagesColor.White:
					_logger.LogMessage("Taking the opposites of the third and second.");
					var whiteAnswer = number.ToArray().Select(x => x.ToString()).ToArray();
					whiteAnswer[1] = Opposite(int.Parse(whiteAnswer[1])).ToString();
					whiteAnswer[2] = Opposite(int.Parse(whiteAnswer[2])).ToString();
					return whiteAnswer.Join("");
				default:
					throw new InvalidOperationException(string.Format("invalid color rule SS {0}", colorRule));
			}
		}

		private int Opposite(int i)
		{
			var number = int.Parse(i.ToString());
			if (opposites[0].Any(x => x == number))
			{
				return opposites[1][opposites[0].IndexOf(number)];
			}

			return opposites[0][opposites[1].IndexOf(number)];
		}

		public string Name
		{
			get
			{
				return "Simon's Stages";
			}
		}
		
		public MethodId Id
		{
			get
			{
				return MethodId.SimonsStages;
			}
		}
	}
}