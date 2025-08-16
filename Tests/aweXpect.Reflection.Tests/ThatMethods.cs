using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethods
{
	public static Filtered.Methods GetMethods(string methodPrefix)
		=> In.AssemblyContaining<ClassWithMethods>().Types().Which(t => t == typeof(ClassWithMethods))
			.Methods().Which(methodInfo => methodInfo.Name.StartsWith(methodPrefix));

	public static Filtered.Types GetTypes<T>()
		=> In.AssemblyContaining<T>().Types().Which(t => t == typeof(T));

	private class DummyBase
	{
	}

	private class Dummy : DummyBase
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
	private class TestClass
	{
		public string GetString() => "test";
		public int GetInt() => 42;
		public bool GetBool() => true;
		public DummyBase GetDummyBase() => new();
		public Dummy GetDummy() => new();
		public async Task AsyncMethod() => await Task.CompletedTask;
	}
	// ReSharper restore UnusedMember.Local
#pragma warning restore CA1822
}
