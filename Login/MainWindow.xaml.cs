using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Net.Sockets;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml.Media;
using WinUIEx;
// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Login;
/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow : WindowEx
{
    public MainWindow()
    {
        InitializeComponent();
        ExtendsContentIntoTitleBar = true;
        SystemBackdrop = new DesktopAcrylicBackdrop();
    }

    private async void LoginButton_Click(object sender, RoutedEventArgs e)
    {
        var username = UsernameTextBox.Text;
        var password = PasswordBox.Password;
        if (username == null || password == null)
        {
            var failureDialog = new ContentDialog
            {
                Title = "",
                Content = "�������û���������",
                CloseButtonText = "ȷ��",
                XamlRoot = Content.XamlRoot  // ���� XamlRoot
            };
            await failureDialog.ShowAsync(); // ����ʧ����ʾ
        }

        // ���͵�¼����
        var success = await SendLoginRequest(username, password);

        // ���ݵ�¼�����ʾ��ʾ
        if (success)
        {
            var successDialog = new ContentDialog
            {
                Title = "��¼�ɹ�",
                Content = "��ӭ " + username + "��",
                CloseButtonText = "ȷ��",
                XamlRoot = this.Content.XamlRoot  // ���� XamlRoot
            };
            await successDialog.ShowAsync(); // �����ɹ���ʾ

            // ���´���
            ((App)Application.Current).OpenNewWindow();
        }
        else
        {
            var failureDialog = new ContentDialog
            {
                Title = "��¼ʧ��",
                Content = "�û������������",
                CloseButtonText = "ȷ��",
                XamlRoot = this.Content.XamlRoot  // ���� XamlRoot
            };
            await failureDialog.ShowAsync(); // ����ʧ����ʾ
        }
    }

    private async Task<bool> SendLoginRequest(string username, string password)
    {
        try
        {
            // ����Socket����
            using var clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            // ���ӵ�������
            await clientSocket.ConnectAsync("127.0.0.1", 3594);

            // ����JSON����
            var loginData = new
            {
                username = username,
                password = password
            };

            // ���������л�ΪJSON
            var jsonData = JsonSerializer.Serialize(loginData);
            var dataToSend = Encoding.UTF8.GetBytes(jsonData);

            // �������ݵ�������
            await clientSocket.SendAsync(dataToSend, SocketFlags.None);

            // ���շ�������Ӧ
            var buffer = new byte[1024];
            var bytesReceived = await clientSocket.ReceiveAsync(buffer, SocketFlags.None);

            // ��ȡ��Ӧ���ݲ������л�
            var responseJson = Encoding.UTF8.GetString(buffer, 0, bytesReceived);
            var result = JsonSerializer.Deserialize<LoginResponse>(responseJson);

            // �жϵ�¼�Ƿ�ɹ�
            return result.status == "success";
        }
        catch (Exception ex)
        {
            // �쳣����
            ContentDialog errorDialog = new ContentDialog
            {
                Title = "����",
                Content = $"�޷����ӵ�������: {ex.Message}",
                CloseButtonText = "ȷ��",
                XamlRoot = Content.XamlRoot  // ���� XamlRoot
            };
            await errorDialog.ShowAsync();
            return false;
        }
    }
    // �����¼��Ӧ������ģ��
    public class LoginResponse
    {
        public string status
        {
            get; set;
        }
        public string message
        {
            get; set;
        }
    }
    // ע�ᰴť����¼�
    private async void RegisterButton_Click(object sender, RoutedEventArgs e)
    {
        // �����ﴦ��ע���߼�
        ContentDialog registerDialog = new ContentDialog
        {
            Title = "ע��",
            Content = "ע�Ṧ�ܼ������š�",
            CloseButtonText = "ȷ��",
            XamlRoot = Content.XamlRoot  // ���� XamlRoot
        };
        await registerDialog.ShowAsync();
    }

    // ���๦�ܰ�ť����¼�
    private async void MoreFeaturesButton_Click(object sender, RoutedEventArgs e)
    {
        // �����ﴦ����๦���߼�
        ContentDialog featuresDialog = new ContentDialog
        {
            Title = "���๦��",
            Content = "���๦�ܼ������š�",
            CloseButtonText = "ȷ��",
            XamlRoot = Content.XamlRoot  // ���� XamlRoot
        };
        await featuresDialog.ShowAsync();
    }
    // ������ Enter ��ʱ������¼
    private void PasswordBox_KeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            // ���õ�¼��ť����¼�
            LoginButton_Click(sender, e);
        }
    }
}
