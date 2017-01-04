using System;
using NUnit.Framework;

using SUT = Paraesthesia.Web.Configuration.UrlAbsolutifier;

namespace Paraesthesia.Test.Unit.Web.Configuration.UrlAbsolutifier
{
	[TestFixture]
	public class RuleConfigurationElement
	{
		[Test(Description = "Invokes the constructor with no parameters.")]
		public void Ctor_NoParams()
		{
			SUT.RuleConfigurationElement actual = new SUT.RuleConfigurationElement();
			Assert.AreEqual(SUT.RuleProcess.Include, actual.Process, "The Process property was not correctly set.");
			Assert.IsNull(actual.Type, "The Type property should be null.");
			Assert.AreEqual("none", actual.Value, "The Value property should be set to its non-empty default value.");
		}

		[Test(Description = "Invokes the constructor with a null rule type.")]
		[ExpectedException(typeof(System.ArgumentNullException))]
		public void Ctor_TypeNull()
		{
			SUT.RuleConfigurationElement actual = new SUT.RuleConfigurationElement(SUT.RuleProcess.Include, null, "valid_value");
		}

		[Test(Description = "Invokes the constructor with a rule type that doesn't derive from Rule.")]
		[ExpectedException(typeof(System.Configuration.ConfigurationErrorsException))]
		public void Ctor_TypeWrongSubclass()
		{
			SUT.RuleConfigurationElement actual = new SUT.RuleConfigurationElement(SUT.RuleProcess.Include, typeof(SUT.RuleConfigurationElement), "valid_value");
		}

		[Test(Description = "Invokes the constructor when all parameters are in acceptable ranges.")]
		public void Ctor_ValidParams()
		{
			SUT.RuleConfigurationElement actual = new SUT.RuleConfigurationElement(SUT.RuleProcess.Include, typeof(SUT.Rule), "valid_value");
			Assert.AreEqual(SUT.RuleProcess.Include, actual.Process, "The Process property was not correctly set.");
			Assert.AreEqual(typeof(SUT.Rule), actual.Type, "The Type property was not correctly set.");
			Assert.AreEqual("valid_value", actual.Value, "The Value property was not correctly set.");
		}

		[Test(Description = "Invokes the constructor with an empty rule value.")]
		[ExpectedException(typeof(System.Configuration.ConfigurationErrorsException))]
		public void Ctor_ValueEmpty()
		{
			SUT.RuleConfigurationElement actual = new SUT.RuleConfigurationElement(SUT.RuleProcess.Include, typeof(SUT.Rule), "");
		}

		[Test(Description = "Invokes the constructor with a null rule value.")]
		[ExpectedException(typeof(System.Configuration.ConfigurationErrorsException))]
		public void Ctor_ValueNull()
		{
			SUT.RuleConfigurationElement actual = new SUT.RuleConfigurationElement(SUT.RuleProcess.Include, typeof(SUT.Rule), "");
		}
	}
}
