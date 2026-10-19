using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperties
{
	public static Filtered.Properties GetProperties(string propertyPrefix)
		=> In.AssemblyContaining<ClassWithProperties>().Types().Which(t => t == typeof(ClassWithProperties))
			.Properties().Which(propertyInfo => propertyInfo.Name.StartsWith(propertyPrefix));

	public static Filtered.Types GetTypes<T>()
		=> In.AssemblyContaining<T>().Types().Which(t => t == typeof(T));

	public class ClassWithProperties
	{
		public int PublicProperty1 { get; set; }
		public int PublicProperty2 { get; set; }
		internal int InternalProperty1 { get; set; }
		internal int InternalProperty2 { get; set; }
		protected int ProtectedProperty1 { get; set; }
		protected int ProtectedProperty2 { get; set; }
		private int PrivateProperty1 { get; set; }
		private int PrivateProperty2 { get; set; }
		protected internal int ProtectedInternalProperty1 { get; set; }
		protected internal int ProtectedInternalProperty2 { get; set; }
		private protected int PrivateProtectedProperty1 { get; set; }
		private protected int PrivateProtectedProperty2 { get; set; }
	}
}
