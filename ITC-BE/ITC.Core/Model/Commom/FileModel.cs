using System;
namespace ITC.Core.Model.Commom
{
	public class FileModel
	{
        public string Id { get; set; }
        public string FileType { get; set; }
        public byte[] Data { get; set; }
        public string FileName { get; set; }
    }
}

