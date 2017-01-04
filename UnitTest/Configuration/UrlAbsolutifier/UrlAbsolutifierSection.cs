using System;
using System.Configuration;
using System.Reflection;
using NUnit.Framework;
using SUT = Paraesthesia.Web.Configuration.UrlAbsolutifier;

namespace Paraesthesia.Test.Unit.Web.Configuration.UrlAbsolutifier
{
	[TestFixture]
	public class UrlAbsolutifierSection
	{
		// A few tests will suffice to make sure things seem to be working right;
		// too many and we're just testing the framework (which we have to assume has already been tested).

		SUT.UrlAbsolutifierSection _section = null;

		[SetUp]
		public void SetUp()
		{
			this._section = new SUT.UrlAbsolutifierSection();
		}

		[Test(Description = "Deserializes an empty section.")]
		public void DeserializeEmpty()
		{
			DeserializeSection(this._section, "<section><clear /></section>");
			Assert.IsEmpty(this._section.Rules, "The section should not contain any rules.");
		}

		[Test(Description = "Deserializes a section with multiple rules.")]
		public void DeserializeMultiple()
		{
			DeserializeSection(
				this._section,
				"<section>" +
				"<add process=\"Include\" type=\"Paraesthesia.Web.Configuration.UrlAbsolutifier.Rule, Paraesthesia.Web.UrlAbsolutifierModule\" value=\"value1\" />" +
				"<add process=\"Include\" type=\"Paraesthesia.Web.Configuration.UrlAbsolutifier.Rule, Paraesthesia.Web.UrlAbsolutifierModule\" value=\"value2\" />" +
				"<add process=\"Exclude\" type=\"Paraesthesia.Web.Configuration.UrlAbsolutifier.Rule, Paraesthesia.Web.UrlAbsolutifierModule\" value=\"value3\" />" +
				"</section>");
			Assert.AreEqual(3, this._section.Rules.Count, "The wrong number of rules was deserialized.");
			SUT.RuleConfigurationElement[] values = new SUT.RuleConfigurationElement[3];
			this._section.Rules.CopyTo(values, 0);
			Assert.AreEqual(SUT.RuleProcess.Include, values[0].Process, "The Process on the first rule was not set correctly.");
			Assert.AreEqual(typeof(SUT.Rule), values[0].Type, "The Type on the first rule was not set correctly.");
			Assert.AreEqual("value1", values[0].Value, "The Value on the first rule was not set correctly.");
			Assert.AreEqual(SUT.RuleProcess.Include, values[1].Process, "The Process on the second rule was not set correctly.");
			Assert.AreEqual(typeof(SUT.Rule), values[1].Type, "The Type on the second rule was not set correctly.");
			Assert.AreEqual("value2", values[1].Value, "The Value on the second rule was not set correctly.");
			Assert.AreEqual(SUT.RuleProcess.Exclude, values[2].Process, "The Process on the third rule was not set correctly.");
			Assert.AreEqual(typeof(SUT.Rule), values[2].Type, "The Type on the third rule was not set correctly.");
			Assert.AreEqual("value3", values[2].Value, "The Value on the third rule was not set correctly.");
		}

		[Test(Description = "Deserializes a section with a single rule.")]
		public void DeserializeSingle()
		{
			DeserializeSection(this._section, "<section><add process=\"Include\" type=\"Paraesthesia.Web.Configuration.UrlAbsolutifier.Rule, Paraesthesia.Web.UrlAbsolutifierModule\" value=\"some_value_here\" /></section>");
			Assert.AreEqual(1, this._section.Rules.Count, "The section should contain one rule.");
			SUT.RuleConfigurationElement[] values = new SUT.RuleConfigurationElement[1];
			this._section.Rules.CopyTo(values, 0);
			Assert.AreEqual(SUT.RuleProcess.Include, values[0].Process, "The Process was not set correctly.");
			Assert.AreEqual(typeof(SUT.Rule), values[0].Type, "The Type was not set correctly.");
			Assert.AreEqual("some_value_here", values[0].Value, "The Value was not set correctly.");
		}

		[Test(Description = "Serializes an empty section.")]
		public void SerializeEmpty()
		{
			string serialized = SerializeSection(this._section);
			Assert.AreEqual("<section><clear /></section>", serialized, "The section did not serialize correctly.");
		}


		#region Helpers

		private static void DeserializeSection(SUT.UrlAbsolutifierSection section, string xml)
		{
			using (System.IO.StringReader reader = new System.IO.StringReader(xml))
			using (System.Xml.XmlTextReader xmlReader = new System.Xml.XmlTextReader(reader))
			{
				typeof(SUT.UrlAbsolutifierSection).InvokeMember("DeserializeSection", BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.NonPublic, null, section, new object[] { xmlReader });
			}
		}

		private static string SerializeSection(SUT.UrlAbsolutifierSection section)
		{
			string serialized = (string)typeof(SUT.UrlAbsolutifierSection).InvokeMember("SerializeSection", BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.NonPublic, null, section, new object[] { null, "section", ConfigurationSaveMode.Full });
			serialized = CleanupWhitespace(serialized);
			return serialized;
		}

		private static string CleanupWhitespace(string input)
		{
			return System.Text.RegularExpressions.Regex.Replace(input, @">\s+<", "><");
		}

		#endregion
	}
}
