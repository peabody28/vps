using System.Diagnostics;
using vps.Interfaces;
using vps.Models;

namespace vps.Operations
{
    public class DockerOperation : IDockerOperation
    {
        public ILogger<DockerOperation> Logger { get; set; }

        public ITcpOperation TcpOperation { get; set; }

        public DockerOperation(ILogger<DockerOperation> logger, ITcpOperation tcpOperation)
        {
            Logger = logger;
            TcpOperation = tcpOperation;
        }

        public DockerContainerModel CreateContainer(string username, string password)
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
                return null;

            var host = "0.0.0.0";
            var sshPort = TcpOperation.FreePort(host);
            if (sshPort.Equals(-1))
                return null;

            Logger.LogInformation($"find free port: {sshPort}");

            string command = string.Format(@"
                useradd -ms /bin/sh {0} &&
                usermod -aG /bin/bash {0} &&
                echo -e '{1}\n{1}' | passwd {0} &&
                cd /home/{0} &&
                mkdir .ssh && chmod 700 .ssh &&
                touch .ssh/authorized_keys && chmod 600 .ssh/authorized_keys &&
                chown -R {0}:{0} /home/{0} &&
                service ssh start &&
                exit", username, password);

            using (var sw = process.StandardInput)
            {
                if (sw.BaseStream.CanWrite)
                {
                    sw.WriteLine($"docker run --name {username} -p {sshPort}:22 -d test tail -f /dev/null");
                    sw.WriteLine($"docker exec -d {username} /bin/bash -c \"{command}\"");
                }
            }

            process.WaitForExit();
            var output = process.StandardOutput.ReadToEnd();
            var error = process.StandardError.ReadToEnd();

            Logger.LogInformation($"try to create container. username {username}, passwd {password}, output: {output}, error {error}");

            return new DockerContainerModel
            {
                SshPort = sshPort
            };
        }
    }
}
