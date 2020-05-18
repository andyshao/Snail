using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NSwag;
using Snail.Core.Permission;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Snail.Permission.Test
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<TestDbContext>(options =>
            {
                options.UseMySql("Server=localhost;Port=3306;Database=permissionTest;User Id=root;Password = root;");
            });
            #region Ĭ��Ȩ�����ݽṹ
            //services.AddDefaultPermission<TestDbContext>(options =>
            //{
            //    Configuration.GetSection("PermissionOptions").Bind(options);
            //    options.ResourceAssemblies = new List<Assembly> { Assembly.GetExecutingAssembly() };
            //});
            #endregion

            #region �Զ���Ȩ�����ݽṹ
            services.AddPermission<TestDbContext, User>(options =>
            {
                Configuration.GetSection("PermissionOptions").Bind(options);
                options.ResourceAssemblies = new List<Assembly> { Assembly.GetExecutingAssembly() };
            });
            services.TryAddScoped<IPermissionStore, CustomPermissionStore>();
            #endregion


            
            services.AddControllers();
            #region swagger
            services.AddOpenApiDocument(conf => {
                conf.Description = "change the description";
                conf.DocumentName = "change the document name";
                conf.GenerateExamples = true;
                conf.Title = "change the title";
                conf.PostProcess = document =>
                {
                    document.SecurityDefinitions.Add(
                          "Jwt��֤",
                          new OpenApiSecurityScheme
                          {
                              Type = OpenApiSecuritySchemeType.Http,
                              Name = "Authorization",//token��ŵ�header��authorization��
                              In = OpenApiSecurityApiKeyLocation.Header,
                              Description = "������ : JWT token",
                              Scheme = "bearer"//����bearer�����ܸ�
                          });
                    document.Security.Add(new OpenApiSecurityRequirement { { "Jwt��֤", new string[0] } });

                };
            }); // add OpenAPI v3 document
            #endregion


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env,IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseAuthentication();

            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(config =>
            {
                config.MapControllers();
            });
            

            #region swag
            //* ����������´���Fetch errorundefined / swagger / v1 / swagger.json
            //* �����ԭ����swagger ��api�ڽ���ʱ������chrome f12����������swagger.json�Ĵ��󣬽��
            app.UseOpenApi(config =>
            {
                config.PostProcess = (document, req) =>
                {
                    //��������swag����https��http�����ַ�ʽ
                    document.Schemes.Add(OpenApiSchema.Https);
                    document.Schemes.Add(OpenApiSchema.Http);
                };
            });
            app.UseSwaggerUi3();
            //app.UseReDoc();
            #endregion
            serviceProvider.GetService<TestDbContext>().Database.Migrate();//�Զ�migrate��ǰ���ǳ�������add-migrate�����
        }
    }
}
