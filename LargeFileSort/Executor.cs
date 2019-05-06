using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using System.Diagnostics;

namespace LargeFileSort
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch SW = new Stopwatch();

            //Source File Path containing the .GZ file, change this accordingly for your run
            
            string Sp = "C:\\Users\\Nikhil\\Downloads\\bigfile.txt.gz";

            //Path and File name you want the Final result to be in, change this accordingly for your run
            string Flp = "C:\\Users\\Nikhil\\Downloads\\bigfileResult.txt";

            try
            {
                //Allocating object
                LargeFileProcessor BFP = new LargeFileProcessor(Sp, Flp);

                //Starting a timer
                SW.Start();
                Console.WriteLine("Starting to Decompress the GZIp file");

                //Decompress the original File
                string DecompressedFileName = BFP.Decompress();
                Console.WriteLine("Elapsed time for Decompress  {0}", SW.Elapsed);

                SW.Reset();
                SW.Start();

                //Create Sorted Files with 5000 records each
                int NoOfTempFiles = BFP.CreateTempFiles(DecompressedFileName);
                Console.WriteLine("Elapsed time for Temp File Creations  {0}", SW.Elapsed);

                SW.Reset();
                SW.Start();

                //Logic to process the files containing 5000 records, spitting out the 29th character
                BFP.ReadTempFileFirstLinesProcessAndWrite(NoOfTempFiles);
                Console.WriteLine("Elapsed time for File Processing logic  {0}", SW.Elapsed);

                SW.Reset();
                SW.Start();

                //Compress the .txt file containing the final result.
                BFP.Compress();
                Console.WriteLine("Elapsed time for File compression:  {0}", SW.Elapsed);
                SW.Stop();

                Console.WriteLine("Final Result at :  {0}", Flp + ".gz");
                Console.ReadLine();
            }

            catch (Exception Ex)
            {

                Console.WriteLine(Ex.Message);
                Console.ReadLine();
            }


        }


    }
}