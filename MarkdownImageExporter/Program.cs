// See https://aka.ms/new-console-template for more information

using System.Text.RegularExpressions;

Regex urlRegex = new Regex(@"!\[.*?\]\(<?(.*?)>?\)");
Regex nameRegex = new Regex(@"/([^/]+)$");
HttpClient httpClient = new HttpClient();
Random random = new Random();

ConsoleFrame.Serve(
    "将 {目标文件夹} 及子目录的.md文件中的所有图片下载到指定导出目录，并自动根据相对位置替换markdown中的原链接。\n操作不可逆，注意备份！！！",
    async () =>
    {
        if (args.Length < 1 || Directory.Exists(args[0]) == false)
            throw new Exception("目标文件夹不存在！");

        Console.WriteLine("请输入图片导出目录：");
        string? path = Console.ReadLine();
        if (path == null || Directory.Exists(path) == false)
            throw new Exception("导出目录不存在！");

        async Task ProcessDirectory(DirectoryInfo directoryInfo)
        {
            foreach (FileInfo fileInfo in directoryInfo.GetFiles("*.md"))
                await ProcessMarkdown(fileInfo);

            foreach (DirectoryInfo subDirectoryInfo in directoryInfo.GetDirectories())
                await ProcessDirectory(subDirectoryInfo);
        }

        async Task ProcessMarkdown(FileInfo fileInfo)
        {
            string markdown = File.ReadAllText(fileInfo.FullName);
            MatchCollection matchCollection = urlRegex.Matches(markdown);

            foreach (Match match in matchCollection)
            {
                //获取图片网址
                string url = match.Groups[1].Value;
                GroupCollection groupCollection = nameRegex.Match(url).Groups;
                if (groupCollection.Count < 2)
                {
                    ConsoleFrame.WriteWarning($"跳过非http图片:<{url}>");
                    continue;
                }

                //获取图片名称，并计算保存位置
                string fileName = groupCollection[1].Value;
                string newPath = Path.Combine(path, fileName);
                if (File.Exists(newPath) == false)
                {
                    //下载图片并保存
                    byte[]? data = null;
                    try
                    {
                        HttpResponseMessage message = await httpClient.GetAsync(url);
                        data = await message.Content.ReadAsByteArrayAsync();
                    }
                    catch
                    {
                        // ignored
                    }

                    if (data == null)
                        throw new Exception($"下载图片失败:<{url}>");

                    File.WriteAllBytes(newPath, data);
                }
                else
                {
                    ConsoleFrame.WriteWarning($"存在已导出图片:<{url}>");
                }

                //设置markdown相对图片的路径
                string relativePath = Path.GetRelativePath(fileInfo.DirectoryName!, newPath);
                relativePath = relativePath.Replace("\\", "/");
                markdown = markdown.Replace(url, relativePath);

                Console.WriteLine($"图片导出成功:\n<{url}>\n->\n<{relativePath}>\n");
                await Task.Delay(random.Next(300, 500)); //防止操作太频繁被封，我不确定这是否有用
            }

            File.WriteAllText(fileInfo.FullName, markdown);
        }

        await ProcessDirectory(new DirectoryInfo(args[0]));
    });