using System;
using System.Diagnostics;
using System.IO;
using Gtk;
using Newtonsoft.Json;

public class ImageDetectionApp
{
    private Window _window;
    private Image _imagePreview;
    private Button _selectButton;
    private string _pythonScriptPath = "detect.py"; // 确保Python脚本路径正确

    public ImageDetectionApp()
    {
        Application.Init();
        SetupUI();
        Application.Run();
    }

    private void SetupUI()
    {
        // 创建主窗口
        _window = new Window("目标检测系统");
        _window.SetDefaultSize(800, 600);
        _window.DeleteEvent += (sender, e) => Application.Quit();

        // 创建垂直布局容器
        var vbox = new VBox(false, 5);
        
        // 创建选择图片按钮
        _selectButton = new Button("选择图片");
        _selectButton.Clicked += OnSelectImageClicked;
        
        // 创建图片预览区域
        _imagePreview = new Image();
        
        // 添加控件到布局
        vbox.PackStart(_selectButton, false, false, 0);
        vbox.PackStart(_imagePreview, true, true, 0);
        
        // 添加布局到窗口
        _window.Add(vbox);
        _window.ShowAll();
    }

    private void OnSelectImageClicked(object sender, EventArgs e)
    {
        // 创建文件选择对话框
        var dialog = new FileChooserDialog(
            "选择图片文件",
            _window,
            FileChooserAction.Open,
            "取消", ResponseType.Cancel,
            "打开", ResponseType.Accept);
        
        // 设置图片过滤器
        var filter = new FileFilter();
        filter.AddPattern("*.jpg");
        filter.AddPattern("*.jpeg");
        filter.AddPattern("*.png");
        filter.Name = "图片文件";
        dialog.AddFilter(filter);

        if (dialog.Run() == (int)ResponseType.Accept)
        {
            string imagePath = dialog.Filename;
            ProcessImageDetection(imagePath);
        }
        
        dialog.Destroy();
    }

    private void ProcessImageDetection(string imagePath)
    {
        try
        {
            Console.WriteLine(imagePath);
            // 设置Python命令参数
            string arguments = $"--image \"{imagePath}\" --model best.pt --output output --conf 0.25";
            
            // 配置进程启动信息
            var startInfo = new ProcessStartInfo
            {
                FileName = "python",
                Arguments = $"{_pythonScriptPath} {arguments}",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };
            
            // 执行Python脚本
            using (var process = Process.Start(startInfo))
            {
                string output = process.StandardOutput.ReadToEnd();
                // Console.WriteLine(output);
                string error = process.StandardError.ReadToEnd();
                
                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    Console.WriteLine(error);
                    ShowError($"检测失败:\n{error}");
                    return;
                }

                // 解析JSON输出
                Console.WriteLine("success!!!!!!!!!!");
                string cleaned_output=CleanJson(output);
                var result = JsonConvert.DeserializeObject<DetectionResult>(cleaned_output);
                
                // 显示结果图片
                if (File.Exists(result.output_image))
                {
                    _imagePreview.Pixbuf = new Gdk.Pixbuf(result.output_image);
                }
                else
                {
                    ShowError($"结果图片不存在: {result.output_image}");
                }
            }
        }
        catch (Exception ex)
        {
            ShowError($"处理过程中出错: {ex.Message}");
        }
    }

    private void ShowError(string message)
    {
        var dialog = new MessageDialog(
            _window,
            DialogFlags.Modal,
            MessageType.Error,
            ButtonsType.Ok,
            message);
        
        dialog.Run();
        dialog.Destroy();
    }

    public static void Main()
    {
        new ImageDetectionApp();
    }
    string CleanJson(string json)
{
    // 移除不可见字符
    json = new string(json.Where(c => !char.IsControl(c)).ToArray());
    
    // 查找第一个有效字符位置
    int startIndex = json.IndexOf('{');
    if (startIndex < 0) startIndex = json.IndexOf('[');
    
    // 查找最后一个有效字符位置
    int endIndex = json.LastIndexOf('}');
    if (endIndex < 0) endIndex = json.LastIndexOf(']');
    
    if (startIndex >= 0 && endIndex > startIndex)
    {
        return json.Substring(startIndex, endIndex - startIndex + 1);
    }
    
    // 备选方案：尝试移除已知前缀
    string[] prefixes = { "info:", "result:", "image:", "output:" };
    foreach (var prefix in prefixes)
    {
        if (json.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
        {
            return json.Substring(prefix.Length).Trim();
        }
    }
    
    return json.Trim();
}
}

// JSON结果解析类
public class DetectionResult
{
    public string output_image { get; set; }
    public Detection[] detections { get; set; }
}

public class Detection
{
    public string @class { get; set; }
    public float confidence { get; set; }
    public float[] bbox { get; set; }
}