using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Drawing;

class Program
{
    public static bool isRunningThing = false;
    public static string programVer = "1.1";
    public static string fileFormatVer = "v1";
    public static string[] urls;
    public static void Main(string[] args)
    {
        try
        {
            if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\temp"))
            {
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "\\temp");
            }
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
                Console.WriteLine("Welcome to the File Biggener v" + programVer + "!!\nThis tool makes your files bigger.\nBy greensoupdev.\n");
                int thing = 0;
                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i] == "-d")
                    {
                        continue;
                    }
                    else if (args[i] == "-b")
                    {
                        continue;
                    }
                    else
                    {
                        thing++;
                    }
                }
                urls = new string[thing];
                for (int i = 0; i < args.Length; i++)
                {

                    if (args[i] == "-d")
                    {
                        continue;
                    }
                    else if (args[i] == "-b")
                    {
                        continue;
                    }
                    else
                    {
                        urls[i] = args[i];

                    }

                }
                if (Path.GetExtension(args[0]) == ".gbf" && args.Length == 1)
                {
                    RunThing();
                }
                else if (args[args.Length - 1] == "-d")
                {
                    Debiggen();
                }
                else if (args[args.Length - 1] == "-b")
                {
                    Biggen();
                }
                else
                {
                    Console.WriteLine("What do you want to do? (Biggen: 1 / Debiggen: 2)");
                    string response = Console.ReadLine()?.Trim();

                    switch (response)
                    {
                        case "1":
                            Biggen();
                            break;

                        case "2":
                            Debiggen();
                            break;

                        default:
                            Console.WriteLine("Invalid option.");
                            break;
                    }
                }
            }
            else
            {
                Console.WriteLine("Welcome to the File Biggener v" + programVer + "!!\nThis tool makes your files bigger.\nBy greensoupdev.\n\nIf you want to biggen or debiggen stuff, you can just grab the files or folders to this program and boom.");
            }

        }
        catch (Exception e)
        {
            Console.WriteLine($"Error: {e}");
            throw;
        }
        Console.WriteLine("Press any key to exit..");
        string end = Console.ReadLine();




        /*  byte[] bytes = File.ReadAllBytes("hmm.wav");
          //yo mama
          File.WriteAllText("heeey.txt", string.Join(",", bytes));
          string result = string.Join(",", bytes);
          string hhhh = result;//File.ReadAllText("heeey.txt");
          splittedThing = hhhh.Split(new string[] { "," }, StringSplitOptions.None);


          var byt = splittedThing.Select(byte.Parse).ToArray();
          File.WriteAllBytes("hahaaa.wav", byt);*/

    }

    public static void Biggen()
    {
        try
        {
            Console.WriteLine("Biggening file(s)...");
            for (int i = 0; i < urls.Length; i++)
            {
                if (!File.Exists(urls[i]))
                {
                    Console.WriteLine("File not found.");
                    continue;
                }

                if (Path.GetExtension(urls[i]) != ".gbf")
                {

                    string outputFile = Path.ChangeExtension(urls[i], ".gbf");

                    using (FileStream input = new FileStream(urls[i], FileMode.Open, FileAccess.Read))
                    using (FileStream output = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
                    {
                        // Write file name as hex at the beginning
                        string fileNameHex = BitConverter.ToString(Encoding.UTF8.GetBytes(Path.GetFileName(urls[i]))).Replace("-", "");
                        Console.WriteLine("Biggening: " + Path.GetFileName(urls[i]));
                        byte[] header = Encoding.UTF8.GetBytes(fileNameHex + "::" + fileFormatVer + "\n");
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



                }


                else
                {
                    Console.WriteLine("You cannot make already biggened files more big...");
                    continue;
                }

            }
            Console.WriteLine($"File(s) BIGGENED!");
        }

        catch (Exception e)
        {
            Console.WriteLine($"Error: {e}");
            throw;
        }
    }

    public static void Debiggen()
    {
        try
        {

            Console.WriteLine("Debiggening file(s)...");
            for (int i = 0; i < urls.Length; i++)
            {
                if (!File.Exists(urls[i]))
                {
                    Console.WriteLine("File not found.");
                    continue;
                }
                if (Path.GetExtension(urls[i]) != ".gbf")
                {
                    Console.WriteLine("This is not a .GBF file.");
                    continue;
                }


                string[] lines = File.ReadAllLines(urls[i]);

                // Extract original file name from hex
                string[] thiinger = lines[0].Split(new string[] { "::" }, StringSplitOptions.None);
                string fileName = Encoding.UTF8.GetString(FromHex(thiinger[0].Trim()));
                Console.WriteLine("Debiggening: " + fileName);
                string fileVersion = thiinger[1];
                string outputPath = "";
                if (isRunningThing)
                {
                    outputPath = AppDomain.CurrentDomain.BaseDirectory + "temp\\" + fileName;
                }
                else
                {

                    outputPath = Path.Combine(Path.GetDirectoryName(urls[i]), "BG_" + fileName);
                }

                using (FileStream output = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
                {
                    string binaryData = lines[1];
                    for (int j = 0; j < binaryData.Length; j += 8)
                    {
                        string byteString = binaryData.Substring(j, 8);
                        byte b = Convert.ToByte(byteString, 2);
                        output.WriteByte(b);
                    }
                }


            }
            Console.WriteLine($"File(s) DEBIGGENED!");

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

    public static void RunThing()
    {
        try
        {
            Console.WriteLine("What do you want to do? (Run: 1 / Debiggen: 2)");
            string response = Console.ReadLine()?.Trim();

            switch (response)
            {
                case "2":
                    isRunningThing = false;
                    Debiggen();
                    return;

                case "1":
                    isRunningThing = true;
                    break;

                default:
                    Console.WriteLine("Invalid option. \nPress any key to exit..");
                    break;
            }
            Console.WriteLine("Opening file...");
            string tempDir = AppDomain.CurrentDomain.BaseDirectory + "\\temp";
            Directory.CreateDirectory(tempDir);

            string[] lines = File.ReadAllLines(urls[0]);
            string[] thiinger = lines[0].Split(new string[] { "::" }, StringSplitOptions.None);
            string fileName = Encoding.UTF8.GetString(FromHex(thiinger[0].Trim()));
            string tempFilePath = tempDir + "\\" + fileName;

            if (!File.Exists(tempFilePath))
            {
                Debiggen();
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
