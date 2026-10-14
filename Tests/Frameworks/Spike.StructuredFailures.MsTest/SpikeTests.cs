using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace aweXpect.Spike.MsTest;

[TestClass]
public sealed class SpikeTests
{
	private const string Actual = "The quick brown fox\njumps over the lazy dog";
	private const string Expected = "The quick brown cat\njumps over the lazy dog";

	[TestMethod]
	public async Task AweXpect_StringIsEqualTo()
		=> await Expect.That(Actual).IsEqualTo(Expected);

	[TestMethod]
	public async Task AweXpect_StringIsEqualTo_ShowsExceptionData()
	{
		try
		{
			await Expect.That(Actual).IsEqualTo(Expected);
		}
		catch (Exception ex)
		{
			Assert.Fail($"expected=[{ex.Data["assert.expected"]}] actual=[{ex.Data["assert.actual"]}] type={ex.GetType()}");
		}
	}

	[TestMethod]
	public void Inspect_AssertFailedException()
	{
		System.Reflection.BindingFlags all = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static |
		                                     System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic |
		                                     System.Reflection.BindingFlags.DeclaredOnly;
		Type type = typeof(AssertFailedException);
		System.Text.StringBuilder sb = new();
		foreach (var m in type.GetMembers(all))
		{
			sb.AppendLine($"{m.MemberType} {m}");
		}

		AssertFailedException ex = new("x");
		ex.Data["assert.expected"] = "E";
		ex.Data["assert.actual"] = "A";
		sb.AppendLine($"ExpectedText after Data: [{ex.ExpectedText}] ActualText: [{ex.ActualText}]");
		try
		{
			Assert.AreEqual(1, 2);
		}
		catch (AssertFailedException native)
		{
			foreach (System.Collections.DictionaryEntry e in native.Data)
			{
				sb.AppendLine($"native Data[{e.Key}] = [{e.Value}]");
			}
		}

		Assert.Fail(sb.ToString());
	}

	[TestMethod]
	public void MsTest_AreEqual()
		=> Assert.AreEqual(Expected, Actual);
}
