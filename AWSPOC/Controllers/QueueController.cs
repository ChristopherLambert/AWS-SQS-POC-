using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class QueueController : ControllerBase
{
    private readonly IAmazonSQS _sqs;
    private readonly IConfiguration _config;

    public QueueController(IAmazonSQS sqs, IConfiguration config)
    {
        _sqs = sqs;
        _config = config;
    }

    [HttpPost("publish")]
    public async Task<IActionResult> Publish([FromBody] object body)
    {
        var queueUrl = _config["AWS:QueueUrl"];

        try
        {
            var response = await _sqs.SendMessageAsync(new SendMessageRequest
            {
                QueueUrl = queueUrl,
                MessageBody = body.ToString()
            });
            return Ok(new { response.MessageId });
        }
        catch (Exception ex)
        {
            return BadRequest("Error: " + ex.Message);
        }
    }
}
