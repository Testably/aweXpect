using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethods
{
	public static Filtered.Methods GetMethods(string methodPrefix)
		=> In.AssemblyContaining<ClassWithMethods>().Types().Which(t => t == typeof(ClassWithMethods))
			.Methods().Which(methodInfo => methodInfo.Name.StartsWith(methodPrefix));

	public static Filtered.Types GetTypes<T>()
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
		private int PrivateMethod1() => 0;
		private int PrivateMethod2() => 0;
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
		public void GenericVoidMethod<T>() { }
		public T GenericMethodWithConstraint<T>(T value) where T : class => value;
	}
	// ReSharper restore UnusedMember.Local
#pragma warning restore CA1822
}
