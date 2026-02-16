namespace UCourses_Back_End.Core.Settings
{
    public class FileUploadSettings
    {
        public bool EnableVirusScanning { get; set; } = true;
        public ClamAVSettings ClamAV { get; set; } = new();
    }

    public class ClamAVSettings
    {
        public string Host { get; set; } = "localhost";
        public int Port { get; set; } = 3310;
    }
}
