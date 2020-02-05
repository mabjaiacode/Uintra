﻿using System.Collections.Generic;
using Compent.Shared.DependencyInjection.Contract;
using Uintra20.Features.Navigation.ApplicationSettings;
using Uintra20.Features.Navigation.EqualityComparers;
using Uintra20.Features.Navigation.Models.MyLinks;
using Uintra20.Infrastructure;
using Uintra20.Infrastructure.UintraInformation;

namespace Uintra20.Features.Navigation
{
    public class NavigationInjectModule: IInjectModule
    {
        public IDependencyCollection Register(IDependencyCollection services)
        {
            services.AddScoped<INavigationModelsBuilder, NavigationModelsBuilder>();
            services.AddScoped<IUintraInformationService, UintraInformationService>();
            
            services.AddScoped<IMyLinksHelper, MyLinksHelper>();
            services.AddSingleton<IEqualityComparer<MyLinkItemModel>, MyLinkItemModelComparer>();

            services.AddSingleton<INavigationApplicationSettings, NavigationApplicationSettings>();

            return services;
        }
    }
}