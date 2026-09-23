using System.Collections.Generic;
using System.Linq;

namespace aweXpect.Tests;

public sealed class PluralMemberGrammar
{
	public sealed class Tests
	{
		[Fact]
		public async Task Booleans_ShouldUsePluralVerb()
		{
			Container<bool> subject = new(true, false);

			async Task Act()
				=> await That(subject).Whose(c => c.Items, items => items.All().ComplyWith(x => x.IsTrue()));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             whose Items are True for all items,
				             but only 1 of 2 were
				             *
				             """).AsWildcard();
		}

		[Fact]
		public async Task Booleans_WhenNegated_ShouldUsePluralVerb()
		{
			Container<bool> subject = new(true, false);

			async Task Act()
				=> await That(subject).Whose(c => c.Items, items => items.All().ComplyWith(x => x.IsNotEqualTo(false)));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             whose Items are not False for all items,
				             but only 1 of 2 were
				             *
				             """).AsWildcard();
		}

		[Fact]
		public async Task Chars_ShouldUsePluralVerb()
		{
			Container<char> subject = new('a', '1');

			async Task Act()
				=> await That(subject).Whose(c => c.Items, items => items.All().ComplyWith(x => x.IsALetter()));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             whose Items are a letter for all items,
				             but only 1 of 2 were
				             *
				             """).AsWildcard()
				.Because("the generated expectations derive the plural form from their leading verb");
		}

		[Fact]
		public async Task Chars_WhenNegated_ShouldUsePluralVerb()
		{
			Container<char> subject = new('a', '1');

			async Task Act()
				=> await That(subject).Whose(c => c.Items, items => items.All().ComplyWith(x => x.IsNotALetter()));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             whose Items are not a letter for all items,
				             but only 1 of 2 were
				             *
				             """).AsWildcard();
		}

		[Fact]
		public async Task DateTimes_ShouldUsePluralVerb()
		{
			DateTime earlier = new(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			DateTime later = new(2021, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			Container<DateTime> subject = new(earlier, later);

			async Task Act()
				=> await That(subject).Whose(c => c.Items, items => items.All().ComplyWith(x => x.IsBefore(later)));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             whose Items are before 2021-01-01T00:00:00.0000000Z for all items,
				             but only 1 of 2 were
				             *
				             """).AsWildcard();
		}

		[Fact]
		public async Task DateTimes_WhenNegated_ShouldUsePluralVerb()
		{
			DateTime earlier = new(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			DateTime later = new(2021, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			Container<DateTime> subject = new(earlier, later);

			async Task Act()
				=> await That(subject).Whose(c => c.Items, items => items.All().ComplyWith(x => x.IsNotBefore(later)));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             whose Items are not before 2021-01-01T00:00:00.0000000Z for all items,
				             but only 1 of 2 were
				             *
				             """).AsWildcard();
		}

		[Fact]
		public async Task Dictionaries_ShouldUseSingularVerb()
		{
			Container<int> subject = new(1);

			async Task Act()
				=> await That(subject).Whose(c => c.Map, map => map.ContainsKey(2));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             whose Map contains key 2,
				             *
				             """).AsWildcard()
				.Because("a dictionary reads as a single lookup, not as a plural noun");
		}

		[Fact]
		public async Task Dictionaries_WhenDeclaredAsReadOnlyInterface_ShouldUseSingularVerb()
		{
			Container<int> subject = new(1);

			async Task Act()
				=> await That(subject).Whose(c => c.Lookup, lookup => lookup.ContainsKey(2));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             whose Lookup contains key 2,
				             *
				             """).AsWildcard()
				.Because("IReadOnlyDictionary<,> does not implement the non-generic IDictionary");
		}

		[Fact]
		public async Task Enums_ShouldUsePluralVerb()
		{
			Container<MyFlags> subject = new(MyFlags.A, MyFlags.B);

			async Task Act()
				=> await That(subject).Whose(c => c.Items, items => items.All().ComplyWith(x => x.HasFlag(MyFlags.A)));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             whose Items have flag A for all items,
				             but only 1 of 2 did
				             *
				             """).AsWildcard();
		}

		[Fact]
		public async Task Enums_WhenNegated_ShouldUsePluralVerb()
		{
			Container<MyFlags> subject = new(MyFlags.A, MyFlags.B);

			async Task Act()
				=> await That(subject).Whose(c => c.Items,
					items => items.All().ComplyWith(x => x.DoesNotHaveFlag(MyFlags.A)));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             whose Items do not have flag A for all items,
				             but only 1 of 2 did
				             *
				             """).AsWildcard();
		}

		[Fact]
		public async Task NestedSingularMember_ShouldUseSingularVerb()
		{
			Container<Container<int>> subject = new(new Container<int>(1), new Container<int>(2));

			async Task Act()
				=> await That(subject).Whose(c => c.Items,
					items => items.All().ComplyWith(x => x.Whose(y => y.Name, name => name.IsEqualTo("bar"))));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             whose Items have Name which is equal to "bar" for all items,
				             but none of 2 did
				             *
				             """).AsWildcard()
				.Because("the number of a member follows its own type, not the number of the enclosing subject");
		}

		[Fact]
		public async Task Numbers_ShouldUsePluralVerb()
		{
			Container<int> subject = new(5, 6);

			async Task Act()
				=> await That(subject).Whose(c => c.Items, items => items.All().ComplyWith(x => x.IsLessThan(6)));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             whose Items are less than 6 for all items,
				             but only 1 of 2 were
				             *
				             """).AsWildcard();
		}

		[Fact]
		public async Task Numbers_WhenNegated_ShouldUsePluralVerb()
		{
			Container<int> subject = new(5, 6);

			async Task Act()
				=> await That(subject).Whose(c => c.Items, items => items.All().ComplyWith(x => x.IsNotEqualTo(6)));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             whose Items are not equal to 6 for all items,
				             but only 1 of 2 were
				             *
				             """).AsWildcard();
		}

		[Fact]
		public async Task Objects_ShouldUsePluralVerb()
		{
			Container<object> subject = new(1, "a");

			async Task Act()
				=> await That(subject).Whose(c => c.Items, items => items.All().ComplyWith(x => x.IsEqualTo(1)));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             whose Items are equal to 1 for all items,
				             but only 1 of 2 were
				             *
				             """).AsWildcard();
		}

		[Fact]
		public async Task Objects_WhenAppliedToTheMemberItself_ShouldUsePluralVerbInResult()
		{
			Container<int> subject = new(1, 2);

			async Task Act()
				=> await That(subject).Whose(c => c.Items, items => items.IsNull());

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             whose Items are null,
				             but Items were [
				               1,
				               2
				             ]
				             """);
		}

		[Fact]
		public async Task Objects_WhenNegated_ShouldUsePluralVerb()
		{
			Container<object> subject = new(1, "a");

			async Task Act()
				=> await That(subject).Whose(c => c.Items, items => items.All().ComplyWith(x => x.IsNotEqualTo(1)));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             whose Items are not equal to 1 for all items,
				             but only 1 of 2 were
				             *
				             """).AsWildcard();
		}

		[Fact]
		public async Task Signs_ShouldUsePluralVerb()
		{
			Container<int> subject = new(-1, 1);

			async Task Act()
				=> await That(subject).Whose(c => c.Items, items => items.All().ComplyWith(x => x.IsPositive()));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             whose Items are positive for all items,
				             but only 1 of 2 were
				             *
				             """).AsWildcard();
		}

		[Fact]
		public async Task Signs_WhenNegated_ShouldUsePluralVerb()
		{
			Container<int> subject = new(-1, 1);

			async Task Act()
				=> await That(subject).Whose(c => c.Items, items => items.All().ComplyWith(x => x.IsNotPositive()));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             whose Items are not positive for all items,
				             but only 1 of 2 were
				             *
				             """).AsWildcard();
		}

		[Fact]
		public async Task Strings_ShouldUsePluralVerb()
		{
			Container<string> subject = new("", "a");

			async Task Act()
				=> await That(subject).Whose(c => c.Items, items => items.All().ComplyWith(x => x.IsEmpty()));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             whose Items are empty for all items,
				             but only 1 of 2 were
				             *
				             """).AsWildcard();
		}

		[Fact]
		public async Task Strings_WhenMemberIsAString_ShouldUseSingularVerb()
		{
			Container<int> subject = new(1);

			async Task Act()
				=> await That(subject).Whose(c => c.Name, name => name.IsEmpty());

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             whose Name is empty,
				             but Name was "foo"
				             """)
				.Because("a string is not treated as a collection of characters");
		}

		[Fact]
		public async Task Strings_WhenNegated_ShouldUsePluralVerb()
		{
			Container<string> subject = new("", "a");

			async Task Act()
				=> await That(subject).Whose(c => c.Items, items => items.All().ComplyWith(x => x.IsNotEmpty()));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             whose Items are not empty for all items,
				             but only 1 of 2 were
				             *
				             """).AsWildcard();
		}

		[Fact]
		public async Task Types_ShouldUsePluralVerb()
		{
			Container<object> subject = new(1, "a");

			async Task Act()
				=> await That(subject).Whose(c => c.Items, items => items.All().ComplyWith(x => x.IsExactly<string>()));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             whose Items are exactly of type string for all items,
				             but only 1 of 2 were
				             *
				             """).AsWildcard();
		}

		[Fact]
		public async Task Types_WhenNegated_ShouldUsePluralVerb()
		{
			Container<object> subject = new(1, "a");

			async Task Act()
				=> await That(subject).Whose(c => c.Items, items => items.All().ComplyWith(x => x.IsNotExactly<string>()));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             whose Items are not exactly of type string for all items,
				             but only 1 of 2 were
				             *
				             """).AsWildcard();
		}

		[Fact]
		public async Task WhenMemberIsSingular_ShouldUseSingularVerb()
		{
			Container<int> subject = new(1);

			async Task Act()
				=> await That(subject).Whose(c => c.Count, count => count.IsLessThan(1));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             whose Count is less than 1,
				             but Count was 1
				             """);
		}

		[Flags]
		private enum MyFlags
		{
			A = 1,
			B = 2,
		}

		private sealed class Container<T>(params T[] items)
		{
			public int Count => items.Length;
			public IEnumerable<T> Items => items;
			public IReadOnlyDictionary<int, T> Lookup => Map;
			public Dictionary<int, T> Map => Enumerable.Range(1, items.Length).ToDictionary(i => i, i => items[i - 1]);
			public string Name => "foo";
		}
	}
}
