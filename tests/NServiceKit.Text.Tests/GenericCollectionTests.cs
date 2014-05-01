using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NServiceKit.Common.Tests.Models;

namespace NServiceKit.Text.Tests
{
    /// <summary>A generic collection tests.</summary>
	[TestFixture]
	public class GenericCollectionTests
		: TestBase
	{
        /// <summary>Can serialize queue string.</summary>
		[Test]
		public void Can_serialize_Queue_string()
		{
			var queue = new Queue<string>();

			queue.Enqueue("one");
			queue.Enqueue("two");
			queue.Enqueue("three");

			Serialize(queue);

			Assert.That(CsvSerializer.SerializeToString(queue), Is.EqualTo("one,two,three" + Environment.NewLine));
		}

        /// <summary>Can serialize queue int.</summary>
		[Test]
		public void Can_serialize_Queue_int()
		{
			var queue = new Queue<int>();

			queue.Enqueue(1);
			queue.Enqueue(2);
			queue.Enqueue(3);

			Serialize(queue);

			Assert.That(CsvSerializer.SerializeToString(queue), Is.EqualTo("1,2,3" + Environment.NewLine));
		}

        /// <summary>Can serialize queue generic.</summary>
		[Test]
		public void Can_serialize_Queue_Generic()
		{
			var queue = new Queue<ModelWithIdAndName>();

			queue.Enqueue(ModelWithIdAndName.Create(1));
			queue.Enqueue(ModelWithIdAndName.Create(2));
			queue.Enqueue(ModelWithIdAndName.Create(3));

			Serialize(queue);

			Assert.That(CsvSerializer.SerializeToString(queue), 
				Is.EqualTo(
					"Id,Name"
					+ Environment.NewLine
					+ "1,Name1"
					+ Environment.NewLine
					+ "2,Name2"
					+ Environment.NewLine
					+ "3,Name3"
					+ Environment.NewLine
				));
		}

        /// <summary>Can serialize stack string.</summary>
		[Test]
		public void Can_serialize_Stack_string()
		{
			var stack = new Stack<string>();

			stack.Push("one");
			stack.Push("two");
			stack.Push("three");

			Serialize(stack);

			Assert.That(CsvSerializer.SerializeToString(stack), Is.EqualTo("three,two,one" + Environment.NewLine));
		}

        /// <summary>Can serialize stack int.</summary>
		[Test]
		public void Can_serialize_Stack_int()
		{
			var stack = new Stack<int>();

			stack.Push(1);
			stack.Push(2);
			stack.Push(3);

			Serialize(stack);

			Assert.That(CsvSerializer.SerializeToString(stack), Is.EqualTo("3,2,1" + Environment.NewLine));
		}

        /// <summary>Can serialize stack generic.</summary>
		[Test]
		public void Can_serialize_Stack_Generic()
		{
			var stack = new Stack<ModelWithIdAndName>();

			stack.Push(ModelWithIdAndName.Create(1));
			stack.Push(ModelWithIdAndName.Create(2));
			stack.Push(ModelWithIdAndName.Create(3));

			Serialize(stack);

			Assert.That(CsvSerializer.SerializeToString(stack),
				Is.EqualTo(
					"Id,Name"
					+ Environment.NewLine
					+ "3,Name3"
					+ Environment.NewLine
					+ "2,Name2"
					+ Environment.NewLine
					+ "1,Name1"
					+ Environment.NewLine
				));
		}
	}

}