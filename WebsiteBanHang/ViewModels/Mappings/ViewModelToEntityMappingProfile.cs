using AutoMapper;
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
            CreateMap<ProductCategories, Menu>();

            CreateMap<EvaluationQuestions, EvaluationQuestionsViewModel>().ForMember(v => v.FullName, map => map.MapFrom(e => e.User.FullName));
            CreateMap<Comments, CommentsViewModel>().ForMember(v => v.FullName, map => map.MapFrom(c => c.User.FullName));

            CreateMap<Address, ShowAddressListViewModel>().ForPath(v => v.Location.Ward, map => map.MapFrom(c => c.Wards.Name))
                        .ForPath(v => v.Location.District, map => map.MapFrom(c => c.Wards.Districts.Name))
                        .ForPath(v => v.Location.Province, map => map.MapFrom(c => c.Wards.Districts.Provinces.Name));
        }
    }
}
