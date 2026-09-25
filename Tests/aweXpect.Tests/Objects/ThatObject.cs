using System.Collections.Generic;

// ReSharper disable UnusedAutoPropertyAccessor.Local

namespace aweXpect.Tests;

public sealed partial class ThatObject
{
	private class MyBaseClass
	{
		public int Value { get; set; }
	}

	private sealed class MyClass : MyBaseClass;

	private sealed class OtherClass;

	private sealed class InnerClass
	{
		public IEnumerable<string>? Collection { get; set; }

		public InnerClass? Inner { get; set; }
		public string? Value { get; set; }
		
		public int IntValue { get; set; }
	}

	private sealed class OuterClass
	{
		public InnerClass? Inner { get; set; }
		public string? Value { get; set; }
	}

	private struct MyStruct
	{
		public int Value { get; set; }
	}

	private class MyGenericBaseClass : List<int>;

	private sealed class MyGenericDerivedClass : MyGenericBaseClass;

	private sealed class MyDictionaryClass : Dictionary<string, int>;

	private sealed class ThrowingEqualsClass(Exception exception)
	{
		public override bool Equals(object? obj) => throw exception;

		public override int GetHashCode() => 0;
	}
}
