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
            return string.Format(@"
                useradd -ms /bin/bash {0} &&
                usermod -aG sudo {0} &&
                echo -e '{1}\n{1}' | passwd {0} &&
                cd /home/{0} &&
                mkdir .ssh && chmod 700 .ssh &&
                touch .ssh/authorized_keys && chmod 600 .ssh/authorized_keys &&
                chown -R {0}:{0} /home/{0}", userName, password);
        }
    }
}
