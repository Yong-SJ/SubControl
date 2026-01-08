using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using EasyModbus;

namespace SubControl.Barrel
{
    public class BarrelControl
    {
        private ModbusClient modbusClient;

        public BarrelControl(string ipAddress = "192.168.1.111", int port = 502)
        {
            modbusClient = new ModbusClient(ipAddress, port);
        }

        public bool Connect()
        {
            try
            {
                if (modbusClient.Connected)
                {
                    modbusClient.Disconnect();
                    MessageBox.Show("기존 연결이 있어 해제했습니다.");
                }

                modbusClient.ConnectionTimeout = 3000; // 3초 타임아웃 설정
                modbusClient.Connect(); // 여기서 타임아웃 발생 가능

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Connect error: " + ex.Message + "\n" + ex.StackTrace);
                return false;
            }
        }

        public void Disconnect()
        {
            try
            {
                if (modbusClient.Connected)
                    modbusClient.Disconnect();
            }
            catch
            {
                // 필요 시 로그
            }
        }

        public int[] ReadRegisters(int startAddress, int length)
        {
            if (!modbusClient.Connected)
                throw new InvalidOperationException("Modbus에 연결되어 있지 않습니다.");

            return modbusClient.ReadHoldingRegisters(startAddress, length);
        }

        public bool ReadBitFromCoil(int coilAddress)
        {
            if (!modbusClient.Connected)
                throw new InvalidOperationException("Modbus에 연결되어 있지 않습니다.");

            bool[] coil = modbusClient.ReadCoils(coilAddress, 1);
            return coil[0];
        }


        public void WriteRegister(int address, int value)
        {
            if (!modbusClient.Connected)
                throw new InvalidOperationException("Modbus에 연결되어 있지 않습니다.");

            modbusClient.WriteSingleRegister(address, value);
        }

        public void WriteBitToCoil(int coilAddress, bool value)
        {
            if (!modbusClient.Connected)
                throw new InvalidOperationException("Modbus에 연결되어 있지 않습니다.");

            modbusClient.WriteSingleCoil(coilAddress, value);
        }
        

    }
}
