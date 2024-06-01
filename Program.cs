using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace IotPortal.DevicePlaceholder.src
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            string serverIP = "192.168.56.1"; //Environment.GetEnvironmentVariable("SERVER_HOST") ?? "localhost";
            int serverPort = 8888; // Change this to your server's port

            try
            {
                // Create a TcpClient
                TcpClient client = new TcpClient(serverIP, serverPort);
                Console.WriteLine(client.Connected);

                // Get a network stream
                using NetworkStream stream = client.GetStream();

                int counts = 0;
                Random random = new();
                while (true)
                {
                    // Create telemetry data
                    string name = "Temperature";
                    DateTime timestamp = DateTime.Now;
                    double value = random.NextDouble() * 100;

                    // Format telemetry data as string
                    string telemetryData = JsonSerializer.Serialize(new TelemetryData(name, value, timestamp));
                   
                    // Add delimiter to the telemetry data
                    string delimitedData = telemetryData + "<|endofmessage|>";

                    // Convert the string data to bytes
                    byte[] data = Encoding.ASCII.GetBytes(delimitedData);

                    // Send the data over the network
                     await stream.WriteAsync(data);
                    await Task.Delay(200);
                }

                // Clean up
                stream.Close();
                client.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
    }
}

public record class TelemetryData(string Name, double Value, DateTime TimeStamp);

