using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace NServiceKit.Text.Tests.DynamicModels
{
    /// <summary>A dynamic message.</summary>
	public class DynamicMessage : IMessageHeaders
	{
        /// <summary>Gets or sets the identifier.</summary>
        /// <value>The identifier.</value>
		public Guid Id { get; set; }

        /// <summary>Gets or sets the reply to.</summary>
        /// <value>The reply to.</value>
		public string ReplyTo { get; set; }

        /// <summary>Gets or sets the priority.</summary>
        /// <value>The priority.</value>
		public int Priority { get; set; }

        /// <summary>Gets or sets the retry attempts.</summary>
        /// <value>The retry attempts.</value>
		public int RetryAttempts { get; set; }

        /// <summary>Gets or sets the body.</summary>
        /// <value>The body.</value>
		public object Body { get; set; }

        /// <summary>Gets or sets the type.</summary>
        /// <value>The type.</value>
		public Type Type { get; set; }

        /// <summary>Gets the body.</summary>
        /// <returns>The body.</returns>
		public object GetBody()
		{
			//When deserialized this.Body is a string so use the serilaized 
			//this.Type to deserialize it back into the original type
			return this.Body is string
			? TypeSerializer.DeserializeFromString((string)this.Body, this.Type)
			: this.Body;
		}
	}

    /// <summary>A generic message.</summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
	public class GenericMessage<T> : IMessageHeaders
	{
        /// <summary>Gets or sets the identifier.</summary>
        /// <value>The identifier.</value>
		public Guid Id { get; set; }

        /// <summary>Gets or sets the reply to.</summary>
        /// <value>The reply to.</value>
		public string ReplyTo { get; set; }

        /// <summary>Gets or sets the priority.</summary>
        /// <value>The priority.</value>
		public int Priority { get; set; }

        /// <summary>Gets or sets the retry attempts.</summary>
        /// <value>The retry attempts.</value>
		public int RetryAttempts { get; set; }

        /// <summary>Gets or sets the body.</summary>
        /// <value>The body.</value>
		public T Body { get; set; }
	}

    /// <summary>A strict message.</summary>
	public class StrictMessage : IMessageHeaders
	{
        /// <summary>Gets or sets the identifier.</summary>
        /// <value>The identifier.</value>
		public Guid Id { get; set; }

        /// <summary>Gets or sets the reply to.</summary>
        /// <value>The reply to.</value>
		public string ReplyTo { get; set; }

        /// <summary>Gets or sets the priority.</summary>
        /// <value>The priority.</value>
		public int Priority { get; set; }

        /// <summary>Gets or sets the retry attempts.</summary>
        /// <value>The retry attempts.</value>
		public int RetryAttempts { get; set; }

        /// <summary>Gets or sets the body.</summary>
        /// <value>The body.</value>
		public MessageBody Body { get; set; }
	}

    /// <summary>A message body.</summary>
	public class MessageBody
	{
        /// <summary>
        /// Initializes a new instance of the NServiceKit.Text.Tests.DynamicModels.MessageBody class.
        /// </summary>
		public MessageBody()
		{
			this.Arguments = new List<string>();
		}

        /// <summary>Gets or sets the action.</summary>
        /// <value>The action.</value>
		public string Action { get; set; }

        /// <summary>Gets or sets the arguments.</summary>
        /// <value>The arguments.</value>
		public List<string> Arguments { get; set; }
	}

    /// <summary>Interface for message headers.</summary>
	public interface IMessageHeaders
	{
        /// <summary>Gets or sets the identifier.</summary>
        /// <value>The identifier.</value>
		Guid Id { get; set; }

        /// <summary>Gets or sets the reply to.</summary>
        /// <value>The reply to.</value>
		string ReplyTo { get; set; }

        /// <summary>Gets or sets the priority.</summary>
        /// <value>The priority.</value>
		int Priority { get; set; }

        /// <summary>Gets or sets the retry attempts.</summary>
        /// <value>The retry attempts.</value>
		int RetryAttempts { get; set; }
	}

    /// <summary>A dynamic message tests.</summary>
	[TestFixture]
	public class DynamicMessageTests
	{
        /// <summary>Tests object set to object.</summary>
        [Test]
        public void Object_Set_To_Object_Test()
        {
            var original = new DynamicMessage
            {
                Id = Guid.NewGuid(),
                Priority = 3,
                ReplyTo = "http://path/to/reply.svc",
                RetryAttempts = 1,
                Type = typeof(MessageBody),
                Body = new Object()
            };

            var jsv = TypeSerializer.SerializeToString(original);
            var json = JsonSerializer.SerializeToString(original);
            var jsvDynamicType = TypeSerializer.DeserializeFromString<DynamicMessage>(jsv);
            var jsonDynamicType = JsonSerializer.DeserializeFromString<DynamicMessage>(json);

            AssertHeadersAreEqual(jsvDynamicType, original);
            AssertHeadersAreEqual(jsonDynamicType, original);
            AssertHeadersAreEqual(jsvDynamicType, jsonDynamicType);
        }

        /// <summary>Can deserialize between dynamic generic and strict messages.</summary>
        [Test]
		public void Can_deserialize_between_dynamic_generic_and_strict_messages()
		{
			var original = new DynamicMessage
			{
				Id = Guid.NewGuid(),
				Priority = 3,
				ReplyTo = "http://path/to/reply.svc",
				RetryAttempts = 1,
				Type = typeof(MessageBody),
				Body = new MessageBody
				{
					Action = "Alphabet",
					Arguments = { "a", "b", "c" }
				}
			};

			var jsv = TypeSerializer.SerializeToString(original);
			var dynamicType = TypeSerializer.DeserializeFromString<DynamicMessage>(jsv);
			var genericType = TypeSerializer.DeserializeFromString<GenericMessage<MessageBody>>(jsv);
			var strictType = TypeSerializer.DeserializeFromString<StrictMessage>(jsv);

			AssertHeadersAreEqual(dynamicType, original);
			AssertBodyIsEqual(dynamicType.GetBody(), (MessageBody)original.Body);

			AssertHeadersAreEqual(genericType, original);
			AssertBodyIsEqual(genericType.Body, (MessageBody)original.Body);

			AssertHeadersAreEqual(strictType, original);
			AssertBodyIsEqual(strictType.Body, (MessageBody)original.Body);

			//Debug purposes
			Console.WriteLine(strictType.Dump());
			/*
			 {
				Id: 891653ea2d0a4626ab0623fc2dc9dce1,
				ReplyTo: http://path/to/reply.svc,
				Priority: 3,
				RetryAttempts: 1,
				Body: 
				{
					Action: Alphabet,
					Arguments: 
					[
						a,
						b,
						c
					]
				}
			}
			*/
		}

        /// <summary>Assert headers are equal.</summary>
        /// <param name="actual">  The actual.</param>
        /// <param name="expected">The expected.</param>
		public void AssertHeadersAreEqual(IMessageHeaders actual, IMessageHeaders expected)
		{
			Assert.That(actual.Id, Is.EqualTo(expected.Id));
			Assert.That(actual.ReplyTo, Is.EqualTo(expected.ReplyTo));
			Assert.That(actual.Priority, Is.EqualTo(expected.Priority));
			Assert.That(actual.RetryAttempts, Is.EqualTo(expected.RetryAttempts));
		}

        /// <summary>Assert body is equal.</summary>
        /// <param name="actual">  The actual.</param>
        /// <param name="expected">The expected.</param>
		public void AssertBodyIsEqual(object actual, MessageBody expected)
		{
			var actualBody = actual as MessageBody;
			Assert.That(actualBody, Is.Not.Null);
			Assert.That(actualBody.Action, Is.EqualTo(expected.Action));
			Assert.That(actualBody.Arguments, Is.EquivalentTo(expected.Arguments));
		}
	}
}