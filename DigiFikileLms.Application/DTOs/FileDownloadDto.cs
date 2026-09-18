using DigiFikileLms.Application.DTOs;

namespace DigiFikileLms.Application.DTOs;

public class FileDownloadDto
{
    public byte[] FileContent { get; set; } = Array.Empty<byte>();
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
}