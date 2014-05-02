using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using NServiceKit.Common.Extensions;
using NServiceKit.DataAnnotations;

namespace NServiceKit.Text.Tests.Support
{
    /// <summary>The movies data.</summary>
	public static class MoviesData
	{
        /// <summary>The movies.</summary>
		public static List<Movie> Movies = new List<Movie>
		{
			new Movie { ImdbId = "tt0111161", Title = "The Shawshank Redemption", Rating = 9.2m, Director = "Frank Darabont", ReleaseDate = new DateTime(1995,2,17), TagLine = "Fear can hold you prisoner. Hope can set you free.", Genres = new List<string>{"Crime","Drama"}, },
			new Movie { ImdbId = "tt0068646", Title = "The Godfather", Rating = 9.2m, Director = "Francis Ford Coppola", ReleaseDate = new DateTime(1972,3,24), TagLine = "An offer you can't refuse.", Genres = new List<string> {"Crime","Drama", "Thriller"}, },
			new Movie { ImdbId = "tt1375666", Title = "Inception", Rating = 9.2m, Director = "Christopher Nolan", ReleaseDate = new DateTime(2010,7,16), TagLine = "Your mind is the scene of the crime", Genres = new List<string>{"Action", "Mystery", "Sci-Fi", "Thriller"}, },
			new Movie { ImdbId = "tt0071562", Title = "The Godfather: Part II", Rating = 9.0m, Director = "Francis Ford Coppola", ReleaseDate = new DateTime(1974,12,20), Genres = new List<string> {"Crime","Drama", "Thriller"}, },
			new Movie { ImdbId = "tt0060196", Title = "The Good, the Bad and the Ugly", Rating = 9.0m, Director = "Sergio Leone", ReleaseDate = new DateTime(1967,12,29), TagLine = "They formed an alliance of hate to steal a fortune in dead man's gold", Genres = new List<string>{"Adventure","Western"}, },
		};
		
	}

    /// <summary>A movie.</summary>
	[DataContract]
	public class Movie
	{
        /// <summary>
        /// Initializes a new instance of the NServiceKit.Text.Tests.Support.Movie class.
        /// </summary>
		public Movie()
		{
			this.Genres = new List<string>();
		}

        /// <summary>Gets or sets the identifier.</summary>
        /// <value>The identifier.</value>
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        [AutoIncrement]
		public int Id { get; set; }

        /// <summary>Gets or sets the identifier of the imdb.</summary>
        /// <value>The identifier of the imdb.</value>
        [DataMember(Order = 3, EmitDefaultValue = false, IsRequired = false)]
        public string ImdbId { get; set; }

        /// <summary>Gets or sets the title.</summary>
        /// <value>The title.</value>
        [DataMember(Order = 2, EmitDefaultValue = false, IsRequired = false)]
        public string Title { get; set; }

        /// <summary>Gets or sets the rating.</summary>
        /// <value>The rating.</value>
        [DataMember(Order = 4, EmitDefaultValue = false, IsRequired = false)]
        public decimal Rating { get; set; }

        /// <summary>Gets or sets the director.</summary>
        /// <value>The director.</value>
        [DataMember(Order = 5, EmitDefaultValue = true, IsRequired = false)]
        public string Director { get; set; }

        /// <summary>Gets or sets the release date.</summary>
        /// <value>The release date.</value>
        [DataMember(Order = 6, EmitDefaultValue = false, IsRequired = false)]
        public DateTime ReleaseDate { get; set; }

        /// <summary>Gets or sets the tag line.</summary>
        /// <value>The tag line.</value>
        [DataMember(Order = 6, EmitDefaultValue = false, IsRequired = false)]
        public string TagLine { get; set; }

        /// <summary>Gets or sets the genres.</summary>
        /// <value>The genres.</value>
        [DataMember(Order = 8, EmitDefaultValue = false, IsRequired = false)]
        public List<string> Genres { get; set; }

		#region AutoGen ReSharper code, only required by tests
        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object" /> is equal to the current
        /// <see cref="T:System.Object" />.
        /// </summary>
        /// <param name="other">The movie to compare to this object.</param>
        /// <returns>
        /// true if the specified <see cref="T:System.Object" /> is equal to the current
        /// <see cref="T:System.Object" />; otherwise, false.
        /// </returns>
		public bool Equals(Movie other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other.ImdbId, ImdbId)
				&& Equals(other.Title, Title)
				&& other.Rating == Rating
				&& Equals(other.Director, Director)
				&& other.ReleaseDate.Equals(ReleaseDate)
				&& Equals(other.TagLine, TagLine)
				&& Genres.EquivalentTo(other.Genres);
		}

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object" /> is equal to the current
        /// <see cref="T:System.Object" />.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object" /> to compare with the current
        /// <see cref="T:System.Object" />.</param>
        /// <returns>
        /// true if the specified <see cref="T:System.Object" /> is equal to the current
        /// <see cref="T:System.Object" />; otherwise, false.
        /// </returns>
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(Movie)) return false;
			return Equals((Movie)obj);
		}

        /// <summary>Serves as a hash function for a particular type.</summary>
        /// <returns>A hash code for the current <see cref="T:System.Object" />.</returns>
		public override int GetHashCode()
		{
			return ImdbId != null ? ImdbId.GetHashCode() : 0;
		}
		#endregion
	}

    /// <summary>A movie response.</summary>
    [DataContract]
    public class MovieResponse
    {
        /// <summary>Gets or sets the movie.</summary>
        /// <value>The movie.</value>
        [DataMember]
        public Movie Movie { get; set; }
    }

    /// <summary>The movies response.</summary>
    [DataContract]
    public class MoviesResponse
    {
        /// <summary>Gets or sets the identifier.</summary>
        /// <value>The identifier.</value>
        [DataMember]
        public int Id { get; set; }

        /// <summary>Gets or sets the movies.</summary>
        /// <value>The movies.</value>
        [DataMember]
        public List<Movie> Movies { get; set; }
    }

    /// <summary>The movies response 2.</summary>
    [Csv(CsvBehavior.FirstEnumerable)]
    public class MoviesResponse2
    {
        /// <summary>Gets or sets the identifier.</summary>
        /// <value>The identifier.</value>
        public int Id { get; set; }

        /// <summary>Gets or sets the movies.</summary>
        /// <value>The movies.</value>
        public List<Movie> Movies { get; set; }
    }

}