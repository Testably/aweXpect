using System.Reflection;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperty
{
	private static PropertyInfo? GetProperty(string propertyName)
		=> typeof(ClassWithProperties).GetProperty(propertyName,
			BindingFlags.DeclaredOnly |
			BindingFlags.NonPublic |
			BindingFlags.Public |
			BindingFlags.Instance);

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
