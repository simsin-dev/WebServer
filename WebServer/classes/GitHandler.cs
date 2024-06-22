using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServer.classes
{
    public static class GitHandler
    {
        public static void RunGitPull()
        {
            try
            {
                using (var repo = new Repository(Config.GetGitConfigValue("git-repo-dir")))
                {
                    PullOptions options = new PullOptions();
                    options.FetchOptions = new FetchOptions();
                    options.FetchOptions.CredentialsProvider = (url, usernameFromUrl, types) =>
                        new UsernamePasswordCredentials
                        {
                            Username = Config.GetGitConfigValue("git-username"),
                            Password = Config.GetGitConfigValue("git-passwd")
                        };

                    Commands.Pull(repo, new Signature(Config.GetGitConfigValue("git-username"), Config.GetGitConfigValue("git-mail"), DateTimeOffset.Now), options);
                    Console.WriteLine("Pull successful.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
    }
}
