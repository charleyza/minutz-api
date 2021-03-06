﻿using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using AspnetAuthenticationRepository;
using AspnetAuthenticationRepository.Interfaces;
using Core;
using Core.ExternalServices;
using Core.Feature.Admin;
using Core.Feature.Dashboard;
using Core.Feature.Invite;
using Core.Feature.Meeting;
using Core.Feature.Meeting.Action;
using Core.Feature.Meeting.Agenda;
using Core.Feature.Meeting.Attachment;
using Core.Feature.Meeting.Decision;
using Core.Feature.Meeting.Header;
using Core.Feature.Meeting.Note;
using Core.Feature.Meeting.Status;
using Core.Helper;
using Core.LogProvider;
using Core.Validation;
using DinkToPdf;
using DinkToPdf.Contracts;
using Interface;
using Interface.Helper;
using Interface.Repositories;
using Interface.Repositories.Feature.Admin;
using Interface.Repositories.Feature.Dashboard;
using Interface.Repositories.Feature.Meeting;
using Interface.Repositories.Feature.Meeting.Action;
using Interface.Repositories.Feature.Meeting.Agenda;
using Interface.Repositories.Feature.Meeting.Attachment;
using Interface.Repositories.Feature.Meeting.Decision;
using Interface.Repositories.Feature.Meeting.Header;
using Interface.Repositories.Feature.Meeting.Note;
using Interface.Repositories.Feature.Meeting.Status;
using Interface.Services;
using Interface.Services.Admin;
using Interface.Services.Feature.Dashboard;
using Interface.Services.Feature.Invite;
using Interface.Services.Feature.Meeting;
using Interface.Services.Feature.Meeting.Action;
using Interface.Services.Feature.Meeting.Agenda;
using Interface.Services.Feature.Meeting.Attachment;
using Interface.Services.Feature.Meeting.Decision;
using Interface.Services.Feature.Meeting.Header;
using Interface.Services.Feature.Meeting.Note;
using Interface.Services.Feature.Meeting.Status;
using Interface.Services.Feature.Notification;
using jsreport.AspNetCore;
using jsreport.Local;
using jsreport.Binary;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

using Notifications;
using Reports;
using SqlRepository;
using SqlRepository.Features.Admin;
using SqlRepository.Features.Dashboard;
using SqlRepository.Features.Meeting;
using SqlRepository.Features.Meeting.Action;
using SqlRepository.Features.Meeting.Agenda;
using SqlRepository.Features.Meeting.Attachment;
using SqlRepository.Features.Meeting.Attendee;
using SqlRepository.Features.Meeting.Decision;
using SqlRepository.Features.Meeting.Header;
using SqlRepository.Features.Meeting.Note;
using SqlRepository.Features.Meeting.Status;
using SqlRepository.User;
using Swashbuckle.AspNetCore.Swagger;
using HttpService = AuthenticationRepository.HttpService;

