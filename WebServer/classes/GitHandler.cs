using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServer.classes
{
    public class GitHandler : IDisposable
    {
        Configuration config;

        public GitHandler(Configuration config)
        {
            this.config = config;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public void RunGitPull()
        {
            try
            {
                using (var repo = new Repository(config.GetValue("git-repo-dir")))
                {
                    PullOptions options = new PullOptions();
                    options.FetchOptions = new FetchOptions();
                    options.FetchOptions.CredentialsProvider = (url, usernameFromUrl, types) =>
                        new UsernamePasswordCredentials
                        {
                            Username = config.GetValue("git-username"),
                            Password = config.GetValue("git-passwd")
                        };

                    Commands.Pull(repo, new Signature(config.GetValue("git-username"), config.GetValue("git-mail"), DateTimeOffset.Now), options);
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
