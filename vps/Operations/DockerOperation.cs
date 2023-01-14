using vps.Constants;
using vps.Helpers;
using vps.Interfaces;
using vps.Models;

namespace vps.Operations
{
    public class DockerOperation : IDockerOperation
    {
        #region [ Dependency -> Operations ]

        public INetworkOperation NetworkOperation { get; set; }

        public IProcessOperation ProcessOperation { get; set; }

        #endregion

        public ILogger<DockerOperation> Logger { get; set; }

        public IConfiguration Configuration { get; set; }

        public DockerOperation(ILogger<DockerOperation> logger, INetworkOperation networkOperation, IProcessOperation processOperation, IConfiguration configuration)
        {
            Logger = logger;
            NetworkOperation = networkOperation;
            ProcessOperation = processOperation;
            Configuration = configuration;
        }

        public bool TryCreateContainer(string username, string password, out DockerContainerModel model)
        {
            model = null;
            var containerName = username;

            var sshPort = NetworkOperation.FreeLocalPort();

            Logger.LogInformation($"Free port is: {sshPort}");

            if (sshPort.Equals(NetworkConstants.UnsupportedPort)) return false;

            var imageName = Configuration.GetSection("Docker:ImageName").Value;

            var isPrivelegiesAllowed = Configuration.GetValue<bool>("Docker:PrivelegiesAllowed");

            var exposedPorts = new Dictionary<int, int>{ { sshPort, 22 } };

            var dockerRunCommand = DockerHelper.BuildRunCommand(isPrivelegiesAllowed, containerName, exposedPorts, imageName);

            var isContainerStarted = ProcessOperation.ExecuteCommand(dockerRunCommand, out var exitCode, out var output, out var error);

            Logger.LogInformation($"try to create container {containerName}. Command: {dockerRunCommand}, ExitCode: {exitCode}, Output: {output}, Error {error}");

            if (!isContainerStarted) return false;

            var innerContainerCommand = DockerHelper.BuildUserCreateCommand(username, password);

            var dockerExecCommand = DockerHelper.BuildExecCommand(containerName, innerContainerCommand);

            var isUserCreated = ProcessOperation.ExecuteCommand(dockerExecCommand, out exitCode, out output, out error);

            Logger.LogInformation($"try to create user in container '{containerName}'. ExitCode: {exitCode}, Output: {output}, Error: {error}");

            if (!isUserCreated) return false;

            dockerExecCommand = DockerHelper.BuildExecCommand(containerName, "service ssh start");

            var isSshDaemonStarted = ProcessOperation.ExecuteCommand(dockerExecCommand, out exitCode, out output, out error);

            Logger.LogInformation($"try to start SSH daemon. ExitCode: {exitCode}, Output: {output}, Error: {error}");

            model = new DockerContainerModel { SshPort = sshPort };

            return isSshDaemonStarted;
        }
    }
}
