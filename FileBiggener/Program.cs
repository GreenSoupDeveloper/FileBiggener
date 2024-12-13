using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Drawing;

class Program
{
    public static bool isRunningThing = false;
    public static string programVer = "1.0";
    public static string fileFormatVer = "v1";
    public static void Main(string[] args)
    {
        try
        {
            System.IO.DirectoryInfo di = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + "temp");
            FileInfo[] TXTFiles = di.GetFiles("*.*");
            if (TXTFiles.Length != 0)
            {
                foreach (FileInfo file in di.GetFiles())
                {
                    try
                    {
                        file.Delete();
                    }
                    catch (IOException)
                    {
                        continue;
                    }
                }
            }
            if (args.Length != 0)
            {

                if (args.Length == 1)
                {
                    if (File.Exists(args[0]))
                    {
                        isRunningThing = true;
                        RunThing(args[0]);
                    }
                    else
                    {
                        Console.WriteLine("File Doesnt Exist.");
                    }
                }
                else if (args.Length == 2)
                {
                    if (args[1] == "-d")
                    {
                        Debiggen(args[0],false);
                    }
                    else if (args[1] == "-b")
                    {
                        Biggen(args[0],false);
                    }
                    else
                    {
                        Console.WriteLine("Unknown Argument.\n");
                    }
                }
                else if (args.Length == 3)
                {
                    if (args[2] == "-dir")
                    {
                        Debiggen(args[0], true);
                    }
                    else if (args[2] == "-f")
                    {
                        Biggen(args[0], false);
                    }
                    else
                    {
                        Console.WriteLine("Unknown Argument.\n");
                    }
                }
                Environment.Exit(0);
            }

            Console.WriteLine("Welcome to the File Biggener v"+ programVer+"!!\nThis tool makes your files bigger.\nBy greensoupdev.\n");
            Console.WriteLine("What do you want to do? (Biggen: 1 / Debiggen: 2)");
            string response = Console.ReadLine()?.Trim();

            switch (response)
            {
                case "1":
                    Console.Write("Enter the path of your file: ");
                    string biggenPath = Console.ReadLine()?.Trim();
                    Biggen(biggenPath, false);
                    Console.WriteLine("Press any key to exit..");
                    string end1 = Console.ReadLine();
                    break;

                case "2":
                    Console.Write("Enter the path of your file: ");
                    string debiggenPath = Console.ReadLine()?.Trim();
                    Debiggen(debiggenPath, false);
                    Console.WriteLine("Press any key to exit..");
                    string end2 = Console.ReadLine();
                    break;

                default:
                    Console.WriteLine("Invalid option. \nPress any key to exit..");
                    string end = Console.ReadLine();
                    break;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error: {e}");
            throw;
        }
    }

    public static void Biggen(string filePath, bool isBulk)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine("File not found.");
                return;
            }
            if (Path.GetExtension(filePath) != ".gbf")
            {
                Console.WriteLine("Biggening file...");
                string outputFile = Path.ChangeExtension(filePath, ".gbf");

                using (FileStream input = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                using (FileStream output = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
                {
                    // Write file name as hex at the beginning
                    string fileNameHex = BitConverter.ToString(Encoding.UTF8.GetBytes(Path.GetFileName(filePath))).Replace("-", "");
                    byte[] header = Encoding.UTF8.GetBytes(fileNameHex + "::"+ fileFormatVer + "\n");
                    output.Write(header, 0, header.Length);

                    // Process file in chunks
                    byte[] buffer = new byte[8192];
                    int bytesRead;
                    int progress = 0;
                    while ((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        foreach (byte b in buffer[..bytesRead])
                        {
                            /*  progress++;
                              Console.WriteLine(progress+" bytes of " + bytesRead);*/
                            string binary = Convert.ToString(b, 2).PadLeft(8, '0');
                            byte[] binaryBytes = Encoding.UTF8.GetBytes(binary);
                            output.Write(binaryBytes, 0, binaryBytes.Length);
                        }
                    }
                }

                Console.WriteLine($"File BIGGENED! Output: {outputFile}");
            }
            else
            {
                Console.WriteLine("You cannot make already biggened files more big...");
                return;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error: {e}");
            throw;
        }
    }

    public static void Debiggen(string filePath, bool isBulk)
    {
        try
        {
            if (!isBulk)
            {
                if (!File.Exists(filePath))
                {
                    Console.WriteLine("File not found.");
                    return;
                }
                if (Path.GetExtension(filePath) != ".gbf")
                {
                    Console.WriteLine("This is not a .GBF file.");
                    return;
                }

                Console.WriteLine("Debiggening file...");
                string[] lines = File.ReadAllLines(filePath);

                // Extract original file name from hex
                string[] thiinger = lines[0].Split(new string[] { "::" }, StringSplitOptions.None);
                string fileName = Encoding.UTF8.GetString(FromHex(thiinger[0].Trim()));
                string fileVersion = thiinger[1];
                string outputPath = "";
                if (isRunningThing)
                {
                    outputPath = AppDomain.CurrentDomain.BaseDirectory + "temp\\" + fileName;
                }
                else
                {
                    outputPath = Path.Combine(Path.GetDirectoryName(filePath), "GBF_" + fileName);
                }

                using (FileStream output = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
                {
                    string binaryData = lines[1];
                    for (int i = 0; i < binaryData.Length; i += 8)
                    {
                        string byteString = binaryData.Substring(i, 8);
                        byte b = Convert.ToByte(byteString, 2);
                        output.WriteByte(b);
                    }
                }

                Console.WriteLine($"File DEBIGGENED! Output: {outputPath}");
            }
            //pls write bulk decompression and compression tihng
            else
            {
              
               
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error: {e}");
            throw;
        }
    }

    public static byte[] FromHex(string hex)
    {
        try
        {
            byte[] bytes = new byte[hex.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }
            return bytes;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error: {e}");
            throw;
        }
    }

    public static void RunThing(string filePath)
    {
        try
        {
            Console.WriteLine("Opening file...");
            string tempDir = AppDomain.CurrentDomain.BaseDirectory + "\\temp";
            Directory.CreateDirectory(tempDir);

            string[] lines = File.ReadAllLines(filePath);
            string[] thiinger = lines[0].Split(new string[] { "::" }, StringSplitOptions.None);
            string fileName = Encoding.UTF8.GetString(FromHex(thiinger[0].Trim()));
            string tempFilePath = tempDir + "\\" + fileName;

            if (!File.Exists(tempFilePath))
            {
                Debiggen(filePath, false);
            }
            File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "\\exec.bat", "\"" + tempFilePath + "\"");
            Process.Start(AppDomain.CurrentDomain.BaseDirectory + "\\exe.bat");
            Environment.Exit(0);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error: {e}");
            throw;
        }
    }
}
