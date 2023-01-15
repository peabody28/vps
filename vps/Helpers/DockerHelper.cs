using vps.Constants;

namespace vps.Helpers
{
    public static class DockerHelper
    {
        public static string BuildRunCommand(bool isPrivelegiesAllowed, string containerName, IDictionary<int, int> ports, string imageName)
        {
            var exposePortsString = string.Join(" ", ports.Select(item => string.Format("-p {0}:{1}", item.Key, item.Value)));

            return string.Format("docker run {0} --name {1} {2} -d {3}",
               isPrivelegiesAllowed ? "--privileged" : string.Empty,
               containerName, exposePortsString, imageName);
        }

        public static string BuildExecCommand(string containerName, string innerCommand)
        {
            return string.Format("docker exec -d {0} /bin/bash -c \"{1}\"", containerName, innerCommand);
        }

        public static string BuildUserCreateCommand(string userName, string password)
        {
            var commands = new List<string>();
            commands.Add(string.Format("useradd -ms /bin/bash {0}", userName));
            commands.Add(string.Format("usermod -aG sudo {0}", userName));
            commands.Add(string.Format("echo -e '{1}\n{1}' | passwd {0}", userName, password));
            commands.Add(string.Format("mkdir /home/{0}/.ssh && chmod 700 /home/{0}/.ssh", userName));
            commands.Add(string.Format("touch /home/{0}/.ssh/authorized_keys && chmod 600 /home/{0}/.ssh/authorized_keys", userName));
            commands.Add(string.Format("chown -R {0}:{0} /home/{0}", userName));

            return string.Join(CommonConstants.BashCommandsSeparator, commands);
        }
    }
}
