using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace NServiceKit.Text.Tests.DynamicModels
{
    /// <summary>A google maps API tests.</summary>
    public class GoogleMapsApiTests
    {
        /// <summary>The JSON dto.</summary>
        private const string JsonDto =
            @"{
   ""results"" : [
      {
         ""address_components"" : [
            {
               ""long_name"" : ""108"",
               ""short_name"" : ""108"",
               ""types"" : [ ""street_number"" ]
            },
            {
               ""long_name"" : ""S Almansor St"",
               ""short_name"" : ""S Almansor St"",
               ""types"" : [ ""route"" ]
            },
            {
               ""long_name"" : ""Alhambra"",
               ""short_name"" : ""Alhambra"",
               ""types"" : [ ""locality"", ""political"" ]
            },
            {
               ""long_name"" : ""Los Angeles"",
               ""short_name"" : ""Los Angeles"",
               ""types"" : [ ""administrative_area_level_2"", ""political"" ]
            },
            {
               ""long_name"" : ""California"",
               ""short_name"" : ""CA"",
               ""types"" : [ ""administrative_area_level_1"", ""political"" ]
            },
            {
               ""long_name"" : ""United States"",
               ""short_name"" : ""US"",
               ""types"" : [ ""country"", ""political"" ]
            },
            {
               ""long_name"" : ""91801"",
               ""short_name"" : ""91801"",
               ""types"" : [ ""postal_code"" ]
            }
         ],
         ""formatted_address"" : ""108 S Almansor St, Alhambra, CA 91801, USA"",
         ""geometry"" : {
            ""location"" : {
               ""lat"" : 34.096680,
               ""lng"" : -118.1197330
            },
            ""location_type"" : ""ROOFTOP"",
            ""viewport"" : {
               ""northeast"" : {
                  ""lat"" : 34.09802898029150,
                  ""lng"" : -118.1183840197085
               },
               ""southwest"" : {
                  ""lat"" : 34.09533101970850,
                  ""lng"" : -118.1210819802915
               }
            }
         },
         ""types"" : [ ""street_address"" ]
      },
      {
         ""address_components"" : [
            {
               ""long_name"" : ""91801"",
               ""short_name"" : ""91801"",
               ""types"" : [ ""postal_code"" ]
            },
            {
               ""long_name"" : ""Alhambra"",
               ""short_name"" : ""Alhambra"",
               ""types"" : [ ""locality"", ""political"" ]
            },
            {
               ""long_name"" : ""Los Angeles"",
               ""short_name"" : ""Los Angeles"",
               ""types"" : [ ""administrative_area_level_2"", ""political"" ]
            },
            {
               ""long_name"" : ""California"",
               ""short_name"" : ""CA"",
               ""types"" : [ ""administrative_area_level_1"", ""political"" ]
            },
            {
               ""long_name"" : ""United States"",
               ""short_name"" : ""US"",
               ""types"" : [ ""country"", ""political"" ]
            }
         ],
         ""formatted_address"" : ""Alhambra, CA 91801, USA"",
         ""geometry"" : {
            ""bounds"" : {
               ""northeast"" : {
                  ""lat"" : 34.1111460,
                  ""lng"" : -118.1081760
               },
               ""southwest"" : {
                  ""lat"" : 34.069770,
                  ""lng"" : -118.160660
               }
            },
            ""location"" : {
               ""lat"" : 34.08379580,
               ""lng"" : -118.11811990
            },
            ""location_type"" : ""APPROXIMATE"",
            ""viewport"" : {
               ""northeast"" : {
                  ""lat"" : 34.1111460,
                  ""lng"" : -118.1081760
               },
               ""southwest"" : {
                  ""lat"" : 34.069770,
                  ""lng"" : -118.160660
               }
            }
         },
         ""types"" : [ ""postal_code"" ]
      },
      {
         ""address_components"" : [
            {
               ""long_name"" : ""Alhambra"",
               ""short_name"" : ""Alhambra"",
               ""types"" : [ ""locality"", ""political"" ]
            },
            {
               ""long_name"" : ""Los Angeles"",
               ""short_name"" : ""Los Angeles"",
               ""types"" : [ ""administrative_area_level_2"", ""political"" ]
            },
            {
               ""long_name"" : ""California"",
               ""short_name"" : ""CA"",
               ""types"" : [ ""administrative_area_level_1"", ""political"" ]
            },
            {
               ""long_name"" : ""United States"",
               ""short_name"" : ""US"",
               ""types"" : [ ""country"", ""political"" ]
            }
         ],
         ""formatted_address"" : ""Alhambra, CA, USA"",
         ""geometry"" : {
            ""bounds"" : {
               ""northeast"" : {
                  ""lat"" : 34.1111460,
                  ""lng"" : -118.1081810
               },
               ""southwest"" : {
                  ""lat"" : 34.05992090,
                  ""lng"" : -118.1648350
               }
            },
            ""location"" : {
               ""lat"" : 34.0952870,
               ""lng"" : -118.12701460
            },
            ""location_type"" : ""APPROXIMATE"",
            ""viewport"" : {
               ""northeast"" : {
                  ""lat"" : 34.1111460,
                  ""lng"" : -118.1081810
               },
               ""southwest"" : {
                  ""lat"" : 34.05992090,
                  ""lng"" : -118.1648350
               }
            }
         },
         ""types"" : [ ""locality"", ""political"" ]
      },
      {
         ""address_components"" : [
            {
               ""long_name"" : ""Los Angeles"",
               ""short_name"" : ""Los Angeles"",
               ""types"" : [ ""administrative_area_level_2"", ""political"" ]
            },
            {
               ""long_name"" : ""California"",
               ""short_name"" : ""CA"",
               ""types"" : [ ""administrative_area_level_1"", ""political"" ]
            },
            {
               ""long_name"" : ""United States"",
               ""short_name"" : ""US"",
               ""types"" : [ ""country"", ""political"" ]
            }
         ],
         ""formatted_address"" : ""Los Angeles, CA, USA"",
         ""geometry"" : {
            ""bounds"" : {
               ""northeast"" : {
                  ""lat"" : 34.82319290,
                  ""lng"" : -117.6456040
               },
               ""southwest"" : {
                  ""lat"" : 32.79837620,
                  ""lng"" : -118.94490370
               }
            },
            ""location"" : {
               ""lat"" : 34.38718210,
               ""lng"" : -118.11226790
            },
            ""location_type"" : ""APPROXIMATE"",
            ""viewport"" : {
               ""northeast"" : {
                  ""lat"" : 34.82319290,
                  ""lng"" : -117.6456040
               },
               ""southwest"" : {
                  ""lat"" : 32.79837620,
                  ""lng"" : -118.94490370
               }
            }
         },
         ""types"" : [ ""administrative_area_level_2"", ""political"" ]
      },
      {
         ""address_components"" : [
            {
               ""long_name"" : ""California"",
               ""short_name"" : ""CA"",
               ""types"" : [ ""administrative_area_level_1"", ""political"" ]
            },
            {
               ""long_name"" : ""United States"",
               ""short_name"" : ""US"",
               ""types"" : [ ""country"", ""political"" ]
            }
         ],
         ""formatted_address"" : ""California, USA"",
         ""geometry"" : {
            ""bounds"" : {
               ""northeast"" : {
                  ""lat"" : 42.00951690,
                  ""lng"" : -114.1312110
               },
               ""southwest"" : {
                  ""lat"" : 32.53420710,
                  ""lng"" : -124.40961950
               }
            },
            ""location"" : {
               ""lat"" : 36.7782610,
               ""lng"" : -119.41793240
            },
            ""location_type"" : ""APPROXIMATE"",
            ""viewport"" : {
               ""northeast"" : {
                  ""lat"" : 42.00951690,
                  ""lng"" : -114.1312110
               },
               ""southwest"" : {
                  ""lat"" : 32.53420710,
                  ""lng"" : -124.40961950
               }
            }
         },
         ""types"" : [ ""administrative_area_level_1"", ""political"" ]
      },
      {
         ""address_components"" : [
            {
               ""long_name"" : ""United States"",
               ""short_name"" : ""US"",
               ""types"" : [ ""country"", ""political"" ]
            }
         ],
         ""formatted_address"" : ""United States"",
         ""geometry"" : {
            ""bounds"" : {
               ""northeast"" : {
                  ""lat"" : 71.3898880,
                  ""lng"" : -66.94539480000002
               },
               ""southwest"" : {
                  ""lat"" : 18.91106430,
                  ""lng"" : 172.45469670
               }
            },
            ""location"" : {
               ""lat"" : 37.090240,
               ""lng"" : -95.7128910
            },
            ""location_type"" : ""APPROXIMATE"",
            ""viewport"" : {
               ""northeast"" : {
                  ""lat"" : 71.3898880,
                  ""lng"" : -66.94539480000002
               },
               ""southwest"" : {
                  ""lat"" : 18.91106430,
                  ""lng"" : 172.45469670
               }
            }
         },
         ""types"" : [ ""country"", ""political"" ]
      }
   ],
   ""status"" : ""OK""
}";

        /// <summary>A geo location response.</summary>
        public class GeoLocationResponse
        {
            /// <summary>Gets or sets the status.</summary>
            /// <value>The status.</value>
            public string Status { get; set; }

            /// <summary>Gets or sets the results.</summary>
            /// <value>The results.</value>
            public List<GeoLocationResults> Results { get; set; }
        }

        /// <summary>A geo location results.</summary>
        public class GeoLocationResults
        {
            /// <summary>Gets or sets the address components.</summary>
            /// <value>The address components.</value>
            public List<AddressComponent> Address_Components { get; set; }

            /// <summary>Gets or sets the formatted address.</summary>
            /// <value>The formatted address.</value>
            public string Formatted_Address { get; set; }

            /// <summary>Gets or sets the geometry.</summary>
            /// <value>The geometry.</value>
            public Geometry Geometry { get; set; }

            /// <summary>Gets or sets the types.</summary>
            /// <value>The types.</value>
            public string[] Types { get; set; }
        }

        /// <summary>A geometry.</summary>
        public class Geometry
        {
            /// <summary>Gets or sets the bounds.</summary>
            /// <value>The bounds.</value>
            public GeometryBounds Bounds { get; set; }

            /// <summary>Gets or sets the location.</summary>
            /// <value>The location.</value>
            public GeometryLatLong Location { get; set; }

            /// <summary>Gets or sets the type of the location.</summary>
            /// <value>The type of the location.</value>
            public string Location_Type { get; set; }

            /// <summary>Gets or sets the viewport.</summary>
            /// <value>The viewport.</value>
            public GeometryBounds Viewport { get; set; }
        }

        /// <summary>A geometry bounds.</summary>
        public class GeometryBounds
        {
            /// <summary>Gets or sets the north east.</summary>
            /// <value>The north east.</value>
            public GeometryLatLong NorthEast { get; set; }

            /// <summary>Gets or sets the south west.</summary>
            /// <value>The south west.</value>
            public GeometryLatLong SouthWest { get; set; }
        }

        /// <summary>A geometry lat long.</summary>
        public class GeometryLatLong
        {
            /// <summary>Gets or sets the lat.</summary>
            /// <value>The lat.</value>
            public string Lat { get; set; }

            /// <summary>Gets or sets the. </summary>
            /// <value>The lng.</value>
            public string Lng { get; set; }
        }

        /// <summary>The address component.</summary>
        public class AddressComponent
        {
            /// <summary>Gets or sets the name of the long.</summary>
            /// <value>The name of the long.</value>
            public string Long_Name { get; set; }

            /// <summary>Gets or sets the name of the short.</summary>
            /// <value>The name of the short.</value>
            public string Short_Name { get; set; }

            /// <summary>Gets or sets the types.</summary>
            /// <value>The types.</value>
            public List<string> Types { get; set; }
        }

        /// <summary>Can parse g maps API.</summary>
        [Test]
        public void Can_parse_GMaps_api()
        {
            //short for JsonSerializer.DeserializeFromString<GeoLocationResults>(Json)
            var geoApiResponse = JsonDto.FromJson<GeoLocationResponse>();
            //Console.WriteLine(geoApiResponse.Dump());
            Assert.IsNotNullOrEmpty(geoApiResponse.Dump());
        }
    }
}