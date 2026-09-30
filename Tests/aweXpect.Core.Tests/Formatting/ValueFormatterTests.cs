using System.Collections;
using System.Collections.Generic;
using System.Net;
#if NET8_0_OR_GREATER
using System.Runtime.InteropServices;
#endif
using System.Text;
using System.Threading;

namespace aweXpect.Core.Tests.Formatting;

public class ValueFormatterTests
{
	[Fact]
	public async Task CustomFormatter_InFailureMessage_ShouldBeUsedForTheSubjectAndTheExpectation()
	{
		DateTime subject = new(2020, 1, 2, 3, 4, 5);
		using IDisposable lifetime = ValueFormatter.Register(new CurrentFlowFormatter<DateTime>());

		async Task Act()
			=> await That(subject).IsEqualTo(subject.AddDays(1));

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that subject
			             is equal to custom,
			             but it was custom, which differs by -1.00:00:00
			             """);
	}

	[Fact]
	public async Task CustomFormatter_ShouldBeUsedByEveryTypedOverload()
	{
		string[] results;
		using (ValueFormatter.Register(new CurrentFlowFormatter<object>()))
		{
			results =
			[
				Formatter.Format(true),
				Formatter.Format((bool?)true),
				Formatter.Format('a'),
				Formatter.Format((char?)'a'),
				Formatter.Format("text"),
				Formatter.Format(typeof(int)),
				Formatter.Format(new InvalidOperationException("message")),
				Formatter.Format(MyEnum.Value),
				Formatter.Format(HttpStatusCode.OK),
				Formatter.Format(Guid.Empty),
				Formatter.Format((Guid?)Guid.Empty),
				Formatter.Format(new DateTime(2020, 1, 2)),
				Formatter.Format((DateTime?)new DateTime(2020, 1, 2)),
				Formatter.Format(new DateTimeOffset(2020, 1, 2, 3, 4, 5, TimeSpan.Zero)),
				Formatter.Format((DateTimeOffset?)new DateTimeOffset(2020, 1, 2, 3, 4, 5, TimeSpan.Zero)),
				Formatter.Format(TimeSpan.FromSeconds(1)),
				Formatter.Format((TimeSpan?)TimeSpan.FromSeconds(1)),
#if NET8_0_OR_GREATER
				Formatter.Format(new DateOnly(2020, 1, 2)),
				Formatter.Format((DateOnly?)new DateOnly(2020, 1, 2)),
				Formatter.Format(new TimeOnly(3, 4, 5)),
				Formatter.Format((TimeOnly?)new TimeOnly(3, 4, 5)),
				Formatter.Format((Half)1),
				Formatter.Format((Half?)1),
				Formatter.Format((NFloat)1),
				Formatter.Format((NFloat?)1),
#endif
				Formatter.Format((byte)1),
				Formatter.Format((byte?)1),
				Formatter.Format((sbyte)1),
				Formatter.Format((sbyte?)1),
				Formatter.Format((short)1),
				Formatter.Format((short?)1),
				Formatter.Format((ushort)1),
				Formatter.Format((ushort?)1),
				Formatter.Format(1),
				Formatter.Format((int?)1),
				Formatter.Format(1U),
				Formatter.Format((uint?)1),
				Formatter.Format(1L),
				Formatter.Format((long?)1),
				Formatter.Format(1UL),
				Formatter.Format((ulong?)1),
				Formatter.Format((nint)1),
				Formatter.Format((nint?)1),
				Formatter.Format((nuint)1),
				Formatter.Format((nuint?)1),
				Formatter.Format(1.5F),
				Formatter.Format((float?)1.5F),
				Formatter.Format(1.5),
				Formatter.Format((double?)1.5),
				Formatter.Format(1.5M),
				Formatter.Format((decimal?)1.5M),
				Formatter.Format(new[] { 1, 2, }),
				Formatter.Format(new Dictionary<int, int> { [1] = 2, }),
				Formatter.Format(new KeyValuePair<int, int>(1, 2)),
			];
		}

		await That(results).All().AreEqualTo("custom");
	}

	[Fact]
	public async Task CustomFormatter_ShouldBeUsedByEveryTypedOverloadWithStringBuilder()
	{
		string[] results;
		using (ValueFormatter.Register(new CurrentFlowFormatter<object>()))
		{
			results =
			[
				Append(sb => Formatter.Format(sb, true)),
				Append(sb => Formatter.Format(sb, (bool?)true)),
				Append(sb => Formatter.Format(sb, 'a')),
				Append(sb => Formatter.Format(sb, (char?)'a')),
				Append(sb => Formatter.Format(sb, "text")),
				Append(sb => Formatter.Format(sb, typeof(int))),
				Append(sb => Formatter.Format(sb, new InvalidOperationException("message"))),
				Append(sb => Formatter.Format(sb, MyEnum.Value)),
				Append(sb => Formatter.Format(sb, HttpStatusCode.OK)),
				Append(sb => Formatter.Format(sb, Guid.Empty)),
				Append(sb => Formatter.Format(sb, (Guid?)Guid.Empty)),
				Append(sb => Formatter.Format(sb, new DateTime(2020, 1, 2))),
				Append(sb => Formatter.Format(sb, (DateTime?)new DateTime(2020, 1, 2))),
				Append(sb => Formatter.Format(sb, new DateTimeOffset(2020, 1, 2, 3, 4, 5, TimeSpan.Zero))),
				Append(sb => Formatter.Format(sb,
					(DateTimeOffset?)new DateTimeOffset(2020, 1, 2, 3, 4, 5, TimeSpan.Zero))),
				Append(sb => Formatter.Format(sb, TimeSpan.FromSeconds(1))),
				Append(sb => Formatter.Format(sb, (TimeSpan?)TimeSpan.FromSeconds(1))),
#if NET8_0_OR_GREATER
				Append(sb => Formatter.Format(sb, new DateOnly(2020, 1, 2))),
				Append(sb => Formatter.Format(sb, (DateOnly?)new DateOnly(2020, 1, 2))),
				Append(sb => Formatter.Format(sb, new TimeOnly(3, 4, 5))),
				Append(sb => Formatter.Format(sb, (TimeOnly?)new TimeOnly(3, 4, 5))),
				Append(sb => Formatter.Format(sb, (Half)1)),
				Append(sb => Formatter.Format(sb, (Half?)1)),
				Append(sb => Formatter.Format(sb, (NFloat)1)),
				Append(sb => Formatter.Format(sb, (NFloat?)1)),
#endif
				Append(sb => Formatter.Format(sb, (byte)1)),
				Append(sb => Formatter.Format(sb, (byte?)1)),
				Append(sb => Formatter.Format(sb, (sbyte)1)),
				Append(sb => Formatter.Format(sb, (sbyte?)1)),
				Append(sb => Formatter.Format(sb, (short)1)),
				Append(sb => Formatter.Format(sb, (short?)1)),
				Append(sb => Formatter.Format(sb, (ushort)1)),
				Append(sb => Formatter.Format(sb, (ushort?)1)),
				Append(sb => Formatter.Format(sb, 1)),
				Append(sb => Formatter.Format(sb, (int?)1)),
				Append(sb => Formatter.Format(sb, 1U)),
				Append(sb => Formatter.Format(sb, (uint?)1)),
				Append(sb => Formatter.Format(sb, 1L)),
				Append(sb => Formatter.Format(sb, (long?)1)),
				Append(sb => Formatter.Format(sb, 1UL)),
				Append(sb => Formatter.Format(sb, (ulong?)1)),
				Append(sb => Formatter.Format(sb, (nint)1)),
				Append(sb => Formatter.Format(sb, (nint?)1)),
				Append(sb => Formatter.Format(sb, (nuint)1)),
				Append(sb => Formatter.Format(sb, (nuint?)1)),
				Append(sb => Formatter.Format(sb, 1.5F)),
				Append(sb => Formatter.Format(sb, (float?)1.5F)),
				Append(sb => Formatter.Format(sb, 1.5)),
				Append(sb => Formatter.Format(sb, (double?)1.5)),
				Append(sb => Formatter.Format(sb, 1.5M)),
				Append(sb => Formatter.Format(sb, (decimal?)1.5M)),
				Append(sb => Formatter.Format(sb, (IEnumerable)new[] { 1, 2, })),
				Append(sb => Formatter.Format(sb, new[] { 1, 2, })),
				Append(sb => Formatter.Format(sb, new Dictionary<int, int> { [1] = 2, })),
				Append(sb => Formatter.Format(sb, new KeyValuePair<int, int>(1, 2))),
			];
		}

		await That(results).All().AreEqualTo("custom");

		static string Append(Action<StringBuilder> format)
		{
			StringBuilder stringBuilder = new();
			format(stringBuilder);
			return stringBuilder.ToString();
		}
	}

	[Fact]
	public async Task CustomFormatter_ShouldBeUsedDuringLifetime()
	{
		MyFormattableClass value = new()
		{
			Value = 42,
		};
		using (ValueFormatter.Register(new MyCustomFormatter("my-string")))
		{
			string customObjectResult = Formatter.Format(value);

			await That(customObjectResult).IsEqualTo("my-string");
		}

		string objectResult = Formatter.Format(value);

		await That(objectResult).IsEqualTo("""
		                                   ValueFormatterTests.MyFormattableClass {
		                                     Value = 42
		                                   }
		                                   """);
	}

	[Fact]
	public async Task CustomFormatter_WhenDisposedTwice_ShouldOnlyRemoveItsOwnRegistration()
	{
		MyFormattableClass value = new();
		MyCustomFormatter formatter = new("my-string");
		using IDisposable first = ValueFormatter.Register(formatter);
		IDisposable second = ValueFormatter.Register(formatter);

		second.Dispose();
		second.Dispose();

		await That(Formatter.Format(value)).IsEqualTo("my-string")
			.Because("each registration is removed on its own, even when the same formatter was registered twice");
	}

	[Fact]
	public async Task CustomFormatter_WhenItFormatsACyclicChildCollection_ShouldFormatTheCycleAsRecursive()
	{
		Node a = new("a");
		Node b = new("b");
		a.Children.Add(b);
		b.Children.Add(a);
		string result;
		using (ValueFormatter.Register(new NodeFormatter()))
		{
			result = Formatter.Format(a, FormattingOptions.SingleLine);
		}

		await That(result).IsEqualTo("a with [b with [{ *recursive* }]]")
			.Because("the formatting context is carried into the formatting calls of a registered formatter");
	}

	[Fact]
	public async Task CustomFormatter_WhenItFormatsTheSameInstanceAsAChild_ShouldFormatItAsRecursive()
	{
		Node a = new("a");
		a.Parent = a;
		string result;
		using (ValueFormatter.Register(new NodeFormatter()))
		{
			result = Formatter.Format(a, FormattingOptions.SingleLine);
		}

		await That(result).IsEqualTo("a in { *recursive* }")
			.Because("a value is tracked while a registered formatter formats it");
	}

	[Fact]
	public async Task CustomFormatter_WhenItThrows_ShouldRenderAPlaceholderInTheFailureMessage()
	{
		MyThrowingFormattableClass subject = new();
		using IDisposable lifetime = ValueFormatter.Register(
			new MyThrowingCustomFormatter(new InvalidOperationException("formatter failed")));

		async Task Act()
			=> await That(subject).IsNull();

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that subject
			             is null,
			             but it was [the formatter did throw an InvalidOperationException: formatter failed]
			             """)
			.Because("what the formatter appended before it threw is discarded");
	}

	[Fact]
	public async Task CustomFormatter_WhenItThrowsForEveryValue_ShouldRenderAPlaceholder()
	{
		DateTime value = new(2020, 1, 2);
		string result;
		using (ValueFormatter.Register(
			       new CurrentFlowThrowingFormatter(new InvalidOperationException("formatter failed"))))
		{
			result = Formatter.Format(value);
		}

		await That(result).IsEqualTo("[the formatter did throw an InvalidOperationException: formatter failed]")
			.Because("the type of the exception in the placeholder is not passed to the formatter again");
	}

	[Fact]
	public async Task CustomFormatter_WhenMultipleAreRegistered_ShouldUseTheMostRecentOne()
	{
		MyFormattableClass value = new();
		using IDisposable first = ValueFormatter.Register(new MyCustomFormatter("first"));
		using IDisposable second = ValueFormatter.Register(new MyCustomFormatter("second"));
		using IDisposable third = ValueFormatter.Register(new MyCustomFormatter("third"));

		await That(Formatter.Format(value)).IsEqualTo("third")
			.Because("the most recently registered formatter takes precedence");
	}

	[Fact]
	public async Task CustomFormatter_WhenNull_ShouldUseDefaultNullString()
	{
		using IDisposable lifetime = ValueFormatter.Register(new MyCustomFormatter("my-string"));
		bool? value = null;

		string objectResult = Formatter.Format((object?)value);

		await That(objectResult).IsEqualTo(ValueFormatter.NullString);
	}

	[Fact]
	public async Task CustomFormatter_WhenSameInstanceIsContainedTwice_ShouldFormatBoth()
	{
		Node leaf = new("leaf");
		Node root = new("root");
		root.Children.Add(leaf);
		root.Children.Add(new Node("x")
		{
			Parent = leaf,
		});
		string result;
		using (ValueFormatter.Register(new NodeFormatter()))
		{
			result = Formatter.Format(root, FormattingOptions.SingleLine);
		}

		await That(result).IsEqualTo("root with [leaf, x in leaf]")
			.Because("a value is only tracked while it is being formatted, not after");
	}

	[Fact]
	public async Task CustomFormatter_WhenTheMostRecentOneIsDisposed_ShouldFallBackToTheEarlierOne()
	{
		MyFormattableClass value = new();
		using IDisposable first = ValueFormatter.Register(new MyCustomFormatter("first"));
		using IDisposable second = ValueFormatter.Register(new MyCustomFormatter("second"));
		IDisposable third = ValueFormatter.Register(new MyCustomFormatter("third"));

		third.Dispose();

		await That(Formatter.Format(value)).IsEqualTo("second")
			.Because("disposing the most recent formatter restores the precedence of the earlier one");
	}

	[Fact]
	public async Task CustomFormatter_WhenTypeDoesNotMatch_ShouldDoNothing()
	{
		int value = 1;
		using (ValueFormatter.Register(new MyCustomFormatter("my-string")))
		{
			string customObjectResult = Formatter.Format((object?)value);

			await That(customObjectResult).IsEqualTo("1");
		}
	}

	/// <remarks>
	///     Registrations are process-wide, so it only formats values in the test that created it and leaves the
	///     tests running in parallel alone.
	/// </remarks>
	private sealed class CurrentFlowFormatter<T> : IValueFormatter
	{
		private readonly AsyncLocal<bool> _isCurrentFlow = new()
		{
			Value = true,
		};

		public bool TryFormat(StringBuilder stringBuilder, object value, FormattingOptions? options)
		{
			if (_isCurrentFlow.Value && value is T)
			{
				stringBuilder.Append("custom");
				return true;
			}

			return false;
		}
	}

	/// <remarks>
	///     Registrations are process-wide, so it only throws in the test that created it and leaves the tests
	///     running in parallel alone.
	/// </remarks>
	private sealed class CurrentFlowThrowingFormatter(Exception exception) : IValueFormatter
	{
		private readonly AsyncLocal<bool> _isCurrentFlow = new()
		{
			Value = true,
		};

		public bool TryFormat(StringBuilder stringBuilder, object value, FormattingOptions? options)
		{
			if (_isCurrentFlow.Value)
			{
				throw exception;
			}

			return false;
		}
	}

	private sealed class MyCustomFormatter(string formatString) : IValueFormatter
	{
		public bool TryFormat(StringBuilder stringBuilder, object value, FormattingOptions? options)
		{
			if (value is MyFormattableClass)
			{
				stringBuilder.Append(formatString);
				return true;
			}

			return false;
		}
	}

	private sealed class MyFormattableClass
	{
		public int Value { get; set; }
	}

	private enum MyEnum
	{
		Value,
	}

	private sealed class MyThrowingCustomFormatter(Exception exception) : IValueFormatter
	{
		public bool TryFormat(StringBuilder stringBuilder, object value, FormattingOptions? options)
		{
			if (value is MyThrowingFormattableClass)
			{
				stringBuilder.Append("partial");
				throw exception;
			}

			return false;
		}
	}

	private sealed class MyThrowingFormattableClass;

	private sealed class Node(string name)
	{
		public List<Node> Children { get; } = [];
		public string Name { get; } = name;
		public Node? Parent { get; set; }
	}

	/// <remarks>
	///     Throws instead of overflowing the stack when a cycle is not detected, so that a failing test does not
	///     crash the test host.
	/// </remarks>
	private sealed class NodeFormatter : IValueFormatter
	{
		private int _depth;

		public bool TryFormat(StringBuilder stringBuilder, object value, FormattingOptions? options)
		{
			if (value is not Node node)
			{
				return false;
			}

			try
			{
				if (++_depth > 10)
				{
					throw new InvalidOperationException("the cycle was not detected");
				}

				stringBuilder.Append(node.Name);
				if (node.Parent is not null)
				{
					stringBuilder.Append(" in ");
					Formatter.Format(stringBuilder, node.Parent, options);
				}

				if (node.Children.Count > 0)
				{
					stringBuilder.Append(" with ");
					Formatter.Format(stringBuilder, node.Children, options);
				}

				return true;
			}
			finally
			{
				_depth--;
			}
		}
	}
}
