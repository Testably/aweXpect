using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethods
{
	internal static Filtered.Methods GetMethods(string methodPrefix = "")
		=> In.AssemblyContaining<ClassWithMethods>().Types().Which(t => t == typeof(ClassWithMethods))
			.Methods().Which(methodInfo => methodInfo.Name.StartsWith(methodPrefix));

	internal static Filtered.Types GetTypes<T>()
		=> In.AssemblyContaining<T>().Types().Which(t => t == typeof(T));

	internal class DummyBase
	{
	}

	internal class Dummy : DummyBase
	{
	}

#pragma warning disable CA1822 // Mark members as static
	public class ClassWithMethods
	{
		public int PublicMethod1() => 0;
		public int PublicMethod2() => 0;
		internal int InternalMethod1() => 0;
		internal int InternalMethod2() => 0;
		protected int ProtectedMethod1() => 0;
		protected int ProtectedMethod2() => 0;

		// ReSharper disable once UnusedMember.Local
		private int PrivateMethod1() => 0;

		// ReSharper disable once UnusedMember.Local
		private int PrivateMethod2() => 0;
		protected internal int ProtectedInternalMethod1() => 0;
		protected internal int ProtectedInternalMethod2() => 0;
		private protected int PrivateProtectedMethod1() => 0;
		private protected int PrivateProtectedMethod2() => 0;

		public T GenericMethod1<T>(T value) => value;
		public U GenericMethod2<T, U>(T first, U second) => second;
		public int NonGenericMethod1() => 1;
		public int NonGenericMethod2() => 2;

		public void GenericWithUnrestrictedArgumentMethod<TFoo>()
		{
		}

		public void GenericWithRestrictedSecondArgumentMethod<TFoo, TBar>()
			where TBar : ThatMethod.BaseClass
		{
		}
	}

	// ReSharper disable UnusedMember.Local
	internal class TestClass
	{
		public string GetString() => "test";
		public int GetInt() => 42;
		public bool GetBool() => true;
		public DummyBase GetDummyBase() => new();
		public Dummy GetDummy() => new();
		public async Task AsyncMethod() => await Task.CompletedTask;
		public T GenericMethod<T>(T value) => value;

		// ReSharper disable once UnusedTypeParameter
		public void GenericVoidMethod<T>() { }
		public T GenericMethodWithConstraint<T>(T value) where T : class => value;
	}
	// ReSharper restore UnusedMember.Local
#pragma warning restore CA1822
}
