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

图形处理库基于[SixLabors.ImageSharp](https://github.com/SixLabors/ImageSharp)中的代码和Sample。
## Command命令
### Resize
```json
  {
    "command": "resize",
    "width": 400,
	"height":200,
    "output": {
      "format": "jpg",
      "quality": 90
    }
  }
```
通过指定width和height缩放到指定像素大小。当height不指定时根据width的比例自动缩放。
### crop
```json
  {
    "command": "crop",
    "x": 100,
    "y": 100,
    "height": 100,
    "width": 400
  }
 ```
 通过x,y值指定剪裁的起始位置。width和height指定剪裁的大小
 ### round
 ```json
  {
    "command": "round",
    "height": 100,
    "width": 400,
    "radius": 15,
    "output": {
      "format": "gif"
    }
  }
 ```
 通过指定width和height来缩放图片，并且指定radius来指定圆角的半径
 
 ### watermark
 ```json
 {
	"command":"watermark",
	"watermark_url":"http://www.sample.com/watermark.png",
	"pos":0,
	"opacity":100
}
 ```
 给图片打水印
 1.watermark_url :水印图片的地址
 2.pos:水印图片的位置(0:左上，1：右上，2：左下，3：右下,4:中间)
 3.opacity:水印图片的不透明度（0-100),0是完全透明，100是完全不透明。
 ### combo
 组合命令，通过commands中的command来组合处理图片，譬如是先剪裁再缩放再加圆角操作
 ```json
  {
    "command": "combo",
    "commands": [
      {
        "command": "crop",
        "x": 100,
        "y": 100,
        "height": 100,
        "width": 400
      },
      {
        "command": "round",
        "height": 100,
        "width": 400,
        "radius": 15,
      
      }
    ],
	  "output": {
          "format": "gif"
        }
  }
 ```
 ### Command中的output设置
 ```json
   "output": {
          "format": "jpg",
		  "quality":90
        }
```
format参数指定输出图片的格式，现在支持gif,png,jpg这几种格式。因为圆角是通过圆角以外的部分透明来实现的，所以当使用圆角命令后需要指定格式为gif或者Png才有效果。
quality支持jpeg的压缩，数值范围是1到100，意义为图像质量的百分比
当输出格式为png是quality的数值表示图像的压缩比，数值范围是1到6
combo命令中的每个子command不需要指定output.output设置在子命令中无效。