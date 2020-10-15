﻿using Snail.Core.Dto;

namespace Snail.Web.Dtos.Config
{
    public class ConfigResultDto:DefaultBaseDto
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public string Name { get; set; }
        public string ParentId { get; set; }
        public string ExtraInfo { get; set; }
    }
}
