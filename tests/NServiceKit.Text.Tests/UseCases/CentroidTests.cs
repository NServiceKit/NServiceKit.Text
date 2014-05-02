using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace NServiceKit.Text.Tests.UseCases
{
    /// <summary>
    /// Solution in response to: http://stackoverflow.com/questions/5057684/json-c-deserializing-a-
    /// changing-content-or-a-piece-of-json-response.
    /// </summary>
	public class CentroidTests
	{
        /// <summary>A centroid.</summary>
		public class Centroid
		{
            /// <summary>
            /// Initializes a new instance of the NServiceKit.Text.Tests.UseCases.CentroidTests.Centroid
            /// class.
            /// </summary>
            /// <param name="latitude"> The latitude.</param>
            /// <param name="longitude">The longitude.</param>
			public Centroid(decimal latitude, decimal longitude)
			{
				Latitude = latitude;
				Longitude = longitude;
			}

            /// <summary>Gets the lat lon.</summary>
            /// <value>The lat lon.</value>
			public string LatLon
			{
				get
				{
					return String.Format("{0};{1}", Latitude, Longitude);
				}
			}

            /// <summary>Gets or sets the latitude.</summary>
            /// <value>The latitude.</value>
			public decimal Latitude { get; set; }

            /// <summary>Gets or sets the longitude.</summary>
            /// <value>The longitude.</value>
			public decimal Longitude { get; set; }
		}

        /// <summary>A bounding box.</summary>
		public class BoundingBox
		{
            /// <summary>Gets or sets the south west.</summary>
            /// <value>The south west.</value>
			public Centroid SouthWest { get; set; }

            /// <summary>Gets or sets the north east.</summary>
            /// <value>The north east.</value>
			public Centroid NorthEast { get; set; }
		}

        /// <summary>A place.</summary>
		public class Place
		{
            /// <summary>Gets or sets the identifier of the woe.</summary>
            /// <value>The identifier of the woe.</value>
			public int WoeId { get; set; }

            /// <summary>Gets or sets the name of the place type.</summary>
            /// <value>The name of the place type.</value>
			public string PlaceTypeName { get; set; }

            /// <summary>Gets or sets the name.</summary>
            /// <value>The name.</value>
			public string Name { get; set; }

            /// <summary>Gets or sets the place type name attributes.</summary>
            /// <value>The place type name attributes.</value>
			public Dictionary<string, string> PlaceTypeNameAttrs { get; set; }

            /// <summary>Gets or sets the country.</summary>
            /// <value>The country.</value>
			public string Country { get; set; }

            /// <summary>Gets or sets the country attributes.</summary>
            /// <value>The country attributes.</value>
			public Dictionary<string, string> CountryAttrs { get; set; }

            /// <summary>Gets or sets the admin 1.</summary>
            /// <value>The admin 1.</value>
			public string Admin1 { get; set; }

            /// <summary>Gets or sets the admin 1 attributes.</summary>
            /// <value>The admin 1 attributes.</value>
			public Dictionary<string, string> Admin1Attrs { get; set; }

            /// <summary>Gets or sets the admin 2.</summary>
            /// <value>The admin 2.</value>
			public string Admin2 { get; set; }

            /// <summary>Gets or sets the admin 3.</summary>
            /// <value>The admin 3.</value>
			public string Admin3 { get; set; }

            /// <summary>Gets or sets the locality 1.</summary>
            /// <value>The locality 1.</value>
			public string Locality1 { get; set; }

            /// <summary>Gets or sets the locality 2.</summary>
            /// <value>The locality 2.</value>
			public string Locality2 { get; set; }

            /// <summary>Gets or sets the postal.</summary>
            /// <value>The postal.</value>
			public string Postal { get; set; }

            /// <summary>Gets or sets the centroid.</summary>
            /// <value>The centroid.</value>
			public Centroid Centroid { get; set; }

            /// <summary>Gets or sets the bounding box.</summary>
            /// <value>The bounding box.</value>
			public BoundingBox BoundingBox { get; set; }

            /// <summary>Gets or sets the area rank.</summary>
            /// <value>The area rank.</value>
			public int AreaRank { get; set; }

            /// <summary>Gets or sets the pop rank.</summary>
            /// <value>The pop rank.</value>
			public int PopRank { get; set; }

            /// <summary>Gets or sets URI of the document.</summary>
            /// <value>The URI.</value>
			public string Uri { get; set; }

            /// <summary>Gets or sets the language.</summary>
            /// <value>The language.</value>
			public string Lang { get; set; }
		}

        /// <summary>The JSON centroid.</summary>
		private const string JsonCentroid = @"{
   ""place"":{
      ""woeid"":12345,
      ""placeTypeName"":""State"",
      ""placeTypeName attrs"":{
         ""code"":8
      },
      ""name"":""My Region"",
      ""country"":"""",
      ""country attrs"":{
         ""type"":""Country"",
         ""code"":""XX""
      },
      ""admin1"":""My Region"",
      ""admin1 attrs"":{
         ""type"":""Region"",
         ""code"":""""
      },
      ""admin2"":"""",
      ""admin3"":"""",
      ""locality1"":"""",
      ""locality2"":"""",
      ""postal"":"""",
      ""centroid"":{
         ""latitude"":30.12345,
         ""longitude"":40.761292
      },
      ""boundingBox"":{
         ""southWest"":{
            ""latitude"":32.2799,
            ""longitude"":50.715958
         },
         ""northEast"":{
            ""latitude"":29.024891,
            ""longitude"":12.1234
         }
      },
      ""areaRank"":10,
      ""popRank"":0,
      ""uri"":""http:\/\/where.yahooapis.com"",
      ""lang"":""en-US""
   }
}";

        /// <summary>Can parse centroid using JSON object.</summary>
		[Test]
		public void Can_Parse_Centroid_using_JsonObject()
		{
			Func<JsonObject, Centroid> toCentroid = map => 
				new Centroid(map.Get<decimal>("latitude"), map.Get<decimal>("longitude"));

			var place = JsonObject.Parse(JsonCentroid)
				.Object("place")
				.ConvertTo(x => new Place
				{
					WoeId = x.Get<int>("woeid"),
					PlaceTypeName = x.Get(""),
					PlaceTypeNameAttrs = x.Object("placeTypeName attrs"),
					Name = x.Get("Name"),
					Country = x.Get("Country"),
					CountryAttrs = x.Object("country attrs"),
					Admin1 = x.Get("admin1"),
					Admin1Attrs = x.Object("admin1 attrs"),
					Admin2 = x.Get("admin2"),
					Admin3 = x.Get("admin3"),
					Locality1 = x.Get("locality1"),
					Locality2 = x.Get("locality2"),
					Postal = x.Get("postal"),
					
					Centroid = x.Object("centroid")
						.ConvertTo(toCentroid),
					
					BoundingBox = x.Object("boundingBox")
						.ConvertTo(y => new BoundingBox
						{
							SouthWest = y.Object("southWest").ConvertTo(toCentroid),
							NorthEast = y.Object("northEast").ConvertTo(toCentroid)
						}),
					
					AreaRank = x.Get<int>("areaRank"),
					PopRank = x.Get<int>("popRank"),
					Uri = x.Get("uri"),
					Lang = x.Get("lang"),
				});

			// Console.WriteLine(place.Dump());
            Assert.IsNotNullOrEmpty(place.Dump());
			/*Outputs:
			{
				WoeId: 12345,
				PlaceTypeNameAttrs: 
				{
					code: 8
				},
				CountryAttrs: 
				{
					type: Country,
					code: XX
				},
				Admin1: My Region,
				Admin1Attrs: 
				{
					type: Region,
					code: 
				},
				Admin2: ,
				Admin3: ,
				Locality1: ,
				Locality2: ,
				Postal: ,
				Centroid: 
				{
					LatLon: 30.12345;40.761292,
					Latitude: 30.12345,
					Longitude: 40.761292
				},
				BoundingBox: 
				{
					SouthWest: 
					{
						LatLon: 32.2799;50.715958,
						Latitude: 32.2799,
						Longitude: 50.715958
					},
					NorthEast: 
					{
						LatLon: 29.024891;12.1234,
						Latitude: 29.024891,
						Longitude: 12.1234
					}
				},
				AreaRank: 10,
				PopRank: 0,
				Uri: "http://where.yahooapis.com",
				Lang: en-US
			}
			**/
		}


	}
}