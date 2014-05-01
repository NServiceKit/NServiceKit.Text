using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
#if !MONOTOUCH
using System.Runtime.Serialization.Json;
#endif
using System.Text.RegularExpressions;
using NUnit.Framework;
using System.Linq;

namespace NServiceKit.Text.Tests.JsonTests
{
    /// <summary>Interface for category.</summary>
	public interface ICat
	{
        /// <summary>Gets or sets the name.</summary>
        /// <value>The name.</value>
		string Name { get; set; }
	}

    /// <summary>Interface for dog.</summary>
	public interface IDog
	{
        /// <summary>Gets or sets the name.</summary>
        /// <value>The name.</value>
		string Name { get; set; }
	}

    /// <summary>[KnownType(typeof(Dog))] [KnownType(typeof(Cat))].</summary>
	public abstract class Animal
	{
        /// <summary>Gets or sets the name.</summary>
        /// <value>The name.</value>
		public abstract string Name
		{
			get;
			set;
		}
	}

    /// <summary>A dog.</summary>
	public class Dog : Animal, IDog
	{
        /// <summary>Gets or sets the name.</summary>
        /// <value>The name.</value>
		public override string Name { get; set; }

        /// <summary>Gets or sets the dog bark.</summary>
        /// <value>The dog bark.</value>
		public string DogBark { get; set; }
	}

    /// <summary>A collie.</summary>
    public class Collie : Dog
    {
        /// <summary>Gets or sets a value indicating whether this object is lassie.</summary>
        /// <value>true if this object is lassie, false if not.</value>
        public bool IsLassie { get; set; }
    }

    /// <summary>A category.</summary>
	public class Cat : Animal, ICat
	{
        /// <summary>Gets or sets the name.</summary>
        /// <value>The name.</value>
		public override string Name { get; set; }

        /// <summary>Gets or sets the category meow.</summary>
        /// <value>The category meow.</value>
		public string CatMeow { get; set; }
	}

    /// <summary>A zoo.</summary>
	public class Zoo
	{
        /// <summary>
        /// Initializes a new instance of the NServiceKit.Text.Tests.JsonTests.Zoo class.
        /// </summary>
		public Zoo()
		{
			Animals = new List<Animal>
			{
				new Dog { Name = @"Fido", DogBark = "woof" },
				new Cat { Name = @"Tigger", CatMeow = "meow" },
			};
		}

        /// <summary>Gets or sets the animals.</summary>
        /// <value>The animals.</value>
		public List<Animal> Animals
		{
			get;
			set;
		}

        /// <summary>Gets or sets the name.</summary>
        /// <value>The name.</value>
		public string Name
		{
			get;
			set;
		}
	}

    /// <summary>Interface for term.</summary>
    public interface ITerm { }

    /// <summary>A foo term.</summary>
    public class FooTerm : ITerm { }

    /// <summary>A terms.</summary>
    public class Terms : IEnumerable<ITerm>
    {
        /// <summary>The list.</summary>
        private readonly List<ITerm> _list = new List<ITerm>();

        /// <summary>
        /// Initializes a new instance of the NServiceKit.Text.Tests.JsonTests.Terms class.
        /// </summary>
        public Terms()
            : this(Enumerable.Empty<ITerm>())
        {

        }

        /// <summary>
        /// Initializes a new instance of the NServiceKit.Text.Tests.JsonTests.Terms class.
        /// </summary>
        /// <param name="terms">The terms.</param>
        public Terms(IEnumerable<ITerm> terms)
        {
            _list.AddRange(terms);
        }

        /// <summary>Gets the enumerator.</summary>
        /// <returns>The enumerator.</returns>
        public IEnumerator<ITerm> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        /// <summary>Gets the enumerator.</summary>
        /// <returns>The enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>Adds term.</summary>
        /// <param name="term">The term to add.</param>
        public void Add(ITerm term)
        {
            _list.Add(term);
        }
    }

    /// <summary>A polymorphic list tests.</summary>
	[TestFixture]
	public class PolymorphicListTests : TestBase
	{
        /// <summary>Name of the assembly.</summary>
		String assemblyName;

        /// <summary>Sets the up.</summary>
		[SetUp]
		public void SetUp()
		{
			JsConfig.Reset();
			JsConfig<ICat>.ExcludeTypeInfo = false;
			assemblyName = GetType().Assembly.GetName().Name;
		}

