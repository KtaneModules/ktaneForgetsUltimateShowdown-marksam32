using System;
using System.Collections.Generic;
using System.Linq;

namespace ForgetsUltimateShowdownModule
{
    public class ForgetMeLaterComponent : IFUSComponentSolver
    {
        private readonly List<Func<int, int>> Funcs = new List<Func<int, int>>();
        private FUSLogger _logger;
        private List<int> _answer;
        private List<int> _number;
        public string Encrypt(KMBombInfo bombInfo, ComponentInfo componentInfo, NumberInfo numberInfo)
        {
            var rules = componentInfo.Rules;
            _number = numberInfo.Number.Select(x => int.Parse(x.ToString())).ToList();
            _answer = new List<int>();
            _logger = numberInfo.Logger;
            _logger.LogMessage("------Start of Forget Me Later------");
            
            for (int j = 0; j < 12; j++)
            {
                switch (rules[j])
                {
                    case 0:
                        AddFunction((i) => _number[i]);
                        break;
                    case 1:
                        AddFunction((i) => _number[i] + 1);
                        break;
                    case 2:
                        AddFunction((i) => _number[i] * 2);
                        break;
                    case 3:
                        AddFunction((i) => _number[i] + lastInput(i));
                        break;
                    case 4:
                        AddFunction((i) => lastInput(i) - _number[i]);
                        break;
                    case 6:
                        AddFunction((i) => secondLastInput(i) - _number[i]);
                        break;
                    case 7:
                        AddFunction((i) => _number[i] + lastInput(i) + 1);
                        break;
                    case 8:
                        AddFunction((i) => _number[i] + secondLastInput(i) + 1);
                        break;
                    case 14:
                        AddFunction((i) => _number[i] - 1);
                        break;
                    case 17:
                        AddFunction((i) => lastInput(i) + _number[i] - 1);
                        break;
                    case 18:
                        AddFunction((i) => secondLastInput(i) + _number[i] - 1);
                        break;
                    case 23:
                        AddFunction((i) => secondLastInput(i) + _number[i]);
                        break;
                    case 25:
                        AddFunction((i) => (lastInput(i) + _number[i]) * 2);
                        break;
                    case 26:
                        AddFunction((i) => (secondLastInput(i) + _number[i]) * 2);
                        break;
                    case 28:
                        AddFunction((i) => Math.Abs(lastInput(i) - _number[i]) * 2);
                        break;
                    case 29:
                        AddFunction((i) => Math.Abs(secondLastInput(i) - _number[i]) * 2);
                        break;
                    case 30:
                        AddFunction((i) => _number[i] * 3);
                        break;
                    case 35:
                        AddFunction((i) => lastInput(i) + (3 * _number[i]));
                        break;
                    case 36:
                        AddFunction((i) => secondLastInput(i) + (3 * _number[i]));
                        break;
                    case 37:
                        AddFunction((i) => _number[i] + (3 * lastInput(i)));
                        break;
                    case 38:
                        AddFunction((i) => _number[i] + (3 * secondLastInput(i)));
                        break;
                    case 40:
                        AddFunction((i) => _number[i] + 5);
                        break;
                    case 43:
                        AddFunction((i) => _number[i] + (2 * lastInput(i)));
                        break;
                    case 44:
                        AddFunction((i) => _number[i] + (2 * secondLastInput(i)));
                        break;
                    case 45:
                        AddFunction((i) => lastInput(i) + (2 * _number[i]));
                        break;
                    case 46:
                        AddFunction((i) => secondLastInput(i) + (2 * _number[i]));
                        break;
                    case 48:
                        AddFunction((i) => Math.Abs(_number[i] - (2 * lastInput(i))));
                        break;
                    case 50:
                        AddFunction((i) => 9 - _number[i]);
                        break;
                    case 53:
                        AddFunction((i) => 18 - (_number[i] + lastInput(i)));
                        break;
                    case 54:
                        AddFunction((i) => 18 - (_number[i] + secondLastInput(i)));
                        break;
                    case 56:
                        AddFunction((i) => 18 - (_number[i] * 2));
                        break;
                    case 57:
                        AddFunction((i) => 9 - Math.Abs(_number[i] - lastInput(i)));
                        break;
                    case 58:
                        AddFunction((i) => 9 - Math.Abs(_number[i] - secondLastInput(i)));
                        break;
                    default:
                        throw new InvalidOperationException(string.Format("Invalid rule #{0}", rules[j]));
                }
            }
            CalculateAnswer();
            return _answer.Join("");
        }

        private void AddFunction(Func<int, int> func)
        {
            Funcs.Add(func);
        }

        private int lastInput(int i)
        {
            return i == 0 ? 0 : _answer[i - 1];
        }

        private int secondLastInput(int i)
        {
            return i == 0 || i == 1 ? 0 : _answer[i - 2];
        }

        private void CalculateAnswer()
        {
            for (int i = 0; i < 12; i++)
            {
                var number = Math.Abs(Funcs[i](i)) % 10;
                _logger.LogMessage("Number {0}: The final number is: {1}", i + 1, number);
                _answer.Add(number);
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