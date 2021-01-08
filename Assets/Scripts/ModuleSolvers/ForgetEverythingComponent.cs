using System;
using System.Collections.Generic;
using System.Linq;

namespace ForgetsUltimateShowdownModule
{
    public class ForgetEverythingComponent : IFUSComponentSolver
    {
        private FUSLogger _logger;
        private int _modId;
        public string Encrypt(KMBombInfo bombInfo, ComponentInfo componentInfo, NumberInfo numberInfo)
        {
            var answer = new List<int>();
            var number = numberInfo.Number.Select(x => int.Parse(x.ToString())).ToList();
            var bottomNumber = numberInfo.BottomNumber.Select(x => int.Parse(x.ToString())).ToList();
            _logger = numberInfo.Logger;
            
            _logger.LogMessage("------Start of Forget Everything------");
            
            ForgetEverythingColor primaryCol;
            if (componentInfo.FEColors[0] == componentInfo.FEColors[1] ||
                componentInfo.FEColors[0] == componentInfo.FEColors[2])
                primaryCol = componentInfo.FEColors[0];
            else if (componentInfo.FEColors[1] == componentInfo.FEColors[2])
                primaryCol = componentInfo.FEColors[1];
            else
            {
                if (componentInfo.FEColors[0] != 0 && componentInfo.FEColors[1] != 0 && componentInfo.FEColors[2] != 0)
                    primaryCol = 0;
                else if (componentInfo.FEColors[0] != (ForgetEverythingColor) 1 &&
                         componentInfo.FEColors[1] != (ForgetEverythingColor) 1 &&
                         componentInfo.FEColors[2] != (ForgetEverythingColor) 1)
                    primaryCol = (ForgetEverythingColor) 1;
                else if (componentInfo.FEColors[0] != (ForgetEverythingColor) 2 &&
                         componentInfo.FEColors[1] != (ForgetEverythingColor) 2 &&
                         componentInfo.FEColors[2] != (ForgetEverythingColor) 2)
                    primaryCol = (ForgetEverythingColor) 2;
                else
                    primaryCol = (ForgetEverythingColor) 3;
            }
            _logger.LogMessage("The rule used is {0}", primaryCol.ToString());

            switch (primaryCol)
            {
                case ForgetEverythingColor.Red:
                    ApplyRule(answer, (i) => (bottomNumber[i] + number[i]) % 10);
                    break;
                case ForgetEverythingColor.Yellow:
                    ApplyRule(answer, (i) => (bottomNumber[i] - number[i] + 10) % 10);
                    break;
                case ForgetEverythingColor.Green:
                    ApplyRule(answer, (i) => (bottomNumber[i] + number[i] + 5) % 10);
                    break;
                case ForgetEverythingColor.Blue:
                    ApplyRule(answer, (i) => (number[i] - bottomNumber[i] + 10) % 10);
                    break;
                default:
                    throw new InvalidOperationException(string.Format("invalid color rule {0}", primaryCol));
            }

            return answer.Join("");
        }

        private void ApplyRule(List<int> answer, Func<int, int> func)
        {
            for (var i = 0; i < 12; i++)
            {
                var number = func(i);
                _logger.LogMessage("Number {0}: The final number is: {1}", i + 1, number);
                answer.Add(number);
            }
        }

        public string Name
        {
            get
            {
                return "Forget Everything";
            }
        }
        
        public MethodId Id
        {
            get
            {
                return MethodId.ForgetEverything;
            }
        }
    }
}