namespace ITC.Core.Model
{
    public class AzureBlobStorageModel
    {
        public string? Uri { get; set; }
        public string? Name { get; set; }
        public string? ContentType { get; set; }
        public Stream? Content { get; set; }
    }

    public class BlobResponseDto
    {
        public string? Status { get; set; }
        public bool Error { get; set; }
        public AzureBlobStorageModel Blob { get; set; }

        public BlobResponseDto()
        {
            Blob = new AzureBlobStorageModel();
        }
    }
}
