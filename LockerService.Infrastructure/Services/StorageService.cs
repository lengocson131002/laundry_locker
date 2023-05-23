using System.Net;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using LockerService.Application.Common.Enums;
using LockerService.Application.Common.Exceptions;
using LockerService.Application.Common.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace LockerService.Infrastructure.Services;

public class StorageService : IStorageService
{
    private readonly string _bucketName;
    private readonly string _region;
    private readonly ILogger<StorageService> _logger;
    private readonly IAmazonS3 _s3Client;

    public StorageService(IConfiguration configuration, ILogger<StorageService> logger)
    {
        _logger = logger;
        _bucketName = configuration["AWS:S3:BucketName"] ?? throw new ArgumentException("AWS:S3:BucketName is required");
        _region = configuration["AWS:S3:Region"] ?? throw new ArgumentException("AWS:S3:Region is required");
        
        var accessKey = configuration["AWS:S3:AccessKey"] ??
                        throw new ArgumentException("AWS:S3:AccessKey is required");
        var secretKey = configuration["AWS:S3:SecretKey"] ??
                        throw new ArgumentException("AWS:S3:SecretKey is required");

        _s3Client = new AmazonS3Client(accessKey, secretKey, RegionEndpoint.GetBySystemName(_region));
    }

    public async Task<byte[]> DownloadFileAsync(string fileName)
    {
        var getObjectRequest = new GetObjectRequest
        {
            BucketName = _bucketName,
            Key = fileName
        };

        using var response = await _s3Client.GetObjectAsync(getObjectRequest);
        if (response.HttpStatusCode != HttpStatusCode.OK)
        {
            throw new ApiException(ResponseCode.ErrorFileNotFound);
        }
        
        using var ms = new MemoryStream();
        await response.ResponseStream.CopyToAsync(ms);
        return ms.ToArray();

    }

    public async Task<string> GetContentType(string fileName)
    {
        var getObjectRequest = new GetObjectRequest
        {
            BucketName = _bucketName,
            Key = fileName
        };

        using var response = await _s3Client.GetObjectAsync(getObjectRequest);
        if (response.HttpStatusCode != HttpStatusCode.OK)
        {
            throw new ApiException(ResponseCode.ErrorFileNotFound);
        }

        return response.Headers.ContentType;
    }

    public async Task<string> UploadFileAsync(IFormFile file)
    {
        using var ms = new MemoryStream();
        await file.CopyToAsync(ms);

        var fileName = $"{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}-{file.FileName}";
        var uploadFileRequest = new TransferUtilityUploadRequest
        {
            InputStream = ms,
            Key = fileName, // create unique file name
            BucketName = _bucketName,
            ContentType = file.ContentType
        };

        var fileTransferUtility = new TransferUtility(_s3Client);

        await fileTransferUtility.UploadAsync(uploadFileRequest);

        return fileName;
    }

    public async Task<bool> DeleteFileAsync(string fileName, string versionId = "")
    {
        var deleteFileRequest = new DeleteObjectRequest()
        {
            BucketName = _bucketName,
            Key = fileName,
            VersionId = !string.IsNullOrWhiteSpace(versionId) ? versionId : null
        };

        if (!string.IsNullOrEmpty(versionId))
        {
            deleteFileRequest.VersionId = versionId;
        }

        var response = await _s3Client.DeleteObjectAsync(deleteFileRequest);
        if (response.HttpStatusCode != HttpStatusCode.OK)
        {
            throw new ApiException(ResponseCode.ErrorDeleteFileFailed);
        }
        
        return true;
    }

    public async Task<bool> IsFileExists(string fileName, string versionId = "")
    {
        try
        {
            var getMetaObjectRequest = new GetObjectMetadataRequest()
            {
                BucketName = _bucketName,
                Key = fileName,
                VersionId = !string.IsNullOrWhiteSpace(versionId) ? versionId : null
            };

            var response = await _s3Client.GetObjectMetadataAsync(getMetaObjectRequest);
            return response.HttpStatusCode == HttpStatusCode.OK;
        }
        catch (Exception ex)
        {
            if (ex.InnerException != null && ex.InnerException is AmazonS3Exception awsEx)
            {
                if (string.Equals(awsEx.ErrorCode, "NoSuchBucket"))
                    return false;

                if (string.Equals(awsEx.ErrorCode, "NotFound"))
                    return false;
            }

            throw;
        }
    }

    public Task<string> GetPresignedUrlAsync(string fileName)
    {
        var urlRequest = new GetPreSignedUrlRequest()
        {
            BucketName = _bucketName,
            Key = fileName,
            Expires = DateTime.UtcNow.AddMinutes(1)
        };

        return Task.FromResult(_s3Client.GetPreSignedURL(urlRequest));
    }

    public string GetObjectUrl(string fileName)
    {
        return $"https://{_bucketName}.s3.{_region}.amazonaws.com/{fileName}";
    }
}