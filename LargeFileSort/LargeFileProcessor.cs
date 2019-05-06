using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using System.Configuration;

namespace LargeFileSort
{
    class LargeFileProcessor  
    {
        public string SourcePath { get; set; }
        public string FinalPath { get; set; }
        public string FolderPath { get; set; }
        public FileInfo FiletoDecompress { get; set; }
        public FileInfo FiletoCompress { get; set; }

        const int NoOfLinesAtATime = 5000;
        public LargeFileProcessor(string Sp, string Flp)
        {

            this.SourcePath = Sp;
            this.FinalPath = Flp;
            this.FiletoDecompress = new FileInfo(this.SourcePath);
            this.FiletoCompress = new FileInfo(this.FinalPath);
            this.FolderPath = FiletoDecompress.DirectoryName;


        }

        public string Decompress()
        {
            using (FileStream originalFileStream = this.FiletoDecompress.OpenRead())
            {
                string currentFileName = this.FiletoDecompress.FullName;

                string newFileName = currentFileName.Remove(currentFileName.Length - this.FiletoDecompress.Extension.Length);

                using (FileStream decompressedFileStream = File.Create(newFileName))
                {
                    using (GZipStream decompressionStream = new GZipStream(originalFileStream, CompressionMode.Decompress))
                    {
                        decompressionStream.CopyTo(decompressedFileStream);
                        Console.WriteLine("Decompressed: {0}", this.FiletoDecompress.Name);
                    }
                }
                return newFileName;
            }


        }

        public int CreateTempFiles(string sourcefilename)
        {
            string Line, s;
            List<string> ReadLines = new List<string>();
            using (StreamReader SourceFile = new StreamReader(sourcefilename))
            {
                int j = 0, i;
                do
                {
                    for (i = 1; i <= NoOfLinesAtATime; i++)
                    {
                        if ((Line = SourceFile.ReadLine()) != null)
                        {
                            ReadLines.Add(Line);

                        }

                        else
                        {
                            break;
                        }

                    }


                    ReadLines.Sort();

                    File.WriteAllLines(this.FolderPath + "\\" + "file_" + j + ".txt", ReadLines);
                    j = j + 1;
                    ReadLines.Clear();

                } while (!SourceFile.EndOfStream);

                return (j - 1);

            }

        }

        public void ReadTempFileFirstLinesProcessAndWrite(int NoOfTempFiles)
        {
            int i = 0;
            string line, removedmergeline, MergeLine;
            StringBuilder FinalResult = new StringBuilder();
            List<string> mergedLinesArray = new List<string>();
            string[] fileArray = new string[NoOfTempFiles];
            Dictionary<string, StreamReader> lineFileLookup = new Dictionary<string, StreamReader>();


            while (i < NoOfTempFiles)
            {
                StreamReader TempFile = new StreamReader(this.FolderPath + "\\" + "file_" + i + ".txt");
                line = TempFile.ReadLine();
                mergedLinesArray.Add(line);
                lineFileLookup.Add(line, TempFile);
                i = i + 1;
            }




            mergedLinesArray.Sort();
            FinalResult.Append(mergedLinesArray[0].Substring(28, 1));
            removedmergeline = mergedLinesArray[0];
            mergedLinesArray.Remove(mergedLinesArray[0]);


            while (mergedLinesArray.Count != 0)
            {
                int index;
                var filematch = lineFileLookup[removedmergeline];
                lineFileLookup.Remove(removedmergeline);

                if ((MergeLine = filematch.ReadLine()) != null)
                {
                    index = mergedLinesArray.BinarySearch(MergeLine);
                    if (index < 0)
                    {
                        mergedLinesArray.Insert(~index, MergeLine);

                    }

                    else if (~index == mergedLinesArray.Count)
                    {
                        mergedLinesArray.Add(MergeLine);

                    }

                    else
                    {
                        mergedLinesArray.Insert(index, MergeLine);
                    }

                    if (!lineFileLookup.ContainsKey(MergeLine))
                    {
                        lineFileLookup.Add(MergeLine, filematch);
                    }
                }

                FinalResult.Append(mergedLinesArray[0].Substring(28, 1));
                removedmergeline = mergedLinesArray[0];
                mergedLinesArray.Remove(mergedLinesArray[0]);


            }

            File.WriteAllText(FinalPath, FinalResult.ToString());

        }

        public void Compress()
        {

            using (FileStream originalFileStream = this.FiletoCompress.OpenRead())
            {
                using (FileStream compressedFileStream = File.Create(this.FiletoCompress.FullName + ".gz"))
                {
                    using (GZipStream compressionStream = new GZipStream(compressedFileStream,
                       CompressionMode.Compress))
                    {
                        originalFileStream.CopyTo(compressionStream);

                    }
                }

            }


        }


    }

}
