using aweXpect.Core.Extending;
using aweXpect.Core.Sources;

namespace aweXpect.Core.Tests.Core.Extending;

public sealed class ThatExtensionsTests
{
	[Test]
	public async Task Get_WhenClassDoesNotHaveAnExpectationBuilder_ShouldThrowNotSupportedException()
	{
		IThat<int> subject = new ThatWith();

		IExpectThat<int> Act() => subject.Get();

		await That(Act).Throws<NotSupportedException>()
			.WithMessage("IThat<T> must also implement IExpectThat<T>.");
	}

	[Test]
	public async Task Get_WhenClassImplementsBoth_ShouldReturnSameObject()
	{
		IThat<int> subject = new ThatWithExpectThat();

		IExpectThat<int> result = subject.Get();

		await That(result).IsSameAs(subject);
	}

	private sealed class ThatWith : IThat<int>;

	private sealed class ThatWithExpectThat : IExpectThat<int>
	{
		public ExpectationBuilder ExpectationBuilder { get; }
			= new ExpectationBuilder<int>(new ValueSource<int>(0), "");
	}
}
