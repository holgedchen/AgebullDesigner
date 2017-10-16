using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using Agebull.EntityModel.Config;
using Agebull.EntityModel.RobotCoder;

namespace Agebull.EntityModel.Designer.WebApi
{


    public sealed class ApiMarkBuilder : EntityCoderBase
    {

        #region 主体代码
        /// <summary>
        /// 是否可写
        /// </summary>
        protected override bool CanWrite => true;

        /// <summary>
        /// 名称
        /// </summary>
        protected override string FileSaveConfigName => "File_API_Description_md";
        /// <summary>
        /// 是否客户端代码
        /// </summary>
        protected override bool IsClient => true;

        /// <summary>
        ///     生成实体代码
        /// </summary>
        protected override void CreateBaCode(string path)
        {
            string file = Path.Combine(path, "project_api.md");
            StringBuilder code = new StringBuilder();
            code.Append($@"## Best practice
## 版本信息：
   V 1.0
   
## 开发人员：

| 名称   |    Email   |    手机    |      说明      |
|--------|:----------:|:----------:|:---------------|");
            code.Append($@"

# {Project.Caption} 项目 API 列表：");
            foreach (var item in Project.ApiItems)
            {
                code.Append($@"
            *{item.Method}[{item.RoutePath}){item.Name}({item.Argument?.Name}) - {item.Caption}");
            }

            foreach (var item in Project.ApiItems)
            {
                ItemReadMe(code, item);
            }


            SaveCode(file, code.ToString());
        }

        /// <summary>
        ///     生成扩展代码
        /// </summary>
        protected override void CreateExCode(string path)
        {
            StringBuilder code = new StringBuilder();
            code.Append($@"## Best practice
## 版本信息：
   V 1.0
   
## 开发人员：

| 名称   |    Email   |    手机    |      说明      |
|--------|:----------:|:----------:|:---------------|");
            code.Append($@"

# {Project.Caption} 实体操作 API 列表：");
            foreach (var entity in Project.Entities)
            {
                code.Append($@"
    # {entity.Caption}
            *POST[entity/{entity.Name}/AddNew](#{entity.Name}) - 创建{entity.Caption}
            *POST[entity/{entity.Name}/Update](#{entity.Name}) - 更新{entity.Caption}
            *POST[entity/{entity.Name}/Delete](#Argument<int>) - 删除{entity.Caption}
            *POST[entity/{entity.Name}/Query](#PageArgument) - 分页{entity.Caption}");
            }

            foreach (var entity in Project.Entities)
            {
                ItemReadMe(code, new ApiItem
                {
                    RoutePath = $"entity/{entity.Name}/AddNew",
                    CallArg = entity.Name,
                    ResultArg = entity.Name,
                    Name = "AddNew",
                    Caption = $"创建{entity.Caption}"
                });
                ItemReadMe(code, new ApiItem
                {
                    RoutePath = $"entity/{entity.Name}/Update",
                    CallArg = entity.Name,
                    ResultArg = entity.Name,
                    Name = "Update",
                    Caption = $"更新{entity.Caption}"
                });
                ItemReadMe(code, new ApiItem
                {
                    RoutePath = $"entity/{entity.Name}/Delete",
                    Argument = new EntityConfig
                    {
                        Name = "Argument<int>",
                        Caption = "ID参数",
                        Properties = new ConfigCollection<PropertyConfig>
                        {
                            new PropertyConfig
                            {
                                Name = "Value",
                                Caption="值",
                                CsType = "int"
                            }
                        }
                    },
                    Name = "Delete",
                    Caption = $"删除{entity.Caption}"
                });
                ItemReadMe(code, new ApiItem
                {
                    RoutePath = $"entity/{entity.Name}/Delete",
                    Argument = new EntityConfig
                    {
                        Name = "PageArgument",
                        Caption = "分页参数",
                        Properties = new ConfigCollection<PropertyConfig>
                        {
                            new PropertyConfig
                            {
                                Name = "Page",
                                Caption="页号",
                                Description= "从0开始的页号",
                                CsType = "int"
                            },
                            new PropertyConfig
                            {
                                Name = "PageSize",
                                Caption="每页行数",
                                CsType = "int"
                            },
                            new PropertyConfig
                            {
                                Name = "Order",
                                Caption="排序字段",
                                CsType = "string",
                                CanEmpty = true
                            },
                            new PropertyConfig
                            {
                                Name = "Desc",
                                Caption="是否反序排列",
                                CsType = "bool",
                                CanEmpty = true
                            },
                        }
                    },
                    Name = "Delete",
                    Caption = $"分页{entity.Caption}"
                });
            }

            string file = Path.Combine(path, "entity_api.md");
            SaveCode(file, code.ToString());
        }


        private static void ItemReadMe(StringBuilder code, ApiItem item)
        {
            code.Append($@"
<a id=""{item.Name}""></a>
## {item.Name}

#### 功能
{item.Caption}
{item.Description}
### url
**{item.RoutePath}**

### 请求方法
**{item.Method}**

#### 身份验证
**HeaderParams**
*[Authorization: Bearer #AccessToken/*DeviceId]*

### 请求参数");
            if (item.Argument == null)
            {
                code.Append(@"无");
            }
            else
            {
                code.Append($@"
    参数类型：{item.Name}({item.Argument.Caption})
    参数结构：
        | 名称  | 类型 |  必填 |  说明 |");
                foreach (var property in item.Argument.Properties.Where(p => p.CanUserInput))
                {
                    code.Append($@"
        {property.Name} | {property.LastCsType} | {(property.CanEmpty ? "是" : "否")} | {property.Caption}:{
                            property.Description
                        }");
                }
            }
            code.Append(@"
### 返回值");
            if (item.Argument == null)
            {
                code.Append(@"无(标准返回)");
            }
            else
            {
                code.Append($@"
    返回类型：{item.Name}({item.Argument.Caption})
    数据结构：
        | 名称  | 类型 |  必填 |  说明 |");
                foreach (var property in item.Argument.Properties.Where(p => p.CanUserInput))
                {
                    code.Append($@"
        {property.Name} | {property.LastCsType} | {(property.CanEmpty ? "是" : "否")} | {property.Caption}:{
                            property.Description
                        }");
                }
            }

            code.Append($@"
}}
```

### 请求示例
    *URL：http://基地址/{item.RoutePath}");
            if (item.Argument == null)
            {
                code.Append(@"
    *Request>(无参数)");
            }
            else
            {
                code.Append(@"
>Request
{");
                bool isFirst = true;
                foreach (var property in item.Argument.Properties.Where(p => p.CanUserInput))
                {
                    if (isFirst)
                        isFirst = false;
                    else
                        code.Append(",");
                    if (property.CsType == "string")
                    {
                        code.Append($@"
    ""{property.Name}"":""你的例子""");
                    }
                    else
                    {
                        code.Append($@"
    ""{property.Name}"":*你的参数");
                    }
                }
                code.Append(@"
}");
            }

            code.Append(@"    
>Success
{
    ""Result"": true,
    ""Message"": ""操作成功""");
            if (item.Result != null)
            {
                code.Append(@",
    ""ResultData"",{ ");
                bool isFirst = true;
                foreach (var property in item.Result.Properties.Where(p => p.CanUserInput))
                {
                    if (isFirst)
                        isFirst = false;
                    else
                        code.Append(",");
                    if (property.CsType == "string")
                    {
                        code.Append($@"
        ""{property.Name}"":""你的数据""");
                    }
                    else
                    {
                        code.Append($@"
        ""{property.Name}"":*你的数据");
                    }
                }
                code.Append(@" 
    }");
            }
            code.Append(@" 
}
```
>Failed
{
    ""Result"": false,
    ""Message"": ""错误消息"",
    ""ErrorCode"": 错误代码
}
```");
        }

        #endregion


    }

}

