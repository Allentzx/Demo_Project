using System;
namespace ITC.Core.Model
{
    public class ProgramModel
    {
        public string? Name { get; set; }
        public bool Status { get; set; }
        public string? Description { get; set; }
    }

    public class CreateProgramModel
    {
        public string? Name { get; set; }
        public bool Status { get; set; }
        public string? Description { get; set; }
    }

    public class UpdateProgramModel
    {
        public string? Name { get; set; }
        public bool Status { get; set; }
        public string? Description { get; set; }
    }
}