        /// <summary>Can serialise polymorphic list.</summary>
		[Test]
		public void Can_serialise_polymorphic_list()
		{
			var list = new List<Animal>
			{
				new Dog { Name = @"Fido", DogBark = "woof" },
				new Cat { Name = @"Tigger", CatMeow = "meow" },
			};

			var asText = JsonSerializer.SerializeToString(list);

			Log(asText);

			Assert.That(asText,
				Is.EqualTo(
					"[{\"__type\":\""
					+ typeof(Dog).ToTypeString()
					+ "\",\"Name\":\"Fido\",\"DogBark\":\"woof\"},{\"__type\":\""
					+ typeof(Cat).ToTypeString()
					+ "\",\"Name\":\"Tigger\",\"CatMeow\":\"meow\"}]"));
		}

        /// <summary>Can serialise an entity with a polymorphic list.</summary>
		[Test]
		public void Can_serialise_an_entity_with_a_polymorphic_list()
		{
			var zoo = new Zoo {
				Name = @"City Zoo"
			};

			string asText = JsonSerializer.SerializeToString(zoo);

			Log(asText);

			Assert.That(
				asText,
				Is.EqualTo(
					"{\"Animals\":[{\"__type\":\""
					+ typeof(Dog).ToTypeString()
					+ "\",\"Name\":\"Fido\",\"DogBark\":\"woof\"},{\"__type\":\""
					+ typeof(Cat).ToTypeString()
					+ "\",\"Name\":\"Tigger\",\"CatMeow\":\"meow\"}],\"Name\":\"City Zoo\"}"));
		}

        /// <summary>Can serialise polymorphic entity with customised typename.</summary>
		[Test]
		public void Can_serialise_polymorphic_entity_with_customised_typename()
		{
			try
			{
				JsConfig.TypeWriter = type => type.Name;

				Animal dog = new Dog { Name = @"Fido", DogBark = "woof" };
				var asText = JsonSerializer.SerializeToString(dog);

				Log(asText);

				Assert.That(asText,
					Is.EqualTo(
						"{\"__type\":\"Dog\",\"Name\":\"Fido\",\"DogBark\":\"woof\"}"));
			} finally {
				JsConfig.Reset();
			}
		}

        /// <summary>Can deserialise polymorphic list.</summary>
		[Test]
		public void Can_deserialise_polymorphic_list()
		{
			var list =
				JsonSerializer.DeserializeFromString<List<Animal>>(
					"[{\"__type\":\""
					+ typeof(Dog).ToTypeString()
					+ "\",\"Name\":\"Fido\"},{\"__type\":\""
					+ typeof(Cat).ToTypeString()
					+ "\",\"Name\":\"Tigger\"}]");

			Assert.That(list.Count, Is.EqualTo(2));

			Assert.That(list[0].GetType(), Is.EqualTo(typeof(Dog)));
			Assert.That(list[1].GetType(), Is.EqualTo(typeof(Cat)));

			Assert.That(list[0].Name, Is.EqualTo(@"Fido"));
			Assert.That(list[1].Name, Is.EqualTo(@"Tigger"));
		}

#if !MONOTOUCH
        /// <summary>
        /// Can deserialise polymorphic list serialized by datacontractjsonserializer.
        /// </summary>
		[Test]
		public void Can_deserialise_polymorphic_list_serialized_by_datacontractjsonserializer()
		{
		    Func<string, Type> typeFinder = value => {
		        var regex = new Regex(@"^(?<type>[^:]+):#(?<namespace>.*)$");
		        var match = regex.Match(value);
		        var typeName = string.Format("{0}.{1}", match.Groups["namespace"].Value, match.Groups["type"].Value.Replace(".", "+"));
                return AssemblyUtils.FindType(typeName);
		    };

		    try {
		        var originalList = new List<Animal> {new Dog {Name = "Fido"}, new Cat {Name = "Tigger"}};

		        var dataContractJsonSerializer = new DataContractJsonSerializer(typeof (List<Animal>), new[] {typeof (Dog), typeof (Cat)}, int.MaxValue, true, null, true);
		        JsConfig.TypeFinder = typeFinder;
		        List<Animal> deserializedList = null;
		        using (var stream = new MemoryStream()) {
		            dataContractJsonSerializer.WriteObject(stream, originalList);
		            stream.Position = 0;
		            using (var reader = new StreamReader(stream)) {
		                var json = reader.ReadToEnd();
		                deserializedList = JsonSerializer.DeserializeFromString<List<Animal>>(json);
		            }
		        }

		        Assert.That(deserializedList.Count, Is.EqualTo(originalList.Count));

		        Assert.That(deserializedList[0].GetType(), Is.EqualTo(originalList[0].GetType()));
		        Assert.That(deserializedList[1].GetType(), Is.EqualTo(originalList[1].GetType()));

		        Assert.That(deserializedList[0].Name, Is.EqualTo(originalList[0].Name));
		        Assert.That(deserializedList[1].Name, Is.EqualTo(originalList[1].Name));
		    } finally {
		        JsConfig.Reset();
		    }
		}
#endif

