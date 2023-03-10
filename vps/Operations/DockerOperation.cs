using vps.Constants;
using vps.Helpers;
using vps.Interfaces.Entities;
using vps.Interfaces.Operations;

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

        public IServiceProvider ServiceProvider { get; set; }

        public DockerOperation(ILogger<DockerOperation> logger, INetworkOperation networkOperation,
            IProcessOperation processOperation, IConfiguration configuration, IServiceProvider serviceProvider)
        {
            Logger = logger;
            NetworkOperation = networkOperation;
            ProcessOperation = processOperation;
            Configuration = configuration;
            ServiceProvider = serviceProvider;
        }
        

        public IDockerContainer CreateContainer(string username, string password, out bool isSuccess)
        {
            var dockerContainer = ServiceProvider.GetRequiredService<IDockerContainer>();
            dockerContainer.Name = username;
            dockerContainer.UserName = username;
            dockerContainer.Password = password;
            isSuccess = false;

            var isContainerStarted = TryStartContainer(dockerContainer);
            if (!isContainerStarted) return dockerContainer;

            var isUserCreated = TryCreateUser(dockerContainer);
            if (!isUserCreated) return dockerContainer;

            var isServicesStarted = TryStartServices(dockerContainer);
            if (!isServicesStarted) return dockerContainer;

            var isHttpForwardingEnabled = Configuration.GetValue<bool>("Server:HttpForwardingEnabled");
            if (isHttpForwardingEnabled && dockerContainer.HttpPort.HasValue)
            {
                // open server port
            }

            // save container info to DB

            isSuccess = true;
            return dockerContainer;
        }

        #region [ Private Methods ]

        private bool TryStartContainer(IDockerContainer dockerContainer)
        {
            Logger.LogInformation("TRY GET SSH PORT");

            var sshPort = NetworkOperation.FreeLocalPort();

            Logger.LogInformation($"SSH port is: {sshPort}");

            if (sshPort.Equals(NetworkConstants.UnsupportedPort)) return false;

            dockerContainer.SshPort = sshPort;

            var containerLocalSshPort = Configuration.GetValue<int>("Container:SshDefaultPort");
            var exposedPorts = new Dictionary<int, int> { { sshPort, containerLocalSshPort } };

            var isHttpForwardingEnabled = Configuration.GetValue<bool>("Server:HttpForwardingEnabled");
            if (isHttpForwardingEnabled)
            {
                var excludedPorts = new List<int> { sshPort };
                var httpPort = NetworkOperation.FreeLocalPort(excludedPorts);

                dockerContainer.HttpPort = httpPort;

                var containerLocalHttpPort = Configuration.GetValue<int>("Container:HttpDefaultPort");
                exposedPorts.Add(httpPort, containerLocalHttpPort);
            }

            var imageName = Configuration.GetSection("Docker:ImageName").Value;
            var isPrivelegiesAllowed = Configuration.GetValue<bool>("Docker:PrivelegiesAllowed");

            var dockerRunCommand = DockerHelper.BuildRunCommand(isPrivelegiesAllowed, dockerContainer.Name, exposedPorts, imageName);

            var isContainerStarted = ProcessOperation.ExecuteCommand(dockerRunCommand, out var exitCode, out var output, out var error);

            Logger.LogInformation($"try to create container {dockerContainer.Name}. Command: {dockerRunCommand}, ExitCode: {exitCode}, Output: {output}, Error {error}");

            return isContainerStarted;
        }

        private bool TryCreateUser(IDockerContainer dockerContainer)
        {
            var innerContainerCommand = DockerHelper.BuildUserCreateCommand(dockerContainer.UserName, dockerContainer.Password);

            var dockerExecCommand = DockerHelper.BuildExecCommand(dockerContainer.Name, innerContainerCommand);

            var isUserCreated = ProcessOperation.ExecuteCommand(dockerExecCommand, out var exitCode, out var output, out var error);

            Logger.LogInformation($"try to create user in container '{dockerContainer.Name}'. ExitCode: {exitCode}, Output: {output}, Error: {error}");

            return isUserCreated;
        }

        private bool TryStartServices(IDockerContainer dockerContainer)
        {
            var dockerExecCommand = DockerHelper.BuildExecCommand(dockerContainer.Name, "service ssh start");

            var isSshDaemonStarted = ProcessOperation.ExecuteCommand(dockerExecCommand, out var exitCode, out var output, out var error);

            Logger.LogInformation($"try to start SSH daemon. ExitCode: {exitCode}, Output: {output}, Error: {error}");

            return isSshDaemonStarted;
        }

        #endregion
    }
}
