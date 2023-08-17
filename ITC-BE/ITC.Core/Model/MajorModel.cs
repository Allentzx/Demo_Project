using System;
namespace ITC.Core.Model
{
	public class MajorModel
	{
	}


	public class CreateMajorModel
	{
        public string? Name { get; set; }
        public string? MajorFullName { get; set; }
    }


    public class UpdateMajorModel
    {
        public string? Name { get; set; }
        public string? MajorFullName { get; set; }
        public bool Status { get; set; }
    }
}

