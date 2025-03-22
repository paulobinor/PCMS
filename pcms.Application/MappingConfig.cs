using AutoMapper;
using pcms.Application.Dto;
using pcms.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pcms.Application
{
    public class MappingConfig
    {
        public static MapperConfiguration PcmsMapConfig()
        {
            var MappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<MemberDto, Member>().ReverseMap();
                config.CreateMap<ContributionDto, Contribution>().ReverseMap();
            });
            return MappingConfig;
        }
    }
}
