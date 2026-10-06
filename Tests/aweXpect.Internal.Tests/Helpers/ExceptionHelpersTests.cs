using System.Collections.Generic;
using System.Linq;
using aweXpect.Helpers;

namespace aweXpect.Internal.Tests.Helpers;

public class ExceptionHelpersTests
{
	[Test]
	public async Task GetInnerExceptions_WhenNull_ShouldBeEmpty()
	{
		Exception? exception = null;

		IEnumerable<Exception> result = exception.GetInnerExceptions();

		await That(result).IsEmpty();
	}

	[Test]
	public async Task WithoutNullElements_WhenCollectionContainsNull_ShouldThrowAtTheCall()
	{
		string[] parameter = ["a", null!,];

		void Act()
			=> _ = parameter.WithoutNullElements();

		await That(Act).Throws<ArgumentException>()
			.WithParamName("parameter").And
			.WithMessage("The 'parameter' collection cannot contain <null>.").AsPrefix();
	}

	[Test]
	public async Task WithoutNullElements_WhenCollectionHasNoNull_ShouldReturnTheSameInstance()
	{
		string[] parameter = ["a", "b",];

		IEnumerable<string> result = parameter.WithoutNullElements();

		await That(result).IsSameAs(parameter);
	}

	[Test]
	public async Task WithoutNullElements_WhenCollectionOfALessSpecificTypeContainsNull_ShouldThrowAtTheCall()
	{
		List<Action<object>> list = [_ => { }, null!,];
		IEnumerable<Action<string>> parameter = list;

		void Act()
			=> _ = parameter.WithoutNullElements();

		await That(Act).Throws<ArgumentException>()
			.WithParamName("parameter");
	}

	[Test]
	[Arguments(false, "expected")]
	[Arguments(true, "unexpected")]
	public async Task WithoutNullElements_WhenNegated_ShouldNameTheParameterAfterThePolarity(bool negated,
		string expectedParamName)
	{
		string[] parameter = [null!,];

		void Act()
			=> _ = parameter.WithoutNullElements(negated);

		await That(Act).Throws<ArgumentException>()
			.WithParamName(expectedParamName).And
			.WithMessage($"The '{expectedParamName}' collection cannot contain <null>.").AsPrefix();
	}

	[Test]
	public async Task WithoutNullElements_WhenNull_ShouldReturnNull()
	{
		IEnumerable<string>? parameter = null;

		IEnumerable<string>? result = parameter.WithoutNullElements();

		await That(result).IsNull();
	}

	[Test]
	public async Task WithoutNullElements_WhenSequenceContainsNull_ShouldOnlyThrowWhenTheNullIsEnumerated()
	{
		int enumerations = 0;

		IEnumerable<string> Parameter()
		{
			enumerations++;
			yield return "a";
			yield return null!;
		}

		IEnumerable<string> result = Parameter().WithoutNullElements(true);
		int enumerationsAfterTheCall = enumerations;
		string firstElement = result.First();

		void Act()
			=> _ = result.ToArray();

		await That(enumerationsAfterTheCall).IsEqualTo(0);
		await That(firstElement).IsEqualTo("a");
		await That(Act).Throws<ArgumentException>()
			.WithParamName("unexpected").And
			.WithMessage("The 'unexpected' collection cannot contain <null>.").AsPrefix();
	}

	[Test]
	public async Task WithoutNullElements_WhenSequenceHasNoNull_ShouldEnumerateItOncePerEnumeration()
	{
		int enumerations = 0;

		IEnumerable<string> Parameter()
		{
			enumerations++;
			yield return "a";
			yield return "b";
		}

		IEnumerable<string> result = Parameter().WithoutNullElements();
		string[] elements = result.ToArray();

		await That(elements).IsEqualTo(["a", "b",]);
		await That(enumerations).IsEqualTo(1);
	}
}
