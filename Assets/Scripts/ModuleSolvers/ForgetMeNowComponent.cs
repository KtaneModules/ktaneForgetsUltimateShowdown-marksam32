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
		private static readonly int[] fOfX = { 3, 4, 9, 6, 1, 9, 7, 4, 9, 1, 7, 9, 5, 6, 9, 8, 0, 9, 0, 0, 0};
		private static readonly int[] gOfX = { 2, 3, 4, 4, 4, 5, 5, 5, 6, 6, 6, 8, 8, 8, 10, 10, 10, 10, 12, 12, 12};
		private static readonly int[] hOfX = { 1, 2, 3, 3, 5, 5, 7, 7, 10, 10, 12, 12, 15, 15, 15, 15, 15, 15, 15, 15, 15};
		
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

				if (i == 0) 
				{
					prevDigit1 = componentInfo.StartingNumber;
					prevDigit2 = lastDigit;
				}
				else if (i == 1) 
				{
					prevDigit1 = answer[i - 1];
					prevDigit2 = componentInfo.StartingNumber;
				}
				else 
				{
					prevDigit1 = answer[i - 2];
					prevDigit2 = answer[i - 1];
				}
				
				if (prevDigit1 == 0 || prevDigit2 == 0) 
				{
					_logger.LogMessage("One of the previous 2 calculated numbers were 0:");
					digit = (int) Math.Ceiling((double) hOfX[i] * firstDigit / 5.0);
				}
				else if (prevDigit1 % 2 == 0 && prevDigit2 % 2 == 0) 
				{
					_logger.LogMessage("Both of the previous 2 calculated numbers were even:");
					digit = Math.Abs(gOfX[i] * 4 - 12);
				}
				else 
				{
					_logger.LogMessage("Otherwise rule:");
					digit = prevDigit1 + prevDigit2 + fOfX[i];
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