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
            string serverIP =  Environment.GetEnvironmentVariable("SERVER_HOST") ?? "localhost";
            int serverPort = 8888; 


            TcpClient client = new TcpClient(serverIP, serverPort);
            Console.WriteLine(client.Connected);

            using NetworkStream stream = client.GetStream();

            while (true)
            {
                // Read temperature from sensor
                double temperature = ReadTemperature();

                string measurementName = "Temperature";
                DateTime timestamp = DateTime.Now;
                double value = temperature;
                string telemetryData = JsonSerializer.Serialize(new TelemetryData(measurementName, value, timestamp));

                await SendData(stream, telemetryData);
                await Task.Delay(200);
            }

            stream.Close();
            client.Close();

        }

        private static async Task SendData(NetworkStream stream, string telemetryData)
        {
            string delimitedData = telemetryData + "<|endofmessage|>";

            byte[] data = Encoding.ASCII.GetBytes(delimitedData);

            await stream.WriteAsync(data);
        }

        static double ReadTemperature()
        {

            Random random = new();
            double temperature = random.NextDouble() * 100;
            return temperature;
        }
    }
}

public record class TelemetryData(string Name, double Value, DateTime TimeStamp);

