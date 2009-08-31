using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;

namespace ArduinoBuildLED
{
    public class Arduino : IDisposable
    {
        public enum Color
        {
            Red,
            Green,
            Blue,
        }

        private SerialPort _serialPort;

        public Arduino(string portName, string baudRate)
        {
            _serialPort = new SerialPort();
            _serialPort.PortName = "COM3";
            _serialPort.BaudRate = 9600;

            if (!_serialPort.IsOpen)
            {
                _serialPort.Open();
            }
        }

        public bool IsConnected
        {
            get { return this._serialPort.IsOpen; }
        }

        public void ChangeColor(int red, int green, int blue)
        {
            this.SendComPortValue((byte)red, (byte)green, (byte)blue);
        }

        /// <summary>
        /// Sends the value to the COM port.
        /// </summary>
        /// <param name="red"></param>
        /// <param name="green"></param>
        /// <param name="blue"></param>
        private void SendComPortValue(byte red, byte green, byte blue)
        {
            byte[] buffer = new byte[] { red, green, blue };

            _serialPort.Write(buffer, 0, 3);
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (_serialPort.IsOpen)
            {
                _serialPort.Close();
            }
        }

        #endregion
    }
}
