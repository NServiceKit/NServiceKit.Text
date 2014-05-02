using NUnit.Framework;

namespace NServiceKit.Text.Tests.JsonTests
{
    /// <summary>A JSON object tests.</summary>
    [TestFixture]
    public class JsonObjectTests
    {
        /// <summary>Can parse empty object.</summary>
        [Test]
        public void Can_parse_empty_object()
        {
            Assert.That(JsonObject.Parse("{}"), Is.Empty);
        }

        /// <summary>Can parse empty object with whitespaces.</summary>
        [Test]
        public void Can_parse_empty_object_with_whitespaces()
        {
            Assert.That(JsonObject.Parse("{    }"), Is.Empty);
            Assert.That(JsonObject.Parse("{\n\n}"), Is.Empty);
            Assert.That(JsonObject.Parse("{\t\t}"), Is.Empty);
        }

        /// <summary>Can parse empty object with mixed whitespaces.</summary>
        [Test]
        public void Can_parse_empty_object_with_mixed_whitespaces()
        {
            Assert.That(JsonObject.Parse("{ \n\t  \n\r}"), Is.Empty);
        }

        /// <summary>A jackalope.</summary>
        public class Jackalope
        {
            /// <summary>Gets or sets the name.</summary>
            /// <value>The name.</value>
            public string Name { get; set; }

            /// <summary>Gets or sets the baby jackalope.</summary>
            /// <value>The baby jackalope.</value>
            public Jackalope BabyJackalope { get; set; }
        }

        /// <summary>Can serialise JSON object deserialise typed object.</summary>
        [Test]
        public void Can_serialise_json_object_deserialise_typed_object()
        {
            var jacks = new {
                Jack = new Jackalope { BabyJackalope = new Jackalope { Name = "in utero" } }
            };

            var jackString = JsonSerializer.SerializeToString(jacks.Jack);

            var jackJson = JsonObject.Parse(jackString);
            var jack = jackJson.Get<Jackalope>("BabyJackalope");

            Assert.That(jacks.Jack.BabyJackalope.Name, Is.EqualTo(jack.Name));

            var jackJsonString = jackJson.SerializeToString();
            Assert.That(jackString, Is.EqualTo(jackJsonString));

            var jackalope = JsonSerializer.DeserializeFromString<Jackalope>(jackJsonString);
            Assert.That(jackalope.BabyJackalope.Name, Is.EqualTo("in utero"));
        }

        /// <summary>The text.</summary>
        readonly TextElementDto text = new TextElementDto {
            ElementId = "text_1",
            ElementType = "text",
            // Raw nesting - won't be escaped
            Content = new ElementContentDto { ElementId = "text_1", Content = "text goes here" },
            Action = new ElementActionDto { ElementId = "text_1", Action = "action goes here" }
        };

        /// <summary>The image.</summary>
        readonly ImageElementDto image = new ImageElementDto {
            ElementId = "image_1",
            ElementType = "image",
            // String nesting - will be escaped
            Content = new ElementContentDto { ElementId = "image_1", Content = "image url goes here" }.ToJson(),
            Action = new ElementActionDto { ElementId = "image_1", Action = "action goes here" }.ToJson()
        };

        /// <summary>Can serialize typed container dto.</summary>
        [Test]
        public void Can_Serialize_TypedContainerDto()
        {
            var container = new TypedContainerDto {
                Source = text,
                Destination = image
            };

            var json = container.ToJson();

            var fromJson = json.FromJson<TypedContainerDto>();

            Assert.That(container.Source.Action.ElementId, Is.EqualTo(fromJson.Source.Action.ElementId));

            var imgContent = container.Destination.Content.FromJson<ElementContentDto>();
            var fromContent = fromJson.Destination.Content.FromJson<ElementContentDto>();

            Assert.That(imgContent.ElementId, Is.EqualTo(fromContent.ElementId));
        }

        /// <summary>Can de serialize typed container dto with JSON object.</summary>
        [Test]
        public void Can_DeSerialize_TypedContainerDto_with_JsonObject()
        {
            var container = new TypedContainerDto {
                Source = text,
                Destination = image
            };

            var json = container.ToJson();

            var fromText = JsonObject.Parse(json).Get<TextElementDto>("Source");

            Assert.That(container.Source.Action.ElementId, Is.EqualTo(fromText.Action.ElementId));
        }

