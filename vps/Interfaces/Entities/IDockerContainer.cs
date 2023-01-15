namespace vps.Interfaces.Entities
{
    public interface IDockerContainer
    {
        string Name { get; set; }
        int SshPort { get; set; }
        int? HttpPort { get; set; }

        string UserName { get; set; }

        string Password { get; set; }
    }
}
