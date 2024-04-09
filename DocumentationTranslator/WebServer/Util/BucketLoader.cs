using Amazon.S3;
using Amazon.S3.Model;

namespace DocTranslatorServer.Data;

public class BucketLoader
{
  public static async Task<bool> PostDocumentToS3Async(string bucketName, string key, string content)
  {
    try
    {
      // Create an instance of the Amazon S3 client
      using (var s3Client = new AmazonS3Client())
      {
        // Create a request to upload the object to the S3 bucket
        var request = new PutObjectRequest
        {
          BucketName = bucketName,
          Key = key,
          ContentBody = content
        };

        // Execute the request and get the response
        PutObjectResponse response = await s3Client.PutObjectAsync(request);

        // Check if the object was successfully uploaded
        if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
        {
          return true;
        }
        else
        {
          return false;
        }
      }
    }
    catch (AmazonS3Exception ex)
    {
      // Handle any Amazon S3 specific exceptions
      Console.WriteLine($"An Amazon S3 exception occurred: {ex.Message}");
      return false;
    }
    catch (Exception ex)
    {
      // Handle any other exceptions
      Console.WriteLine($"An error occurred while uploading the object to S3: {ex.Message}");
      return false;

    }
  }

  public static async Task<string> GetDocumentFromS3Async(string bucketName, string key)
  {
    try
    {
      // Create an instance of the Amazon S3 client
      using (var s3Client = new AmazonS3Client())
      {
        // Create a request to get the object from the S3 bucket
        var request = new GetObjectRequest
        {
          BucketName = bucketName,
          Key = key
        };

        // Execute the request and get the response
        using (var response = await s3Client.GetObjectAsync(request))
        using (var responseStream = response.ResponseStream)
        using (var reader = new StreamReader(responseStream))
        {
          // Read the contents of the object from the response stream
          string content = await reader.ReadToEndAsync();
          return content;
        }
      }
    }
    catch (AmazonS3Exception ex)
    {
      // Handle any Amazon S3 specific exceptions
      Console.WriteLine($"An Amazon S3 exception occurred: {ex.Message}");
      return null;
    }
    catch (Exception ex)
    {
      // Handle any other exceptions
      Console.WriteLine($"An error occurred while retrieving the object from S3: {ex.Message}");
      return null;
    }
  }
}
