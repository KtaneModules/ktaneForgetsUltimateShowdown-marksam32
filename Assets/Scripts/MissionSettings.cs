using System.Collections.Generic;

namespace ForgetsUltimateShowdownModule
{
    public class MissionSettings
    {
        public List<List<MethodId>> SelectedMethods { get; private set; }
        
        public EncryptionOrder EncryptionOrder { get; private set; }

        public MissionSettings(List<List<MethodId>> selectedMethods, EncryptionOrder encryptionOrder)
        {
            SelectedMethods = selectedMethods;
            EncryptionOrder = encryptionOrder;
        }
    }

    public enum EncryptionOrder
    {
        Random, Fixed
    }
}