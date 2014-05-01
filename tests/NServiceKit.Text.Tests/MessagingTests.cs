using NUnit.Framework;
using NServiceKit.Messaging;

namespace NServiceKit.Text.Tests
{
    /// <summary>An increment.</summary>
	public class Incr
	{
        /// <summary>Gets or sets the value.</summary>
        /// <value>The value.</value>
		public int Value { get; set; }
	}

    /// <summary>A ping.</summary>
    public class Ping {}

    /// <summary>A messaging tests.</summary>
	[TestFixture]
	public class MessagingTests : TestBase
	{
        /// <summary>Can serialize i message into typed message.</summary>
        [Test]
        public void Can_serialize_IMessage_into_typed_Message()
        {
            var dto = new Incr { Value = 1 };
            IMessage iMsg = MessageFactory.Create(dto);
            var json = iMsg.ToJson();
            var typedMessage = json.FromJson<Message<Incr>>();

            Assert.That(typedMessage.GetBody().Value, Is.EqualTo(dto.Value));
        }

        /// <summary>Can serialize i message into typed message ping.</summary>
        [Test]
        public void Can_serialize_IMessage_into_typed_Message_Ping()
        {
            var dto = new Ping();
            IMessage iMsg = MessageFactory.Create(dto);
            var json = iMsg.ToJson();
            var typedMessage = json.FromJson<Message<Ping>>();

            Assert.That(typedMessage.GetBody(), Is.Not.Null);
        }

        /// <summary>Can serialize object i message into typed message.</summary>
        [Test]
		public void Can_serialize_object_IMessage_into_typed_Message()
		{
			var dto = new Incr { Value = 1 };
			var iMsg = MessageFactory.Create(dto);
			var json = ((object)iMsg).ToJson();
			var typedMessage = json.FromJson<Message<Incr>>();

			Assert.That(typedMessage.GetBody().Value, Is.EqualTo(dto.Value));
		}

        /// <summary>Can serialize i message to bytes into typed message.</summary>
		[Test]
		public void Can_serialize_IMessage_ToBytes_into_typed_Message()
		{
			var dto = new Incr { Value = 1 };
            var iMsg = MessageFactory.Create(dto);
			var bytes = iMsg.ToBytes();
			var typedMessage = bytes.ToMessage<Incr>();

			Assert.That(typedMessage.GetBody().Value, Is.EqualTo(dto.Value));
		}

        /// <summary>A dto with interface.</summary>
		public class DtoWithInterface
		{
            /// <summary>Gets or sets the results.</summary>
            /// <value>The results.</value>
			public IMessage<string> Results { get; set; }
		}

        /// <summary>Can deserialize interface into concrete type.</summary>
		[Test]
		public void Can_deserialize_interface_into_concrete_type()
		{
			var dto = Serialize(new DtoWithInterface { Results = new Message<string>("Body") }, includeXml: false);
			Assert.That(dto.Results, Is.Not.Null);
			Assert.That(dto.Results.GetBody(), Is.Not.Null);
		}
	}
}