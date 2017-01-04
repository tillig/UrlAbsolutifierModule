using System;
using System.ComponentModel;
using NUnit.Framework;

using SUT = Paraesthesia.Web.Configuration.UrlAbsolutifier;

namespace Paraesthesia.Test.Unit.Web.Configuration.UrlAbsolutifier
{
	[TestFixture]
	public class Rule
	{
		[Test(Description = "If there is no type converter, the rule value should pass through the constructor.")]
		public void Ctor_NullConverterPassesValueThrough()
		{
			RuleNullConverter rule = new RuleNullConverter(SUT.RuleProcess.Exclude, "value!");
			Assert.AreEqual("value!", rule.Value, "The value should have been passed directly through.");
		}

		[Test(Description = "If the value is null it should pass through.")]
		public void Ctor_NullValue()
		{
			RuleIntConverter rule = new RuleIntConverter(SUT.RuleProcess.Exclude, null);
			Assert.IsNull(rule.Value, "The value should remain null.");
		}

		[Test(Description = "If there is a type converter, the rule value should be converted.")]
		public void Ctor_TypeConverterValueConversion()
		{
			RuleIntConverter rule = new RuleIntConverter(SUT.RuleProcess.Exclude, "13579");
			Assert.AreEqual(13579, rule.Value, "The value should have been converted by the converter.");
		}

		[Test(Description = "Checks that the constructor sets the Process property.")]
		public void Ctor_SetsProcess()
		{
			// Try two different enum values to ensure we're not just getting the default.
			RuleNullConverter rule1 = new RuleNullConverter(SUT.RuleProcess.Exclude, null);
			Assert.AreEqual(SUT.RuleProcess.Exclude, rule1.Process, "The value should have been set to Exclude.");
			RuleNullConverter rule2 = new RuleNullConverter(SUT.RuleProcess.Include, null);
			Assert.AreEqual(SUT.RuleProcess.Include, rule2.Process, "The value should have been set to Include.");
		}

		/// <summary>
		/// Test implementation of a rule that converts the value to an int.
		/// </summary>
		private class RuleIntConverter : SUT.Rule
		{
			public RuleIntConverter(SUT.RuleProcess process, string ruleValue) : base(process, ruleValue) { }

			public override bool ContextMatchesRule(System.Web.HttpContext context)
			{
				throw new Exception("The method or operation is not implemented.");
			}

			public override TypeConverter ValueConverter
			{
				get { return new Int32Converter(); }
			}
		}

		/// <summary>
		/// Test implementation of a rule that doesn't do type conversion.
		/// </summary>
		private class RuleNullConverter : SUT.Rule
		{
			public RuleNullConverter(SUT.RuleProcess process, string ruleValue) : base(process, ruleValue) { }

			public override bool ContextMatchesRule(System.Web.HttpContext context)
			{
				throw new Exception("The method or operation is not implemented.");
			}

			public override TypeConverter ValueConverter
			{
				get { return null; }
			}
		}
	}
}
