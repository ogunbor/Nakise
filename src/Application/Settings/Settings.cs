namespace Application.Settings
{
    public class FileSettings
    {
        public string[] DefaultFileExtensionsAllowed { get; set; }
        public int DefaultFileSizeLimitInMb { get; set; }
        public string DefaultStorageProvider { get; set; }
    }

	public class AwsSettings
	{
		public string AccessKey { get; set; }
		public string SecretKey { get; set; }
		public AwsStorage Storage { get; set; }
	}
	public class AwsStorage
	{
		public string Name { get; set; }
		public string BucketName { get; set; }
	}

	public class AzureSettings
	{
		public AzureStorage Storage { get; set; }
	}

	public class AzureStorage
	{
		public string Name { get; set; }
		public string ContainerName { get; set; }
		public string ConnectionString { get; set; }
	}
}
