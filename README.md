# Andreal

基于ArcaeaUnlimitedApi和Konata的开源ArcaeaBot。

> 遇到问题看文末.jpg

----

#### 感谢

> 本项目的Arcaea数据来源于ArcaeaUnlimitedApi。
>
> 本项目的图片UI设计来源于 GNAQ、linwenxuan04、雨笙Fracture (按首字母排序)。
>
> 感谢所有赞助者的支持。

#### 数据共享协议

> 使用Andreal服务将默认您允许Andreal收集/记录关于您的使用记录，包括且不限于Arcaea用户名、游玩记录等；
>
> **我们将与其他ArcaeaBot共享我们从您那里收集的Arcaea用户名**。

#### 用户须知

> 您应知悉，使用本项目将违反 [Arcaea使用条款](https://arcaea.lowiro.com/zh/terms_of_service) 中的 3.2-4 和 3.2-6，以及 [Arcaea二次创作管理条例](https://arcaea.lowiro.com/zh/derivative_policy) 。
>
> 因使用本项目而造成的损失，Andreal开发组不予承担任何责任。

----

#### 部署方法

* 向Awbugl申请 ArcaeaUnlimitedApi Token（申请方法详见*常见问题*）

* [下载最新版本Andreal](https://github.com/Awbugl/Andreal/releases/)

* 安装 Fonts 目录下的字体文件

* 运行 Andreal.Window.exe

* 填写基础配置页面的配置项并保存

* 账号管理界面右键添加新帐号

* 若一切顺利，此时您的Bot就已可用。


注：ArcaeaUnlimitedApi: 由TheSnowfield与Awbugl基于BotArcAPI开发的项目。详情请咨询Awbugl。

----

#### 配置文件介绍

**请勿自行操作、删除、泄露给第三方**

###### 文件目录:

> ..\Andreal\Andreal\Config\

**config.json**

* Andreal主要配置文件

**positioninfo.json**

* Arcaea图查立绘位置配置文件

**replytemplate.json**

* Bot回复模板

**Andreal.db**

* 用户绑定信息数据库

**BotInfo/**

* Bot登录信息文件夹

###### Bot迁移时，只迁移配置文件目录下的文件即可。

----

#### 常见问题

Q: **使用Andreal有何限制？**

> A: 除了违法行为与商业盈利行为以外，您可以任意应用这份开源项目。

----
Q: **用户信息在哪里存储？**

> A: Andreal使用本地sqlite存储用户数据。由于**数据共享协议**，我们将共享用户的Arcaea用户名、游玩记录等，请知悉。

----
Q: **想要自己部署Andreal需要具备什么知识？部署在哪里？**

> A: 按照教程/指北逐步操作即可；可部署在您的电脑（不建议），或您自行租赁的云服务器上。

----
Q:  **如何申请ArcaeaUnlimitedApi Token？**

> A:
> 请在QQ/Discord上私聊联系Awbugl（或加入QQ群 191234485），并提供Token预计用途、预计每日峰值调用次数、(如果是Bot)Bot代称及依赖的框架；

> Token申请一般会在5个工作日内回复。友善交流（可能）会提升处理效率；Token申请成功前请不要退群，以免无法联系您。

> 使用Andreal搭建Bot请在Token申请成功后加入 IOLab QA Center（574250621），Andreal技术问题请在QA群提问。

----
Q:  **Bot都有哪些指令可用？**

> A:  请查看Wiki：https://www.showdoc.com.cn/andrea

----
Q:  **Bot是怎么实现Arcaea查分的？**

> A:  请查看专栏：https://www.bilibili.com/read/cv15871643

----
Q:  **为什么部署之后Bot无法查分？**

> A:  请检查
>
> 配置项配置是否正确
>
> Andreal是否为最新版本
>
> ApiToken是否申请并正确填写

----
Q:  **无法访问Github下载文件？**

> A:  可加入QQ群 191234485 ，提供文件副本下载。

----

#### 赞助

###### 赞助所得将全部用于API的服务器维护。

> [爱发电](https://afdian.net/a/Awbugl)
