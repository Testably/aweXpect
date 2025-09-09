namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	private class BaseClass;

	private class DerivedClass1 : BaseClass;

	private class DerivedClass2 : BaseClass;

	private class GrandChildClass : DerivedClass1;

	private class UnrelatedClass;

	private class ClassWithInterface1 : ITestInterface;

	private class ClassWithInterface2 : ITestInterface;

	private interface ITestInterface;

	private class GenericClassWithOneArgument<T>;

	private class GenericClassWithTwoArguments<TFoo, TBar> where TBar : BaseClass;
}
