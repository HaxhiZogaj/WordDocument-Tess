using Microsoft.AspNetCore.Mvc;
using Syncfusion.DocIO;
using EJ2DocumentEditor = Syncfusion.EJ2.DocumentEditor;
using Newtonsoft.Json;
using WordDocumentEditor.Models;
namespace DocumentEditor.Controllers;

public class DocumenteditorController : Controller
{
    private IHostEnvironment hostEnvironment;
    private readonly DocTemplateDBContext _context; // Add your EF Core DbContext here

    public DocumenteditorController(IHostEnvironment environment, DocTemplateDBContext context)
    {
        this.hostEnvironment = environment;
        _context = context;

    }

    public string Import(IFormCollection data)
    {
        if (data.Files.Count == 0)
            return null;
        Stream stream = new MemoryStream();
        IFormFile file = data.Files[0];
        int index = file.FileName.LastIndexOf('.');
        string type = index > -1 && index < file.FileName.Length - 1 ?
            file.FileName.Substring(index) : ".docx";
        file.CopyTo(stream);
        stream.Position = 0;

        EJ2DocumentEditor.WordDocument document = EJ2DocumentEditor.WordDocument.Load(stream, GetFormatType(type.ToLower()));
        string json = Newtonsoft.Json.JsonConvert.SerializeObject(document);
        document.Dispose();
        return json;
    }

    public string ImportFile([FromBody] CustomParams param)
    {
        string path = this.hostEnvironment.ContentRootPath + "\\wwwroot\\SavedFiles\\" + param.fileName;
        try
        {
            Stream stream = System.IO.File.Open(path, FileMode.Open, FileAccess.ReadWrite);
            Syncfusion.EJ2.DocumentEditor.WordDocument document = Syncfusion.EJ2.DocumentEditor.WordDocument.Load(stream, GetFormatType(path));
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(document);
            document.Dispose();
            stream.Dispose();
            return json;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return "Failure";
        }
    }

    public string Save([FromBody] CustomParameter param)
    {
        string path = this.hostEnvironment.ContentRootPath + "\\wwwroot\\SavedFiles\\" + param.fileName;
        Byte[] byteArray = Convert.FromBase64String(param.documentData);
        Stream stream = new MemoryStream(byteArray);
        EJ2DocumentEditor.FormatType type = GetFormatType(path);
        try
        {
            FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);

            if (type != EJ2DocumentEditor.FormatType.Docx)
            {
                Syncfusion.DocIO.DLS.WordDocument document = new Syncfusion.DocIO.DLS.WordDocument(stream, Syncfusion.DocIO.FormatType.Docx);
                document.Save(fileStream, GetDocIOFomatType(type));
                document.Close();
            }
            else
            {
                stream.Position = 0;
                stream.CopyTo(fileStream);
            }
            stream.Dispose();
            fileStream.Dispose();
            return "Sucess";
        }
        catch
        {
            Console.WriteLine("err");
            return "Failure";
        }
    }

    public string ConvertMailMerge([FromBody] CustomParameter exportData)
    {
        Byte[] data = Convert.FromBase64String(exportData.documentData.Split(',')[1]);
        MemoryStream stream = new MemoryStream();
        stream.Write(data, 0, data.Length);
        stream.Position = 0;

        string SFDT = "";

        var descriptions = _context.DocTemplates
                                    .Select(d => d.Description)  
                                    .ToList();  
        using (Syncfusion.DocIO.DLS.WordDocument document = new Syncfusion.DocIO.DLS.WordDocument(stream, Syncfusion.DocIO.FormatType.Docx))
        {
            string[] fieldNames = descriptions.ToArray();  
            string[] fieldValues = new string[fieldNames.Length];

            for (int i = 0; i < fieldNames.Length; i++)
            {
                fieldValues[i] = "Sample Value for " + fieldNames[i];  
            }
            document.MailMerge.Execute(fieldNames, fieldValues);

            Syncfusion.EJ2.DocumentEditor.WordDocument wordDocument = Syncfusion.EJ2.DocumentEditor.WordDocument.Load(document);
            SFDT = JsonConvert.SerializeObject(wordDocument);
            wordDocument.Dispose();
        }

        return SFDT;
    }

    public string GetDataSource()
    {
        string path = this.hostEnvironment.ContentRootPath + "\\wwwroot\\SavedFiles\\";
        List<DocumentInfo> dataSource = new List<DocumentInfo>();
        if (Directory.Exists(path))
        {
            string[] docxFiles = Directory.GetFiles(path, "*.docx");


            int index = 0;
            foreach (string filePath in docxFiles)
            {
                ++index;
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                dataSource.Add(new DocumentInfo { FileIndex = index, FileName = fileName });
            }


        }
        return Newtonsoft.Json.JsonConvert.SerializeObject(dataSource);
    }

    [HttpPost]
    public JsonResult FileExists([FromBody] CustomParams param)
    {
        string path = this.hostEnvironment.ContentRootPath + "\\wwwroot\\SavedFiles\\" + param.fileName;
        bool exists = System.IO.File.Exists(path);
        return Json(new { exists = exists });
    }



  
    internal static EJ2DocumentEditor.FormatType GetFormatType(string fileName)
    {
        int index = fileName.LastIndexOf('.');
        string format = index > -1 && index < fileName.Length - 1 ? fileName.Substring(index + 1) : "";

        if (string.IsNullOrEmpty(format))
            throw new NotSupportedException("EJ2 Document editor does not support this file format.");
        switch (format.ToLower())
        {
            case "dotx":
            case "docx":
            case "docm":
            case "dotm":
                return EJ2DocumentEditor.FormatType.Docx;
            case "dot":
            case "doc":
                return EJ2DocumentEditor.FormatType.Doc;
            case "rtf":
                return EJ2DocumentEditor.FormatType.Rtf;
            case "txt":
                return EJ2DocumentEditor.FormatType.Txt;
            case "xml":
                return EJ2DocumentEditor.FormatType.WordML;
            default:
                throw new NotSupportedException($"EJ2 Document editor does not support this file format: {format}");
        }
    }

    internal static Syncfusion.DocIO.FormatType GetDocIOFomatType(EJ2DocumentEditor.FormatType type)
    {
        switch (type)
        {
            case EJ2DocumentEditor.FormatType.Docx:
                return FormatType.Docx;
            case EJ2DocumentEditor.FormatType.Doc:
                return FormatType.Doc;
            case EJ2DocumentEditor.FormatType.Rtf:
                return FormatType.Rtf;
            case EJ2DocumentEditor.FormatType.Txt:
                return FormatType.Txt;
            case EJ2DocumentEditor.FormatType.WordML:
                return FormatType.WordML;
            default:
                throw new NotSupportedException("DocIO does not support this file format.");
        }
    }

}


