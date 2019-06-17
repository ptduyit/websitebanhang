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
            CreateMap<UserInfoViewModel, UserInfo>().ForPath(v => v.User.PhoneNumber, m => m.MapFrom(c => c.PhoneNumber)).ReverseMap();

            CreateMap<Products, ProductShowcaseViewModel>().ReverseMap();
            CreateMap<ProductCategories, ProductCategoryViewModel>();
            CreateMap<ProductCategories, Breadcrumbs>().ForMember(b => b.Label, map => map.MapFrom(p => p.CategoryName));
            CreateMap<ProductCategories, Menu>();

            CreateMap<EvaluationQuestions, EvaluationQuestionsViewModel>().ForMember(v => v.FullName, map => map.MapFrom(e => e.User.FullName));
            CreateMap<Comments, CommentsViewModel>().ForMember(v => v.FullName, map => map.MapFrom(c => c.User.FullName));

            CreateMap<Address, AddressListViewModel>().ForPath(v => v.Location.Ward, map => map.MapFrom(c => c.Wards.Name))
                        .ForPath(v => v.Location.District, map => map.MapFrom(c => c.Wards.Districts.Name))
                        .ForPath(v => v.Location.Province, map => map.MapFrom(c => c.Wards.Districts.Provinces.Name));

            CreateMap<OrdersImportGoods, OrderImportViewModel>()
                .ForMember(v => v.OrderDetails, map => map.MapFrom(c => c.OrderImportGoodsDetails))
                .ForMember(v => v.FullName, map => map.MapFrom(c => c.User.FullName))
                .ForMember(v => v.CompanyName, map => map.MapFrom(c => c.Supplier.CompanyName));
            
            CreateMap<OrderImportGoodsDetails, ImportDetailProductViewModel>()
                .ForMember(v => v.ProductName, map => map.MapFrom(c => c.Product.ProductName));

            CreateMap<Products, ProductInformationViewModel>();

            CreateMap<CartDetails, CartViewModel>().ForMember(v => v.ProductName, m => m.MapFrom(c => c.Product.ProductName))
                .ForMember(v => v.UnitPrice, m => m.MapFrom(c => c.Product.UnitPrice))
                .ForMember(v => v.Discount, m => m.MapFrom(c => c.Product.Discount))
                .ForMember(v => v.Image, m => m.MapFrom(c => c.Product.ProductImages.FirstOrDefault(p => p.IsThumbnail == true).Url))
                .ForMember(v => v.Stock, m => m.MapFrom(c => c.Product.Stock))
                .ForMember(v => v.Discontinued, m => m.MapFrom(c => c.Product.Discontinued));

            CreateMap<CartDetails, CartOrderViewModel>()
                .ForMember(v => v.UnitPrice, m => m.MapFrom(c => c.Product.UnitPrice))
                .ForMember(v => v.Discontinued, m => m.MapFrom(c => c.Product.Discontinued));
        }
    }
}
