namespace ForgetsUltimateShowdownModule
{
	public interface IFUSComponentSolver
	{
		string Encrypt(KMBombInfo bombInfo, ComponentInfo componentInfo, NumberInfo numberInfo);

		string Name
		{
			get;
		}

		MethodId Id
		{
			get;
		}
	}
}