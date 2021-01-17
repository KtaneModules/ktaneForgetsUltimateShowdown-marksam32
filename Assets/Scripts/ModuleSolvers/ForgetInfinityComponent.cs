using System;
using System.Linq;
using KModkit;
using UnityEngine;

namespace ForgetsUltimateShowdownModule
{
    public class ForgetInfinityComponent : IFUSComponentSolver
    {
        private FUSLogger _logger;
        public string Encrypt(KMBombInfo bombInfo, ComponentInfo componentInfo, NumberInfo numberInfo)
        {
            var number = numberInfo.Number;

            var firstItemRemove = Math.Max(componentInfo.IgnoredNumbers.Item1, componentInfo.IgnoredNumbers.Item2) - 1;
            var secondItemRemove = Math.Min(componentInfo.IgnoredNumbers.Item1, componentInfo.IgnoredNumbers.Item2) - 1;
            var removedFirstItem = number[firstItemRemove];
            var removedSecondItem = number[secondItemRemove];
            _logger = numberInfo.Logger;
            _logger.LogMessage("------Start of Forget Infinity------");

            var allModNames = bombInfo.GetModuleNames().ToList();

            var numberList = number.ToList();
            numberList.RemoveAt(firstItemRemove);
            numberList.RemoveAt(secondItemRemove);
            number = numberList.Join("");
            _logger.LogMessage("After removing the items at: {0}, the new number is: {1}", new[]{firstItemRemove + 1, secondItemRemove + 1}.Join(" and "), number);
            

            var solution = new[] {new int[5], new int[5]};

            if (number.Length != 10)
            {
                throw new InvalidOperationException(string.Format("Invalid length of number, length is {0}, expected 10", number.Length));
            }

            var calculationNumber = new[] {number.Take(5).Select(x => int.Parse(x.ToString())).ToArray(), number.TakeLast(5).Select(x => int.Parse(x.ToString())).ToArray()};
            _logger.LogMessage("The two numbers are: {0}", new[]{calculationNumber[0].Join(""), calculationNumber[1].Join("")}.Join(" and "));

            for (var x = 0; x < calculationNumber.Length; x++)
            {
                var hasSwapped = false;
                var finalStageNumbers = new int[5];
                calculationNumber[x].CopyTo(finalStageNumbers, 0);

                var lastDigitInSerial = bombInfo.GetSerialNumberNumbers().Any() ? bombInfo.GetSerialNumberNumbers().Last() : 0;
                var firstDigitInSerial =
                    bombInfo.GetSerialNumberNumbers().Any() ? bombInfo.GetSerialNumberNumbers().First() : 0;
                var smallestDigitInSerial =
                    bombInfo.GetSerialNumberNumbers().Any() ? bombInfo.GetSerialNumberNumbers().Min() : 0;
                var largestDigitInSerial =
                    bombInfo.GetSerialNumberNumbers().Any() ? bombInfo.GetSerialNumberNumbers().Max() : 0;
                // Begin Solution Calculations
                // Culumulative Slot Calculations
                if (bombInfo.IsPortPresent(Port.StereoRCA))
                {
                    finalStageNumbers = finalStageNumbers.Reverse().ToArray();
                    hasSwapped = true;
                }

                var batterycount = bombInfo.GetBatteryCount();
                for (var idx = 0; idx < finalStageNumbers.Length; idx++)
                    finalStageNumbers[idx] += batterycount;

                var LettersInSerial = bombInfo.GetSerialNumberLetters().ToList();
                if (LettersInSerial.Contains('F') || LettersInSerial.Contains('I'))
                    for (var idx = 0; idx < finalStageNumbers.Length; idx++)
                        finalStageNumbers[idx] -= LettersInSerial.Count;
                // Individual Slots
                // Slot 1
                if (allModNames.Contains("Tetris"))
                    finalStageNumbers[0] = calculationNumber[x][0] + 7;
                else if (finalStageNumbers[0] >= 10 && finalStageNumbers[0] % 2 == 0)
                    finalStageNumbers[0] /= 2;
                else if (finalStageNumbers[0] < 5)
                    finalStageNumbers[0] += lastDigitInSerial;
                else
                    finalStageNumbers[0] += 1;
                // Slot 2
                if (bombInfo.CountDuplicatePorts() > 0)
                    finalStageNumbers[1] += bombInfo.CountDuplicatePorts();
                else if (bombInfo.GetPortCount() == 0)
                    finalStageNumbers[1] += calculationNumber[x].ToArray()[0] + calculationNumber[x].ToArray()[2];
                else
                    finalStageNumbers[1] += bombInfo.GetPortCount();
                // Slot 3
                if (!hasSwapped)
                {
                    if (finalStageNumbers[2] >= 7)
                    {
                        var currentValue = calculationNumber[x][2];
                        var finalValueSlot3 = 0;
                        while (currentValue > 0)
                        {
                            finalValueSlot3 += currentValue % 2;
                            currentValue /= 2;
                        }

                        finalStageNumbers[2] = finalValueSlot3;
                    }
                    else if (finalStageNumbers[2] < 3)
                        finalStageNumbers[2] = Math.Abs(finalStageNumbers[2]);
                    else
                        finalStageNumbers[2] = calculationNumber[x][2] + smallestDigitInSerial;
                }

                // Slot 4
                if (finalStageNumbers[3] < 0)
                    finalStageNumbers[3] += largestDigitInSerial;
                // Slot 5
                var slotTable5th = new [,]
                {
                    {0, 1, 2, 3, 4},
                    {5, 6, 7, 8, 9},
                    {calculationNumber[x][4], 1 + calculationNumber[x][4], 9 - calculationNumber[x][4], calculationNumber[x][4] - 1, calculationNumber[x][4] + 5},
                    {9, 8, 5, 6, 7},
                    {4, 3, 0, 1, 2}
                };
                var rowCellToGrab = finalStageNumbers[4] - (Mathf.FloorToInt(finalStageNumbers[4] / 5.0f) * 5);
                finalStageNumbers[4] = slotTable5th[rowCellToGrab, firstDigitInSerial / 2];
                // Within 0-9
                while (!finalStageNumbers.ToList().TrueForAll(a => a >= 0 && a <= 9))
                {
                    for (var idx = 0; idx < finalStageNumbers.Length; idx++)
                        if (finalStageNumbers[idx] < 0)
                            finalStageNumbers[idx] += 10;
                        else if (finalStageNumbers[idx] > 9)
                            finalStageNumbers[idx] -= 10;
                }

                solution[x] = finalStageNumbers;
                _logger.LogMessage("The final answer for the {0} string is: {1}", x == 0 ? "first" : "second", solution[x].Join(""));
            }

            var finalString = solution[0].Concat(solution[1]).Join("");
            finalString = finalString.Insert(secondItemRemove, removedSecondItem.ToString());
            finalString = finalString.Insert(firstItemRemove, removedFirstItem.ToString());
            _logger.LogMessage("After concatinating and inserting the removed numbers, the answer is: {0}", finalString);

            return finalString;
        }

        public string Name
        {
            get
            {
                return "Forget Infinity";
            }
        }
        
        public MethodId Id
        {
            get
            {
                return MethodId.ForgetInfinity;
            }
        }
    }
}