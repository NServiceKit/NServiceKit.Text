﻿using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NServiceKit.Text.Tests.UseCases
{
    /// <summary>
    /// Stand-alone C# client for the Github v3 API
    /// Uses only NServiceKit.Text (+NUnit for tests)
    /// </summary>
    [TestFixture, Explicit]
    public class GithubV3ApiGatewayTests
    {
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

    public class GithubV3ApiGateway
    {
        public const string GithubApiBaseUrl = "https://api.github.com/";

        public T GetJson<T>(string route, params object[] routeArgs)
        {
            return GithubApiBaseUrl.AppendPath(route.Fmt(routeArgs))
                .GetJsonFromUrl()
                .FromJson<T>();
        }

        public List<GithubRepo> GetUserRepos(string githubUsername)
        {
            return GetJson<List<GithubRepo>>("users/{0}/repos", githubUsername);
        }

        public List<GithubRepo> GetOrgRepos(string githubOrgName)
        {
            return GetJson<List<GithubRepo>>("orgs/{0}/repos", githubOrgName);
        }

        public GithubRepo GetUserRepo(string githubUsername, string projectName)
        {
            return GetJson<GithubRepo>("repos/{0}/{1}", githubUsername, projectName);
        }

        public List<GithubUser> GetUserRepoContributors(string githubUsername, string projectName)
        {
            return GetJson<List<GithubUser>>("repos/{0}/{1}/contributors", githubUsername, projectName);
        }

        public List<GithubUser> GetUserRepoWatchers(string githubUsername, string projectName)
        {
            return GetJson<List<GithubUser>>("repos/{0}/{1}/watchers", githubUsername, projectName);
        }

        public List<GithubRepo> GetReposUserIsWatching(string githubUsername)
        {
            return GetJson<List<GithubRepo>>("users/{0}/watched", githubUsername);
        }

        public List<GithubOrg> GetUserOrgs(string githubUsername)
        {
            return GetJson<List<GithubOrg>>("users/{0}/orgs", githubUsername);
        }

        public List<GithubUser> GetUserFollowers(string githubUsername)
        {
            return GetJson<List<GithubUser>>("users/{0}/followers", githubUsername);
        }

        public List<GithubUser> GetOrgMembers(string githubOrgName)
        {
            return GetJson<List<GithubUser>>("orgs/{0}/members", githubOrgName);
        }

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

    public class GithubRepo
    {
        public int Id { get; set; }
        public string Open_Issues { get; set; }
        public int Watchers { get; set; }
        public DateTime? Pushed_At { get; set; }
        public string Homepage { get; set; }
        public string Svn_Url { get; set; }
        public DateTime? Updated_At { get; set; }
        public string Mirror_Url { get; set; }
        public bool Has_Downloads { get; set; }
        public string Url { get; set; }
        public bool Has_issues { get; set; }
        public string Language { get; set; }
        public bool Fork { get; set; }
        public string Ssh_Url { get; set; }
        public string Html_Url { get; set; }
        public int Forks { get; set; }
        public string Clone_Url { get; set; }
        public int Size { get; set; }
        public string Git_Url { get; set; }
        public bool Private { get; set; }
        public DateTime Created_at { get; set; }
        public bool Has_Wiki { get; set; }
        public GithubUser Owner { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class GithubUser
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Avatar_Url { get; set; }
        public string Url { get; set; }
        public int? Followers { get; set; }
        public int? Following { get; set; }
        public string Type { get; set; }
        public int? Public_Gists { get; set; }
        public string Location { get; set; }
        public string Company { get; set; }
        public string Html_Url { get; set; }
        public int? Public_Repos { get; set; }
        public DateTime? Created_At { get; set; }
        public string Blog { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public bool? Hireable { get; set; }
        public string Gravatar_Id { get; set; }
        public string Bio { get; set; }
    }

    public class GithubOrg
    {
        public int Id { get; set; }
        public string Avatar_Url { get; set; }
        public string Url { get; set; }
        public string Login { get; set; }
    }
}