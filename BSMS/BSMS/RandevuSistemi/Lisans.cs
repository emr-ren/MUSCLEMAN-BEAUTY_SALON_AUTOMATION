using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BEAUTYLIFE
{
    internal class Lisans
    {
        public static void MUSCLEMAN_Lisanslama()
        {
            DateTime expirationDate = new DateTime(2026, 5, 17); // YYYY, MM, DD formatında son kullanma tarihi
            DateTime currentDate;

            try
            {
                currentDate = Lisans.GetNetworkTime();
            }
            catch (Exception)
            {
                MessageBox.Show("Zaman sunucusuna bağlanılamadı: İnternetinizi kontrol edin.", "Bağlantı Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Windows.Forms.Application.Exit(); // Uygulamayı kapat
                return; // Çıkış yapılacağı için return ekliyoruz
            }

            if (currentDate > expirationDate)
            {
                MessageBox.Show("Bu yazılımın kullanım süresi sona ermiştir veya bilgisayar tarihi değiştirilmeye çalışılmıştır. Lütfen yazılımın güncel bir versiyonunu edininiz.", "ldu", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                System.Windows.Forms.Application.Exit(); // Uygulamayı kapat
            }
        }

        public static DateTime GetNetworkTime()
        {
            const string ntpServer = "time.windows.com";
            var ntpData = new byte[48];
            ntpData[0] = 0x1B;

            var addresses = Dns.GetHostEntry(ntpServer).AddressList;
            var ipEndPoint = new IPEndPoint(addresses[0], 123);
            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
            {
                socket.Connect(ipEndPoint);
                socket.Send(ntpData);
                socket.Receive(ntpData);
                socket.Close();
            }

            ulong intPart = BitConverter.ToUInt32(ntpData, 40);
            ulong fractPart = BitConverter.ToUInt32(ntpData, 44);
            intPart = Lisans.SwapEndianness(intPart);
            fractPart = Lisans.SwapEndianness(fractPart);

            var milliseconds = (intPart * 1000) + ((fractPart * 1000) / 0x100000000L);
            var networkDateTime = (new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddMilliseconds((long)milliseconds);

            return networkDateTime.ToLocalTime();
        }

        static uint SwapEndianness(ulong x)
        {
            return (uint)(((x & 0x000000ff) << 24) + ((x & 0x0000ff00) << 8) + ((x & 0x00ff0000) >> 8) + ((x & 0xff000000) >> 24));
        }
    }
}

