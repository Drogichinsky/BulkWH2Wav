using System;
using System.IO;
using System.Text;

string[] arguments = Environment.GetCommandLineArgs();
string path = arguments[1];
string[] filePaths = Directory.GetFiles(@path, "*.wav");
//Console.WriteLine(filePaths[0]);
int i = 0;

    //Pass the filepath and filename to the StreamWriter Constructor
    StreamWriter sw = new StreamWriter("BulkWH2Wav.log");
    //Write a line of text
    sw.WriteLine("Start");
 
foreach (string filePath in filePaths)
{
 
    const int HEADER_SIZE = 4;
    byte[] bytesFile = new byte[HEADER_SIZE];
    using (FileStream fs = File.OpenRead(@filePath))
    {
        fs.Read(bytesFile, 0, HEADER_SIZE);
        fs.Close();
    }

    string hex = BitConverter.ToString(bytesFile);
    if (hex == "52-49-46-46")
    {
        //Console.WriteLine("Found header wav, skipped...");
        sw.WriteLine("Found header wav, skipped...");
    }
   // Console.WriteLine(hex);
   else
    {
        sw.WriteLine(filePath);
        i++;
        string FileToWrite = filePath;
        var size = new FileInfo(filePath).Length;
        int subChunk1Size = 16;
        short audioFormat = 1;
        short bitsPerSample = 16;
        short numChannels = 1;
        int sampleRate = 8000;
        int byteRate = sampleRate * numChannels * (bitsPerSample / 8);
        short blockAlign = (short)(numChannels * (bitsPerSample / 8));
        int chunkSize = unchecked((int)size) + 36;
        int subChunk2Size = unchecked((int)size);
        byte[] Readb1 = File.ReadAllBytes(FileToWrite);
        File.Delete(FileToWrite);
        FileStream f = new FileStream(FileToWrite, FileMode.Create);
        BinaryWriter wr = new BinaryWriter(f);
        wr.Write(Encoding.ASCII.GetBytes("RIFF"));
        wr.Write(chunkSize);
        wr.Write(Encoding.ASCII.GetBytes("WAVE"));
        wr.Write(Encoding.ASCII.GetBytes("fmt"));
        wr.Write((byte)32);
        wr.Write(subChunk1Size);
        wr.Write(audioFormat);
        wr.Write(numChannels);
        wr.Write(sampleRate);
        wr.Write(byteRate);
        wr.Write(blockAlign);
        wr.Write(bitsPerSample);
        wr.Write(Encoding.ASCII.GetBytes("data"));
        wr.Write(subChunk2Size);
        wr.Write(Readb1);
        wr.Close();
        wr.Dispose();

    }
    
}
//Console.WriteLine(i + " Files headed");
sw.WriteLine(i + " Files headed");
sw.Close();