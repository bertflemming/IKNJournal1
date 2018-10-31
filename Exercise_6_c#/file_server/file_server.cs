using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace tcp
{
	class file_server
	{
		/// <summary>
		/// The PORT
		/// </summary>
		const int PORT = 9000;
		/// <summary>
		/// The BUFSIZE
		/// </summary>
		const int BUFSIZE = 1000;

		/// <summary>
		/// Initializes a new instance of the <see cref="file_server"/> class.
		/// Opretter en socket.
		/// Venter på en connect fra en klient.
		/// Modtager filnavn
		/// Finder filstørrelsen
		/// Kalder metoden sendFile
		/// Lukker socketen og programmet
 		/// </summary>
		private file_server ()
		{
			// TO DO Your own code
			IPAddress ipAddress = IPAddress.Parse("10.0.0.1");
			TcpListener serverSocket = new TcpListener(ipAddress, PORT);
			TcpClient clientSocket = default(TcpClient);
			serverSocket.Start();
			Console.WriteLine(" >> Server Started");
			while (true)
            {
                try
                {
					clientSocket = serverSocket.AcceptTcpClient();
                    Console.WriteLine(" >> Accept connection from client");
                    NetworkStream networkStream = clientSocket.GetStream();
					Console.WriteLine(" >> Filename requested from client!");
                    string fileName = LIB.readTextTCP(networkStream);
					Console.WriteLine($" >> Filename from client: {fileName}");
					Console.WriteLine(" >> Filesize requested from client!");
					long fileSize = LIB.check_File_Exists(fileName);
					Console.WriteLine($" >> Filesize from server: {fileSize}");
					LIB.writeTextTCP(networkStream, fileSize.ToString());
                    if(fileSize != 0)
					{
						Console.WriteLine(" >> Sending file...");
						sendFile(fileName, fileSize, networkStream);
						Console.WriteLine(" >> File sent!");
					}
					else
					{
						LIB.writeTextTCP(networkStream, "File not found");
						Console.WriteLine(" >> Oops, file not found");
					}
                    networkStream.Flush();
					clientSocket.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
		}

		/// <summary>
		/// Sends the file.
		/// </summary>
		/// <param name='fileName'>
		/// The filename.
		/// </param>
		/// <param name='fileSize'>
		/// The filesize.
		/// </param>
		/// <param name='io'>
		/// Network stream for writing to the client.
		/// </param>
		private void sendFile (String fileName, long fileSize, NetworkStream io)
		{
			// TO DO Your own code
			FileStream fileStream = File.OpenRead(@fileName);
			byte[] sendBytes = new byte[fileSize];
			fileStream.Read(sendBytes, 0, sendBytes.Length);
			long bytesLeft = fileSize;
			long bytesSent = 0;
				for (int i = 0; i < fileSize; i += 1000)
				{
					if (bytesLeft > 1000)
					{
						io.Write(sendBytes, i, 1000);   
						bytesLeft -= 1000;
						bytesSent += 1000;
					}
					else if (bytesLeft < 1000 && bytesLeft > 0)
                    {
                        io.Write(sendBytes, (int)bytesSent, (int)bytesLeft);
					    bytesSent += bytesLeft;
					    bytesLeft = 0;
                    }
				Console.WriteLine(" >> Sent bytes: " + bytesSent.ToString());
                Console.WriteLine(" >> Bytes left: " + bytesLeft.ToString());
			    }
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
			Console.WriteLine ("Server starts...");
			new file_server();
		}
	}
}
