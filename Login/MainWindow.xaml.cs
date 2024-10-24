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
                Content = "请输入用户名或密码",
                CloseButtonText = "确定",
                XamlRoot = Content.XamlRoot  // 设置 XamlRoot
            };
            await failureDialog.ShowAsync(); // 弹出失败提示
        }

        // 发送登录请求
        var success = await SendLoginRequest(username, password);

        // 根据登录结果显示提示
        if (success)
        {
            var successDialog = new ContentDialog
            {
                Title = "登录成功",
                Content = "欢迎 " + username + "！",
                CloseButtonText = "确定",
                XamlRoot = this.Content.XamlRoot  // 设置 XamlRoot
            };
            await successDialog.ShowAsync(); // 弹出成功提示

            // 打开新窗口
            ((App)Application.Current).OpenNewWindow();
        }
        else
        {
            var failureDialog = new ContentDialog
            {
                Title = "登录失败",
                Content = "用户名或密码错误",
                CloseButtonText = "确定",
                XamlRoot = this.Content.XamlRoot  // 设置 XamlRoot
            };
            await failureDialog.ShowAsync(); // 弹出失败提示
        }
    }

    private async Task<bool> SendLoginRequest(string username, string password)
    {
        try
        {
            // 创建Socket连接
            using var clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            // 连接到服务器
            await clientSocket.ConnectAsync("127.0.0.1", 3594);

            // 构建JSON数据
            var loginData = new
            {
                username = username,
                password = password
            };

            // 将数据序列化为JSON
            var jsonData = JsonSerializer.Serialize(loginData);
            var dataToSend = Encoding.UTF8.GetBytes(jsonData);

            // 发送数据到服务器
            await clientSocket.SendAsync(dataToSend, SocketFlags.None);

            // 接收服务器响应
            var buffer = new byte[1024];
            var bytesReceived = await clientSocket.ReceiveAsync(buffer, SocketFlags.None);

            // 获取响应内容并反序列化
            var responseJson = Encoding.UTF8.GetString(buffer, 0, bytesReceived);
            var result = JsonSerializer.Deserialize<LoginResponse>(responseJson);

            // 判断登录是否成功
            return result.status == "success";
        }
        catch (Exception ex)
        {
            // 异常处理
            ContentDialog errorDialog = new ContentDialog
            {
                Title = "错误",
                Content = $"无法连接到服务器: {ex.Message}",
                CloseButtonText = "确定",
                XamlRoot = Content.XamlRoot  // 设置 XamlRoot
            };
            await errorDialog.ShowAsync();
            return false;
        }
    }
    // 定义登录响应的数据模型
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
    // 注册按钮点击事件
    private async void RegisterButton_Click(object sender, RoutedEventArgs e)
    {
        // 在这里处理注册逻辑
        ContentDialog registerDialog = new ContentDialog
        {
            Title = "注册",
            Content = "注册功能即将开放。",
            CloseButtonText = "确定",
            XamlRoot = Content.XamlRoot  // 设置 XamlRoot
        };
        await registerDialog.ShowAsync();
    }

    // 更多功能按钮点击事件
    private async void MoreFeaturesButton_Click(object sender, RoutedEventArgs e)
    {
        // 在这里处理更多功能逻辑
        ContentDialog featuresDialog = new ContentDialog
        {
            Title = "更多功能",
            Content = "更多功能即将开放。",
            CloseButtonText = "确定",
            XamlRoot = Content.XamlRoot  // 设置 XamlRoot
        };
        await featuresDialog.ShowAsync();
    }
    // 处理按下 Enter 键时触发登录
    private void PasswordBox_KeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            // 调用登录按钮点击事件
            LoginButton_Click(sender, e);
        }
    }
}
