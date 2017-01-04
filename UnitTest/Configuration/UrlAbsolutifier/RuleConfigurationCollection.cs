using System;
using System.Reflection;
using NUnit.Framework;

using SUT = Paraesthesia.Web.Configuration.UrlAbsolutifier;

namespace Paraesthesia.Test.Unit.Web.Configuration.UrlAbsolutifier
{
	[TestFixture]
	public class RuleConfigurationCollection
	{
		private SUT.RuleConfigurationCollection _collection;
		private SUT.RuleConfigurationElementComparer _comparer;

		[SetUp]
		public void SetUp()
		{
			this._collection = new SUT.RuleConfigurationCollection();
			this._comparer = new SUT.RuleConfigurationElementComparer();
			Assert.IsEmpty(this._collection, "The collection should begin empty.");
		}


		[Test(Description = "Adds multiple elements to the collection.")]
		public void Add_MultipleElements()
		{
			int count = 5;
			SUT.RuleConfigurationElement[] expected = new SUT.RuleConfigurationElement[count];
			for (int i = 0; i < count; i++)
			{
				expected[i] = new SUT.RuleConfigurationElement(SUT.RuleProcess.Include, typeof(SUT.Rule), "value" + i.ToString());
			}

			for (int i = 0; i < count; i++)
			{
				this._collection.Add(expected[i]);
			}

			Assert.AreEqual(count, this._collection.Count, "The wrong number of elements were found in the collection.");
			SUT.RuleConfigurationElement[] values = new SUT.RuleConfigurationElement[count];
			this._collection.CopyTo(values, 0);
			for (int i = 0; i < count; i++)
			{
				Assert.IsTrue(this.RuleConfigurationElementsEqual(expected[i], values[i]), "The rule found at index " + i.ToString() + " was not what was expected.");
			}
		}

		[Test(Description = "Adds multiple elements to the collection, one of which has already been added.")]
		public void Add_MultipleElementsDuplicateObject()
		{
			int count = 5;
			SUT.RuleConfigurationElement[] expected = new SUT.RuleConfigurationElement[count];
			for (int i = 0; i < count; i++)
			{
				expected[i] = new SUT.RuleConfigurationElement(SUT.RuleProcess.Include, typeof(SUT.Rule), "value" + i.ToString());
			}

			for (int i = 0; i < count; i++)
			{
				this._collection.Add(expected[i]);
			}
			this._collection.Add(expected[0]);

			Assert.AreEqual(count, this._collection.Count, "The wrong number of elements were found in the collection.");
			SUT.RuleConfigurationElement[] values = new SUT.RuleConfigurationElement[count];
			this._collection.CopyTo(values, 0);
			
			for (int i = 0; i < count - 1; i++)
			{
				Assert.IsTrue(this.RuleConfigurationElementsEqual(expected[i + 1], values[i]), "The rule found at index " + i.ToString() + " was not what was expected.");
			}
			Assert.IsTrue(this.RuleConfigurationElementsEqual(expected[0], values[count - 1]), "The rule found at index " + (count - 1).ToString() + " was not what was expected.");
		}

		[Test(Description = "Adds multiple elements to the collection, then adds an element with the same values again.")]
		public void Add_MultipleElementsDuplicateValues()
		{
			int count = 5;
			SUT.RuleConfigurationElement[] expected = new SUT.RuleConfigurationElement[count];
			for (int i = 0; i < count; i++)
			{
				expected[i] = new SUT.RuleConfigurationElement(SUT.RuleProcess.Include, typeof(SUT.Rule), "value" + i.ToString());
			}

			for (int i = 0; i < count; i++)
			{
				this._collection.Add(expected[i]);
			}
			this._collection.Add(new SUT.RuleConfigurationElement(SUT.RuleProcess.Include, typeof(SUT.Rule), "value0"));

			Assert.AreEqual(count, this._collection.Count, "The wrong number of elements were found in the collection.");
			SUT.RuleConfigurationElement[] values = new SUT.RuleConfigurationElement[count];
			this._collection.CopyTo(values, 0);

			for (int i = 0; i < count - 1; i++)
			{
				Assert.IsTrue(this.RuleConfigurationElementsEqual(expected[i + 1], values[i]), "The rule found at index " + i.ToString() + " was not what was expected.");
			}
			Assert.IsTrue(this.RuleConfigurationElementsEqual(expected[0], values[count - 1]), "The rule found at index " + (count - 1).ToString() + " was not what was expected.");
		}

