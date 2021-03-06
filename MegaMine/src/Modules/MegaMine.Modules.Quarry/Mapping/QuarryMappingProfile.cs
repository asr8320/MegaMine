﻿//-------------------------------------------------------------------------------------------------
// <copyright file="QuarryMappingProfile.cs" company="Nootus">
//  Copyright (c) Nootus. All rights reserved.
// </copyright>
// <description>
//  Automapper mapping definitions
// </description>
//-------------------------------------------------------------------------------------------------
namespace MegaMine.Modules.Quarry.Mapping
{
    using AutoMapper;
    using MegaMine.Modules.Quarry.Entities;
    using MegaMine.Modules.Quarry.Models;

    public class QuarryMappingProfile : Profile
    {
        public override string ProfileName
        {
            get
            {
                return "QuarryMappingProfile";
            }
        }

        protected override void Configure()
        {
            Mapper.CreateMap<MaterialColourEntity, MaterialColourModel>().ReverseMap();
            Mapper.CreateMap<ProductTypeEntity, ProductTypeModel>().ReverseMap();
            Mapper.CreateMap<TextureEntity, TextureModel>().ReverseMap();
            Mapper.CreateMap<QuarryEntity, QuarryModel>().ReverseMap();
            Mapper.CreateMap<YardEntity, YardModel>().ReverseMap();
            Mapper.CreateMap<MaterialEntity, MaterialModel>().ReverseMap();
            Mapper.CreateMap<ProductSummaryEntity, ProductSummaryModel>();
            Mapper.CreateMap<StockEntity, StockModel>();
        }
    }
}