        /// <summary>Can deserialise polymorphic list with nonabstract base.</summary>
	    public void Can_deserialise_polymorphic_list_with_nonabstract_base()
		{
			var list =
				JsonSerializer.DeserializeFromString<List<Dog>>(
					"[{\"__type\":\""
					+ typeof(Dog).ToTypeString()
					+ "\",\"Name\":\"Fido\"},{\"__type\":\""
					+ typeof(Collie).ToTypeString()
					+ "\",\"Name\":\"Lassie\",\"IsLassie\":true}]");

			Assert.That(list.Count, Is.EqualTo(2));

			Assert.That(list[0].GetType(), Is.EqualTo(typeof(Dog)));
			Assert.That(list[1].GetType(), Is.EqualTo(typeof(Collie)));

			Assert.That(list[0].Name, Is.EqualTo(@"Fido"));
			Assert.That(list[1].Name, Is.EqualTo(@"Lassie"));
		}

        /// <summary>
        /// Can deserialise polymorphic item with nonabstract base deserializes derived properties
        /// correctly.
        /// </summary>
		[Test]
	    public void Can_deserialise_polymorphic_item_with_nonabstract_base_deserializes_derived_properties_correctly()
		{
			var collie =
				JsonSerializer.DeserializeFromString<Dog>(
					"{\"__type\":\""
					+ typeof(Collie).ToTypeString()
					+ "\",\"Name\":\"Lassie\",\"IsLassie\":true}");

			Assert.That(collie.GetType(), Is.EqualTo(typeof(Collie)));
			Assert.That(collie.Name, Is.EqualTo(@"Lassie"));
			Assert.That(((Collie)collie).IsLassie, Is.True);
		}

        /// <summary>Can deserialise an entity containing a polymorphic list.</summary>
		[Test]
		public void Can_deserialise_an_entity_containing_a_polymorphic_list()
		{
			var zoo =
				JsonSerializer.DeserializeFromString<Zoo>(
					"{\"Animals\":[{\"__type\":\""
					+ typeof(Dog).ToTypeString()
					+ "\",\"Name\":\"Fido\"},{\"__type\":\""
					+ typeof(Cat).ToTypeString()
					+ "\",\"Name\":\"Tigger\"}],\"Name\":\"City Zoo\"}");

			Assert.That(zoo.Name, Is.EqualTo(@"City Zoo"));

			var animals = zoo.Animals;

			Assert.That(animals[0].GetType(), Is.EqualTo(typeof(Dog)));
			Assert.That(animals[1].GetType(), Is.EqualTo(typeof(Cat)));

			Assert.That(animals[0].Name, Is.EqualTo(@"Fido"));
			Assert.That(animals[1].Name, Is.EqualTo(@"Tigger"));
		}

#if !MONOTOUCH
        /// <summary>
        /// Can deserialise an entity containing a polymorphic property serialized by
        /// datacontractjsonserializer.
        /// </summary>
		[Test]
		public void Can_deserialise_an_entity_containing_a_polymorphic_property_serialized_by_datacontractjsonserializer()
		{
		    Func<string, Type> typeFinder = value => {
                    var regex = new Regex(@"^(?<type>[^:]+):#(?<namespace>.*)$");
		            var match = regex.Match(value);
		            var typeName = string.Format("{0}.{1}", match.Groups["namespace"].Value, match.Groups["type"].Value.Replace(".", "+"));
                    return AssemblyUtils.FindType(typeName);
		        };

            try {
                var originalPets = new Pets {Cat = new Cat {Name = "Tigger"}, Dog = new Dog {Name = "Fido"}};

		        var dataContractJsonSerializer = new DataContractJsonSerializer(typeof (Pets), new[] {typeof (Dog), typeof (Cat)}, int.MaxValue, true, null, true);
                JsConfig.TypeFinder = typeFinder;
		        Pets deserializedPets = null;
		        using (var stream = new MemoryStream()) {
                    dataContractJsonSerializer.WriteObject(stream, originalPets);
		            stream.Position = 0;
                    using (var reader = new StreamReader(stream)) {
                        var json = reader.ReadToEnd();
		                deserializedPets = JsonSerializer.DeserializeFromString<Pets>(json);
                    }
		        }

			    Assert.That(deserializedPets.Cat.GetType(), Is.EqualTo(originalPets.Cat.GetType()));
			    Assert.That(deserializedPets.Dog.GetType(), Is.EqualTo(originalPets.Dog.GetType()));

			    Assert.That(deserializedPets.Cat.Name, Is.EqualTo(originalPets.Cat.Name));
			    Assert.That(deserializedPets.Dog.Name, Is.EqualTo(originalPets.Dog.Name));
            } finally {
                JsConfig.Reset();
            }
		}
#endif

