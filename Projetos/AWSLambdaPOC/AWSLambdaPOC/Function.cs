using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace AWSLambdaPOC;

public class Function
{
    private static readonly AmazonDynamoDBClient _ddb = new AmazonDynamoDBClient();
    private static readonly string _table = Environment.GetEnvironmentVariable("TABLE_NAME");

    public async Task Handler(SQSEvent evnt, ILambdaContext context)
    {
        var tasks = new List<Task>();
        foreach (var rec in evnt.Records)
        {
            var sentTs = rec.Attributes != null && rec.Attributes.TryGetValue("SentTimestamp", out var ts)
                ? DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(ts)).UtcDateTime.ToString("o")
                : DateTime.UtcNow.ToString("o");

            // Converte MessageAttributes (SQS) → AttributeValue (DDB)
            var attrMap = new Dictionary<string, AttributeValue>();
            foreach (var kv in rec.MessageAttributes)
            {
                var v = kv.Value;
                if (!string.IsNullOrEmpty(v.StringValue))
                    attrMap[kv.Key] = new AttributeValue { S = v.StringValue };
                else if (v.BinaryValue != null)
                    attrMap[kv.Key] = new AttributeValue { B = v.BinaryValue }; // já é bytes
                else if (v.StringListValues != null && v.StringListValues.Count > 0)
                    attrMap[kv.Key] = new AttributeValue { L = v.StringListValues.ConvertAll(s => new AttributeValue { S = s }) };
            }

            var item = new Dictionary<string, AttributeValue>
            {
                ["MessageId"] = new AttributeValue { S = rec.MessageId },
                ["Body"] = new AttributeValue { S = rec.Body ?? "" },
                ["Attributes"] = new AttributeValue { M = attrMap },
                ["SentTimestamp"] = new AttributeValue { S = sentTs },
                ["ApproxReceiveCount"] = new AttributeValue { N = (rec.Attributes?.TryGetValue("ApproximateReceiveCount", out var arc) ?? false) ? arc : "1" },
                ["ReceivedAt"] = new AttributeValue { S = DateTime.UtcNow.ToString("o") }
            };

            tasks.Add(_ddb.PutItemAsync(new PutItemRequest { TableName = _table, Item = item }));
        }

        await Task.WhenAll(tasks);
    }
}