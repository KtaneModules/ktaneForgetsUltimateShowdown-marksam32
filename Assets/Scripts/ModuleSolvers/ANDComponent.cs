using System;
using System.Linq;
using System.Collections.Generic;

namespace ForgetsUltimateShowdownModule
{
    public class ANDComponent : IFUSComponentSolver
    {
        private FUSLogger _logger;
        public string Encrypt(KMBombInfo bombInfo, ComponentInfo componentInfo, NumberInfo numberInfo)
        {
            var answer = new List<int>();
            _logger = numberInfo.Logger;
            var number = numberInfo.Number;
            var bottomNumber = numberInfo.BottomNumber;
            var logicGates = componentInfo.LogicGates;
            _logger.LogMessage("------Start of A>N<D------");

            for (int i = 0; i < 12; i++)
            {
                _logger.LogMessage("Digit {0}:", i + 1);
                var bits1 = Convert.ToString(number[i], 2).TakeLast(4).Select(x => x == '1').ToArray();
                var bits2 = Convert.ToString(bottomNumber[i], 2).TakeLast(4).Select(x => x == '1').ToArray();
                _logger.LogMessage("The first digit is: {0}, and that in binary is: {1}", number[i],
                    bits1.Select(x => x ? "1" : "0").Join(""));
                _logger.LogMessage("The second digit is: {0}, and that in binary is: {1}", bottomNumber[i],
                    bits2.Select(x => x ? "1" : "0").Join(""));
                var finalBinary = new[] {false, false, false, false};
                if (logicGates[i] == ANDLogicGate.Not)
                {
                    _logger.LogMessage("The gate is NOT, so only the first bits are required.");
                    finalBinary = bits1.Select(x => !x).ToArray();
                    var finalNotNumber = Convert.ToInt32(finalBinary.Select(x => x ? 1 : 0).Join(""), 2);
                    _logger.LogMessage("Final binary: {0}", finalBinary.Select(x => x ? 1 : 0).Join(""));
                    _logger.LogMessage("The number obtained is: {0}", finalNotNumber);
                    if (finalNotNumber > 9)
                    {
                        if (i % 2 == 0)
                        {
                            finalNotNumber = (finalNotNumber / 10) + (finalNotNumber % 10);
                            _logger.LogMessage("Applying the digital root: {0}", finalNotNumber);
                        }
                        else
                        {
                            finalNotNumber %= 10;
                            _logger.LogMessage("Taking the last digit: {0}", finalNotNumber);
                        }
                    }

                    answer.Add(finalNotNumber);
                    continue;
                }

                switch (logicGates[i])
                {
                    case ANDLogicGate.AND:
                        _logger.LogMessage("The gate is AND");
                        break;
                    case ANDLogicGate.OR:
                        _logger.LogMessage("The gate is OR");
                        break;
                    case ANDLogicGate.XOR:
                        _logger.LogMessage("The gate is XOR");
                        break;
                    case ANDLogicGate.NAND:
                        _logger.LogMessage("The gate is NAND");
                        break;
                    case ANDLogicGate.NOR:
                        _logger.LogMessage("The gate is NOR");
                        break;
                    case ANDLogicGate.XNOR:
                        _logger.LogMessage("The gate is XNOR");
                        break;
                    case ANDLogicGate.ImpLeft:
                        _logger.LogMessage("The gate is ImpLeft");
                        break;
                    case ANDLogicGate.ImpRight:
                        _logger.LogMessage("The gate is ImpRight");
                        break;
                    default:
                        throw new InvalidOperationException(string.Format("invalid operator {0}", logicGates[i]));
                }

                for (int k = 0; k < 4; k++)
                {
                    switch (logicGates[i])
                    {
                        case ANDLogicGate.AND:
                            finalBinary[3 - k] = bits1[3 - k] && bits2[3 - k];
                            break;
                        case ANDLogicGate.OR:
                            finalBinary[3 - k] = bits1[3 - k] || bits2[3 - k];
                            break;
                        case ANDLogicGate.XOR:
                            finalBinary[3 - k] = bits1[3 - k] ^ bits2[3 - k];
                            break;
                        case ANDLogicGate.NAND:
                            finalBinary[3 - k] = !(bits1[3 - k] && bits2[3 - k]);
                            break;
                        case ANDLogicGate.NOR:
                            finalBinary[3 - k] = !(bits1[3 - k] || bits2[3 - k]);
                            break;
                        case ANDLogicGate.XNOR:
                            finalBinary[3 - k] = !(bits1[3 - k] ^ bits2[3 - k]);
                            break;
                        case ANDLogicGate.ImpLeft:
                            finalBinary[3 - k] = !(bits1[3 - k] && !bits2[3 - k]);
                            break;
                        case ANDLogicGate.ImpRight:
                            finalBinary[3 - k] = !(!bits1[3 - k] && bits2[3 - k]);
                            break;
                        default:
                            throw new InvalidOperationException(string.Format("invalid operator {0}", logicGates[i]));
                    }
                }
                _logger.LogMessage("Final binary: {0}", finalBinary.Select(x => x ? 1 : 0).Join(""));
                var finalNumber = Convert.ToInt32(finalBinary.Select(x => x ? 1 : 0).Join(""), 2);
                _logger.LogMessage("The number obtained is: {0}", finalNumber);
                if (finalNumber > 9)
                {
                    if (i % 2 == 0)
                    {
                        finalNumber = (finalNumber / 10) + (finalNumber % 10);
                        _logger.LogMessage("Applying the digital root: {0}", finalNumber);
                    }
                    else
                    {
                        finalNumber %= 10;
                        _logger.LogMessage("Taking the last digit: {0}", finalNumber);
                    }
                }

                answer.Add(finalNumber);
            }

            return answer.Join("");
        }

        public string Name
        {
            get { return "A>N<D"; }
        }

        public MethodId Id
        {
            get { return MethodId.AND; }
        }
    }
}