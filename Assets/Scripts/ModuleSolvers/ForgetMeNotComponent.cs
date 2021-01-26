using System;
using System.Linq;
using KModkit;
using System.Collections.Generic;
using UnityEngine;

namespace ForgetsUltimateShowdownModule
{
	public class ForgetMeNotComponent : IFUSComponentSolver
	{
		private FUSLogger _logger;
		public string Encrypt(KMBombInfo bombInfo, ComponentInfo componentInfo, NumberInfo numberInfo)
		{
			var answer = new List<int>();
			var number = numberInfo.Number;
			_logger = numberInfo.Logger;
			_logger.LogMessage("------Start of Forget Me Not------");
			for (var i = 0; i < number.Length; i++)
			{
				var nr = int.Parse(number[i].ToString());
				switch (i)
				{
					case 0:
						if (bombInfo.IsIndicatorOff(Indicator.CAR))
						{
							_logger.LogMessage("Number {0}, rule 1 applies.", i + 1);
							answer.Add(Sum(nr, 2));
						}
						else if(bombInfo.GetOffIndicators().Count() > bombInfo.GetOnIndicators().Count())
						{
							_logger.LogMessage("Number {0}, rule 2 applies.", i + 1);
							answer.Add(Sum(nr, 7));
						}
						else if(!bombInfo.GetOffIndicators().Any())
						{
							_logger.LogMessage("Number {0}, rule 3 applies.", i + 1);
							answer.Add(Sum(nr, bombInfo.GetOnIndicators().Count()));
						}
						else
						{
							_logger.LogMessage("Number {0}, the otherwise rule applies.", i + 1);
							answer.Add(Sum(nr, bombInfo.GetSerialNumberNumbers().Last()));
						}
						break;
					case 1:
						if (bombInfo.IsPortPresent(Port.Serial) && bombInfo.GetSerialNumberNumbers().Count() >= 3)
						{
							_logger.LogMessage("Number {0}, rule 1 applies.", i + 1);
							answer.Add(Sum(nr, 3));
						}
						else if(int.Parse(answer[0].ToString()) % 2 == 0)
						{
							_logger.LogMessage("Number {0}, rule 2 applies.", i + 1);
							answer.Add(Sum(nr, (int.Parse(answer[0].ToString()) + 1)));
						}
						else
						{
							_logger.LogMessage("Number {0}, the otherwise rule applies.", i + 1);
							answer.Add(Sum(nr, (int.Parse(answer[0].ToString()) - 1)));
						}
						break;
					default:
						var prevNr = new Pair<int, int>(int.Parse(answer[i - 1].ToString()), int.Parse(answer[i - 2].ToString()));
						if (new[]{prevNr.Item1, prevNr.Item2}.Contains(0))
						{
							_logger.LogMessage("Number {0}, rule 1 applies.", i + 1);
							answer.Add(Sum(nr, bombInfo.GetSerialNumberNumbers().Max()));
						}
						else if ((prevNr.Item1 % 2 == 0) && (prevNr.Item2 % 2 == 0))
						{
							_logger.LogMessage("Number {0}, rule 2 applies.", i + 1);
							answer.Add(Sum(nr, bombInfo.GetSerialNumberNumbers().Any(x => x % 2 != 0)
								? bombInfo.GetSerialNumberNumbers().Where(x => x % 2 != 0).Min()
								: 9));
						}
						else
						{
							_logger.LogMessage("Number {0}, the otherwise rule applies.", i + 1);
							answer.Add(Sum(nr, (prevNr.Item1 + prevNr.Item2) > 9 ? (prevNr.Item1 + prevNr.Item2) / 10: prevNr.Item1 + prevNr.Item2));
						}
						break;
				}
			}

			var answerToReturn = answer.Join("");
			_logger.LogMessage("The answer for Forget Me Not is: {0}", answerToReturn);
			return answerToReturn;
		}

		private static int Sum(int number, int add)
		{
			return (number + add) % 10;
		}

		public string Name
		{
			get
			{
				return "Forget Me Not";
			}
		}
		
		public MethodId Id
		{
			get
			{
				return MethodId.ForgetMeNot;
			}
		}
	}
}