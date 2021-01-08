using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public partial class ForgetsUltimateShowdownScript
{
#pragma warning disable 414
    private const string TwitchHelpMessage =
        "Press button(s) using !{0} press 1234567890";
#pragma warning restore 414

    private static readonly Regex _tpRegex = new Regex("^press ([0-9 ]*)$");
    
    private IEnumerator ProcessTwitchCommand(string command)
    {
        command = command.ToLowerInvariant().Trim();
        var match = _tpRegex.Match(command);
        if (match.Success)
        {
            if (_animating)
            {
                yield return "sendtochaterror Please wait until the animation is over.";
                yield break;
            }
            var presses = match.Groups[1].ToString().Replace(" ", string.Empty).Split().Join("");
            var selectables = new List<KMSelectable>();
            foreach (var press in presses)
            {
                switch (press)
                {
                    case '0':
                        selectables.Add(NumberButtons[9]);
                        break;
                    default:
                        selectables.Add(NumberButtons[int.Parse(press.ToString()) - 1]);
                        break;
                }
            }

            foreach (var press in selectables)
            {
                yield return "trycancel";
                yield return new WaitForSeconds(.1f);
                yield return "trycancel";
                press.OnInteract();
            }
        }
    }

    private IEnumerator TwitchHandleForcedSolve()
    {
        var answer = _answer;

        while (_animating)
        {
            yield return true;
        }
        if (_showingInfo)
        {
            NumberButtons[9].OnInteract();
            while (_animating)
            {
                yield return true;
            }
        }
        if (!_hasBeenStarted)
        {
            NumberButtons[9].OnInteract();
            while (_animating)
            {
                yield return true;
            }
            NumberButtons[9].OnInteract();
            while (_animating)
            {
                yield return true;
            }
        }
        foreach (var digit in answer)
        {
            switch (digit)
            {
                case '0':
                    NumberButtons[9].OnInteract();
                    break;
                default:
                    NumberButtons[int.Parse(digit.ToString()) - 1].OnInteract();
                    break;
            }
            yield return new WaitForSeconds(.1f);
        }
        yield return true;
    }
}