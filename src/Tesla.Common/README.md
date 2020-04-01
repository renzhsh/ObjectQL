> 2016.11.21
增加Ilog,依赖log4net,需按下面的要求配置
在app.config中增加以下内容
```c#
  <configSections>
    <section name="log4net" type="log4net.Config.Log4NetConfigurationSectionHandler, log4net"/>
  </configSections>
  <log4net configSource="config\log4net.config"/>
```
然后在程序目录中增加log4net.config文件，并进行配置