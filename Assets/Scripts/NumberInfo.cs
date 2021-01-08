namespace ForgetsUltimateShowdownModule
{
    public class NumberInfo
    {
        public string Number { get; set; }
        public string BottomNumber { get; private set; }
        public FUSLogger Logger { get; private set; }

        public NumberInfo(string number, string bottomNumber, FUSLogger logger)
        {
            Number = number;
            BottomNumber = bottomNumber;
            Logger = logger;
        }
    }
}

