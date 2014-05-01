using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NServiceKit.Text.Tests.UseCases
{
    /// <summary>
    /// Stand-alone C# client for the Github v3 API Uses only NServiceKit.Text (+NUnit for tests)
    /// </summary>
    [TestFixture, Explicit]
    public class GithubV3ApiGatewayTests
    {
        /// <summary>Executes all the things operation.</summary>
        [Test]
        [Ignore]
        public void DO_ALL_THE_THINGS()
        {
            var client = new GithubV3ApiGateway();

            Console.WriteLine("\n-- GetUserRepos(mythz): \n" + client.GetUserRepos("mythz").Dump());
            Console.WriteLine("\n-- GetOrgRepos(NServiceKit): \n" + client.GetOrgRepos("NServiceKit").Dump());
            Console.WriteLine("\n-- GetUserRepo(NServiceKit,NServiceKit.Text): \n" + client.GetUserRepo("mythz", "jquip").Dump());
            Console.WriteLine("\n-- GetUserRepoContributors(NServiceKit,NServiceKit.Text): \n" + client.GetUserRepoContributors("NServiceKit", "NServiceKit.Text").Dump());
            Console.WriteLine("\n-- GetUserRepoWatchers(NServiceKit,NServiceKit.Text): \n" + client.GetUserRepoWatchers("NServiceKit", "NServiceKit.Text").Dump());
            Console.WriteLine("\n-- GetReposUserIsWatching(mythz): \n" + client.GetReposUserIsWatching("mythz").Dump());
            Console.WriteLine("\n-- GetUserOrgs(mythz): \n" + client.GetUserOrgs("mythz").Dump());
            Console.WriteLine("\n-- GetUserFollowers(mythz): \n" + client.GetUserFollowers("mythz").Dump());
            Console.WriteLine("\n-- GetOrgMembers(NServiceKit): \n" + client.GetOrgMembers("NServiceKit").Dump());
            Console.WriteLine("\n-- GetAllUserAndOrgsReposFor(mythz): \n" + client.GetAllUserAndOrgsReposFor("mythz").Dump());
        }
    }

    /// <summary>A github v 3 API gateway.</summary>
    public class GithubV3ApiGateway
    {
        /// <summary>URL of the github API base.</summary>
        public const string GithubApiBaseUrl = "https://api.github.com/";

        /// <summary>Gets a JSON.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="route">    The route.</param>
        /// <param name="routeArgs">A variable-length parameters list containing route arguments.</param>
        /// <returns>The JSON.</returns>
        public T GetJson<T>(string route, params object[] routeArgs)
        {
            return GithubApiBaseUrl.AppendPath(route.Fmt(routeArgs))
                .GetJsonFromUrl()
                .FromJson<T>();
        }

        /// <summary>Gets user repos.</summary>
        /// <param name="githubUsername">The github username.</param>
        /// <returns>The user repos.</returns>
        public List<GithubRepo> GetUserRepos(string githubUsername)
        {
            return GetJson<List<GithubRepo>>("users/{0}/repos", githubUsername);
        }

        /// <summary>Gets organisation repos.</summary>
        /// <param name="githubOrgName">Name of the github organisation.</param>
        /// <returns>The organisation repos.</returns>
        public List<GithubRepo> GetOrgRepos(string githubOrgName)
        {
            return GetJson<List<GithubRepo>>("orgs/{0}/repos", githubOrgName);
        }

        /// <summary>Gets user repo.</summary>
        /// <param name="githubUsername">The github username.</param>
        /// <param name="projectName">   Name of the project.</param>
        /// <returns>The user repo.</returns>
        public GithubRepo GetUserRepo(string githubUsername, string projectName)
        {
            return GetJson<GithubRepo>("repos/{0}/{1}", githubUsername, projectName);
        }

        /// <summary>Gets user repo contributors.</summary>
        /// <param name="githubUsername">The github username.</param>
        /// <param name="projectName">   Name of the project.</param>
        /// <returns>The user repo contributors.</returns>
        public List<GithubUser> GetUserRepoContributors(string githubUsername, string projectName)
        {
            return GetJson<List<GithubUser>>("repos/{0}/{1}/contributors", githubUsername, projectName);
        }

        /// <summary>Gets user repo watchers.</summary>
        /// <param name="githubUsername">The github username.</param>
        /// <param name="projectName">   Name of the project.</param>
        /// <returns>The user repo watchers.</returns>
        public List<GithubUser> GetUserRepoWatchers(string githubUsername, string projectName)
        {
            return GetJson<List<GithubUser>>("repos/{0}/{1}/watchers", githubUsername, projectName);
        }

        /// <summary>Gets repos user is watching.</summary>
        /// <param name="githubUsername">The github username.</param>
        /// <returns>The repos user is watching.</returns>
        public List<GithubRepo> GetReposUserIsWatching(string githubUsername)
        {
            return GetJson<List<GithubRepo>>("users/{0}/watched", githubUsername);
        }

        /// <summary>Gets user orgs.</summary>
        /// <param name="githubUsername">The github username.</param>
        /// <returns>The user orgs.</returns>
        public List<GithubOrg> GetUserOrgs(string githubUsername)
        {
            return GetJson<List<GithubOrg>>("users/{0}/orgs", githubUsername);
        }

        /// <summary>Gets user followers.</summary>
        /// <param name="githubUsername">The github username.</param>
        /// <returns>The user followers.</returns>
        public List<GithubUser> GetUserFollowers(string githubUsername)
        {
            return GetJson<List<GithubUser>>("users/{0}/followers", githubUsername);
        }

        /// <summary>Gets organisation members.</summary>
        /// <param name="githubOrgName">Name of the github organisation.</param>
        /// <returns>The organisation members.</returns>
        public List<GithubUser> GetOrgMembers(string githubOrgName)
        {
            return GetJson<List<GithubUser>>("orgs/{0}/members", githubOrgName);
        }

        /// <summary>Gets all user and orgs repos for.</summary>
        /// <param name="githubUsername">The github username.</param>
        /// <returns>all user and orgs repos for.</returns>
        public List<GithubRepo> GetAllUserAndOrgsReposFor(string githubUsername)
        {
            var map = new Dictionary<int, GithubRepo>();
            GetUserRepos(githubUsername).ForEach(x => map[x.Id] = x);
            GetUserOrgs(githubUsername).ForEach(org =>
                GetOrgRepos(org.Login)
                    .ForEach(repo => map[repo.Id] = repo));

            return map.Values.ToList();
        }
    }

    /// <summary>A github repo.</summary>
    public class GithubRepo
    {
        /// <summary>Gets or sets the identifier.</summary>
        /// <value>The identifier.</value>
        public int Id { get; set; }

        /// <summary>Gets or sets the open issues.</summary>
        /// <value>The open issues.</value>
        public string Open_Issues { get; set; }

        /// <summary>Gets or sets the watchers.</summary>
        /// <value>The watchers.</value>
        public int Watchers { get; set; }

        /// <summary>Gets or sets the Date/Time of the pushed at.</summary>
        /// <value>The pushed at.</value>
        public DateTime? Pushed_At { get; set; }

        /// <summary>Gets or sets the homepage.</summary>
        /// <value>The homepage.</value>
        public string Homepage { get; set; }

        /// <summary>Gets or sets URL of the svn.</summary>
        /// <value>The svn URL.</value>
        public string Svn_Url { get; set; }

        /// <summary>Gets or sets the Date/Time of the updated at.</summary>
        /// <value>The updated at.</value>
        public DateTime? Updated_At { get; set; }

        /// <summary>Gets or sets URL of the mirror.</summary>
        /// <value>The mirror URL.</value>
        public string Mirror_Url { get; set; }

        /// <summary>Gets or sets a value indicating whether this object has downloads.</summary>
        /// <value>true if this object has downloads, false if not.</value>
        public bool Has_Downloads { get; set; }

        /// <summary>Gets or sets URL of the document.</summary>
        /// <value>The URL.</value>
        public string Url { get; set; }

        /// <summary>Gets or sets a value indicating whether this object has issues.</summary>
        /// <value>true if this object has issues, false if not.</value>
        public bool Has_issues { get; set; }

        /// <summary>Gets or sets the language.</summary>
        /// <value>The language.</value>
        public string Language { get; set; }

        /// <summary>Gets or sets a value indicating whether the fork.</summary>
        /// <value>true if fork, false if not.</value>
        public bool Fork { get; set; }

        /// <summary>Gets or sets URL of the SSH.</summary>
        /// <value>The SSH URL.</value>
        public string Ssh_Url { get; set; }

        /// <summary>Gets or sets URL of the HTML.</summary>
        /// <value>The HTML URL.</value>
        public string Html_Url { get; set; }

        /// <summary>Gets or sets the forks.</summary>
        /// <value>The forks.</value>
        public int Forks { get; set; }

        /// <summary>Gets or sets URL of the clone.</summary>
        /// <value>The clone URL.</value>
        public string Clone_Url { get; set; }

        /// <summary>Gets or sets the size.</summary>
        /// <value>The size.</value>
        public int Size { get; set; }

        /// <summary>Gets or sets URL of the git.</summary>
        /// <value>The git URL.</value>
        public string Git_Url { get; set; }

        /// <summary>Gets or sets a value indicating whether the private.</summary>
        /// <value>true if private, false if not.</value>
        public bool Private { get; set; }

        /// <summary>Gets or sets the Date/Time of the created at.</summary>
        /// <value>The created at.</value>
        public DateTime Created_at { get; set; }

        /// <summary>Gets or sets a value indicating whether this object has wiki.</summary>
        /// <value>true if this object has wiki, false if not.</value>
        public bool Has_Wiki { get; set; }

        /// <summary>Gets or sets the owner.</summary>
        /// <value>The owner.</value>
        public GithubUser Owner { get; set; }

        /// <summary>Gets or sets the name.</summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>Gets or sets the description.</summary>
        /// <value>The description.</value>
        public string Description { get; set; }
    }

    /// <summary>A github user.</summary>
    public class GithubUser
    {
        /// <summary>Gets or sets the identifier.</summary>
        /// <value>The identifier.</value>
        public int Id { get; set; }

        /// <summary>Gets or sets the login.</summary>
        /// <value>The login.</value>
        public string Login { get; set; }

        /// <summary>Gets or sets URL of the avatar.</summary>
        /// <value>The avatar URL.</value>
        public string Avatar_Url { get; set; }

        /// <summary>Gets or sets URL of the document.</summary>
        /// <value>The URL.</value>
        public string Url { get; set; }

        /// <summary>Gets or sets the followers.</summary>
        /// <value>The followers.</value>
        public int? Followers { get; set; }

        /// <summary>Gets or sets the following.</summary>
        /// <value>The following.</value>
        public int? Following { get; set; }

        /// <summary>Gets or sets the type.</summary>
        /// <value>The type.</value>
        public string Type { get; set; }

        /// <summary>Gets or sets the public gists.</summary>
        /// <value>The public gists.</value>
        public int? Public_Gists { get; set; }

        /// <summary>Gets or sets the location.</summary>
        /// <value>The location.</value>
        public string Location { get; set; }

        /// <summary>Gets or sets the company.</summary>
        /// <value>The company.</value>
        public string Company { get; set; }

        /// <summary>Gets or sets URL of the HTML.</summary>
        /// <value>The HTML URL.</value>
        public string Html_Url { get; set; }

        /// <summary>Gets or sets the public repos.</summary>
        /// <value>The public repos.</value>
        public int? Public_Repos { get; set; }

        /// <summary>Gets or sets the Date/Time of the created at.</summary>
        /// <value>The created at.</value>
        public DateTime? Created_At { get; set; }

        /// <summary>Gets or sets the blog.</summary>
        /// <value>The blog.</value>
        public string Blog { get; set; }

        /// <summary>Gets or sets the email.</summary>
        /// <value>The email.</value>
        public string Email { get; set; }

        /// <summary>Gets or sets the name.</summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>Gets or sets the hireable.</summary>
        /// <value>The hireable.</value>
        public bool? Hireable { get; set; }

        /// <summary>Gets or sets the identifier of the gravatar.</summary>
        /// <value>The identifier of the gravatar.</value>
        public string Gravatar_Id { get; set; }

        /// <summary>Gets or sets the bio.</summary>
        /// <value>The bio.</value>
        public string Bio { get; set; }
    }

    /// <summary>A github organisation.</summary>
    public class GithubOrg
    {
        /// <summary>Gets or sets the identifier.</summary>
        /// <value>The identifier.</value>
        public int Id { get; set; }

        /// <summary>Gets or sets URL of the avatar.</summary>
        /// <value>The avatar URL.</value>
        public string Avatar_Url { get; set; }

        /// <summary>Gets or sets URL of the document.</summary>
        /// <value>The URL.</value>
        public string Url { get; set; }

        /// <summary>Gets or sets the login.</summary>
        /// <value>The login.</value>
        public string Login { get; set; }
    }
}