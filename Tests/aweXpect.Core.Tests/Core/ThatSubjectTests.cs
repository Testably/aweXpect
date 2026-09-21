using System.Collections.Generic;

namespace aweXpect.Core.Tests.Core;

public sealed class ThatSubjectTests
{
	[Fact]
	public async Task Is_WhenSubjectIsNull_ShouldFail()
	{
		object? subject = null;

		async Task Act()
			=> await That(subject).Is<string>();

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that subject
			             is of type string,
			             but it was <null>
			             """);
	}

	[Fact]
	public async Task Is_WhenTypeDoesNotMatch_ShouldIncludeTheActualTypeAndValue()
	{
		object subject = new List<int>
		{
			1,
			2,
		};

		async Task Act()
			=> await That(subject).Is<IDictionary<int, string>>();

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that subject
			             is of type IDictionary<int, string>,
			             but it was List<int>

			             Actual:
			             [
			               1,
			               2
			             ]
			             """);
	}

	[Fact]
	public async Task Is_WhenValueTypeDoesNotMatch_ShouldIncludeTheActualTypeAndValue()
	{
		async Task Act()
			=> await AssertIsString(42);

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that value
			             is of type string,
			             but it was int

			             Actual:
			             42
			             """);

		static async Task AssertIsString<T>(T value)
			=> await That(value).Is<string>();
	}

	[Fact]
	public async Task IsExactly_WhenTypeIsSubtype_ShouldIncludeTheActualTypeAndValue()
	{
		object subject = new Outer<string>.Derived
		{
			Value = 1,
		};

		async Task Act()
			=> await That(subject).IsExactly<Outer<string>.Base>();

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that subject
			             is exactly of type ThatSubjectTests.Outer<string>.Base,
			             but it was ThatSubjectTests.Outer<string>.Derived

			             Actual:
			             ThatSubjectTests.Outer<string>.Derived {
			               Value = 1
			             }
			             """);
	}

	[Fact]
	public async Task IsNot_WhenTypeMatches_ShouldIncludeTheActualTypeAndValue()
	{
		object subject = 5;

		async Task Act()
			=> await That(subject).IsNot<int>();

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that subject
			             is not of type int,
			             but it was int

			             Actual:
			             5
			             """);
	}

	[Fact]
	public async Task IsNotExactly_WhenTypeMatches_ShouldIncludeTheActualTypeAndValue()
	{
		object subject = "foo";

		async Task Act()
			=> await That(subject).IsNotExactly<string>();

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that subject
			             is not exactly of type string,
			             but it was string

			             Actual:
			             "foo"
			             """);
	}

	[Fact]
	public async Task WithInner_WhenTypeDoesNotMatch_ShouldIncludeTheActualTypeAndValue()
	{
		void Throwing()
			=> throw new InvalidOperationException("outer", new ArgumentException("inner"));

		async Task Act()
			=> await That(Throwing).Throws<InvalidOperationException>()
				.WithInner(it => it.Is<InvalidCastException>());

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that Throwing
			             throws an InvalidOperationException with an inner exception which is of type InvalidCastException,
			             but it was ArgumentException

			             Actual:
			             ArgumentException: inner
			             """);
	}

	private sealed class Outer<T>
	{
		public class Base
		{
			public int Value { get; set; }
		}

		public sealed class Derived : Base;
	}
}
