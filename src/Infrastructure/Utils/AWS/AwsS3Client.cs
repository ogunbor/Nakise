using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Domain.ConfigurationModels;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Utils.AWS;

public class AwsS3Client : IAwsS3Client
{
    private readonly IAmazonS3 _amazonS3;
    private readonly IConfiguration _configuration;
    private readonly AwsConfigurations _awsConfigurations;
    
    public AwsS3Client(IAmazonS3 amazonS3, IConfiguration configuration)
    {
        _amazonS3 = amazonS3 ?? throw new ArgumentNullException(nameof(amazonS3));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _awsConfigurations = new AwsConfigurations();
        _configuration.Bind(_awsConfigurations.Section, _awsConfigurations);
    }

    private AmazonS3Client AuthenticateAWS()
    {
        var credentials = new BasicAWSCredentials(_awsConfigurations.AccessKey, _awsConfigurations.SecretKey);
        var config = new AmazonS3Config
        {
            RegionEndpoint = Amazon.RegionEndpoint.EUWest2
        };
        using var client = new AmazonS3Client(credentials, config);
        return client;
    }

    public async Task<string> UploadFileAsync(IFormFile formFile)
    {
        var now = DateTime.Now.ToFileTimeUtc().ToString();
        var location = $"uploads/{now}-{formFile.FileName}";
        var awsBucketName = _configuration.GetValue<string>("AWS:BucketName");
        await using var stream = formFile.OpenReadStream();
        var putRequest = new PutObjectRequest
        {
            Key = location,
            BucketName = awsBucketName,
            InputStream = stream,
            AutoCloseStream = true,
            ContentType = formFile.ContentType
        };
        var s3 = new AmazonS3Client(_awsConfigurations.AccessKey, _awsConfigurations.SecretKey, 
            Amazon.RegionEndpoint.EUWest2);
        var res = await s3.PutObjectAsync(putRequest);
        return GetUploadedUrl(location);
    }

    private string GetUploadedUrl(string location)
    {
        var awsBucketName = _awsConfigurations.BucketName;
        var awsRegion = _awsConfigurations.Region;
        var result = $"https://{awsBucketName}.s3.{awsRegion}.amazonaws.com/{location}";
        return result;
    }

    public async Task<bool> RemoveObject(String fileName)
    {
        var s3 = new AmazonS3Client(_awsConfigurations.AccessKey, _awsConfigurations.SecretKey,
            Amazon.RegionEndpoint.EUWest2);

        var request = new DeleteObjectRequest
        {
            BucketName = _awsConfigurations.BucketName,
            Key = fileName
        };

        var response = await s3.DeleteObjectAsync(request);

        return response.HttpStatusCode == System.Net.HttpStatusCode.NoContent;
    }
}