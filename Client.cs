using System;
using System.IO;
using System.IO.Pipes;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DubnaClient
{
    public partial class Client : Form
    {
        private StreamWriter writer;
        private StreamReader reader;
        private NamedPipeClientStream client;

        public Client()
        {
            InitializeComponent();
            ConnectToServer();
        }

        private async void buttonSend_Click(object sender, EventArgs e)
        {
            string message = messageBox.Text;
            // Сохраняем и отправляем сообщение на сервер, которое ввел пользователь
            await writer.WriteLineAsync(message);
            await writer.FlushAsync();
            messageBox.Clear();
        }

        // Функция присоединения к серверу
        private async void ConnectToServer()
        {
            client = new NamedPipeClientStream(".", "Dubna", PipeDirection.InOut, PipeOptions.Asynchronous);
            await client.ConnectAsync();

            reader = new StreamReader(client);
            writer = new StreamWriter(client);

            await ReadMessages();
        }

        // Функция, которая считывает сообщения со стрима и добавляет их в чат
        private async Task ReadMessages()
        {
            while (true)
            {
                try
                {
                    string message = await reader.ReadLineAsync();

                    if (!string.IsNullOrEmpty(message))
                    {
                        Invoke((MethodInvoker)(() => chatBox.Items.Add(message)));
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}

