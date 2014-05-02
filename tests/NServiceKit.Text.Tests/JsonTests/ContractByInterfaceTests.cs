using System;
using NUnit.Framework;

namespace NServiceKit.Text.Tests.JsonTests
{
    /// <summary>
    /// Service Bus messaging works best if processes can share interface message contracts but not
    /// have to share concrete types.
    /// </summary>
    [TestFixture]
    public class ContractByInterfaceTests
    {
        /// <summary>
        /// Prefer interfaces should work on top level object using extension method.
        /// </summary>
        [Test]
        public void Prefer_interfaces_should_work_on_top_level_object_using_extension_method()
        {
            using (JsConfig.With(preferInterfaces:true))
            {
                var json = new Concrete("boo", 1).ToJson();

                Assert.That(json, Is.StringContaining("\"NServiceKit.Text.Tests.JsonTests.IContract, NServiceKit.Text.Tests\""));
            }
        }

        /// <summary>Should be able to serialise based on an interface.</summary>
        [Test]
        public void Should_be_able_to_serialise_based_on_an_interface()
        {
            using (JsConfig.With(preferInterfaces: true))
            {
                IContract myConcrete = new Concrete("boo", 1);
                var json = JsonSerializer.SerializeToString(myConcrete, typeof(IContract));

                Console.WriteLine(json);
                Assert.That(json, Is.StringContaining("\"NServiceKit.Text.Tests.JsonTests.IContract, NServiceKit.Text.Tests\""));
            }
        }

        /// <summary>Should not use interface type if concrete specified.</summary>
        [Test]
        public void Should_not_use_interface_type_if_concrete_specified()
        {
            using (JsConfig.With(preferInterfaces: false))
            {
                IContract myConcrete = new Concrete("boo", 1);
                var json = JsonSerializer.SerializeToString(myConcrete, typeof(IContract));

                Console.WriteLine(json);
                Assert.That(json, Is.StringContaining("\"NServiceKit.Text.Tests.JsonTests.Concrete, NServiceKit.Text.Tests\""));
            }
        }

        /// <summary>Should be able to deserialise based on an interface with no concrete.</summary>
        [Test]
        public void Should_be_able_to_deserialise_based_on_an_interface_with_no_concrete()
        {
            using (JsConfig.With(preferInterfaces: true))
            {
                var json = new Concrete("boo", 42).ToJson();

                // break the typing so we have to use the dynamic implementation
                json = json.Replace("NServiceKit.Text.Tests.JsonTests.IContract", "NServiceKit.Text.Tests.JsonTests.IIdenticalContract");

                var result = JsonSerializer.DeserializeFromString<IIdenticalContract>(json);

                Assert.That(result.StringValue, Is.EqualTo("boo"));
                Assert.That(result.ChildProp.IntValue, Is.EqualTo(42));
            }
        }
    }

    /// <summary>A concrete.</summary>
    class Concrete : IContract
    {
        /// <summary>
        /// Initializes a new instance of the NServiceKit.Text.Tests.JsonTests.Concrete class.
        /// </summary>
        /// <param name="boo">The boo.</param>
        /// <param name="i">  Zero-based index of the.</param>
        public Concrete(string boo, int i)
        {
            StringValue = boo;
            ChildProp = new ConcreteChild { IntValue = i };
        }

        /// <summary>Gets or sets the string value.</summary>
        /// <value>The string value.</value>
        public string StringValue { get; set; }

        /// <summary>Gets or sets the child property.</summary>
        /// <value>The child property.</value>
        public IChildInterface ChildProp { get; set; }
    }

    /// <summary>A concrete child.</summary>
    class ConcreteChild : IChildInterface
    {
        /// <summary>Gets or sets the int value.</summary>
        /// <value>The int value.</value>
        public int IntValue { get; set; }
    }

    /// <summary>Interface for child interface.</summary>
    public interface IChildInterface
    {
        /// <summary>Gets or sets the int value.</summary>
        /// <value>The int value.</value>
        int IntValue { get; set; }
    }

    /// <summary>Interface for contract.</summary>
    public interface IContract
    {
        /// <summary>Gets or sets the string value.</summary>
        /// <value>The string value.</value>
        string StringValue { get; set; }

        /// <summary>Gets or sets the child property.</summary>
        /// <value>The child property.</value>
        IChildInterface ChildProp { get; set; }
    }

    /// <summary>Interface for identical contract.</summary>
    public interface IIdenticalContract
    {
        /// <summary>Gets or sets the string value.</summary>
        /// <value>The string value.</value>
        string StringValue { get; set; }

        /// <summary>Gets or sets the child property.</summary>
        /// <value>The child property.</value>
        IChildInterface ChildProp { get; set; }
    }
}
