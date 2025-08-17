using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatFields
{
	public static Filtered.Fields GetFields(string fieldPrefix)
		=> In.AssemblyContaining<ClassWithFields>().Types().Which(t => t == typeof(ClassWithFields))
			.Fields().Which(fieldInfo => fieldInfo.Name.StartsWith(fieldPrefix));

	public static Filtered.Types GetTypes<T>()
		=> In.AssemblyContaining<T>().Types().Which(t => t == typeof(T));

#pragma warning disable CS0169
#pragma warning disable CS0649
	public class ClassWithFields
	{
		internal int InternalField1;
		internal int InternalField2;
		private int PrivateField1;
		private int PrivateField2;
		protected int ProtectedField1;
		protected int ProtectedField2;
		public int PublicField1;
		public int PublicField2;
		protected internal int ProtectedInternalField1;
		protected internal int ProtectedInternalField2;
		private protected int PrivateProtectedField1;
		private protected int PrivateProtectedField2;
	}

#pragma warning restore CS0649
#pragma warning restore CS0169
}
