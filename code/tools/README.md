# pytools 
##   _for ptotobuf & flatbuf_ 

####  介绍
###### 功能1：利用Python读取Excel并生成.proto文件，之后生成对应的C#以及Python代码。
###### 功能2：利用生成的Python代码读取Excel内数据并生成为一个被压缩的?.bin.bz2文件，供其余的C#工程使用（配合生成的C#代码）

#### 使用说明
 1. 请按模板*.xlsx文件进行设置。
 2. 目前支持数据类型仅为基本数据类型，复杂数据类型谨慎使用（代码熟悉后也是可以的 :smile: ）。

----
# 支持的数据类型对照表
数据类型|proto2对应类型|proto3对应类型
--|--|--
bool|optional bool|bool
bool[]|repeated bool|repeated bool
int|optional int32|int32
int[]|repeated int32|repeated int32
long|optional int64|int64
long[]|repeated int64|repeated int64
float|optional float|float
float[]|repeated float|repeated float
double|optional double|double
double[]|repeated double|repeated double
string|optional string|string
string[]|repeated string|repeated string

----
# 工具描述
性质|状态|语法|数据结构|描述|适用范围|生成代码|生成数据文件|引用文件
--|--|--|--|--|--|--|--|--
genJson.py|已完成|json|单表单结构，不统一|生成json|*.xlsx *.xls|*.cs|*.json|commonutils.py
genpb2.py|已完成|proto2|单表单结构，不统一|生成proto2的数据文件|*.xlsx *.xls|*.cs|*.bin *.bin.bz2|commonutils.py
genpb3.py|已完成|proto3|单表单结构，不统一|生成proto3的数据文件|*.xlsx *.xls|*.cs|*.bin *.bin.bz2|commonutils.py
genpb3_combine.py|已完成|proto3|多表单结构，统一|生成proto3的数据文件|*.xlsx *.xls|*.cs|*.bin *.bin.bz2|commonutils.py
genpb3_textlist.py|已完成|proto3|多表单结构，统一|生成proto3的数据文件|*.txt|*.cs|*.bin *.bin.bz2|commonutils.py
genfb.py|待完成|flat|单表单结构，不统一|通过生成json后转换到falt|*.xlsx *.xls|*.cs|*.bin|commonutils.py genJson.py