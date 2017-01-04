using System;
using System.ComponentModel;
using NUnit.Framework;
using SUT = Paraesthesia.Web.Configuration.UrlAbsolutifier;

namespace Paraesthesia.Test.Unit.Web.Configuration.UrlAbsolutifier
{
	[TestFixture]
	public class RuleConfigurationElementComparer
	{
		private SUT.RuleConfigurationElementComparer _comparer;

		[SetUp]
		public void SetUp()
		{
			this._comparer = new SUT.RuleConfigurationElementComparer();
		}

		[Test(Description = "Compares two objects that differ only by process.")]
		public void Compare_DifferByProcess_X()
		{
			SUT.RuleConfigurationElement x = new SUT.RuleConfigurationElement(SUT.RuleProcess.Include, typeof(RuleType1), "value1");
			SUT.RuleConfigurationElement y = new SUT.RuleConfigurationElement(SUT.RuleProcess.Exclude, typeof(RuleType1), "value1");
			Assert.AreEqual(-1, this._comparer.Compare(x, y), "X should be greater than Y.");
		}

		[Test(Description = "Compares two objects that differ only by process.")]
		public void Compare_DifferByProcess_Y()
		{
			SUT.RuleConfigurationElement x = new SUT.RuleConfigurationElement(SUT.RuleProcess.Exclude, typeof(RuleType1), "value1");
			SUT.RuleConfigurationElement y = new SUT.RuleConfigurationElement(SUT.RuleProcess.Include, typeof(RuleType1), "value1");
			Assert.AreEqual(1, this._comparer.Compare(x, y), "Y should be greater than X.");
		}

		[Test(Description = "Compares two objects that differ only by type.")]
		public void Compare_DifferByType_X()
		{
			SUT.RuleConfigurationElement x = new SUT.RuleConfigurationElement(SUT.RuleProcess.Include, typeof(RuleType1), "value1");
			SUT.RuleConfigurationElement y = new SUT.RuleConfigurationElement(SUT.RuleProcess.Include, typeof(RuleType2), "value1");
			Assert.AreEqual(-1, this._comparer.Compare(x, y), "X should be greater than Y.");
		}

		[Test(Description = "Compares two objects that differ only by type.")]
		public void Compare_DifferByType_Y()
		{
			SUT.RuleConfigurationElement x = new SUT.RuleConfigurationElement(SUT.RuleProcess.Include, typeof(RuleType2), "value1");
			SUT.RuleConfigurationElement y = new SUT.RuleConfigurationElement(SUT.RuleProcess.Include, typeof(RuleType1), "value1");
			Assert.AreEqual(1, this._comparer.Compare(x, y), "Y should be greater than X.");
		}

		[Test(Description = "Compares two objects that differ only by rule value.")]
		public void Compare_DifferByValue_X()
		{
			SUT.RuleConfigurationElement x = new SUT.RuleConfigurationElement(SUT.RuleProcess.Include, typeof(RuleType1), "value1");
			SUT.RuleConfigurationElement y = new SUT.RuleConfigurationElement(SUT.RuleProcess.Include, typeof(RuleType1), "value2");
			Assert.AreEqual(-1, this._comparer.Compare(x, y), "X should be greater than Y.");
		}

		[Test(Description = "Compares two objects that differ only by rule value.")]
		public void Compare_DifferByValue_Y()
		{
			SUT.RuleConfigurationElement x = new SUT.RuleConfigurationElement(SUT.RuleProcess.Include, typeof(RuleType1), "value2");
			SUT.RuleConfigurationElement y = new SUT.RuleConfigurationElement(SUT.RuleProcess.Include, typeof(RuleType1), "value1");
			Assert.AreEqual(1, this._comparer.Compare(x, y), "Y should be greater than X.");
		}

		[Test(Description = "Compares two equal objects.")]
		public void Compare_Equal()
		{
			SUT.RuleConfigurationElement x = new SUT.RuleConfigurationElement(SUT.RuleProcess.Include, typeof(SUT.Rule), "value1");
			SUT.RuleConfigurationElement y = new SUT.RuleConfigurationElement(SUT.RuleProcess.Include, typeof(SUT.Rule), "value1");
			Assert.AreEqual(0, this._comparer.Compare(x, y), "Two objects with equal values should be equal.");
		}

		[Test(Description = "Compares a non-null with a null.")]
		public void Compare_NonNull_Null()
		{
			SUT.RuleConfigurationElement x = new SUT.RuleConfigurationElement(SUT.RuleProcess.Include, typeof(SUT.Rule), "value1");
			SUT.RuleConfigurationElement y = null;
			Assert.AreEqual(1, this._comparer.Compare(x, y), "Non-null should be greater than null.");
		}

		[Test(Description = "Compares a null with a non-null.")]
		public void Compare_Null_NonNull()
		{
			SUT.RuleConfigurationElement x = null;
			SUT.RuleConfigurationElement y = new SUT.RuleConfigurationElement(SUT.RuleProcess.Include, typeof(SUT.Rule), "value1");
			Assert.AreEqual(-1, this._comparer.Compare(x, y), "Non-null should be greater than null.");
		}

		[Test(Description = "Compares two null values.")]
		public void Compare_Null_Null()
		{
			Assert.AreEqual(0, this._comparer.Compare(null, null), "Two nulls should be equal.");
		}

		[Test(Description = "Compares an object to itself.")]
		public void Compare_SameObject()
		{
			SUT.RuleConfigurationElement x = new SUT.RuleConfigurationElement(SUT.RuleProcess.Include, typeof(SUT.Rule), "value1");
			SUT.RuleConfigurationElement y = x;
			Assert.AreEqual(0, this._comparer.Compare(x, y), "An object should be equal to itself.");
		}

		[Test(Description = "Attempts a comparison with a bad type specified for x.")]
		[ExpectedException(typeof(System.ArgumentException))]
		public void Compare_XWrongType()
		{
			string x = "arbitrary_value";
			SUT.RuleConfigurationElement y = null;
			this._comparer.Compare(x, y);
		}

		[Test(Description = "Attempts a comparison with a bad type specified for y.")]
		[ExpectedException(typeof(System.ArgumentException))]
		public void Compare_YWrongType()
		{
			SUT.RuleConfigurationElement x = null;
			string y = "arbitrary_value";
			this._comparer.Compare(x, y);
		}



		private class RuleType1 : SUT.Rule
		{
			public RuleType1(SUT.RuleProcess process, string ruleValue) : base(process, ruleValue) { }

			public override bool ContextMatchesRule(System.Web.HttpContext context)
			{
				throw new Exception("The method or operation is not implemented.");
			}

			public override TypeConverter ValueConverter
			{
				get { throw new Exception("The method or operation is not implemented."); }
			}
		}

		private class RuleType2 : SUT.Rule
		{
			public RuleType2(SUT.RuleProcess process, string ruleValue) : base(process, ruleValue) { }

			public override bool ContextMatchesRule(System.Web.HttpContext context)
			{
				throw new Exception("The method or operation is not implemented.");
			}

			public override TypeConverter ValueConverter
			{
				get { throw new Exception("The method or operation is not implemented."); }
			}
		}
	}
}
