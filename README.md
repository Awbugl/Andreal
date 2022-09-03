# Andreal

基于 ArcaeaUnlimitedAPI 和 Konata 的开源 ArcaeaBot。

----

#### 用户须知

> 您应知悉，使用本项目将违反 [Arcaea 使用条款](https://arcaea.lowiro.com/zh/terms_of_service) 中的 3.2-4 和 3.2-6，以及 [Arcaea 二次创作管理条例](https://arcaea.lowiro.com/zh/derivative_policy) 。
>
> 因使用本项目而造成的损失，Andreal 开发组不予承担任何责任。

----

#### 部署步骤

1.  申请 `ArcaeaUnlimitedApi Token`（详见常见问题）

2.  [下载最新版本 Andreal](https://github.com/Awbugl/Andreal/releases/)

3.  安装 Fonts 目录下的字体文件

4.  运行 Andreal.Window.exe

5.  填写基础配置页面的配置项并保存

6.  账号管理界面右键添加新帐号

* 若一切顺利，此时您的Bot就已可用。

注：ArcaeaUnlimitedApi: 由 TheSnowfield 与 Awbugl 基于 BotArcAPI 开发的第三方查分API。

----

#### 配置文件夹 `..\Andreal\Andreal\Config\`

>  请勿自行操作、删除、泄露给第三方
> 
>  Bot迁移时，只迁移配置文件夹即可。

| 文件（夹）                | 说明               |
|:---------------------|:-----------------|
| config.json          | Andreal主要配置文件    |
| positioninfo.json    | Arcaea图查立绘位置配置文件 |
| replytemplate.json   | Bot回复模板          |
| Andreal.db           | 用户绑定信息数据库        |
| BotInfo/             | Bot登录信息文件夹       |

----

#### 数据共享协议

> 使用 ArcaeaUnlimitedAPI 服务将默认您允许 ArcaeaUnlimitedAPI 收集/记录关于您的使用记录，包括且不限于 Arcaea 用户名、游玩记录等；
>
> **我们将与其他 ArcaeaBot 共享我们从您那里收集的 Arcaea 用户名**。

----

#### 常见问题


**1. 如何申请ArcaeaUnlimitedApi Token？**

> 请在 QQ / Discord 私聊联系 Awbugl（或加入 IO鸽子窝 `191234485`），并提供 用途、预计每日峰值调用次数、(如果是Bot)Bot代称及依赖的框架；
>
> Token申请一般会在5个工作日内回复。友善交流（可能）会提升处理效率；Token申请成功前请不要退群，以免无法联系您。
>
> 使用Andreal搭建Bot请在Token申请成功后加入 IOLab QA Center `574250621`，Andreal技术问题请在QA群提问。

----
**2. Bot都有哪些指令可用？**

>  请查看 [Wiki](https://www.showdoc.com.cn/andrea)

----
**3. 无法访问Github下载文件？**

>  IO鸽子窝 `191234485` 提供文件副本下载。

----
**4. 用户信息在哪里存储？**

>  Andreal 使用本地 sqlite 存储用户数据。由于**数据共享协议**，我们将共享用户的 Arcaea 用户名、游玩记录等，请知悉。

----
**5. Bot是怎么实现Arcaea查分的？**

>  请查看 [相关专栏](https://www.bilibili.com/read/cv15871643)

----
**6. 为什么部署之后Bot无法查分？**

>  请检查配置项是否正确填写；Andreal 是否为最新版本

----
**7. 使用 Andreal 有何限制？**

>  除了违法行为与商业盈利行为以外，您可以任意应用这份开源项目。

----

#### 赞助

##### 赞助所得将全部用于API的服务器维护。

> [爱发电](https://afdian.net/a/Awbugl)

----

#### 感谢

> 本项目的 Arcaea 数据来源于 ArcaeaUnlimitedApi。
>
> 本项目的图片UI设计来源于 GNAQ、linwenxuan04、雨笙Fracture (按首字母排序)。
>
> 感谢所有赞助者的支持。
