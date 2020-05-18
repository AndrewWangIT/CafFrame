using Caf.AppSetting;
using Caf.AspNetCore;
using Caf.Core.Module;
using Capgemini.Caf;
using Capgemini.Frame.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace AppSettingTest
{
    [UsingModule(typeof(CafAppSettingModule))]
    [UsingModule(typeof(CafAspNetCoreModule))]
    public class sampleModule : CafModule
    {

        public override void BeforeConfigureServices(CafConfigurationContext context)
        {

            context.AddCafCors(o => { o.ConfigurationSection = "App:CorsOrigins"; o.Enable = true; });//添加跨域

        }
        public override void ConfigureServices(CafConfigurationContext context)
        {
            context.Services.AddControllers().AddJsonOptions(options =>
            {
                //格式化日期时间格式

                //options.JsonSerializerOptions.Converters.Add(new DatetimeJsonConverter());
                //options.JsonSerializerOptions.Converters.Add(new DatetimeJsonConverterNullable());
                //options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                //数据格式首字母小写
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                //数据格式原样输出
                //options.JsonSerializerOptions.PropertyNamingPolicy = null;
                //取消Unicode编码
                options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
                //忽略空值
                //options.JsonSerializerOptions.IgnoreNullValues = true;
                //允许额外符号
                options.JsonSerializerOptions.AllowTrailingCommas = true;
                //反序列化过程中属性名称是否使用不区分大小写的比较
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = false;
            }).AddControllersAsServices().AddMvcLocalization();

            #region swagger
            //context.Services.AddSwaggerGen(options =>
            //{
            //    options.SwaggerDoc("v1", new OpenApiInfo
            //    {
            //        Version = "v1",
            //        Title = "caf API",
            //        Description = "API for caf",
            //        Contact = new OpenApiContact() { Name = "Eddy", Email = "" }
            //    });
            //    options.DocInclusionPredicate((docName, description) => true);
            //    var security = new Dictionary<string, IEnumerable<string>>
            //        {
            //            {"Bearer", new string[] { }},
            //        };
            //    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            //    {
            //        Description = "Please enter into field the word 'Bearer' followed by a space and the JWT value",
            //        Name = "Authorization",
            //        BearerFormat = "JWT",
            //        In = ParameterLocation.Header,
            //        Type = SecuritySchemeType.ApiKey,
            //        Scheme = "Bearer",

            //    });
            //    options.AddSecurityRequirement(new OpenApiSecurityRequirement
            //    {
            //        { new OpenApiSecurityScheme
            //        {
            //            Reference = new OpenApiReference()
            //            {
            //                Id = "Bearer",
            //                Type = ReferenceType.SecurityScheme
            //            }
            //        }, new string[] {} }
            //    });
            //    var basePath = Path.GetDirectoryName(typeof(Program).Assembly.Location);//获取应用程序所在目录（绝对，不受工作目录影响，建议采用此方法获取路径）
            //    var xmlPath = Path.Combine(basePath, "caf.WebApi.xml");
            //    // var xmlPath1 = Path.Combine(basePath, "caf.Application.xml");
            //    var xmlPath2 = Path.Combine(basePath, "caf.Core.xml");
            //    //options.IncludeXmlComments(xmlPath);
            //    // options.IncludeXmlComments(xmlPath1);
            //    // options.IncludeXmlComments(xmlPath2);
            //});
            #endregion

        }

        public override void OnApplicationInitialization(CafApplicationContext context)
        {
            var app = context.GetApplicationBuilder();
            var env = context.GetEnvironment();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            #region  Localization
            var supportedCultures = new[] { "zh-CN", "en-US" };
            app.UseRequestLocalization(cultureOptions =>
            {
                cultureOptions.AddSupportedCultures(supportedCultures)
                .AddSupportedUICultures(supportedCultures)
                .SetDefaultCulture(supportedCultures[0]);
            });
            #endregion

            app.UseRouting();
            app.UseCafExceptionHandler();
            app.UseStaticFiles();

            #region swagger
            //app.UseSwagger();
            //app.UseSwaggerUI(option =>
            //{
            //    option.SwaggerEndpoint("/swagger/v1/swagger.json", "caf Docs");
            //    option.DocumentTitle = "caf API";
            //});
            #endregion
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }


    }
}