		[Test(Description = "Adds multiple elements by value to the collection.")]
		public void Add_MultipleValues()
		{
			int count = 5;
			SUT.RuleConfigurationElement[] expected = new SUT.RuleConfigurationElement[count];
			for (int i = 0; i < count; i++)
			{
				expected[i] = new SUT.RuleConfigurationElement(SUT.RuleProcess.Include, typeof(SUT.Rule), "value" + i.ToString());
				this._collection.Add(SUT.RuleProcess.Include, typeof(SUT.Rule), "value" + i.ToString());
			}

			Assert.AreEqual(count, this._collection.Count, "The wrong number of elements were found in the collection.");
			SUT.RuleConfigurationElement[] values = new SUT.RuleConfigurationElement[count];
			this._collection.CopyTo(values, 0);
			for (int i = 0; i < count; i++)
			{
				Assert.IsTrue(this.RuleConfigurationElementsEqual(expected[i], values[i]), "The rule found at index " + i.ToString() + " was not what was expected.");
			}
		}

		[Test(Description = "Adds multiple elements by value to the collection, then adds an element with the same values again.")]
		public void Add_MultipleValuesDuplicateValues()
		{
			int count = 5;
			SUT.RuleConfigurationElement[] expected = new SUT.RuleConfigurationElement[count];
			for (int i = 0; i < count; i++)
			{
				expected[i] = new SUT.RuleConfigurationElement(SUT.RuleProcess.Include, typeof(SUT.Rule), "value" + i.ToString());
				this._collection.Add(SUT.RuleProcess.Include, typeof(SUT.Rule), "value" + i.ToString());
			}
			this._collection.Add(SUT.RuleProcess.Include, typeof(SUT.Rule), "value0");

			Assert.AreEqual(count, this._collection.Count, "The wrong number of elements were found in the collection.");
			SUT.RuleConfigurationElement[] values = new SUT.RuleConfigurationElement[count];
			this._collection.CopyTo(values, 0);

			for (int i = 0; i < count - 1; i++)
			{
				Assert.IsTrue(this.RuleConfigurationElementsEqual(expected[i + 1], values[i]), "The rule found at index " + i.ToString() + " was not what was expected.");
			}
			Assert.IsTrue(this.RuleConfigurationElementsEqual(expected[0], values[count - 1]), "The rule found at index " + (count - 1).ToString() + " was not what was expected.");
		}

		[Test(Description = "Adds a single element to the collection.")]
		public void Add_SingleElement()
		{
			SUT.RuleConfigurationElement element = new SUT.RuleConfigurationElement(SUT.RuleProcess.Exclude, typeof(SUT.Rule), "value");

			this._collection.Add(element);
			Assert.AreEqual(1, this._collection.Count, "There should only be one element in the collection.");
			SUT.RuleConfigurationElement[] values = new SUT.RuleConfigurationElement[1];
			this._collection.CopyTo(values, 0);
			Assert.AreSame(element, values[0], "The only element in the collection should be the one we added.");
		}

		[Test(Description = "Adds a single element to the collection, then adds the same element again.")]
		public void Add_SingleElementDuplicateObject()
		{
			SUT.RuleConfigurationElement element = new SUT.RuleConfigurationElement(SUT.RuleProcess.Exclude, typeof(SUT.Rule), "value");

			this._collection.Add(element);
			this._collection.Add(element);
			Assert.AreEqual(1, this._collection.Count, "There should only be one element in the collection.");
			SUT.RuleConfigurationElement[] values = new SUT.RuleConfigurationElement[1];
			this._collection.CopyTo(values, 0);
			Assert.AreSame(element, values[0], "The only element in the collection should be the one we added.");
		}

		[Test(Description = "Adds a single element to the collection, then adds an element with the same values again.")]
		public void Add_SingleElementDuplicateValues()
		{
			SUT.RuleConfigurationElement element1 = new SUT.RuleConfigurationElement(SUT.RuleProcess.Exclude, typeof(SUT.Rule), "value");
			SUT.RuleConfigurationElement element2 = new SUT.RuleConfigurationElement(SUT.RuleProcess.Exclude, typeof(SUT.Rule), "value");

			this._collection.Add(element1);
			this._collection.Add(element2);
			Assert.AreEqual(1, this._collection.Count, "There should only be one element in the collection.");
			SUT.RuleConfigurationElement[] values = new SUT.RuleConfigurationElement[1];
			this._collection.CopyTo(values, 0);
			Assert.IsTrue(this.RuleConfigurationElementsEqual(element1, values[0]), "The rule found was not what was expected.");
		}

		[Test(Description = "Adds a single element by value to the collection.")]
		public void Add_SingleValues()
		{
			SUT.RuleConfigurationElement element = new SUT.RuleConfigurationElement(SUT.RuleProcess.Exclude, typeof(SUT.Rule), "value");

			this._collection.Add(SUT.RuleProcess.Exclude, typeof(SUT.Rule), "value");
			Assert.AreEqual(1, this._collection.Count, "There should only be one element in the collection.");
			SUT.RuleConfigurationElement[] values = new SUT.RuleConfigurationElement[1];
			this._collection.CopyTo(values, 0);
			Assert.IsTrue(this.RuleConfigurationElementsEqual(element, values[0]), "The only element in the collection should be the one we added.");
		}

