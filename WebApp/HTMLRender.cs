using System.Text;
using Scriban;
public static class HTMLRender
{
    static string staticDirectory = "./WebApp/wwwroot/";
    public static string Render(string fileName, object? data = null) 
    {
        StreamReader sr = new StreamReader(staticDirectory + fileName);
        string page = sr.ReadToEnd();
        var script = Template.Parse(page);
        return script.Render(data);
    }

}

