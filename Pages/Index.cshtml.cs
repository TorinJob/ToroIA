using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using gitChat.Services;
using System.IO;
using System.Threading.Tasks;

public class IndexModel : PageModel
{
    private readonly OllamaService _ollamaService;

    public IndexModel(OllamaService ollamaService)
    {
        _ollamaService = ollamaService;
    }

    [BindProperty]
    public string UserMessage { get; set; }

    [BindProperty]
    public IFormFile UploadedFile { get; set; }

    public string Response { get; set; }

    public async Task<IActionResult> OnPostAsync(string action)
    {
        if (action == "chat" && !string.IsNullOrWhiteSpace(UserMessage))
        {
            Response = await _ollamaService.ChatAsync(UserMessage);
        }
        else if (action == "explain" && !string.IsNullOrWhiteSpace(UserMessage))
        {
            Response = await _ollamaService.ExplainTermAsync(UserMessage);
        }
        else if (action == "sentiment" && !string.IsNullOrWhiteSpace(UserMessage))
        {
            Response = await _ollamaService.SentimentAnalysisAsync(UserMessage);
        }
        else if (action == "summarize" && UploadedFile != null)
        {
            using var stream = UploadedFile.OpenReadStream();
            using var reader = new StreamReader(stream);
            var content = await reader.ReadToEndAsync();
            Response = await _ollamaService.SummarizeTextAsync(content);
        }

        return Page();
    }
}
