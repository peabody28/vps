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

            string command = string.Format(@"
                useradd -ms /bin/sh {0} &&
                usermod -aG /bin/bash {0} &&
                echo -e '{1}\n{1}' | passwd {0} &&
                cd /home/{0} &&
                mkdir .ssh && chmod 700 .ssh &&
                touch .ssh/authorized_keys && chmod 600 .ssh/authorized_keys &&
                chown -R {0}:{0} /home/{0} &&
                service ssh start &&
                exit", model.Username, model.Password);

            using (var sw = process.StandardInput)
            {
                if (sw.BaseStream.CanWrite)
                {
                    sw.WriteLine($"docker run --name {model.Username} -p 2212:22 -d test tail -f /dev/null");
                    sw.WriteLine($"docker exec -d {model.Username} /bin/bash -c \"{command}\"");
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
