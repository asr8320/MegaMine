﻿//-------------------------------------------------------------------------------------------------
// <copyright file="SecurityMappingProfile.cs" company="Nootus">
//  Copyright (c) Nootus. All rights reserved.
// </copyright>
// <description>
//  AutoMapper profile for mapping of entities and models in security assembly
// </description>
//-------------------------------------------------------------------------------------------------
namespace MegaMine.Services.Security.Mapping
{
    using AutoMapper;
    using MegaMine.Core.Exception;
    using MegaMine.Services.Security.Entities;
    using MegaMine.Services.Security.Models;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

    public class SecurityMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "SecurityMappingProfile"; }
        }

        protected override void Configure()
        {
            Mapper.CreateMap<CompanyEntity, CompanyModel>();
            Mapper.CreateMap<IdentityPageEntity, PageModel>(MemberList.Destination);
            Mapper.CreateMap<IdentityPageClaimEntity, PageClaimModel>();
            Mapper.CreateMap<IdentityMenuPageEntity, MenuModel>();
            Mapper.CreateMap<IdentityRoleClaim<string>, ClaimModel>();
            Mapper.CreateMap<IdentityUserClaim<string>, ClaimModel>();
            Mapper.CreateMap<ApplicationRole, RoleModel>(MemberList.Destination);
            Mapper.CreateMap<IdentityError, NTError>();
        }
    }
}