		[Test(Description = "Adds a single element by value to the collection, then adds an element with the same values again.")]
		public void Add_SingleValuesDuplicateValues()
		{
			SUT.RuleConfigurationElement element = new SUT.RuleConfigurationElement(SUT.RuleProcess.Exclude, typeof(SUT.Rule), "value");
			this._collection.Add(SUT.RuleProcess.Exclude, typeof(SUT.Rule), "value");
			this._collection.Add(SUT.RuleProcess.Exclude, typeof(SUT.Rule), "value");

			Assert.AreEqual(1, this._collection.Count, "There should only be one element in the collection.");
			SUT.RuleConfigurationElement[] values = new SUT.RuleConfigurationElement[1];
			this._collection.CopyTo(values, 0);
			Assert.IsTrue(this.RuleConfigurationElementsEqual(element, values[0]), "The rule found was not what was expected.");
		}

		[Test(Description = "Clears the collection.")]
		public void Clear_RemovesAllItems()
		{
			for (int i = 0; i < 8; i++)
			{
				this._collection.Add(SUT.RuleProcess.Include, typeof(SUT.Rule), i.ToString());
			}
			Assert.AreEqual(8, this._collection.Count, "The proper number of items weren't added to the collection.");
			this._collection.Clear();
			Assert.IsEmpty(this._collection, "The collection should be empty after a call to Clear.");
		}

		[Test(Description = "Creates a new/empty element.")]
		public void CreateNewElement_ReturnsNewElement()
		{
			try
			{
				SUT.RuleConfigurationElement element = typeof(SUT.RuleConfigurationCollection).InvokeMember("CreateNewElement", BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.NonPublic, null, this._collection, null) as SUT.RuleConfigurationElement;
				Assert.IsNotNull(element, "The CreateNewElement method should return a new RuleConfigurationElement.");
			}
			catch (TargetInvocationException err)
			{
				if (err.InnerException != null)
				{
					throw err.InnerException;
				}
				throw;
			}
		}

		[Test(Description = "Removes an element by passing an object to remove that has the same values.")]
		public void Remove_ObjectDifferent()
		{
			SUT.RuleConfigurationElement element1 = new SUT.RuleConfigurationElement(SUT.RuleProcess.Include, typeof(SUT.Rule), "value");
			SUT.RuleConfigurationElement element2 = new SUT.RuleConfigurationElement(SUT.RuleProcess.Include, typeof(SUT.Rule), "value");
			this._collection.Add(element1);
			Assert.AreEqual(1, this._collection.Count, "The element should have been added to the collection.");
			this._collection.Remove(element2);
			Assert.IsEmpty(this._collection, "The element should have been removed from the collection.");
		}

		[Test(Description = "Removes an element by passing the exact object to remove.")]
		public void Remove_ObjectSame()
		{
			SUT.RuleConfigurationElement element = new SUT.RuleConfigurationElement(SUT.RuleProcess.Include, typeof(SUT.Rule), "value");
			this._collection.Add(element);
			Assert.AreEqual(1, this._collection.Count, "The element should have been added to the collection.");
			this._collection.Remove(element);
			Assert.IsEmpty(this._collection, "The element should have been removed from the collection.");
		}

		[Test(Description = "Removes an element by passing the values of the object to remove.")]
		public void Remove_ObjectValues()
		{
			SUT.RuleConfigurationElement element = new SUT.RuleConfigurationElement(SUT.RuleProcess.Include, typeof(SUT.Rule), "value");
			this._collection.Add(element);
			Assert.AreEqual(1, this._collection.Count, "The element should have been added to the collection.");
			this._collection.Remove(SUT.RuleProcess.Include, typeof(SUT.Rule), "value");
			Assert.IsEmpty(this._collection, "The element should have been removed from the collection.");
		}

		[Test(Description = "Ensures the ThrowOnDuplicate property is set such that no exceptions should be thrown when duplicate elements are encountered.")]
		public void ThrowOnDuplicate_NeverThrow()
		{
			bool actual = (bool)(typeof(SUT.RuleConfigurationCollection).GetProperty("ThrowOnDuplicate", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(this._collection, null));
			Assert.IsFalse(actual, "The ThrowOnDuplicate property should always return false.");
		}

		/// <summary>
		/// Determines if two elements are equal.
		/// </summary>
		/// <param name="element1">The first element to compare.</param>
		/// <param name="element2">The second element to compare.</param>
		/// <returns><see langword="true" /> if the elements are equal, <see langword="false" /> if not.</returns>
		private bool RuleConfigurationElementsEqual(SUT.RuleConfigurationElement element1, SUT.RuleConfigurationElement element2)
		{
			return this._comparer.Compare(element1, element2) == 0;
		}

	}
}
