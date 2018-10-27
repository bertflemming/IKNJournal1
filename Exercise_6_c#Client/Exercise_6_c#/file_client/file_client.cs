using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace tcp
{
	class file_client
	{
		/// <summary>
		/// The PORT.
		/// </summary>
		const int PORT = 9000;
		/// <summary>
		/// The BUFSIZE.
		/// </summary>
		const int BUFSIZE = 1000;

		/// <summary>
		/// Initializes a new instance of the <see cref="file_client"/> class.
		/// </summary>
		/// <param name='args'>
		/// The command-line arguments. First ip-adress of the server. Second the filename
		/// </param>
		private file_client (string[] args)
		{
			// TO DO Your own code
			TcpClient clientSocket = new TcpClient();
			Console.WriteLine(" >> Client connecting");
			clientSocket.Connect(args[0], PORT);
			NetworkStream serverStream = clientSocket.GetStream();
			LIB.writeTextTCP(serverStream, args[1]);

			long size = LIB.getFileSizeTCP(serverStream);
            if(size != 0)
			{
				string fileName = LIB.extractFileName(args[1]);
				receiveFile(fileName, serverStream, size);          
			}
			else
			{
				Console.WriteLine(LIB.readTextTCP(serverStream));
			}
			serverStream.Flush();
			clientSocket.Close();

		}

		/// <summary>
		/// Receives the file.
		/// </summary>
		/// <param name='fileName'>
		/// File name.
		/// </param>
		/// <param name='io'>
		/// Network stream for reading from the server
		/// </param>
		private void receiveFile (String fileName, NetworkStream io, long fileSize)
		{
			// TO DO Your own code
			Console.WriteLine(" >> Recieving file");
			FileStream fileStream = File.Create("/root/Desktop/IKN/" + fileName);
            byte[] recieveBytes = new byte[fileSize];
			int readBytes = 0;
			int bytesLeft = (int)fileSize;
			while (readBytes < fileSize)
			{
				bytesLeft = (int)fileSize - readBytes;

				if (bytesLeft > 1000)
					readBytes += io.Read(recieveBytes, readBytes, 1000);
				else
					readBytes += io.Read(recieveBytes, readBytes, bytesLeft);
				
				Console.WriteLine(" >> Read bytes: " + readBytes.ToString());
				Console.WriteLine(" >> Bytes left: " + bytesLeft.ToString());
				Console.WriteLine(" >> Recieved bytes: " + recieveBytes.Length);

			}

			fileStream.Write(recieveBytes, 0, (int)fileSize);
            
            fileStream.Close(); 
		}

		/// <summary>
		/// The entry point of the program, where the program control starts and ends.
		/// </summary>
		/// <param name='args'>
		/// The command-line arguments.
		/// </param>
		public static void Main (string[] args)
		{
			Console.WriteLine ("Client starts...");
			new file_client(args);
		}
	}
}