        /// <summary>
        /// Can deserialise an entity containing a polymorphic property serialized by newtonsoft.
        /// </summary>
		[Test]
		public void Can_deserialise_an_entity_containing_a_polymorphic_property_serialized_by_newtonsoft()
		{
			var json =
					"{\"$type\":\""
                    + typeof(Pets).ToTypeString()
					+ "\",\"Dog\":{\"$type\":\""
					+ typeof(Dog).ToTypeString()
					+ "\",\"Name\":\"Fido\"},\"Cat\":{\"$type\":\""
					+ typeof(Cat).ToTypeString()
					+ "\",\"Name\":\"Tigger\"}}";
            try {
                //var originalPets = new Pets {Cat = new Cat {Name = "Tigger"}, Dog = new Dog {Name = "Fido"}};

                //var newtonsoftSerializer = new Newtonsoft.Json.JsonSerializer { TypeNameHandling = TypeNameHandling.All };
                //var buffer = new StringBuilder();
                //using (var writer = new StringWriter(buffer)) {
                //    newtonsoftSerializer.Serialize(writer, originalPets);
                //}
                //var json = buffer.ToString();
                JsConfig.TypeAttr = "$type";
		        var deserializedPets = JsonSerializer.DeserializeFromString<Pets>(json);

			    Assert.That(deserializedPets.Cat.GetType(), Is.EqualTo(typeof(Cat)));
			    Assert.That(deserializedPets.Dog.GetType(), Is.EqualTo(typeof(Dog)));

			    Assert.That(deserializedPets.Cat.Name, Is.EqualTo("Tigger"));
			    Assert.That(deserializedPets.Dog.Name, Is.EqualTo("Fido"));
            } finally {
                JsConfig.Reset();
            }
		}

        /// <summary>Can deserialise polymorphic list serialized by newtonsoft.</summary>
		[Test]
		public void Can_deserialise_polymorphic_list_serialized_by_newtonsoft()
		{
            var json = 
					"[{\"$type\":\""
					+ typeof(Dog).ToTypeString()
					+ "\",\"Name\":\"Fido\"},{\"$type\":\""
					+ typeof(Cat).ToTypeString()
					+ "\",\"Name\":\"Tigger\"}}]";

            try {
		        var originalList = new List<Animal> {new Dog {Name = "Fido"}, new Cat {Name = "Tigger"}};

                //var newtonsoftSerializer = new Newtonsoft.Json.JsonSerializer { TypeNameHandling = TypeNameHandling.All };
                //var buffer = new StringBuilder();
                //using (var writer = new StringWriter(buffer)) {
                //    newtonsoftSerializer.Serialize(writer, originalList);
                //}
                //var json = buffer.ToString();

                JsConfig.TypeAttr = "$type";
		        var deserializedList = JsonSerializer.DeserializeFromString<List<Animal>>(json);

			    Assert.That(deserializedList.Count, Is.EqualTo(originalList.Count));

			    Assert.That(deserializedList[0].GetType(), Is.EqualTo(originalList[0].GetType()));
			    Assert.That(deserializedList[1].GetType(), Is.EqualTo(originalList[1].GetType()));

			    Assert.That(deserializedList[0].Name, Is.EqualTo(originalList[0].Name));
			    Assert.That(deserializedList[1].Name, Is.EqualTo(originalList[1].Name));
            } finally {
                JsConfig.Reset();
            }
		}

        /// <summary>A pets.</summary>
		public class Pets
		{
            /// <summary>Gets or sets the category.</summary>
            /// <value>The category.</value>
			public ICat Cat { get; set; }

            /// <summary>Gets or sets the dog.</summary>
            /// <value>The dog.</value>
			public IDog Dog { get; set; }
		}

        /// <summary>An explicit pets.</summary>
		public class ExplicitPets
		{
            /// <summary>Gets or sets the category.</summary>
            /// <value>The category.</value>
			public Cat Cat { get; set; }

            /// <summary>Gets or sets the dog.</summary>
            /// <value>The dog.</value>
			public OtherDog Dog { get; set; }
		}

