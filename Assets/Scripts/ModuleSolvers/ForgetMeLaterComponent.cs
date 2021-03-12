using System;
using System.Collections.Generic;
using System.Linq;

namespace ForgetsUltimateShowdownModule
{
    public class ForgetMeLaterComponent : IFUSComponentSolver
    {
        private readonly List<Func<int, int>> Funcs = new List<Func<int, int>>();
        private FUSLogger _logger;
        public string Encrypt(KMBombInfo bombInfo, ComponentInfo componentInfo, NumberInfo numberInfo)
        {
            var answer = new List<int>();
            var rules = componentInfo.Rules;
            var number = numberInfo.Number.Select(x => int.Parse(x.ToString())).ToList();
            var bottomNumber = numberInfo.BottomNumber.Select(x => int.Parse(x.ToString())).ToList();
            _logger = numberInfo.Logger;
            _logger.LogMessage("------Start of Forget Me Later------");
            
            for (int j = 0; j < 12; j++)
            {
                switch (rules[j])
                {
                    case 0:
                        AddFunction((i) => Sum(number[i], bottomNumber[i]));
                        break;
                    case 1:
                        AddFunction((i) => Sum(number[i], bottomNumber[i] + 1));
                        break;
                    case 2:
                        AddFunction((i) => Sum(number[i], bottomNumber[i] * 2));
                        break;
                    case 3:
                        AddFunction((i) => Sum(number[i], bottomNumber[i] + (i == 0 ? number[i] : answer[i - 1])));
                        break;
                    case 4:
                        AddFunction((i) => Sum(number[i], (i == 0 ? number[i] :answer[i - 1])  - bottomNumber[i]));
                        break;
                    case 5:
                        AddFunction(
                            (i) => Sum(number[i],
                                Math.Abs((i == 0 ? number[i] : answer[i - 1]) - (i == 0 || i == 1 ? number[i] : answer[i - 2]))));
                        break;
                    case 6:
                        AddFunction(
                            (i) => Sum(number[i], (i == 0 || i == 1 ? number[i] : answer[i - 2]) - bottomNumber[i]));
                        break;
                    case 7:
                        AddFunction((i) => Sum(number[i], bottomNumber[i] + (i == 0 ? number[i] : answer[i - 1]) + 1));
                        break;
                    case 8:
                        AddFunction(
                            (i) => Sum(number[i], bottomNumber[i] + (i == 0 || i == 1 ? number[i] : answer[i - 2]) + 1));
                        break;
                    case 9:
                        AddFunction(
                            (i) => Sum(number[i], (i == 0 ? number[i] : answer[i - 1]) + (i == 0 || i == 1 ? number[i] : answer[i - 2]) + 1));
                        break;
                    case 10:
                        AddFunction((i) => Sum(number[i], (i == 0 ? number[i] : answer[i - 1])));
                        break;
                    case 11:
                        AddFunction((i) => Sum(number[i], (i == 0 ? number[i] : answer[i - 1]) + 1));
                        break;
                    case 12:
                        AddFunction((i) => Sum(number[i], (i == 0 ? number[i] : answer[i - 1]) * 2));
                        break;
                    case 13:
                        AddFunction(
                            (i) => Sum(number[i], (i == 0 ? number[i] : answer[i - 1]) + (i == 0 || i == 1 ? number[i] : answer[i - 2])));
                        break;
                    case 14:
                        AddFunction((i) => Sum(number[i], bottomNumber[i] - 1));
                        break;
                    case 15:
                        AddFunction((i) => Sum(number[i], (i == 0 ? number[i] : answer[i - 1]) - 1));
                        break;
                    case 16:
                        AddFunction((i) => Sum(number[i], (i == 0 || i == 1 ? number[i] : answer[i - 2]) - 1));
                        break;
                    case 17:
                        AddFunction((i) => Sum(number[i], (i == 0 ? number[i] : answer[i - 1]) + bottomNumber[i] - 1));
                        break;
                    case 18:
                        AddFunction(
                            (i) => Sum(number[i], (i == 0 || i == 1 ? number[i] : answer[i - 2]) + bottomNumber[i] - 1));
                        break;
                    case 19:
                        AddFunction(
                            (i) => Sum(number[i], (i == 0 ? number[i] : answer[i - 1]) + (i == 0 || i == 1 ? number[i] : answer[i - 2]) - 1));
                        break;
                    case 20:
                        AddFunction((i) => Sum(number[i], (i == 0 || i == 1 ? number[i] : answer[i - 2])));
                        break;
                    case 21:
                        AddFunction((i) => Sum(number[i], (i == 0 || i == 1 ? number[i] : answer[i - 2]) + 1));
                        break;
                    case 22:
                        AddFunction((i) => Sum(number[i], (i == 0 || i == 1 ? number[i] : answer[i - 2]) * 2));
                        break;
                    case 23:
                        AddFunction(
                            (i) => Sum(number[i], (i == 0 || i == 1 ? number[i] : answer[i - 2]) + bottomNumber[i]));
                        break;
                    case 24:
                        AddFunction(
                            (i) => Sum(number[i], ((i == 0 ? number[i] : answer[i - 1]) + (i == 0 || i == 1 ? number[i] : answer[i - 2])) * 2));
                        break;
                    case 25:
                        AddFunction((i) => Sum(number[i], ((i == 0 ? number[i] : answer[i - 1]) + bottomNumber[i]) * 2));
                        break;
                    case 26:
                        AddFunction(
                            (i) => Sum(number[i], ((i == 0 || i == 1 ? number[i] : answer[i - 2]) + bottomNumber[i]) * 2));
                        break;
                    case 27:
                        AddFunction(
                            (i) => Sum(number[i],
                                Math.Abs((i == 0 || i == 1 ? number[i] : answer[i - 2]) - (i == 0 ? number[i] : answer[i - 1])) * 2));
                        break;
                    case 28:
                        AddFunction(
                            (i) => Sum(number[i], Math.Abs(bottomNumber[i] - (i == 0 ? number[i] : answer[i - 1])) * 2));
                        break;
                    case 29:
                        AddFunction(
                            (i) => Sum(number[i], Math.Abs((i == 0 || i == 1 ? number[i] : answer[i - 2]) - bottomNumber[i]) * 2));
                        break;
                    case 30:
                        AddFunction((i) => Sum(number[i], bottomNumber[i] * 3));
                        break;
                    case 31:
                        AddFunction((i) => Sum(number[i], (i == 0 ? number[i] : answer[i - 1]) * 3));
                        break;
                    case 32:
                        AddFunction((i) => Sum(number[i], (i == 0 || i == 1 ? number[i] : answer[i - 2]) * 3));
                        break;
                    case 33:
                        AddFunction(
                            (i) => Sum(number[i], ((i == 0 || i == 1 ? number[i] : answer[i - 2]) + (i == 0 ? number[i] : answer[i - 1])) * 3));
                        break;
                    case 34:
                        AddFunction(
                            (i) => Sum(number[i], (i == 0 || i == 1 ? number[i] : answer[i - 2]) + ((i == 0 ? number[i] : answer[i - 1]) * 3)));
                        break;
                    case 35:
                        AddFunction((i) => Sum(number[i], (i == 0 ? number[i] : answer[i - 1]) + (bottomNumber[i] * 3)));
                        break;
                    case 36:
                        AddFunction(
                            (i) => Sum(number[i], (i == 0 || i == 1 ? number[i] : answer[i - 2]) + (bottomNumber[i] * 3)));
                        break;
                    case 37:
                        AddFunction((i) => Sum(number[i], bottomNumber[i] + ((i == 0 ? number[i] : answer[i - 1]) * 3)));
                        break;
                    case 38:
                        AddFunction(
                            (i) => Sum(number[i], bottomNumber[i] + ((i == 0 || i == 1 ? number[i] : answer[i - 2]) * 3)));
                        break;
                    case 39:
                        AddFunction(
                            (i) => Sum(number[i], (i == 0 ? number[i] : answer[i - 1]) + ((i == 0 || i == 1 ? number[i] : answer[i - 2]) * 3)));
                        break;
                    case 40:
                        AddFunction((i) => Sum(number[i], bottomNumber[i] + 5));
                        break;
                    case 41:
                        AddFunction((i) => Sum(number[i], (i == 0 ? number[i] : answer[i - 1]) + 5));
                        break;
                    case 42:
                        AddFunction((i) => Sum(number[i], (i == 0 || i == 1 ? number[i] : answer[i - 2]) + 5));
                        break;
                    case 43:
                        AddFunction((i) => Sum(number[i], bottomNumber[i] + (2 * (i == 0 ? number[i] : answer[i - 1]))));
                        break;
                    case 44:
                        AddFunction(
                            (i) => Sum(number[i], bottomNumber[i] + (2 * (i == 0 || i == 1 ? number[i] : answer[i - 2]))));
                        break;
                    case 45:
                        AddFunction((i) => Sum(number[i], (i == 0 ? number[i] : answer[i - 1]) + (2 * bottomNumber[i])));
                        break;
                    case 46:
                        AddFunction(
                            (i) => Sum(number[i], (i == 0 || i == 1 ? number[i] : answer[i - 2]) + (2 * bottomNumber[i])));
                        break;
                    case 47:
                        AddFunction(
                            (i) => Sum(number[i],
                                Math.Abs((i == 0 || i == 1 ? number[i] : answer[i - 2]) - (2 * (i == 0 ? number[i] : answer[i - 1])))));
                        break;
                    case 48:
                        AddFunction(
                            (i) => Sum(number[i], Math.Abs(bottomNumber[i] - (2 * (i == 0 ? number[i] : answer[i - 1])))));
                        break;
                    case 49:
                        AddFunction(
                            (i) => Sum(number[i],
                                Math.Abs((i == 0 ? number[i] : answer[i - 1]) - (2 * (i == 0 || i == 1 ? number[i] : answer[i - 2])))));
                        break;
                    case 50:
                        AddFunction((i) => Sum(number[i], 9 - bottomNumber[i]));
                        break;
                    case 51:
                        AddFunction((i) => Sum(number[i], 9 - (i == 0 ? number[i] : answer[i - 1])));
                        break;
                    case 52:
                        AddFunction((i) => Sum(number[i], 9 - (i == 0 || i == 1 ? number[i] : answer[i - 2])));
                        break;
                    case 53:
                        AddFunction((i) => Sum(number[i], 18 - (bottomNumber[i] + (i == 0 ? number[i] : answer[i - 1]))));
                        break;
                    case 54:
                        AddFunction(
                            (i) => Sum(number[i], 18 - (bottomNumber[i] + (i == 0 || i == 1 ? number[i] : answer[i - 2]))));
                        break;
                    case 55:
                        AddFunction(
                            (i) => Sum(number[i],
                                18 - ((i == 0 ? number[i] : answer[i - 1]) + (i == 0 || i == 1 ? number[i] : answer[i - 2]))));
                        break;
                    case 56:
                        AddFunction((i) => Sum(number[i], 18 - (2 * bottomNumber[i])));
                        break;
                    case 57:
                        AddFunction(
                            (i) => Sum(number[i], 9 - Math.Abs((i == 0 ? number[i] : answer[i - 1]) - bottomNumber[i])));
                        break;
                    case 58:
                        AddFunction(
                            (i) => Sum(number[i], 9 - Math.Abs((i == 0 || i == 1 ? number[i] : answer[i - 2]) - bottomNumber[i])));
                        break;
                    case 59:
                        AddFunction(
                            (i) => Sum(number[i],
                                9 - Math.Abs((i == 0 || i == 1 ? number[i] : answer[i - 2]) - (i == 0 ? number[i] : answer[i - 1]))));
                        break;
                    default:
                        throw new InvalidOperationException(string.Format("Invalid rule #{0}", rules[j]));
                }
            }
            CalculateAnswer(answer);
            return answer.Join("");
        }

        private static int Sum(int number, int add)
        {
            return (number + Math.Abs(add)) % 10;
        }

        private void AddFunction(Func<int, int> func)
        {
            Funcs.Add(func);
        }

        private void CalculateAnswer(List<int> answer)
        {
            for (int i = 0; i < 12; i++)
            {
                var number = Funcs[i](i);
                _logger.LogMessage("Number {0}: The final number is: {1}", i + 1, number);
                answer.Add(number);
            }
        }

        public string Name
        {
            get
            {
                return "Forget Me Later";
            }
        }
        
        public MethodId Id
        {
            get
            {
                return MethodId.ForgetMeLater;
            }
        }
    }
}