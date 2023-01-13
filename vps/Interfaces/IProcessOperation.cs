namespace vps.Interfaces
{
    public interface IProcessOperation
    {
        bool ExecuteCommand(string command, out int exitCode, out string output, out string error);
    }
}