        /// <summary>An other dog.</summary>
		public class OtherDog : IDog
		{
            /// <summary>Gets or sets the name.</summary>
            /// <value>The name.</value>
			public string Name { get; set; }
		}

        /// <summary>Can force specific type information.</summary>
		[Test]
		public void Can_force_specific_TypeInfo()
		{
			//This configuration has to be set before first usage of WriteType<OtherDog>, otherwise this setting change will not be applied
			JsConfig<OtherDog>.IncludeTypeInfo = true;

			var pets = new ExplicitPets()
			{
				Cat = new Cat { Name = "Cat" },
				Dog = new OtherDog { Name = "Dog" },
			};
			Assert.That(pets.ToJson(), Is.EqualTo(
				@"{""Cat"":{""Name"":""Cat""},""Dog"":{""__type"":""NServiceKit.Text.Tests.JsonTests.PolymorphicListTests+OtherDog, " + assemblyName + @""",""Name"":""Dog""}}"));

			Assert.That(new OtherDog { Name = "Dog" }.ToJson(), Is.EqualTo(
				@"{""__type"":""NServiceKit.Text.Tests.JsonTests.PolymorphicListTests+OtherDog, " + assemblyName + @""",""Name"":""Dog""}"));
		}

        /// <summary>Can exclude specific type information.</summary>
		[Test]
		public void Can_exclude_specific_TypeInfo()
		{
			JsConfig<ICat>.ExcludeTypeInfo = true;
			var pets = new Pets {
				Cat = new Cat { Name = "Cat" },
				Dog = new Dog { Name = "Dog" },
			};

			Assert.That(pets.ToJson(), Is.EqualTo(
				@"{""Cat"":{""Name"":""Cat""},""Dog"":{""__type"":""NServiceKit.Text.Tests.JsonTests.Dog, " + assemblyName + @""",""Name"":""Dog""}}"));
		}

        /// <summary>A pet dog.</summary>
		public class PetDog
		{
            /// <summary>Gets or sets the dog.</summary>
            /// <value>The dog.</value>
			public IDog Dog { get; set; }
		}

        /// <summary>A weird category.</summary>
		public class WeirdCat
		{
            /// <summary>Gets or sets the dog.</summary>
            /// <value>The dog.</value>
			public Cat Dog { get; set; }
		}

        /// <summary>Can read as category from dog with typeinfo.</summary>
		[Test]
		public void Can_read_as_Cat_from_Dog_with_typeinfo()
		{
			var petDog = new PetDog { Dog = new Dog { Name = "Woof!" } };
			var json = petDog.ToJson();

			Console.WriteLine(json);

			var weirdCat = json.FromJson<WeirdCat>();

			Assert.That(weirdCat.Dog, Is.Not.Null);
			Assert.That(weirdCat.Dog.Name, Is.EqualTo(petDog.Dog.Name));
		}

        /// <summary>
        /// Can serialize and deserialize an entity containing a polymorphic item with additional
        /// properties correctly.
        /// </summary>
		[Test]
		public void Can_serialize_and_deserialize_an_entity_containing_a_polymorphic_item_with_additional_properties_correctly()
		{
		    Pets pets = new Pets { Cat = new Cat { Name = "Kitty"}, Dog = new Collie { Name = "Lassie", IsLassie = true}};
		    string serializedPets = JsonSerializer.SerializeToString(pets);
		    Pets deserialized = JsonSerializer.DeserializeFromString<Pets>(serializedPets);

		    Assert.That(deserialized.Cat, Is.TypeOf(typeof(Cat)));
		    Assert.That(deserialized.Cat.Name, Is.EqualTo("Kitty"));

		    Assert.That(deserialized.Dog, Is.TypeOf(typeof(Collie)));
		    Assert.That(deserialized.Dog.Name, Is.EqualTo("Lassie"));
		    Assert.That(((Collie)deserialized.Dog).IsLassie, Is.True);
		}

        /// <summary>
        /// Polymorphic serialization of class implementing generic ienumerable works correctly.
        /// </summary>
	    [Test]
	    public void polymorphic_serialization_of_class_implementing_generic_ienumerable_works_correctly()
	    {
	        var terms = new Terms {new FooTerm()};
            var output = JsonSerializer.SerializeToString(terms);
            Log(output);
	        Assert.IsTrue(output.Contains("__type"));
            var terms2 = JsonSerializer.DeserializeFromString<Terms>(output);
	        Assert.IsAssignableFrom<FooTerm>(terms2.First());
	    }

	}
}