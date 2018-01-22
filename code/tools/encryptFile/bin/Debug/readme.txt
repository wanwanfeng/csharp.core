1.此工具用于Excel文件导出为xml、json、csv、txt、bytes等文件。
2.Excel文件定义方式：第一行为描述，第二行为字段类型，第三行为字段名称。字段类型或字段名称任一以“#”符号开头时此列忽略。
3.所支持的字段类型为：bool、int、float、double、long、decimal、byte、short、char、string以及对应的数组类型和泛型列表，类型为数组或泛型列表时数据以“|”进行分割。