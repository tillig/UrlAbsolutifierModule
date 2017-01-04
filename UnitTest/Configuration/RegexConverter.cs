using System;
using System.Text.RegularExpressions;
using NUnit.Framework;
using SUT = Paraesthesia.Web.Configuration;

namespace Paraesthesia.Test.Unit.Web.Configuration
{
	[TestFixture]
	public class RegexConverter
	{
		SUT.RegexConverter _converter = null;

		[SetUp]
		public void SetUp()
		{
			this._converter = new SUT.RegexConverter();
		}

		[Test(Description = "Checks to see if int can be converted.")]
		public void CanConvertFrom_Int()
		{
			Assert.IsFalse(this._converter.CanConvertFrom(typeof(int)), "The converter should not be able to convert from int.");
		}

		[Test(Description = "Checks to see if null can be converted.")]
		public void CanConvertFrom_Null()
		{
			Assert.IsFalse(this._converter.CanConvertFrom(null), "The converter should not be able to convert from null.");
		}

		[Test(Description = "Checks to see if string can be converted.")]
		public void CanConvertFrom_Regex()
		{
			Assert.IsTrue(this._converter.CanConvertFrom(typeof(Regex)), "The converter should be able to convert from Regex.");
		}

		[Test(Description = "Checks to see if string can be converted.")]
		public void CanConvertFrom_String()
		{
			Assert.IsTrue(this._converter.CanConvertFrom(typeof(string)), "The converter should be able to convert from string.");
		}

		[Test(Description = "Attempts to convert an int to a Regex.")]
		[ExpectedException(typeof(System.NotSupportedException))]
		public void ConvertFrom_Int()
		{
			Regex actual = (Regex)this._converter.ConvertFrom((int)7);
		}

		[Test(Description = "Attempts to convert null to a Regex.")]
		[ExpectedException(typeof(System.NotSupportedException))]
		public void ConvertFrom_Null()
		{
			Regex actual = (Regex)this._converter.ConvertFrom(null);
		}

		[Test(Description = "Attempts to convert a Regex to a Regex.")]
		public void ConvertFrom_Regex()
		{
			Regex expected = new Regex("foo");
			Regex actual = (Regex)this._converter.ConvertFrom(expected);
			Assert.AreSame(expected, actual, "The conversion of a Regex to a Regex should pass the same object through.");
		}

		[Test(Description = "Attempts to convert a string to a Regex.")]
		public void ConvertFrom_String()
		{
			Regex actual = (Regex)this._converter.ConvertFrom("bar");
			Assert.AreEqual("bar", actual.ToString(), "The conversion of a string to a Regex should yield a regex made from that string.");
		}
	}
}
