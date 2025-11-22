using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace WakeOnLanApp
{
    public static class WakeOnLan
    {
        public static IReadOnlyCollection<MagicPacketSendResult> SendMagicPacket(
            string macAddress,
            IEnumerable<string> broadcastAddresses,
            int port)
        {
            if (broadcastAddresses == null)
                throw new ArgumentNullException(nameof(broadcastAddresses));

            if (!TryParseMacAddress(macAddress, out var macBytes))
                throw new ArgumentException("MACアドレスの形式が正しくありません。", nameof(macAddress));

            if (port < 1 || port > 65535)
                throw new ArgumentOutOfRangeException(nameof(port));

            var targets = broadcastAddresses
                .Where(a => !string.IsNullOrWhiteSpace(a))
                .Select(a => a.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (targets.Count == 0)
                throw new ArgumentException("送信先のブロードキャストアドレスが指定されていません。", nameof(broadcastAddresses));

            byte[] packet = BuildMagicPacket(macBytes);
            var results = new List<MagicPacketSendResult>();

            using (UdpClient client = new UdpClient())
            {
                client.EnableBroadcast = true;

                foreach (string address in targets)
                {
                    if (!IPAddress.TryParse(address, out IPAddress parsedAddress))
                    {
                        results.Add(new MagicPacketSendResult(address, false, "IPアドレスの書式が不正です。"));
                        continue;
                    }

                    try
                    {
                        client.Send(packet, packet.Length, new IPEndPoint(parsedAddress, port));
                        results.Add(new MagicPacketSendResult(address, true, string.Empty));
                    }
                    catch (Exception ex)
                    {
                        results.Add(new MagicPacketSendResult(address, false, ex.Message));
                    }
                }
            }

            return results;
        }

        public static IEnumerable<string> GetDefaultBroadcastAddresses()
        {
            var addresses = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "255.255.255.255"
            };

            foreach (var networkInterface in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (networkInterface.OperationalStatus != OperationalStatus.Up ||
                    networkInterface.NetworkInterfaceType == NetworkInterfaceType.Loopback)
                {
                    continue;
                }

                var ipProperties = networkInterface.GetIPProperties();
                foreach (var unicast in ipProperties.UnicastAddresses)
                {
                    if (unicast.Address.AddressFamily != AddressFamily.InterNetwork || unicast.IPv4Mask == null)
                        continue;

                    var broadcastAddress = CalculateBroadcast(unicast.Address, unicast.IPv4Mask);
                    if (!string.IsNullOrEmpty(broadcastAddress))
                        addresses.Add(broadcastAddress);
                }
            }

            return addresses;
        }

        private static bool TryParseMacAddress(string macAddress, out byte[] macBytes)
        {
            macBytes = Array.Empty<byte>();

            if (string.IsNullOrWhiteSpace(macAddress))
                return false;

            string[] macParts = macAddress.Split(new[] { ':', '-' }, StringSplitOptions.RemoveEmptyEntries);
            if (macParts.Length != 6)
                return false;

            var bytes = new byte[6];
            for (int i = 0; i < macParts.Length; i++)
            {
                if (!byte.TryParse(macParts[i], System.Globalization.NumberStyles.HexNumber, null, out byte parsed))
                    return false;

                bytes[i] = parsed;
            }

            macBytes = bytes;
            return true;
        }

        private static byte[] BuildMagicPacket(byte[] macBytes)
        {
            byte[] packet = new byte[102];
            for (int i = 0; i < 6; i++)
                packet[i] = 0xFF;

            for (int i = 1; i <= 16; i++)
                Array.Copy(macBytes, 0, packet, i * 6, 6);

            return packet;
        }

        private static string CalculateBroadcast(IPAddress address, IPAddress mask)
        {
            byte[] addressBytes = address.GetAddressBytes();
            byte[] maskBytes = mask.GetAddressBytes();

            if (addressBytes.Length != maskBytes.Length)
                return null;

            byte[] broadcastBytes = new byte[addressBytes.Length];
            for (int i = 0; i < addressBytes.Length; i++)
            {
                broadcastBytes[i] = (byte)(addressBytes[i] | (byte)~maskBytes[i]);
            }

            return new IPAddress(broadcastBytes).ToString();
        }
    }

    public sealed class MagicPacketSendResult
    {
        public MagicPacketSendResult(string address, bool success, string message)
        {
            Address = address;
            Success = success;
            Message = message ?? string.Empty;
        }

        public string Address { get; }
        public bool Success { get; }
        public string Message { get; }
    }
}
