using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NServiceKit.Messaging;
using NServiceKit.ServiceInterface.Auth;
using NServiceKit.Text.Tests.JsonTests;

namespace NServiceKit.Text.Tests
{
    /// <summary>An interface tests.</summary>
	[TestFixture]
	public class InterfaceTests : TestBase
	{
        /// <summary>Can serialize message.</summary>
		[Test]
		public void Can_serialize_Message()
		{
			var message = new Message<string> { Id = new Guid(), CreatedDate = new DateTime(), Body = "test" };
			var messageString = TypeSerializer.SerializeToString(message);

			Assert.That(messageString, Is.EqualTo(
            "{Id:00000000000000000000000000000000,CreatedDate:0001-01-01,Priority:0,RetryAttempts:0,Options:1,Body:test}"));

			Serialize(message);
		}

        /// <summary>Can serialize i message.</summary>
		[Test]
		public void Can_serialize_IMessage()
		{
			var message = new Message<string> { Id = new Guid(), CreatedDate = new DateTime(), Body = "test" };
			var messageString = TypeSerializer.SerializeToString((IMessage<string>)message);

			Assert.That(messageString, Is.EqualTo(
			"{__type:\"NServiceKit.Messaging.Message`1[[System.String, mscorlib]], NServiceKit.Interfaces\","
             + "Id:00000000000000000000000000000000,CreatedDate:0001-01-01,Priority:0,RetryAttempts:0,Options:1,Body:test}"));
		}

        /// <summary>A dto with object.</summary>
		public class DtoWithObject
		{
            /// <summary>Gets or sets the results.</summary>
            /// <value>The results.</value>
			public object Results { get; set; }
		}

        /// <summary>Can deserialize dto with object.</summary>
		[Test]
		public void Can_deserialize_dto_with_object()
		{
			var dto = Serialize(new DtoWithObject { Results = new Message<string>("Body") }, includeXml: false);
			Assert.That(dto.Results, Is.Not.Null);
			Assert.That(dto.Results.GetType(), Is.EqualTo(typeof(Message<string>)));
		}

        /// <summary>Can serialize to string.</summary>
		[Test]
		public void Can_serialize_ToString()
		{
			var type = Type.GetType(typeof(Message<string>).AssemblyQualifiedName);
			Assert.That(type, Is.Not.Null);

			type = AssemblyUtils.FindType(typeof(Message<string>).AssemblyQualifiedName);
			Assert.That(type, Is.Not.Null);

			type = Type.GetType("NServiceKit.Messaging.Message`1[[System.String, mscorlib]], NServiceKit.Interfaces");
			Assert.That(type, Is.Not.Null);
		}

        /// <summary>Does serialize minimum type information whilst still working.</summary>
        /// <param name="type">              The type.</param>
        /// <param name="expectedTypeString">The expected type string.</param>
		[Test, TestCaseSource(typeof(InterfaceTests), "EndpointExpectations")]
		public void Does_serialize_minimum_type_info_whilst_still_working(
			Type type, string expectedTypeString)
		{
			Assert.That(type.ToTypeString(), Is.EqualTo(expectedTypeString));
			var newType = AssemblyUtils.FindType(type.ToTypeString());
			Assert.That(newType, Is.Not.Null);
			Assert.That(newType, Is.EqualTo(type));
		}

        /// <summary>Gets the endpoint expectations.</summary>
        /// <value>The endpoint expectations.</value>
		public static IEnumerable EndpointExpectations
		{
			get
			{
				yield return new TestCaseData(typeof(Message<string>),
					"NServiceKit.Messaging.Message`1[[System.String, mscorlib]], NServiceKit.Interfaces");

				yield return new TestCaseData(typeof(Cat),
					"NServiceKit.Text.Tests.JsonTests.Cat, NServiceKit.Text.Tests");

				yield return new TestCaseData(typeof(Zoo),
					"NServiceKit.Text.Tests.JsonTests.Zoo, NServiceKit.Text.Tests");
			}
		}

        /// <summary>Can deserialize interface into concrete type.</summary>
		[Test]
		public void Can_deserialize_interface_into_concrete_type()
		{
			var dto = Serialize(new MessagingTests.DtoWithInterface { Results = new Message<string>("Body") }, includeXml: false);
			Assert.That(dto.Results, Is.Not.Null);
		}

