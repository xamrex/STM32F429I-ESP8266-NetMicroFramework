using System;
using System.IO.Ports;
using System.Text;
using Microsoft.SPOT;
using System.Threading;
using STM32F429I_Discovery.Netmf.Hardware;

namespace TouchScreenExample
{
    public class SerialPortHelper
    {
        static SerialPort serialPort;

        const int bufferMax = 2048;
        static byte[] buffer = new Byte[bufferMax];
        static int bufferLength = 0;
        public int linia = 0;

        public SerialPortHelper(string portName = SerialPorts.SerialCOM2, int baudRate = 9600, Parity parity = Parity.None, int dataBits = 8, StopBits stopBits = StopBits.One)
        {
            serialPort = new SerialPort(portName, baudRate, parity, dataBits, stopBits);
            serialPort.ReadTimeout = 10; 
            serialPort.DataReceived += new SerialDataReceivedEventHandler(serialPort_DataReceived);
            serialPort.Open();
        }

        private void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            lock (buffer)
            {
                int bytesReceived = serialPort.Read(buffer, bufferLength, bufferMax - bufferLength);
                if (bytesReceived > 0)
                {
                    bufferLength += bytesReceived;
                    if (bufferLength >= bufferMax)
                        throw new ApplicationException("Buffer Overflow.  Send shorter lines, or increase lineBufferMax.");
                }
            }

        }
        public string ReadAllBuf()
        {
            string line = "";
            lock (buffer)
            {
                for (int i = 0; i < bufferLength; i++)
                {
                    line = "" + new string(Encoding.UTF8.GetChars(buffer)); // The "" ensures that if we end up copying zero characters, we'd end up with blank string instead of null string.
                          
                }

            }
            return line;
        }

        public string ReadLine()
        {
            string line = "";
            lock (buffer)
            {
          
                for (int i = 0; i < bufferLength; i++)
                {

                    if (buffer[i] == '\r')
                    {
          
                        buffer[i] = 0; // Turn NewLine into string terminator
                        buffer[i+1] = 0; // Turn RN  into string terminator
                        line = "" + new string(Encoding.UTF8.GetChars(buffer)); // The "" ensures that if we end up copying zero characters, we'd end up with blank string instead of null string.
                                               
                        //bufferLength = bufferLength - i - 1;
                        bufferLength = bufferLength - i - 2; //usuwa \r i \n
                        Array.Copy(buffer, i + 2, buffer, 0, bufferLength); // Shift everything past NewLine to beginning of buffer


                        if (line!="")
                        {
                            Debug.Print(buffer.ToString());
                            linia++; //dodane na potrzeby debuga
                            if (linia >= 21) linia = 1;//dodane na potrzeby debuga
                        }
                        break;
                    }
                }
            }

            return line;
        }

        public void Print(string line)
        {
            System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
            byte[] bytesToSend = encoder.GetBytes(line);
            serialPort.Write(bytesToSend, 0, bytesToSend.Length);
        }

        public void PrintLine(string line)
        {
            Print(line + "\r\n");
        }


        public void ClearAllBufor()
        {
            Array.Clear(buffer, 0, bufferMax);
            bufferLength = 0;
        }



    }
}
