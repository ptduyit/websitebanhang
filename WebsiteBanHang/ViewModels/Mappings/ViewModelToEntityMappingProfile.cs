﻿using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebsiteBanHang.Models;

namespace WebsiteBanHang.ViewModels.Mappings
{
    public class ViewModelToEntityMappingProfile : Profile
    {
        public ViewModelToEntityMappingProfile()
        {
            CreateMap<RegistrationViewModel, User>().ForMember(au => au.UserName, map => map.MapFrom(vm => vm.Email));
            CreateMap<Products, ProductShowcaseViewModel>().ReverseMap();
            CreateMap<ProductCategories, ProductCategoryViewModel>();
            CreateMap<ProductCategories, Breadcrumbs>().ForMember(b => b.Label, map => map.MapFrom(p => p.CategoryName));
            CreateMap<ProductCategories, Route>().ForMember(r => r.Id, map => map.MapFrom(p => p.CategoryId));
            CreateMap<ProductCategories, Menu>();
        }
    }
}
