// See https://aka.ms/new-console-template for more information

using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.Versioning;

[assembly: SupportedOSPlatform("windows6.1")]

ImageCodecInfo? GetEncoder(ImageFormat format)
{
    ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
    return codecs.FirstOrDefault(codec => format.Guid == codec.FormatID);
}


ConsoleFrame.Serve(
    "将 {目标文件夹} 中的png后缀图片另存为jpg图片，不会递归。",
    () =>
    {
        if (args.Length < 1 || Directory.Exists(args[0]) == false)
            throw new Exception("目标文件夹不存在");

        //获取编码器并配置参数
        EncoderParameters encoderParameters = new EncoderParameters(1);
        encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, 90L);
        ImageCodecInfo? jpgEncoder = GetEncoder(ImageFormat.Jpeg);
        if (jpgEncoder == null)
            throw new Exception("无法获取到JPEG编码器");

        DirectoryInfo directoryInfo = new DirectoryInfo(args[0]);
        foreach (FileInfo file in directoryInfo.GetFiles("*.png"))
        {
            Bitmap bitmap = new Bitmap(file.FullName);
            bitmap.Save(Path.GetFileNameWithoutExtension(file.FullName) + ".jpg", jpgEncoder, encoderParameters);
            bitmap.Dispose();
        }
        
        return Task.CompletedTask;
    }
);