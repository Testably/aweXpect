using System.Text;

namespace aweXpect.Core.Tests.Formatting;

public partial class ValueFormatters
{
	public sealed class EnumTests
	{
		public enum Dummy
		{
			Foo,
			Bar,
		}

		[Flags]
		public enum MyFlags
		{
			None = 0,
			A = 1,
			B = 2,
			C = 4,
		}

		[Test]
		[Arguments(Dummy.Foo, "Foo")]
		[Arguments(Dummy.Bar, "Bar")]
		[Arguments(null, "<null>")]
		public async Task Nullable_ShouldUseStringRepresentation(Dummy? value, string expectedResult)
		{
			StringBuilder sb = new();

			string result = Formatter.Format(value);
			string objectResult = Formatter.Format((object?)value);
			Formatter.Format(sb, value);

			await That(result).IsEqualTo(expectedResult);
			await That(objectResult).IsEqualTo(expectedResult);
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Test]
		[Arguments(Dummy.Foo, "ValueFormatters.EnumTests.Dummy Foo")]
		[Arguments(Dummy.Bar, "ValueFormatters.EnumTests.Dummy Bar")]
		[Arguments(null, "<null>")]
		public async Task Nullable_WithType_ShouldUseStringRepresentation(Dummy? value, string expectedResult)
		{
			StringBuilder sb = new();

			string result = Formatter.Format(value, FormattingOptions.WithType);
			string objectResult = Formatter.Format((object?)value, FormattingOptions.WithType);
			Formatter.Format(sb, value, FormattingOptions.WithType);

			await That(result).IsEqualTo(expectedResult);
			await That(objectResult).IsEqualTo(expectedResult);
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Test]
		public async Task InFailureMessage_WhenCombinedFlagsAreCollectionItems_ShouldKeepThemApartFromTheOtherItems()
		{
			MyFlags[] subject = [MyFlags.A | MyFlags.B, MyFlags.A,];

			async Task Act()
				=> await That(subject).IsNull();

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is null,
				             but it was [
				                 A | B,
				                 A
				               ]
				             """);
		}

		[Test]
		[Arguments(MyFlags.A | MyFlags.B, "A | B")]
		[Arguments(MyFlags.A | MyFlags.B | MyFlags.C, "A | B | C")]
		[Arguments(MyFlags.None, "None")]
		[Arguments((MyFlags)8, "8")]
		public async Task ShouldJoinCombinedFlagsLikeInCSharp(MyFlags value, string expectedResult)
		{
			StringBuilder sb = new();

			string result = Formatter.Format(value);
			string objectResult = Formatter.Format((object?)value);
			string withTypeResult = Formatter.Format(value, FormattingOptions.WithType);
			Formatter.Format(sb, value);

			await That(result).IsEqualTo(expectedResult)
				.Because("a comma reads like the separator of collection items");
			await That(objectResult).IsEqualTo(expectedResult);
			await That(withTypeResult).IsEqualTo($"ValueFormatters.EnumTests.MyFlags {expectedResult}");
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Test]
		[Arguments(Dummy.Foo, "Foo")]
		[Arguments(Dummy.Bar, "Bar")]
		public async Task ShouldUseStringRepresentation(Dummy value, string expectedResult)
		{
			StringBuilder sb = new();

			string result = Formatter.Format(value);
			string objectResult = Formatter.Format((object?)value);
			Formatter.Format(sb, value);

			await That(result).IsEqualTo(expectedResult);
			await That(objectResult).IsEqualTo(expectedResult);
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Test]
		public async Task WhenNull_ShouldUseDefaultNullString()
		{
			Dummy? value = null;
			StringBuilder sb = new();

			string result = Formatter.Format(value);
			string objectResult = Formatter.Format((object?)value);
			Formatter.Format(sb, value);

			await That(result).IsEqualTo(ValueFormatter.NullString);
			await That(objectResult).IsEqualTo(ValueFormatter.NullString);
			await That(sb.ToString()).IsEqualTo(ValueFormatter.NullString);
		}

		[Test]
		[Arguments(Dummy.Foo, "ValueFormatters.EnumTests.Dummy Foo")]
		[Arguments(Dummy.Bar, "ValueFormatters.EnumTests.Dummy Bar")]
		public async Task WithType_ShouldUseStringRepresentation(Dummy value, string expectedResult)
		{
			StringBuilder sb = new();

			string result = Formatter.Format(value, FormattingOptions.WithType);
			string objectResult = Formatter.Format((object?)value, FormattingOptions.WithType);
			Formatter.Format(sb, value, FormattingOptions.WithType);

			await That(result).IsEqualTo(expectedResult);
			await That(objectResult).IsEqualTo(expectedResult);
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}
	}
}
