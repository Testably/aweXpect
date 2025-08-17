using System.Reflection;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethod
{
	private static MethodInfo? GetMethod(string methodName)
		=> typeof(ClassWithMethods).GetMethod(methodName,
			BindingFlags.DeclaredOnly |
			BindingFlags.NonPublic |
			BindingFlags.Public |
			BindingFlags.Instance);

#pragma warning disable CA1822 // Mark members as static
	public class ClassWithMethods
	{
		public int PublicMethod() => 0;
		internal int InternalMethod() => 0;
		protected int ProtectedMethod() => 0;
		private int PrivateMethod() => 0;
		protected internal int ProtectedInternalMethod() => 0;
		private protected int PrivateProtectedMethod() => 0;
		
		public T GenericMethod<T>(T value) => value;
		public void AnotherGenericMethod<T, U>(T first, U second) { }
		public int NonGenericMethod() => 1;
	}

	public class ClassWithSingleMethod
	{
		public int MyMethod() => 0;
	}

	public class ClassWithInheritance
	{
		public DerivedClass GetDerived() => new();
	}

	public abstract class BaseClass;

	public class DerivedClass : BaseClass;
#pragma warning restore CA1822 // Mark members as static
}
