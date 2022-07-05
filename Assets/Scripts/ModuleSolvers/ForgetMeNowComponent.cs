using System.Linq;
using System;
using KModkit;
using System.Collections.Generic;
using UnityEngine;

namespace ForgetsUltimateShowdownModule
{
	public class ForgetMeNowComponent : IFUSComponentSolver
	{
		private FUSLogger _logger;

		public string Encrypt(KMBombInfo bombInfo, ComponentInfo componentInfo, NumberInfo numberInfo)
		{
			var answer = new List<int>();
			var number = numberInfo.Number;
			var firstDigit = bombInfo.GetSerialNumberNumbers().First();
			var lastDigit = bombInfo.GetSerialNumberNumbers().Last();
			_logger = numberInfo.Logger;
			_logger.LogMessage("------Start of Forget Me Now------");
			
			for (int i = 0; i < 12; i++)
			{
				var digit = 0;
				var prevDigit1 = 0;
				var prevDigit2 = 0;

				switch (i)
				{
					case 0:
						prevDigit1 = componentInfo.StartingNumber;
						prevDigit2 = lastDigit;
						break;
					case 1:
						prevDigit1 = answer[i - 1];
						prevDigit2 = componentInfo.StartingNumber;
						break;
					default:
						prevDigit1 = answer[i - 2];
						prevDigit2 = answer[i - 1];
						break;
				}

				if (prevDigit1 == 0 || prevDigit2 == 0) 
				{
					_logger.LogMessage("One of the previous 2 calculated numbers were 0:");
					digit = (int) Math.Ceiling((double) Constants.HOfX[i] * firstDigit / 5.0);
				}
				else if (prevDigit1 % 2 == 0 && prevDigit2 % 2 == 0) 
				{
					_logger.LogMessage("Both of the previous 2 calculated numbers were even:");
					digit = Math.Abs(Constants.GOfX[i] * 4 - 12);
				}
				else 
				{
					_logger.LogMessage("Otherwise rule:");
					digit = prevDigit1 + prevDigit2 + Constants.FOfX[i];
				}

				var digitToAdd = (int.Parse(number[i].ToString()) + digit) % 10;
				_logger.LogMessage("Number {0}: The final number is: {1}", i + 1, digitToAdd);
				answer.Add(digitToAdd);
			}

			return answer.Join("");
		}

		public string Name
		{
			get
			{
				return "Forget Me Now";
			}
		}
		
		public MethodId Id
		{
			get
			{
				return MethodId.ForgetMeNow;
			}
		}
	}
}