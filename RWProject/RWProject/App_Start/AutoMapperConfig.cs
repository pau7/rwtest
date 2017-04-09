using RWProject.Database;
using RWProject.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RWProject.App_Start
{
    public class AutoMapperConfig
    {
        public virtual void ConfigureMappings(IConfiguration config)
        {
            config.CreateMap<Product, ProductViewModel>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.Code, o => o.MapFrom(s => s.Code))
                //We the Category doesn't have parent it means that this is the category of the product, if it has parent we use it.
                .ForMember(d => d.CategoryName, o => o.MapFrom(s => s.Category != null ? s.Category.Parent != null ? s.Category.Parent.Name : s.Category.Name : ""))
                //If the category don't have parent we use it as category and we don't show subcategory.
                .ForMember(d => d.SubcategoryName, o => o.MapFrom(s => s.Category != null ? s.Category.Parent != null ? "" : s.Category.Name : ""))
                .ForMember(d => d.Model, o => o.MapFrom(s => s.Model))
                .ForMember(d => d.Name, o => o.MapFrom(s => s.Name))
                .ForMember(d => d.Price, o => o.MapFrom(s => s.Price))
                .ForMember(d => d.Size, o => o.MapFrom(s => s.Size))
                .ForMember(d => d.Colour, o => o.MapFrom(s => s.Colour))
                .ForMember(d => d.Description, o => o.MapFrom(s => s.Description));

            config.CreateMap<ProductViewModel, Product>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.Code, o => o.MapFrom(s => s.Code))
                .ForMember(d => d.Category, o => o.Ignore())
                .ForMember(d => d.Model, o => o.MapFrom(s => s.Model))
                .ForMember(d => d.Name, o => o.MapFrom(s => s.Name))
                .ForMember(d => d.Price, o => o.MapFrom(s => s.Price))
                .ForMember(d => d.Size, o => o.MapFrom(s => s.Size))
                .ForMember(d => d.Colour, o => o.MapFrom(s => s.Colour))
                .ForMember(d => d.Description, o => o.MapFrom(s => s.Description));
}