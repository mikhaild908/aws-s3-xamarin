//AWSSDK 3.3.102.13

using System;
using UIKit;
using Amazon.S3;
using Amazon.S3.Model;
using System.Threading.Tasks;
using Amazon;

namespace aws_s3_xamarin
{
    public partial class ViewController : UIViewController
    {
        private const string BUCKET_NAME = "<bucket name>";
        private const string ACCESS_KEY_ID = "<access key id>";
        private const string SECRET_ACCESS_KEY = "<secret access key>";

        private static IAmazonS3 _s3client;
        private static readonly RegionEndpoint _bucketRegion = RegionEndpoint.USWest2;

        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.

            _s3client = new AmazonS3Client(ACCESS_KEY_ID, SECRET_ACCESS_KEY, _bucketRegion);

            txtView.Text = "Search Spring\n";
            txtView.Text += $"\nBucket: {BUCKET_NAME}\n\n";

            txtView.Text += ListingObjectsAsync().Result;
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        async Task<string> ListingObjectsAsync()
        {
            string result = string.Empty;

            try
            {
                ListObjectsV2Request request = new ListObjectsV2Request
                {
                    BucketName = BUCKET_NAME,
                    MaxKeys = 10
                };

                ListObjectsV2Response response;

                do
                {
                    response = await _s3client.ListObjectsV2Async(request).ConfigureAwait(false);

                    // Process the response.
                    foreach (S3Object entry in response.S3Objects)
                    {
                        //result += $"key = {entry.Key} size = {entry.Size} lastModified = {entry.LastModified}\n";
                        result += $"{entry.Key} Modified = {entry.LastModified}\n";
                    }

                    //result += $"Next Continuation Token: {response.NextContinuationToken}";
                    result += "\n";

                    request.ContinuationToken = response.NextContinuationToken;
                } while (response.IsTruncated);

                return result;
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                result = $"S3 error occurred. Exception: {amazonS3Exception.ToString()}";
            }
            catch (Exception e)
            {
                result = $"Exception: {e.ToString()}";
            }

            return result;
        }
    }
}
