
/// THIS SHIT NOT WORKIN cuz i aint figured out a way to connect to needed port trough unity.netcode's network manager

// using System;
// using System.Collections.Generic;
// using System.IO;
// using System.Linq;
// using System.Net;
// using System.Security.Cryptography;
// using System.Text;
// using System.Threading;

// public class HostedFile
// {
//     public string VirtualPath { get; set; } // Path trough which it can be accessed
//     public string FilePath { get; set; } // Path to file on pc
//     public string MimeType { get; set; } // File type
//     public long Size => File.Exists(FilePath) ? new FileInfo(FilePath).Length : 0;
//     public string Sha256
//     {
//         get { return GetChecksum(FilePath); }
//         protected set;
//     }

//     public HostedFile(string virtualPath, string filePath, string mimeType = null)
//     {
//         VirtualPath = virtualPath.TrimStart('/');
//         FilePath = filePath;
//         MimeType = mimeType ?? GetDefaultMimeType(filePath);
//     }

//     private static string GetDefaultMimeType(string filePath)
//     {
//         string ext = Path.GetExtension(filePath).ToLower();
//         return ext switch
//         {
//             ".txt" => "text/plain",
//             ".pdf" => "application/pdf",
//             ".zip" => "application/zip",
//             ".jpg" or ".jpeg" => "image/jpeg",
//             ".png" => "image/png",
//             ".mp4" => "video/mp4",
//             ".mp3" => "audio/mpeg",
//             _ => "application/octet-stream",
//         };
//     }

//     private static string GetChecksum(string file)
//     {
//         using (FileStream stream = File.OpenRead(file))
//         {
//             var sha = new SHA256Managed();
//             byte[] checksum = sha.ComputeHash(stream);
//             return BitConverter.ToString(checksum).Replace("-", String.Empty);
//         }
//     }
// }

// public class SimpleFileServer : IDisposable
// {
//     private readonly HttpListener _listener;
//     private readonly string _ipport;
//     public List<HostedFile> Files;

//     public SimpleFileServer(string ipport)
//     {
//         _ipport = ipport;
//         _listener = new HttpListener();
//         _listener.Prefixes.Add($"http://{_ipport}/");
//         Files = new List<HostedFile>();
//     }

//     public void AddFile(HostedFile file)
//     {
//         Files.Add(file);
//     }

//     public void RemoveFile(HostedFile file)
//     {
//         Files.Remove(file);
//     }

//     public void UpdateHostedList(List<HostedFile> hostedFiles)
//     {
//         Files = hostedFiles;
//     }

//     public void Start()
//     {
//         try
//         {
//             _listener.Start();
//             Console.WriteLine($"Server started on {_ipport}");

//             ThreadPool.QueueUserWorkItem(
//                 async (state) =>
//                 {
//                     while (_listener.IsListening)
//                     {
//                         try
//                         {
//                             var context = await _listener.GetContextAsync();
//                             ProcessRequest(context);
//                         }
//                         catch (Exception ex)
//                             when (ex is HttpListenerException || ex is ObjectDisposedException)
//                         {
//                             break;
//                         }
//                         catch (Exception ex)
//                         {
//                             Console.WriteLine($"Error: {ex.Message}");
//                         }
//                     }
//                 }
//             );
//         }
//         catch (Exception ex)
//         {
//             Console.WriteLine($"Failed to start: {ex.Message}");
//             throw;
//         }
//     }

//     private void ProcessRequest(HttpListenerContext context)
//     {
//         try
//         {
//             string requestPath = context.Request.Url.LocalPath.TrimStart('/');

//             var file = Files.FirstOrDefault(f =>
//                 f.VirtualPath.Equals(requestPath, StringComparison.OrdinalIgnoreCase)
//             );

//             if (file == null || !File.Exists(file.FilePath))
//             {
//                 Send404(context);
//                 return;
//             }

//             SendFile(context, file);
//         }
//         catch (Exception ex)
//         {
//             Console.WriteLine($"Request error: {ex.Message}");
//             Send500(context, ex.Message);
//         }
//     }

//     private void SendFile(HttpListenerContext context, HostedFile file)
//     {
//         try
//         {
//             var fileInfo = new FileInfo(file.FilePath);

//             context.Response.ContentType = file.MimeType;
//             context.Response.ContentLength64 = fileInfo.Length;

//             // Отправляем файл
//             using (var fs = File.OpenRead(file.FilePath))
//             {
//                 byte[] buffer = new byte[4096];
//                 int bytesRead;

//                 while ((bytesRead = fs.Read(buffer, 0, buffer.Length)) > 0)
//                 {
//                     context.Response.OutputStream.Write(buffer, 0, bytesRead);
//                 }
//             }

//             Console.WriteLine($"Served: {file.VirtualPath} ({fileInfo.Length} bytes)");
//         }
//         catch (Exception ex)
//         {
//             Console.WriteLine($"File send error: {ex.Message}");
//             Send500(context, ex.Message);
//         }
//         finally
//         {
//             context.Response.OutputStream.Close();
//         }
//     }

//     private void Send404(HttpListenerContext context)
//     {
//         context.Response.StatusCode = 404;
//         byte[] buffer = Encoding.UTF8.GetBytes("File not found");
//         context.Response.OutputStream.Write(buffer, 0, buffer.Length);
//         context.Response.OutputStream.Close();
//     }

//     private void Send500(HttpListenerContext context, string message)
//     {
//         context.Response.StatusCode = 500;
//         byte[] buffer = Encoding.UTF8.GetBytes($"Server error: {message}");
//         context.Response.OutputStream.Write(buffer, 0, buffer.Length);
//         context.Response.OutputStream.Close();
//     }

//     public void Stop()
//     {
//         _listener?.Stop();
//     }

//     public void Dispose()
//     {
//         Stop();
//         _listener?.Close();
//     }
// }

// // Пример использования
// // class Program
// // {
// //     static void Main(string[] args)
// //     {
// //         // Создаем сервер на порту 12345
// //         using var server = new SimpleFileServer(12345);

// //         // Добавляем файлы в список
// //         server.AddFile(new HostedFile("document.pdf", @"C:\Files\doc.pdf"));
// //         server.AddFile(new HostedFile("image.jpg", @"C:\Files\photo.jpg"));
// //         server.AddFile(new HostedFile("data.txt", @"C:\Files\info.txt"));

// //         // Запускаем сервер
// //         server.Start();

// //         Console.WriteLine("Files available:");
// //         Console.WriteLine("- document.pdf");
// //         Console.WriteLine("- image.jpg");
// //         Console.WriteLine("- data.txt");
// //         Console.WriteLine("\nPress Ctrl+C to stop");

// //         // Ждем остановки
// //         Thread.Sleep(Timeout.Infinite);
// //     }
// // }