        /// <summary>A user session.</summary>
		public class UserSession
		{
            /// <summary>
            /// Initializes a new instance of the NServiceKit.Text.Tests.InterfaceTests.UserSession class.
            /// </summary>
			public UserSession()
			{
				this.ProviderOAuthAccess = new Dictionary<string, IOAuthTokens>();
			}

            /// <summary>Gets or sets URL of the referrer.</summary>
            /// <value>The referrer URL.</value>
			public string ReferrerUrl { get; set; }

            /// <summary>Gets or sets the identifier.</summary>
            /// <value>The identifier.</value>
			public string Id { get; set; }

            /// <summary>Gets or sets the identifier of the twitter user.</summary>
            /// <value>The identifier of the twitter user.</value>
			public string TwitterUserId { get; set; }

            /// <summary>Gets or sets the name of the twitter screen.</summary>
            /// <value>The name of the twitter screen.</value>
			public string TwitterScreenName { get; set; }

            /// <summary>Gets or sets the request token secret.</summary>
            /// <value>The request token secret.</value>
			public string RequestTokenSecret { get; set; }

            /// <summary>Gets or sets the Date/Time of the created at.</summary>
            /// <value>The created at.</value>
			public DateTime CreatedAt { get; set; }

            /// <summary>Gets or sets the Date/Time of the last modified.</summary>
            /// <value>The last modified.</value>
			public DateTime LastModified { get; set; }

            /// <summary>Gets or sets the provider o authentication access.</summary>
            /// <value>The provider o authentication access.</value>
			public Dictionary<string, IOAuthTokens> ProviderOAuthAccess { get; set; }
		}

        /// <summary>Can serialize user o authentication session map.</summary>
		[Test]
		public void Can_Serialize_User_OAuthSession_map()
		{
			var userSession = new UserSession {
				Id = "1",
				CreatedAt = DateTime.UtcNow,
				LastModified = DateTime.UtcNow,
				ReferrerUrl = "http://referrer.com",
				ProviderOAuthAccess = new Dictionary<string, IOAuthTokens>
                {
                    {"twitter", new OAuthTokens { Provider = "twitter", AccessToken = "TAccessToken", Items = { {"a","1"}, {"b","2"}, }} },
                    {"facebook", new OAuthTokens { Provider = "facebook", AccessToken = "FAccessToken", Items = { {"a","1"}, {"b","2"}, }} },
                }
			};

			var fromDto = Serialize(userSession, includeXml: false);
			Console.WriteLine(fromDto.Dump());

			Assert.That(fromDto.ProviderOAuthAccess.Count, Is.EqualTo(2));
			Assert.That(fromDto.ProviderOAuthAccess["twitter"].Provider, Is.EqualTo("twitter"));
			Assert.That(fromDto.ProviderOAuthAccess["facebook"].Provider, Is.EqualTo("facebook"));
			Assert.That(fromDto.ProviderOAuthAccess["twitter"].Items.Count, Is.EqualTo(2));
			Assert.That(fromDto.ProviderOAuthAccess["facebook"].Items.Count, Is.EqualTo(2));
		}

        /// <summary>Can serialize user authentication session list.</summary>
		[Test]
		public void Can_Serialize_User_AuthSession_list()
		{
			var userSession = new AuthUserSession {
				Id = "1",
				CreatedAt = DateTime.UtcNow,
				LastModified = DateTime.UtcNow,
				ReferrerUrl = "http://referrer.com",
				ProviderOAuthAccess = new List<IOAuthTokens>
                {
                    new OAuthTokens { Provider = "twitter", AccessToken = "TAccessToken", Items = { {"a","1"}, {"b","2"}, }},
                    new OAuthTokens { Provider = "facebook", AccessToken = "FAccessToken", Items = { {"a","1"}, {"b","2"}, }},
                }
			};

			var fromDto = Serialize(userSession, includeXml: false);
			Console.WriteLine(fromDto.Dump());

			Assert.That(fromDto.ProviderOAuthAccess.Count, Is.EqualTo(2));
			Assert.That(fromDto.ProviderOAuthAccess[0].Provider, Is.EqualTo("twitter"));
			Assert.That(fromDto.ProviderOAuthAccess[1].Provider, Is.EqualTo("facebook"));
			Assert.That(fromDto.ProviderOAuthAccess[0].Items.Count, Is.EqualTo(2));
			Assert.That(fromDto.ProviderOAuthAccess[1].Items.Count, Is.EqualTo(2));
		}

