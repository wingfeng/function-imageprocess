# Azure Function图像处理

这个Sample是基于[Azure  Sample Image Upload Resize](https://github.com/Azure-Samples/function-image-upload-resize)改进而来。处理一些常见的图片要求：缩放、剪切、圆角等

## 本地设置

在本地运行前将local.settings.sample.json修改为local.settings.json并且修改存储账号连接串`AzureWebJobsStorage`中的值，或者将下面的json字串拷贝到local.settings.json文件中.

```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet",
    "SRC_CONTAINER_NAME": "images",
    "OUTPUT_CONTAINER_NAME": "outputs",   
    "datatype": "binary"
  }
}
```

要使用这些图形处理功能，
请将sample.json文件改名了commands.json文件，并且将commands.json文件上传到待处理的图片相同的Storage container里。然后上传需要处理的图片。Azure Function自动将这些图片根据commands.json中的命令将图片进行处理并且输出到OUTPUT_CONTAINER_NAME参数中指定的Container中。

图形处理库基于[SixLabors.ImageSharp](https://github.com/SixLabors/ImageSharp)中的代码和Sample