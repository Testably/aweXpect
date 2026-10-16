using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using aweXpect.Core;

namespace aweXpect.Equivalency;

/// <summary>
///     Reads the value for an expected member from a differently named actual member, where both are given as paths
///     relative to an actual and an expected object that are compared with each other.
/// </summary>
internal abstract class MemberMapping(string[] actualPath, string[] expectedPath)
{
	/// <summary>
	///     The names of the members that are read on the actual object, outermost first.
	/// </summary>
	public string[] ActualPath { get; } = actualPath;

	/// <summary>
	///     The names of the members of the expected object that are compared with the value of the
	///     <see cref="ActualPath" />, outermost first.
	/// </summary>
	public string[] ExpectedPath { get; } = expectedPath;

	/// <summary>
	///     The expected member as given by the caller, when it can match no member, so that a failure can hint at it.
	/// </summary>
	public virtual string? ExpectedName => null;

	/// <summary>
	///     Whether the mapping applies to the members of the <paramref name="actual" /> and the
	///     <paramref name="expected" /> object.
	/// </summary>
	public abstract bool AppliesTo(object actual, object expected);

	/// <summary>
	///     Appends the mapping for the list of options.
	/// </summary>
	public abstract void AppendDescription(StringBuilder stringBuilder);

	/// <summary>
	///     Maps by the names of the members, for an expected member at any depth whose path ends with the
	///     <paramref name="expectedMember" />.
	/// </summary>
	internal sealed class ByName(string actualMember, string expectedMember)
		: MemberMapping(Split(actualMember, nameof(actualMember)), Split(expectedMember, nameof(expectedMember)))
	{
		/// <inheritdoc />
		public override string ExpectedName => expectedMember;

		/// <inheritdoc />
		public override bool AppliesTo(object actual, object expected) => true;

		/// <inheritdoc />
		public override void AppendDescription(StringBuilder stringBuilder)
		{
			Formatter.Format(stringBuilder, actualMember);
			stringBuilder.Append(" to ");
			Formatter.Format(stringBuilder, expectedMember);
		}

		private static string[] Split(string member, string paramName)
		{
			string[] names = member.Split('.');
			if (Array.Exists(names, name => name.Length == 0 || name.IndexOf('[') >= 0))
			{
				throw Tracing.WriteException(new ArgumentException(
					$"The '{paramName}' must be a member name or a dot-separated path of member names, e.g. \"Name\" or \"Artist.Name\", but was \"{member}\".",
					paramName));
			}

			return names;
		}
	}

	/// <summary>
	///     Maps the members of an actual <typeparamref name="TActual" /> and an expected
	///     <typeparamref name="TExpected" /> that the expressions select.
	/// </summary>
	/// <remarks>
	///     The expressions are only read for the names of their members and never compiled, so that a mapping also
	///     works when publishing with trimming or Native AOT enabled.
	/// </remarks>
	internal sealed class ByType<TActual, TExpected>(
		Expression<Func<TActual, object?>> actualMember,
		Expression<Func<TExpected, object?>> expectedMember)
		: MemberMapping(GetPath(actualMember, nameof(actualMember)), GetPath(expectedMember, nameof(expectedMember)))
	{
		/// <inheritdoc />
		public override bool AppliesTo(object actual, object expected)
			=> actual is TActual && expected is TExpected;

		/// <inheritdoc />
		public override void AppendDescription(StringBuilder stringBuilder)
		{
			Formatter.Format(stringBuilder, typeof(TActual));
			stringBuilder.Append('.').Append(string.Join(".", ActualPath)).Append(" to ");
			Formatter.Format(stringBuilder, typeof(TExpected));
			stringBuilder.Append('.').Append(string.Join(".", ExpectedPath));
		}

		/// <remarks>
		///     A value type member is boxed to <see langword="object" />, which wraps the member access in a conversion.
		/// </remarks>
		private static string[] GetPath(LambdaExpression expression, string paramName)
		{
			Expression? body = expression.Body;
			if (body is UnaryExpression { NodeType: ExpressionType.Convert or ExpressionType.ConvertChecked, } conversion)
			{
				body = conversion.Operand;
			}

			List<string> names = [];
			while (body is MemberExpression member)
			{
				names.Add(member.Member.Name);
				body = member.Expression;
			}

			if (names.Count == 0 || body != expression.Parameters[0])
			{
				throw Tracing.WriteException(new ArgumentException(
					$"The '{paramName}' must select a member of its parameter, e.g. x => x.Name or x => x.Artist.Name, but was {expression}.",
					paramName));
			}

			names.Reverse();
			return names.ToArray();
		}
	}
}
