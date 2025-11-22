using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Windows.Forms;

namespace WakeOnLanApp
{
    public partial class MainForm : Form
    {
        private readonly BindingList<DeviceEntry> devices = new BindingList<DeviceEntry>();
        private readonly string deviceStorePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "devices.json");

        public MainForm()
        {
            InitializeComponent();
            this.lstDevices.DataSource = devices;
            this.lstDevices.DisplayMember = nameof(DeviceEntry.DisplayName);
            LoadDevicesFromFile();
            this.FormClosing += Form1_FormClosing;
        }

        private void btnWake_Click(object sender, EventArgs e)
        {
            try
            {
                string macAddress = this.txtMacAddress.Text;
                if (!int.TryParse(this.txtPort.Text, out int port) || port < 1 || port > 65535)
                    throw new ArgumentException("ポート番号の指定が正しくありません。");

                var manualAddresses = ParseBroadcastAddresses(this.txtBroadcastAddress.Text);
                var combinedAddresses = manualAddresses
                    .Concat(WakeOnLan.GetDefaultBroadcastAddresses())
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

                var results = WakeOnLan.SendMagicPacket(macAddress, combinedAddresses, port);
                int successCount = results.Count(r => r.Success);
                int totalCount = results.Count;

                this.txtLog.AppendText($"マジックパケット {macAddress} を {successCount}/{totalCount} 件の宛先へ送信しました。{Environment.NewLine}");

                foreach (var result in results)
                {
                    string status = result.Success ? "成功" : $"失敗: {result.Message}";
                    this.txtLog.AppendText($"  - {result.Address}: {status}{Environment.NewLine}");
                }
            }
            catch (Exception ex)
            {
                this.txtLog.AppendText($"エラー: {ex.Message}{Environment.NewLine}");
            }
        }

        private void btnAddDevice_Click(object sender, EventArgs e)
        {
            try
            {
                string name = this.txtDeviceName.Text.Trim();
                string mac = this.txtDeviceMac.Text.Trim();
                var addresses = ParseBroadcastAddresses(this.txtDeviceBroadcast.Text).ToList();

                if (string.IsNullOrWhiteSpace(name))
                    throw new ArgumentException("端末名を入力してください。");

                if (string.IsNullOrWhiteSpace(mac))
                    throw new ArgumentException("MACアドレスを入力してください。");

                var entry = new DeviceEntry(name, mac, addresses);

                int existingIndex = -1;
                for (int i = 0; i < this.devices.Count; i++)
                {
                    if (string.Equals(this.devices[i].Name, name, StringComparison.OrdinalIgnoreCase))
                    {
                        existingIndex = i;
                        break;
                    }
                }

                if (existingIndex >= 0)
                {
                    this.devices[existingIndex] = entry;
                }
                else
                {
                    this.devices.Add(entry);
                }

                this.lstDevices.SelectedItem = entry;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "端末登録エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void lstDevices_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.lstDevices.SelectedItem is DeviceEntry device)
            {
                this.txtMacAddress.Text = device.MacAddress;
                if (device.BroadcastAddresses.Count > 0)
                    this.txtBroadcastAddress.Text = string.Join(", ", device.BroadcastAddresses);
                else
                    this.txtBroadcastAddress.Clear();

                this.txtDeviceName.Text = device.Name;
                this.txtDeviceMac.Text = device.MacAddress;
                this.txtDeviceBroadcast.Text = string.Join(", ", device.BroadcastAddresses);
            }
        }

        private static IEnumerable<string> ParseBroadcastAddresses(string rawInput)
        {
            if (string.IsNullOrWhiteSpace(rawInput))
                return Enumerable.Empty<string>();

            char[] separators = new[] { ',', ';', ' ', '\t', '\r', '\n' };
            return rawInput
                .Split(separators, StringSplitOptions.RemoveEmptyEntries)
                .Select(address => address.Trim());
        }

        private void LoadDevicesFromFile()
        {
            try
            {
                if (!File.Exists(this.deviceStorePath))
                    return;

                using (FileStream stream = File.OpenRead(this.deviceStorePath))
                {
                    var serializer = new DataContractJsonSerializer(typeof(List<DeviceEntry>));
                    if (serializer.ReadObject(stream) is List<DeviceEntry> loaded)
                    {
                        foreach (var entry in loaded)
                        {
                            entry.Normalize();
                            this.devices.Add(entry);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"端末リストの読み込みに失敗しました: {ex.Message}", "読み込みエラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void SaveDevicesToFile()
        {
            try
            {
                foreach (var entry in this.devices)
                {
                    entry.Normalize();
                }

                var serializer = new DataContractJsonSerializer(typeof(List<DeviceEntry>));
                using (FileStream stream = File.Create(this.deviceStorePath))
                {
                    serializer.WriteObject(stream, this.devices.ToList());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"端末リストの保存に失敗しました: {ex.Message}", "保存エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveDevicesToFile();
        }

        [DataContract]
        private sealed class DeviceEntry
        {
            public DeviceEntry()
                : this(string.Empty, string.Empty, Enumerable.Empty<string>())
            {
            }

            public DeviceEntry(string name, string macAddress, IEnumerable<string> broadcastAddresses)
            {
                Name = name;
                MacAddress = macAddress;
                BroadcastAddresses = broadcastAddresses?.ToList() ?? new List<string>();
                Normalize();
            }

            [DataMember(Order = 0)]
            public string Name { get; set; }

            [DataMember(Order = 1)]
            public string MacAddress { get; set; }

            [DataMember(Order = 2)]
            public List<string> BroadcastAddresses { get; set; } = new List<string>();

            [IgnoreDataMember]
            public string DisplayName => $"{Name} ({MacAddress})";

            public void Normalize()
            {
                Name = Name?.Trim() ?? string.Empty;
                MacAddress = MacAddress?.Trim() ?? string.Empty;
                BroadcastAddresses = BroadcastAddresses?
                    .Where(a => !string.IsNullOrWhiteSpace(a))
                    .Select(a => a.Trim())
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList() ?? new List<string>();
            }
        }
    }
}

