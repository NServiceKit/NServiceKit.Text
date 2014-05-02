using NServiceKit.Common.Extensions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NServiceKit.Text.Tests.UseCases
{
    /// <summary>A git hub rest tests.</summary>
    [TestFixture]
    public class GitHubRestTests
    {
        /// <summary>The JSON git response.</summary>
        private const string JsonGitResponse = @"{
  ""pulls"": [
    {
      ""state"": ""open"",
      ""base"": {
        ""label"": ""technoweenie:master"",
        ""ref"": ""master"",
        ""sha"": ""53397635da83a2f4b5e862b5e59cc66f6c39f9c6"",
      },
      ""head"": {
        ""label"": ""smparkes:synchrony"",
        ""ref"": ""synchrony"",
        ""sha"": ""83306eef49667549efebb880096cb539bd436560"",
      },
      ""discussion"": [
        {
          ""type"": ""IssueComment"",
          ""gravatar_id"": ""821395fe70906c8290df7f18ac4ac6cf"",
          ""created_at"": ""2010/10/07 07:38:35 -0700"",
          ""body"": ""Did you intend to remove net/http?  Otherwise, this looks good.  Have you tried running the LIVE tests with it?\r\n\r\n    ruby test/live_server.rb # start the demo server\r\n    LIVE=1 rake"",
          ""updated_at"": ""2010/10/07 07:38:35 -0700"",
          ""id"": 453980,
        },
        {
          ""type"": ""Commit"",
          ""created_at"": ""2010-11-04T16:27:45-07:00"",
          ""sha"": ""83306eef49667549efebb880096cb539bd436560"",
          ""author"": ""Steven Parkes"",
          ""subject"": ""add em_synchrony support"",
          ""email"": ""smparkes@smparkes.net""
        }
      ],
      ""title"": ""Synchrony"",
      ""body"": ""Here's the pull request.\r\n\r\nThis isn't generic EM: require's Ilya's synchrony and needs to be run on its own fiber, e.g., via synchrony or rack-fiberpool.\r\n\r\nI thought about a \""first class\"" em adapter, but I think the faraday api is sync right now, right? Interesting idea to add something like rack's async support to faraday, but that's an itch I don't have right now."",
      ""position"": 4.0,
      ""number"": 15,
      ""votes"": 0,
      ""comments"": 4,
      ""diff_url"": ""https://github.com/technoweenie/faraday/pull/15.diff"",
      ""patch_url"": ""https://github.com/technoweenie/faraday/pull/15.patch"",
      ""labels"": [],
      ""html_url"": ""https://github.com/technoweenie/faraday/pull/15"",
      ""issue_created_at"": ""2010-10-04T12:39:18-07:00"",
      ""issue_updated_at"": ""2010-11-04T16:35:04-07:00"",
      ""created_at"": ""2010-10-04T12:39:18-07:00"",
      ""updated_at"": ""2010-11-04T16:30:14-07:00""
    }
  ]
}";

        /// <summary>A discussion.</summary>
        public class Discussion
        {
            /// <summary>Gets or sets the type.</summary>
            /// <value>The type.</value>
            public string Type { get; set; }

            /// <summary>Gets or sets the identifier of the gravatar.</summary>
            /// <value>The identifier of the gravatar.</value>
            public string GravatarId { get; set; }

            /// <summary>Gets or sets the created at.</summary>
            /// <value>The created at.</value>
            public string CreatedAt { get; set; }

            /// <summary>Gets or sets the body.</summary>
            /// <value>The body.</value>
            public string Body { get; set; }

            /// <summary>Gets or sets the updated at.</summary>
            /// <value>The updated at.</value>
            public string UpdatedAt { get; set; }

            /// <summary>Gets or sets the identifier.</summary>
            /// <value>The identifier.</value>
            public int? Id { get; set; }

            /// <summary>Gets or sets the sha.</summary>
            /// <value>The sha.</value>
            public string Sha { get; set; }

            /// <summary>Gets or sets the author.</summary>
            /// <value>The author.</value>
            public string Author { get; set; }

            /// <summary>Gets or sets the subject.</summary>
            /// <value>The subject.</value>
            public string Subject { get; set; }

            /// <summary>Gets or sets the email.</summary>
            /// <value>The email.</value>
            public string Email { get; set; }
        }

        /// <summary>Can parse discussion using JSON object.</summary>
        [Test]
        public void Can_Parse_Discussion_using_JsonObject()
        {
            List<Discussion> discussions = JsonObject.Parse(JsonGitResponse)
            .ArrayObjects("pulls")[0]
            .ArrayObjects("discussion")
            .ConvertAll(x => new Discussion
            {
                Type = x.Get("type"),
                GravatarId = x.Get("gravatar_id"),
                CreatedAt = x.Get("created_at"),
                Body = x.Get("body"),
                UpdatedAt = x.Get("updated_at"),

                Id = x.JsonTo<int?>("id"),
                Sha = x.Get("sha"),
                Author = x.Get("author"),
                Subject = x.Get("subject"),
                Email = x.Get("email"),
            });

            //Console.WriteLine(discussions.Dump()); //See what's been parsed
            Assert.IsNotNullOrEmpty(discussions.Dump());
            Assert.That(discussions.ConvertAll(x => x.Type), Is.EquivalentTo(new[] { "IssueComment", "Commit" }));
        }

        /// <summary>Can parse discussion using only net collection classes.</summary>
        [Test]
        public void Can_Parse_Discussion_using_only_NET_collection_classes()
        {
            var jsonObj = JsonSerializer.DeserializeFromString<List<JsonObject>>(JsonGitResponse);
            var jsonPulls = JsonSerializer.DeserializeFromString<List<JsonObject>>(jsonObj[0].Child("pulls"));
            var discussions = JsonSerializer.DeserializeFromString<List<JsonObject>>(jsonPulls[0].Child("discussion"))
                .ConvertAll(x => new Discussion
                {
                    Type = x.Get("type"),
                    GravatarId = x.Get("gravatar_id"),
                    CreatedAt = x.Get("created_at"),
                    Body = x.Get("body"),
                    UpdatedAt = x.Get("updated_at"),

                    Id = x.JsonTo<int?>("id"),
                    Sha = x.Get("sha"),
                    Author = x.Get("author"),
                    Subject = x.Get("subject"),
                    Email = x.Get("email"),
                });

            //Console.WriteLine(discussions.Dump()); //See what's been parsed
            Assert.IsNotNullOrEmpty(discussions.Dump());
            Assert.That(discussions.ConvertAll(x => x.Type), Is.EquivalentTo(new[] { "IssueComment", "Commit" }));
        }

        /// <summary>A git hub response.</summary>
        public class GitHubResponse
        {
            /// <summary>Gets or sets the pulls.</summary>
            /// <value>The pulls.</value>
            public List<Pull> pulls { get; set; }
        }

        /// <summary>A pull.</summary>
        public class Pull
        {
            /// <summary>Gets or sets the discussion.</summary>
            /// <value>The discussion.</value>
            public List<discussion> discussion { get; set; }

            /// <summary>Gets or sets the title.</summary>
            /// <value>The title.</value>
            public string title { get; set; }

            /// <summary>Gets or sets the body.</summary>
            /// <value>The body.</value>
            public string body { get; set; }

            /// <summary>Gets or sets the position.</summary>
            /// <value>The position.</value>
            public double position { get; set; }

            /// <summary>Gets or sets the number of. </summary>
            /// <value>The number.</value>
            public int number { get; set; }

            /// <summary>Gets or sets the votes.</summary>
            /// <value>The votes.</value>
            public int votes { get; set; }

            /// <summary>Gets or sets the comments.</summary>
            /// <value>The comments.</value>
            public int comments { get; set; }

            /// <summary>Gets or sets URL of the difference.</summary>
            /// <value>The difference URL.</value>
            public string diff_url { get; set; }

            /// <summary>Gets or sets URL of the patch.</summary>
            /// <value>The patch URL.</value>
            public string patch_url { get; set; }

            /// <summary>Gets or sets URL of the HTML.</summary>
            /// <value>The HTML URL.</value>
            public string html_url { get; set; }

            /// <summary>Gets or sets the issue created date.</summary>
            /// <value>The issue created date.</value>
            public DateTime issue_created_date { get; set; }

            /// <summary>Gets or sets the Date/Time of the issue updated at.</summary>
            /// <value>The issue updated at.</value>
            public DateTime issue_updated_at { get; set; }

            /// <summary>Gets or sets the Date/Time of the created at.</summary>
            /// <value>The created at.</value>
            public DateTime created_at { get; set; }

            /// <summary>Gets or sets the Date/Time of the updated at.</summary>
            /// <value>The updated at.</value>
            public DateTime updated_at { get; set; }
        }

        /// <summary>A discussion.</summary>
        public class discussion
        {
            /// <summary>Gets or sets the type.</summary>
            /// <value>The type.</value>
            public string type { get; set; }

            /// <summary>Gets or sets the identifier of the gravatar.</summary>
            /// <value>The identifier of the gravatar.</value>
            public string gravatar_id { get; set; }

            /// <summary>Gets or sets the created at.</summary>
            /// <value>The created at.</value>
            public string created_at { get; set; }

            /// <summary>Gets or sets the body.</summary>
            /// <value>The body.</value>
            public string body { get; set; }

            /// <summary>Gets or sets the updated at.</summary>
            /// <value>The updated at.</value>
            public string updated_at { get; set; }

            /// <summary>Gets or sets the identifier.</summary>
            /// <value>The identifier.</value>
            public int? id { get; set; }

            /// <summary>Gets or sets the sha.</summary>
            /// <value>The sha.</value>
            public string sha { get; set; }

            /// <summary>Gets or sets the author.</summary>
            /// <value>The author.</value>
            public string author { get; set; }

            /// <summary>Gets or sets the subject.</summary>
            /// <value>The subject.</value>
            public string subject { get; set; }

            /// <summary>Gets or sets the email.</summary>
            /// <value>The email.</value>
            public string email { get; set; }
        }

        /// <summary>Can parse discussion using custom client dt operating system.</summary>
        [Test]
        public void Can_Parse_Discussion_using_custom_client_DTOs()
        {
            var gitHubResponse = JsonSerializer.DeserializeFromString<GitHubResponse>(JsonGitResponse);

            //Console.WriteLine(gitHubResponse.Dump()); //See what's been parsed
            Assert.IsNotNullOrEmpty(gitHubResponse.Dump());
            Assert.That(gitHubResponse.pulls.SelectMany(p => p.discussion).ConvertAll(x => x.type),
                Is.EquivalentTo(new[] { "IssueComment", "Commit" }));
        }

    }
}