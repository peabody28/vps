using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using vps.Models.Server;

namespace vps.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ServerController : ControllerBase
    {
        public ILogger<ServerController> Logger { get; set; }

        public ServerController(ILogger<ServerController> logger) 
        {
            Logger = logger;
        }

        [HttpPost]
        public bool Create(ServerCreateModel model)
        {
            using var process = Process.Start(new ProcessStartInfo
            {
                FileName = "/bin/sh",
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                WorkingDirectory = "/home/max/ubuntu_ssh"
            });

            if (process == null)
                return false;

            using (var sw = process.StandardInput)
            {
                if (sw.BaseStream.CanWrite)
                {
                    sw.WriteLine($"docker run --name {model.Username} -p 2222:22 -d test tail -f /dev/null");
                    sw.WriteLine($"useradd -ms /bin/sh {model.Username}");
                    sw.WriteLine($"usermod -aG /bin/bash {model.Username}");
                    sw.WriteLine(string.Format("echo -e \"{1}\\n{1}\" | passwd {0}", model.Username, model.Password));
                    sw.WriteLine($"cd /home/{model.Username}");
                    sw.WriteLine("mkdir .ssh && chmod 700 .ssh");
                    sw.WriteLine("touch .ssh/authorized_keys && chmod 600 .ssh/authorized_keys");
                    sw.WriteLine(string.Format("chown -R {0}:{0} /home/{0}", model.Username));
                    sw.WriteLine("service ssh start");
                }
            }

            process.WaitForExit();
            var output = process.StandardOutput.ReadToEnd();
            var error = process.StandardError.ReadToEnd();

            Logger.LogInformation($"try to create server. username {model.Username}, passwd {model.Password}, output: {output}, error {error}");

            return process.ExitCode.Equals(0);
        }
    }
}
