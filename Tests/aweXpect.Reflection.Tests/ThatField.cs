namespace aweXpect.Reflection.Tests;

public sealed partial class ThatField
{
#pragma warning disable CS0169
#pragma warning disable CS0649
	public class ClassWithFields
	{
		internal int InternalField;
		private int PrivateField;
		protected int ProtectedField;
		public int PublicField;
	}

	public class ClassWithSingleField
	{
		public int MyField;
	}
#pragma warning restore CS0649
#pragma warning restore CS0169
}
