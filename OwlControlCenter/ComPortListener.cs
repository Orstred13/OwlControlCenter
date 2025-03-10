using System;
using System.IO.Ports;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OwlControlCenter;

public class ComPortListener : IDisposable {
    private SerialPort serialPort;
    private CancellationTokenSource cancellationTokenSource;
    private bool isListening;
    private bool isFirstRead = true;

    public event Action<string> DataReceived;

    public ComPortListener(string portName, int baudRate = 9600, Parity parity = Parity.None, int dataBits = 8,
        StopBits stopBits = StopBits.One) {
        serialPort = new SerialPort(portName, baudRate, parity, dataBits, stopBits);
    }

    public async Task StartListeningAsync() {
        if (isListening) {
            return;
        }

        try {
            serialPort.Open();
            isListening = true;
            cancellationTokenSource = new CancellationTokenSource();

            await ListenToPortAsync(cancellationTokenSource.Token);
        } catch (Exception ex) {
            MessageBox.Show($"Ошибка при открытии порта!", "Ошибка открытии порта",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    public void StopListening() {
        if (!isListening) {
            return;
        }

        isListening = false;
        cancellationTokenSource?.Cancel();
        serialPort.Close();
    }

    private async Task ListenToPortAsync(CancellationToken cancellationToken) {
        string message = string.Empty;

        while (isListening && !cancellationToken.IsCancellationRequested) {
            try {
                if (serialPort.BytesToRead > 0) {
                    var readByte = serialPort.BaseStream.ReadByte();
                    if (readByte != '\n') {
                        message += Encoding.ASCII.GetString(new[] { (byte)readByte });
                    } else {
                        if (isFirstRead) {
                            isFirstRead = false;
                            continue;
                        }
                            
                        OnDataReceived(message);
                    }
                } else {
                    await Task.Delay(100, cancellationToken);
                }
            } catch (OperationCanceledException) {
                break;
            } catch (Exception ex) {
                Console.WriteLine($"Ошибка при чтении данных: {ex.Message}");
            }
        }
    }

    protected virtual void OnDataReceived(string data) {
        DataReceived?.Invoke(data);
    }

    public void Dispose() {
        StopListening();
        serialPort.Dispose();
        cancellationTokenSource?.Dispose();
    }
}
