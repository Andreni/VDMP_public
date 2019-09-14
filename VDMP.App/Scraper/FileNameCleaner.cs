using System;
using System.IO;
using System.Linq;
using static System.String;

namespace VDMP.App.Scraper
{
    /// <summary>Used for getting a "clean" file name</summary>
    internal class FileNameCleaner
    {
        public FileNameCleaner(string filename)
        {
            Filename = filename ?? throw new ArgumentNullException(nameof(filename));
            RemoveFileEnding();
        }

        public string Filename { get; set; }

        // First we need to remove the file ending:
        public void RemoveFileEnding()
        {
            Filename = Path.GetFileNameWithoutExtension(Filename);
            //Remove any illegale  dots with whitespace
            Filename = Filename.Replace(".", " ");
            // Replace whitespace with +
            Filename = Filename.Replace(" ", "+");
            CheckForYear();
        }

        public void CheckForYear()
        {
            // Split array
            var filenameInfo = Filename.Split("+");
            //Check if and part of the array contains a 4 digit combination
            for (var i = 0; i < filenameInfo.Length; i++)
                if (filenameInfo[i].All(char.IsDigit) && filenameInfo[i].Length == 4)
                {
                    var year = "&year=";
                    filenameInfo[i] = Concat(year, filenameInfo[i]);
                    Filename = Join("+", filenameInfo);
                }
        }

        public string ReturnTidyFilename()
        {
            return Filename;
        }
    }
}