        /// <summary>Doesnt serialize type information when set.</summary>
		[Test]
		public void Doesnt_serialize_TypeInfo_when_set()
		{
            try {
			    JsConfig.ExcludeTypeInfo = true;
			    var userSession = new AuthUserSession {
				    Id = "1",
				    CreatedAt = DateTime.UtcNow,
				    LastModified = DateTime.UtcNow,
				    ReferrerUrl = "http://referrer.com",
				    ProviderOAuthAccess = new List<IOAuthTokens>
                    {
                        new OAuthTokens { Provider = "twitter", AccessToken = "TAccessToken", Items = { {"a","1"}, {"b","2"}, }},
                        new OAuthTokens { Provider = "facebook", AccessToken = "FAccessToken", Items = { {"a","1"}, {"b","2"}, }},
                    }
			    };

			    Assert.That(userSession.ToJson().IndexOf("__type") == -1, Is.True);
			    Assert.That(userSession.ToJsv().IndexOf("__type") == -1, Is.True);
            } finally {
			    JsConfig.Reset();
            }
		}

        /// <summary>An aggregate events.</summary>
		public class AggregateEvents
		{
            /// <summary>Gets or sets the identifier.</summary>
            /// <value>The identifier.</value>
			public Guid Id { get; set; }

            /// <summary>Gets or sets the events.</summary>
            /// <value>The events.</value>
			public List<DomainEvent> Events { get; set; }
		}

        /// <summary>A domain event.</summary>
		public abstract class DomainEvent { }

        /// <summary>A user registered event.</summary>
		public class UserRegisteredEvent : DomainEvent
		{
            /// <summary>Gets or sets the identifier of the user.</summary>
            /// <value>The identifier of the user.</value>
			public Guid UserId { get; set; }

            /// <summary>Gets or sets the name.</summary>
            /// <value>The name.</value>
			public string Name { get; set; }
		}

        /// <summary>A user promoted event.</summary>
		public class UserPromotedEvent : DomainEvent
		{
            /// <summary>Gets or sets the identifier of the user.</summary>
            /// <value>The identifier of the user.</value>
			public Guid UserId { get; set; }

            /// <summary>Gets or sets the new role.</summary>
            /// <value>The new role.</value>
			public string NewRole { get; set; }
		}

        /// <summary>Can deserialize domain event into concrete type.</summary>
		[Test]
		public void Can_deserialize_DomainEvent_into_Concrete_Type()
		{
			var userId = Guid.NewGuid();
			var dto = (DomainEvent)new UserPromotedEvent { UserId = userId };
			var json = dto.ToJson();
			var userPromoEvent = (UserPromotedEvent)json.FromJson<DomainEvent>();
			Assert.That(userPromoEvent.UserId, Is.EqualTo(userId));
		}

        /// <summary>A habitat.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
		public class Habitat<T> where T : Animal
		{
            /// <summary>Gets or sets the continent.</summary>
            /// <value>The continent.</value>
			public string Continent { get; set; }

            /// <summary>Gets or sets the species.</summary>
            /// <value>The species.</value>
			public object Species { get; set; }
		}

        /// <summary>An animal.</summary>
		public class Animal
		{
            /// <summary>Gets or sets the name.</summary>
            /// <value>The name.</value>
			public string Name { get; set; }
		}

        /// <summary>A category animal.</summary>
		public class CatAnimal : Animal
		{
            /// <summary>Gets or sets the color.</summary>
            /// <value>The color.</value>
			public string Color { get; set; }
		}

        /// <summary>Can serialize dependent type properties.</summary>
		[Test]
		public void Can_serialize_dependent_type_properties()
		{
			var jungle = new Habitat<Animal> {
				Continent = "South America",
				Species = new CatAnimal { Name = "Tiger", Color = "Orange" }
			};

			Console.WriteLine(jungle.ToJson());
		}
	}
}