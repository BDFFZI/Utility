using System.IO.Compression;

ConsoleFrame.Serve(
    "将 {目标文件夹} 中的所有文件夹挨个压缩为zip文件，不会递归。",
    () =>
    {
        if (args.Length < 1 || Directory.Exists(args[0]) == false)
            throw new Exception("目标文件夹不存在");

        DirectoryInfo directoryInfo = new DirectoryInfo(args[0]);
        foreach (DirectoryInfo directory in directoryInfo.GetDirectories())
        {
            ZipFile.CreateFromDirectory(directory.FullName, directory.FullName + ".zip", default, true);
            Console.WriteLine("已压缩:" + directory.Name);
        }

        return Task.CompletedTask;
    }
);