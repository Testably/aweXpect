namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public class PublicType;

	internal class InternalType;

#pragma warning disable CS0628
	protected class ProtectedType;
#pragma warning restore CS0628

	private class PrivateType;

	private class BaseClass;

	private class DerivedClass : BaseClass;

	private class GrandChildClass : DerivedClass;

	private class UnrelatedClass;

	private class ClassWithInterface : ITestInterface;

	private interface ITestInterface;
}
