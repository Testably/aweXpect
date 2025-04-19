namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperty
{
	public class ClassWithProperties
	{
		public int PublicProperty { get; set; }
		internal int InternalProperty { get; set; }
		protected int ProtectedProperty { get; set; }
		private int PrivateProperty { get; set; }
	}

	public class ClassWithSingleProperty
	{
		public int MyProperty { get; set; }
	}
}
