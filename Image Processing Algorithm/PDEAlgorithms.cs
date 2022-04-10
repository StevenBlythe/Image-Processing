using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Image_Processing_Algorithm
{
    internal class PDEAlgorithms
    {
        internal static String SelectImage()
        {
            // Find the path with the files
            String binPath = Directory.GetParent(path: Environment.CurrentDirectory).Parent.Parent.FullName;
            String path = Path.Combine(binPath, @"Images\");

            // Filter the folder in favor for .jpg, .png, .jpeg, .bmp
            var varfiles = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories)
            .Where(s => s.EndsWith(".jpg") || s.EndsWith(".png") || s.EndsWith(".jpeg") || s.EndsWith(".bmp"));

            // Setup regex expression for readability
            String regex = @"[ \w-]+\.(png|jpeg|jpg|bmp|)";

            int i = 0;
            foreach (string file in varfiles)
            {
                Regex pattern = new Regex(regex);

                Match match = pattern.Match(file);

                Console.WriteLine(i++ + "\t" + match);
            }

            Console.WriteLine("Select a file");

            Console.Write("Select an index: ");

            string? v = Console.ReadLine();

            i = Int32.Parse(v);


            String[] fileList = varfiles.ToArray();
            return fileList[i];
        }
    }
}
