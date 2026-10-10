namespace aweXpect.Tests;

public sealed partial class ThatString
{
	/// <summary>
	///     A subject that a custom match type cannot compare fails the expectation and its negation alike.
	/// </summary>
	public sealed class SubjectNotComparableTests
	{
		[Test]
		public async Task DoesNotComplyWith_IsEqualTo_AsNumber_WhenSubjectIsNoNumber_ShouldFail()
		{
			string subject = "foo";

			async Task Act()
				=> await That(subject).DoesNotComplyWith(it => it.IsEqualTo("1").AsNumber());

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is not the number "1",
				             but it was "foo", which is no number

				             Actual:
				             foo
				             """);
		}

		[Test]
		public async Task IsEqualTo_AsNumber_WhenSubjectIsNoNumber_ShouldFail()
		{
			string subject = "foo";

			async Task Act()
				=> await That(subject).IsEqualTo("1").AsNumber();

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is the number "1",
				             but it was "foo", which is no number

				             Actual:
				             foo
				             """);
		}

		[Test]
		public async Task IsNotEqualTo_AsNumber_WhenSubjectIsNoNumber_ShouldFail()
		{
			string subject = "foo";

			async Task Act()
				=> await That(subject).IsNotEqualTo("1").AsNumber();

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is not the number "1",
				             but it was "foo", which is no number

				             Actual:
				             foo
				             """)
				.Because("a subject that cannot be compared is not different from the unexpected value either");
		}

		[Test]
		public async Task IsNotOneOf_AsNumber_WhenSubjectIsNoNumber_ShouldFail()
		{
			string subject = "foo";

			async Task Act()
				=> await That(subject).IsNotOneOf("1", "2").AsNumber();

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is not one of ["1", "2"] as number,
				             but it was "foo", which is no number
				             """);
		}

		[Test]
		public async Task IsOneOf_AsNumber_WhenSubjectIsNoNumber_ShouldFail()
		{
			string subject = "foo";

			async Task Act()
				=> await That(subject).IsOneOf("1", "2").AsNumber();

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is one of ["1", "2"] as number,
				             but it was "foo", which is no number
				             """);
		}

		[Test]
		public async Task Whose_IsEqualTo_AsNumber_WhenMemberIsNoNumber_ShouldNameTheMember()
		{
			Container subject = new("foo");

			async Task Act()
				=> await That(subject).Whose(x => x.Value, value => value.IsEqualTo("1").AsNumber());

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             whose Value is the number "1",
				             but Value was "foo", which is no number

				             Actual (Value):
				             foo
				             """);
		}

		[Test]
		public async Task Whose_IsNotEqualTo_AsNumber_WhenMemberIsNoNumber_ShouldNameTheMember()
		{
			Container subject = new("foo");

			async Task Act()
				=> await That(subject).Whose(x => x.Value, value => value.IsNotEqualTo("1").AsNumber());

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             whose Value is not the number "1",
				             but Value was "foo", which is no number

				             Actual (Value):
				             foo
				             """)
				.Because("a member that cannot be compared is not different from the unexpected value either");
		}

		[Test]
		public async Task Whose_Whose_IsEqualTo_AsNumber_WhenNestedMemberIsNoNumber_ShouldNameTheNestedMember()
		{
			Outer subject = new(new Container("foo"));

			async Task Act()
				=> await That(subject).Whose(x => x.Inner,
					inner => inner.Whose(x => x.Value, value => value.IsEqualTo("1").AsNumber()));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             whose Inner has Value that is the number "1",
				             but Value was "foo", which is no number

				             Actual (Inner.Value):
				             foo
				             """);
		}

		private sealed class Container(string value)
		{
			public string Value { get; } = value;
		}

		private sealed class Outer(Container inner)
		{
			public Container Inner { get; } = inner;
		}
	}
}
