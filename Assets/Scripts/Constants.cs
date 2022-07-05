using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ForgetsUltimateShowdownModule
{
    public static class Constants
    {
        public static readonly List<Color> ForgetEverythingLedColors = new List<Color>
        {
            new Color(0.7f, 0, 0, 1),
            new Color(0.5f, 0.5f, 0, 1),
            new Color(0, 0.6f, 0, 1),
            new Color(0, 0.3f, 1, 1)
        };

        public static readonly List<List<Pair<Vector3, float>>> ForgetEverythingPositions =
            new List<List<Pair<Vector3, float>>>
            {
                new List<Pair<Vector3, float>>
                {
                    new Pair<Vector3, float>(new Vector3(0f, -0.3f, 0.783f), -90f),
                    new Pair<Vector3, float>(new Vector3(-0.594f, -0.3f, 0.012f), 180f)
                }, //tl
                new List<Pair<Vector3, float>>
                {
                    new Pair<Vector3, float>(new Vector3(0f, -0.3f, 0.783f), -90f),
                    new Pair<Vector3, float>(new Vector3(0.594f, -0.3f, 0.012f), 0f)
                }, //tr
                new List<Pair<Vector3, float>>
                {
                    new Pair<Vector3, float>(new Vector3(0f, -0.3f, -0.783f), 90f),
                    new Pair<Vector3, float>(new Vector3(-0.594f, -0.3f, -0.12f), 180f)
                }, //bl
                new List<Pair<Vector3, float>>
                {
                    new Pair<Vector3, float>(new Vector3(0f, -0.3f, -0.783f), 90f),
                    new Pair<Vector3, float>(new Vector3(0.594f, -0.3f, -0.12f), 0f)
                } //br
            };

        public static readonly List<char> AndLogicGateChars = new List<char>
        {
            '∧', '∨', '⊻', '|', '↓', '↔', '→', '←', '¬'
        };

        public static readonly List<int> ForgetMeLaterRules = new List<int>
        {
            0, 1, 2, 3, 4, 6, 7, 8, 14, 17, 18, 23, 25, 26, 28, 29, 30, 35, 36, 37, 38, 40, 43, 44, 45, 46, 48, 50, 53,
            54, 56, 57, 58
        };

        public const string Version = "1.4";

        public static readonly Regex TpRegex = new Regex("^press ([0-9 ]*)$");
        
        public static readonly int[] FOfX = { 3, 4, 9, 6, 1, 9, 7, 4, 9, 1, 7, 9, 5, 6, 9, 8, 0, 9, 0, 0, 0};
        public static readonly int[] GOfX = { 2, 3, 4, 4, 4, 5, 5, 5, 6, 6, 6, 8, 8, 8, 10, 10, 10, 10, 12, 12, 12};
        public static readonly int[] HOfX = { 1, 2, 3, 3, 5, 5, 7, 7, 10, 10, 12, 12, 15, 15, 15, 15, 15, 15, 15, 15, 15};

        public static readonly List<string> PossibleMissionMethods =
            new List<string> { "ss", "fml", "and", "fmn", "fi", "fmw", "fe", "fun" };
        
        public static readonly Regex MissionRegex = new Regex(@"\[Forget's Ultimate Showdown\] METHODS=((?:\(.+\)\|){3}(?:\(.+\))):ORDER=((?:FIXED)|(?:RANDOM))");
    }
}