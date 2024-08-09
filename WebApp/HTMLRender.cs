using Scriban;

/// <summary>
/// Provides functionality to render HTML templates using the Scriban templating engine.
/// </summary>
/// <remarks>
/// This class reads HTML files from the specified static directory, parses them as Scriban templates, 
/// and renders them with optional data. The static directory is set to "./WebApp/wwwroot/" by default.
/// </remarks>
public static class HTMLRender
{
    static string staticDirectory = "./WebApp/wwwroot/";

    /// <summary>
    /// Renders an HTML page from a file using the Scriban templating engine.
    /// </summary>
    /// <param name="fileName">The name of the HTML file to render, relative to the static directory.</param>
    /// <param name="data">An optional object containing data to be injected into the template.</param>
    /// <returns>The rendered HTML content as a string.</returns>
    public static string Render(string fileName, object? data = null)
    {
        StreamReader sr = new StreamReader(staticDirectory + fileName);
        string page = sr.ReadToEnd();
        var script = Template.Parse(page);
        return script.Render(data);
    }
}
