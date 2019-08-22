# tools 

####  介绍
 C# dll库，方便自己用于C#工程或unity工程

#### 使用说明
 1. 请用vs2017版本打开并编译

----
# 工具描述
工程名称|性质|目的|描述|包含工程
--|--|--|--|--
core|共享代码|被库工程使用|/|/
core-excel|共享代码|被库工程使用|/|/
core-version|共享代码|被库工程使用|用于控制并统一库工程版本号|/
library|库工程|被其余工程使用|基本常用代码集|core-version core
library-excel|库工程|被其余工程使用|基本常用代码集（包含Excel文件处理）|core-version core  core-excel
library-u|库工程|被其余工程使用|unity运行时基本常用代码集|core-version core
library-ue|库工程|被其余工程使用|unity编辑器基本常用代码集（包含Excel文件处理）|core-version core-excel
excel|控制台工程|工具|Excel文件的各种处理|library-excel
test|控制台工程|工具||library-excel
search|控制台工程|工具||library-excel