        /// <summary>Can de serialize typed container dto into JSON value container dto.</summary>
        [Test]
        public void Can_DeSerialize_TypedContainerDto_into_JsonValueContainerDto()
        {
            var container = new TypedContainerDto {
                Source = text,
                Destination = image
            };

            var json = container.ToJson();

            var fromJson = json.FromJson<JsonValueContainerDto>();

            var fromText = fromJson.Source.As<TextElementDto>();
            var fromImage = fromJson.Destination.As<ImageElementDto>();

            Assert.That(container.Source.Action.ElementId, Is.EqualTo(fromText.Action.ElementId));
            Assert.That(container.Destination.ElementId, Is.EqualTo(fromImage.ElementId));

            Assert.That(container.Destination.Action, Is.EqualTo(fromImage.Action));
            Assert.That(container.Destination.Content, Is.EqualTo(fromImage.Content));
        }

        /// <summary>Can serialize string container dto.</summary>
        [Test]
        public void Can_Serialize_StringContainerDto()
        {
            var container = new StringContainerDto {
                Source = text.ToJson(),
                Destination = image.ToJson()
            };

            var json = container.ToJson();

            var fromJson = json.FromJson<StringContainerDto>();

            var src = container.Source.FromJson<TextElementDto>();
            var dst = container.Destination.FromJson<ImageElementDto>();

            var fromSrc = fromJson.Source.FromJson<TextElementDto>();
            var fromDst = fromJson.Destination.FromJson<ImageElementDto>();

            Assert.That(src.Action.ElementId, Is.EqualTo(fromSrc.Action.ElementId));
            Assert.That(dst.Action, Is.EqualTo(fromDst.Action));
        }

        /// <summary>A typed container dto.</summary>
        public class TypedContainerDto
        {
            /// <summary>Gets or sets the source for the.</summary>
            /// <value>The source.</value>
            public TextElementDto Source { get; set; }

            /// <summary>Gets or sets the Destination for the.</summary>
            /// <value>The destination.</value>
            public ImageElementDto Destination { get; set; }
        }

        /// <summary>DTOs.</summary>
        public class StringContainerDto // This is the request dto
        {
            /// <summary>Gets or sets the source for the.</summary>
            /// <value>The source.</value>
            public string Source { get; set; } // This will be some ElementDto

            /// <summary>Gets or sets the Destination for the.</summary>
            /// <value>The destination.</value>
            public string Destination { get; set; } // This will be some ElementDto
        }

        /// <summary>DTOs.</summary>
        public class JsonValueContainerDto // This is the request dto
        {
            /// <summary>Gets or sets the source for the.</summary>
            /// <value>The source.</value>
            public JsonValue Source { get; set; } // This will be some ElementDto

            /// <summary>Gets or sets the Destination for the.</summary>
            /// <value>The destination.</value>
            public JsonValue Destination { get; set; } // This will be some ElementDto
        }

        /// <summary>A text element dto.</summary>
        public class TextElementDto
        {
            /// <summary>Gets or sets the type of the element.</summary>
            /// <value>The type of the element.</value>
            public string ElementType { get; set; }

            /// <summary>Gets or sets the identifier of the element.</summary>
            /// <value>The identifier of the element.</value>
            public string ElementId { get; set; }

            /// <summary>Gets or sets the content.</summary>
            /// <value>The content.</value>
            public ElementContentDto Content { get; set; }

            /// <summary>Gets or sets the action.</summary>
            /// <value>The action.</value>
            public ElementActionDto Action { get; set; }
        }

        /// <summary>An image element dto.</summary>
        public class ImageElementDto
        {
            /// <summary>Gets or sets the type of the element.</summary>
            /// <value>The type of the element.</value>
            public string ElementType { get; set; }

            /// <summary>Gets or sets the identifier of the element.</summary>
            /// <value>The identifier of the element.</value>
            public string ElementId { get; set; }

            /// <summary>Gets or sets the content.</summary>
            /// <value>The content.</value>
            public string Content { get; set; }

            /// <summary>Gets or sets the action.</summary>
            /// <value>The action.</value>
            public string Action { get; set; }
        }

        /// <summary>An element content dto.</summary>
        public class ElementContentDto
        {
            /// <summary>Gets or sets the identifier of the element.</summary>
            /// <value>The identifier of the element.</value>
            public string ElementId { get; set; }

            /// <summary>Gets or sets the content.</summary>
            /// <value>The content.</value>
            public string Content { get; set; }
            // There can be more nested objects in here
        }

        /// <summary>An element action dto.</summary>
        public class ElementActionDto
        {
            /// <summary>Gets or sets the identifier of the element.</summary>
            /// <value>The identifier of the element.</value>
            public string ElementId { get; set; }

            /// <summary>Gets or sets the action.</summary>
            /// <value>The action.</value>
            public string Action { get; set; }
            // There can be more nested objects in here
        }
    }
}
