using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect.Tests;

public sealed partial class ThatDictionary
{
	public sealed class DoesNotContainValue
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenNullValueDoesNotExist_ShouldSucceed()
			{
				IDictionary<int, int?> subject = ToDictionary<int, int?>([1, 2, 3,], [41, 42, 43,]);

				async Task Act()
					=> await That(subject).DoesNotContainValue(null);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenNullValueExists_ShouldFail()
			{
				IDictionary<int, int?> subject = ToDictionary<int, int?>([1, 2, 3,], [41, null, 43,]);

				async Task Act()
					=> await That(subject).DoesNotContainValue(null);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain value <null>,
					             but it did

					             Dictionary:
					             {[1] = 41, [2] = <null>, [3] = 43}
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Dictionary<int, string>? subject = null;

				async Task Act()
					=> await That(subject).DoesNotContainValue("foo");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain value "foo",
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenValueExists_ShouldFail()
			{
				IDictionary<int, int> subject = ToDictionary([1, 2, 3,], [41, 42, 43,]);

				async Task Act()
					=> await That(subject).DoesNotContainValue(42);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain value 42,
					             but it did

					             Dictionary:
					             {[1] = 41, [2] = 42, [3] = 43}
					             """);
			}

			[Fact]
			public async Task WhenValueIsANumberOfADifferentType_ShouldFail()
			{
				Dictionary<string, object> subject = new() { ["a"] = 1, };

				async Task Act()
					=> await That(subject).DoesNotContainValue(1L);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain value 1,
					             but it did

					             Dictionary:
					             {
					               ["a"] = 1
					             }
					             """);
			}

			[Fact]
			public async Task WhenValueIsMissing_ShouldSucceed()
			{
				IDictionary<int, int> subject = ToDictionary([1, 2, 3,], [41, 42, 43,]);

				async Task Act()
					=> await That(subject).DoesNotContainValue(2);

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class OverloadTests
		{
			[Fact]
			public async Task ForASortedDictionary_ShouldBindToTheDictionaryOverload()
			{
				SortedDictionary<string, int> subject = new() { { "a", 1 }, };

				async Task Act()
					=> await (AndOrResult<IDictionary<string, int>, IThat<IDictionary<string, int>?>>)
						That(subject).DoesNotContainValue(2);

				await That(Act).DoesNotThrow()
					.Because("a type that implements both dictionary interfaces must not become ambiguous");
			}
		}
	}
}