// ReSharper disable once CheckNamespace
namespace Minutz.Api
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
            // _hostingEnvironment = env;
        }
        public const string Version = "3.1.0";
        private const string Title = "Minutz Api";

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
            // var physicalProvider = _hostingEnvironment.ContentRootFileProvider;

            services.AddTransient<IEmailValidationService, EmailValidationService>();
            
            services.AddTransient<IEncryptor, MinutzEncryption.Encryptor>();
            services.AddTransient<ICustomPasswordValidator, CustomPasswordValidator>();
            services.AddTransient<IMinutzUserManager, MinuzUserManager>();
            services.AddTransient<IMinutzRoleManager, MinutzRoleManager>();
            services.AddTransient<IMinutzClaimManager, MinutzClaimManager>();
            services.AddTransient<IMinutzJwtSecurityTokenManager, MinutzJwtSecurityTokenManager>();
            
            services.AddTransient<IHttpService, HttpService>();
            services.AddTransient<IValidationService, ValidationService>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IApplicationSetupRepository, ApplicationSetupRepository>();
            services.AddTransient<IMeetingRepository, MeetingRepository>();
            services.AddTransient<IMeetingAgendaRepository, MeetingAgendaRepository>();
            services.AddTransient<IMeetingAttendeeRepository, MeetingAttendeeRepository>();
            services.AddTransient<IMeetingActionRepository, MeetingActionRepository>();
            services.AddTransient<IMeetingAttachmentRepository, MeetingAttachmentRepository>();
            services.AddTransient<IMeetingNoteRepository, MeetingNoteRepository>();
            services.AddTransient<IInstanceRepository, InstanceRepository>();
            services.AddTransient<ILogRepository, LogRepository>();
            services.AddTransient<IReminderRepository, ReminderRepository>();
            services.AddTransient<INotificationRoleRepository, NotificationRoleRepository>();
            services.AddTransient<INotificationTypeRepository, NotificationTypeRepository>();
            services.AddTransient<ISubscriptionRepository, SubscriptionRepository>();
            services.AddTransient<IDecisionRepository, MeetingDecisionRepository>();

            //Services
            services.AddTransient<IApplicationSetting, ApplicationSetting>();
            services.AddTransient<INotify, Notify>();
            services.AddTransient<INotificationService, NotificationService>();
            services.AddTransient<ILogService, LogService>();

            //services.AddTransient< IAuthRepository,Auth0Repository>();
            services.AddTransient<IAuthRepository, AspnetAuthRepository>();
            services.AddTransient<IReportRepository, JsReportRepository>();
            services.AddTransient<ICacheRepository, CacheRepository>();

            //Features
            services.AddTransient<IUserValidationService, UserValidationService>();
            services.AddTransient<IAuthenticationService, AuthenticationService>();
            services.AddTransient<IApplicationManagerService, ApplicationManagerService>();

            services.AddTransient<IMeetingAttachmentService, MeetingAttachmentService>();
            services.AddTransient<IMeetingActionService, MeetingActionService>();
            services.AddTransient<IMeetingDecisionService, MeetingDecisionService>();

            services.AddTransient<IMeetingStatusRepository, MeetingStatusRepository>();
            services.AddTransient<IMeetingStatusService, MeetingStatusService>();
            
            //Main meeting Service
            services.AddTransient<IMeetingService, MeetingService>();

            //todo: redo
            //services.AddTransient<IInvitationService, InvatationService>();
            services.AddTransient<IStartupService, StartupService>();
            services.AddTransient<IReminderService, ReminderService>();
            services.AddTransient<INotificationRoleService, NotificationRoleService>();
            services.AddTransient<INotificationTypeService, NotificationTypeService>();
            services.AddTransient<ISubscriptionService, SubscriptionService>();
            services.AddTransient<IInstanceService, InstanceService>();
            services.AddTransient<IReportService, JsReportService>();

            
            
            //*
            // Features
            // ---- Meeting
            //*
            //Meeting Title
            services.AddTransient<IMinutzTitleRepository, MinutzTitleRepository>();
            services.AddTransient<IMinutzTitleService, MinutzTitleService>();
            //Meeting Location
            services.AddTransient<IMinutzLocationRepository, MinutzLocationRepository>();
            services.AddTransient<IMinutzLocationService, MinutzLocationService>();
            //Meeting Date
            services.AddTransient<IMinutzDateRepository, MinutzDateRepository>();
            services.AddTransient<IMinutzDateService, MinutzDateService>();          
            //Meeting Time
            services.AddTransient<IMinutzTimeRepository, MinutzTimeRepository>();
            services.AddTransient<IMinutzTimeService, MinutzTimeService>();          
            //Meeting Time
            services.AddTransient<IMinutzDurationRepository, MinutzDurationRepository>();
            services.AddTransient<IMinutzDurationService, MinutzDurationService>();
            //Meeting Tag
            services.AddTransient<IMinutzTagRepository, MinutzTagRepository>();
            services.AddTransient<IMinutzTagService, MinutzTagService>();
            
            //*
            // Features
            // ---- Dashboard
            //*
            //User Meetings
            services.AddTransient<IUserMeetingsRepository, UserMeetingsRepository>();
            services.AddTransient<IUserMeetingsService, UserMeetingsService>();
            //User Actions
            services.AddTransient<IUserActionsRepository, UserActionsRepository>();
            services.AddTransient<IUserActionsService, UserActionsService>();

            //*
            // Features
            // ---- Meeting
            // ------- Agenda
            //*MinutzAgenda
            services.AddTransient<IMinutzAgendaRepository, MinutzAgendaRepository>();
            services.AddTransient<IMinutzAgendaService, MinutzAgendaService>();
            //*MinutzAction
            services.AddTransient<IMinutzActionRepository, MinutzActionRepository>();
            services.AddTransient<IMinutzActionService, MinutzActionService>();
            //User Meeting Manager 
            services.AddTransient<IUserManageMeetingRepository, UserManageMeetingRepository>();
            services.AddTransient<IUserManageMeetingService, UserManageMeetingService>();
            // Objective Purpose
            services.AddTransient<IMeetingObjectivePurposeRepository, MeetingObjectivePurposeRepository>();
            services.AddTransient<IMeetingObjectivePurposeService, MeetingObjectivePurposeService>();

            services.AddTransient<IMinutzAvailabilityRepository, MinutzAvailabilityRepository>();
            services.AddTransient<IMinutzAvailabilityService, MinutzAvailabilityService>();
            
            services.AddTransient<IMinutzAttendeeRepository, MinutzAttendeeRepository>();
            services.AddTransient<IMinutzAttendeeService, MinutzAttendeeService>();

            services.AddTransient<IMeetingRecurrenceRepository, MeetingRecurrenceRepository>();
            services.AddTransient<IMinutzRecurrenceService, MinutzRecurrenceService>();

            
            services.AddTransient<IGetMeetingRepository, GetMeetingRepository>();
            services.AddTransient<IGetMeetingService, GetMeetingService>();

            services.AddTransient<IMinutzMeetingAttachmentRepository, MinutzMeetingAttachmentRepository>();
            services.AddTransient<IMinutzMeetingAttachmentService, MinutzMeetingAttachmentService>();

            services.AddTransient<IMinutzDecisionRepository, MinutzDecisionRepository>();
            services.AddTransient<IMinutzDecisionService, MinutzDecisionService>();

            services.AddTransient<IMinutzNoteRepository, MinutzNoteRepository>();
            services.AddTransient<IMinutzNoteService, MinutzNoteService>();
            
            services.AddTransient<IInvitationService, InvitationService>();

            services.AddTransient<IInstanceUserRepository, InstanceUserRepository>();
            services.AddTransient<IInstanceUserService, InstanceUserService>();
            
            services.AddMemoryCache();
            services.AddMvc();
            services.AddJsReport(new LocalReporting()
                .UseBinary(JsReportBinary.GetBinary())
                .AsUtility()
                .Create());
            var appSetting = new ApplicationSetting(new InstanceRepository(), new MinutzEncryption.Encryptor());

            var version = Configuration.GetSection("Version").Value;
            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new Info {Title = Title, Version = version}); });
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                    builder => { builder.AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin(); });
            });
            services.AddDbContext<ApplicationDbContext>();

            // ===== Add Identity ========
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // ===== Add Jwt Authentication ========
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); // => remove default claims
            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(cfg =>
                {
                    cfg.RequireHttpsMetadata = false;
                    cfg.SaveToken = true;
                    cfg.TokenValidationParameters = new TokenValidationParameters
                                                    {
                                                        ValidIssuer =
                                                            appSetting.AuthorityDomain, //Configuration["JwtIssuer"],
                                                        ValidAudience =
                                                            appSetting.AuthorityDomain, //Configuration["JwtIssuer"],
                                                        IssuerSigningKey =
                                                            new SymmetricSecurityKey(
                                                                Encoding.UTF8.GetBytes(appSetting
                                                                    .ClientSecret)), // Configuration["JwtKey"])),
                                                        ClockSkew = TimeSpan.Zero // remove delay of token when expire
                                                    };
                });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, ApplicationDbContext dbContext)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseAuthentication();
            app.UseSwagger();
            var version = Configuration.GetSection("Version").Value;
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", version); });
            app.UseCors("AllowAllOrigins");
            app.UseStaticFiles();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
            dbContext.Database.EnsureCreated();
        }
    }
}