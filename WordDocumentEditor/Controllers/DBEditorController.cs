using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Syncfusion.DocIO;
using System;
using System.IO;
using EJ2DocumentEditor = Syncfusion.EJ2.DocumentEditor;
using Microsoft.Data.SqlClient;
using System.Data;
using WordDocumentEditor.Models;

namespace WordDocumentEditor.Controllers
{
    public class DBEditorController : Controller
    {

        private IHostEnvironment hostEnvironment;
        private string connectionString;
        public DBEditorController(IHostEnvironment environment, IConfiguration config)
        {
            this.hostEnvironment = environment;
            this.connectionString = config.GetConnectionString("DocContext");
        }


        public string FileImport([FromBody] CustomParams param) //ImportFile
        {
            try
            {
                Syncfusion.EJ2.DocumentEditor.WordDocument document = GetDocumentFromDatabase(param.fileName);
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(document);
                document.Dispose();
                return json;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return "Failure";
            }
        }

        public string TakeData() //GetDataSource
        {
            string tableName = "Document";
            using (var connection = new SqlConnection(connectionString))
            {
                string query = "Select * FROM Document";
                connection.Open();
                var command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();
                DataTable table = new DataTable();
                table.Load(reader);

                List<DocumentInfo> dataSource = new List<DocumentInfo>();
                int i = 0;
                foreach (DataRow row in table.Rows)
                {
                    dataSource.Add(new DocumentInfo { FileIndex = i, FileName = row["FileName"].ToString() });
                    i++;
                }
                return Newtonsoft.Json.JsonConvert.SerializeObject(dataSource);
            }
            return "[]";
        }



        private Syncfusion.EJ2.DocumentEditor.WordDocument GetDocumentFromDatabase(string fileName)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT Blob from Document WHERE FileName='" + fileName + "'";
                    connection.Open();
                    var command = new SqlCommand(query, connection);
                    object data = command.ExecuteScalar();

                    Stream stream = new MemoryStream(data as Byte[]);
                    stream.Position = 0;
                    Syncfusion.EJ2.DocumentEditor.WordDocument document = Syncfusion.EJ2.DocumentEditor.WordDocument.Load(stream, Syncfusion.EJ2.DocumentEditor.FormatType.Docx);
                    stream.Dispose();
                    return document;
                }
            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message);
            }

            return null;
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
                    throw new NotSupportedException("EJ2 Document editor does not support this file format.");
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
}
