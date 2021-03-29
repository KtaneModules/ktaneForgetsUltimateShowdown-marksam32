using System;
using System.Collections.Generic;
using System.Linq;
using KModkit;

namespace ForgetsUltimateShowdownModule
{
    public class ForgetUsNotComponent : IFUSComponentSolver
    {
        private FUSLogger _logger;
        private Func<string, int> function;
        
        public string Encrypt(KMBombInfo bombInfo, ComponentInfo componentInfo, NumberInfo numberInfo)
        {
            _logger = numberInfo.Logger;
            var order = componentInfo.Positions;
            var initialNumber = numberInfo.Number;
            var numbers = new List<string>();
            var finalnumbers = new int[12];
            _logger.LogMessage("------Start of Forget Us Not------");
            for (var i = 0; i < 12; ++i)
            {
                numbers.Add(initialNumber.Take(3).Join(""));
                initialNumber = Shift(initialNumber);
            }
            
            _logger.LogMessage("The 3 digit groups are: {0}.", numbers.Join(", "));

            switch (bombInfo.GetSerialNumberLetters().Count() - bombInfo.GetSerialNumberNumbers().Count())
            {
                case 0:
                    function = (s) =>
                    {
                        var digits = s.Select(x => int.Parse(x.ToString())).ToList();
                        return (Math.Abs(digits[1] - bombInfo.GetBatteryCount()) + Math.Abs(digits[0] - digits[2])) % 10;
                    };
                    break;
                case -2:
                    function = (s) =>
                    {
                        var digits = s.Select(x => int.Parse(x.ToString())).ToList();
                        return (Math.Abs(digits[2] - bombInfo.GetBatteryCount()) + Math.Abs(digits[0] - digits[1])) % 10;
                    };
                    break;
                default:
                    function = (s) =>
                    {
                        var digits = s.Select(x => int.Parse(x.ToString())).ToList();
                        return (Math.Abs(digits[0] - bombInfo.GetBatteryCount()) + Math.Abs(digits[1] - digits[2])) % 10;
                    };
                    break;
            }

            for (var i = 0; i < numbers.Count; ++i)
            {
                var str = numbers[i];
                var answer = function(str);
                _logger.LogMessage("The final answer for string {0}({1}), which will be placed in position {2} is {3}.", i + 1, str, order[i], answer);
                finalnumbers[order[i] - 1] = answer;
            }

            return finalnumbers.Join("");
        }

        private static string Shift(string s)
        {
            return s.Substring(1, s.Length - 1) + s.Substring(0, 1); 
        }

        public string Name
        {
            get
            {
                return "Forget Us Not";
            }
        }

        public MethodId Id
        {
            get
            {
                return MethodId.ForgetUsNot;
            }
        }
    }
}